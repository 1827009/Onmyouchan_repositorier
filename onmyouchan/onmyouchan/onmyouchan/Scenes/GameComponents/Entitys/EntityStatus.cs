using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SkinnedModel;
using SkinnedEffectExtension;

namespace onmyouchan.Entity
{
    #region モデルクラス

    /// <summary>
    /// トゥーンエフェクトで描画するステータス
    /// </summary>
    class EntityStatus_Model_Toon : EntityStatus_Model
    {
        #region フィールド

        /// <summary>
        /// 作成したEffect
        /// </summary>
        protected Effect effect;

        /// <summary>
        /// テクスチャ
        /// </summary>
        protected string textureName;
                
        #endregion

        #region コンストラクタ

        public EntityStatus_Model_Toon(Entity entity, Model m,string texName)
            : base(entity, m)
        {
            effect = thisEntity.ThisEntityManager.thisGame.Content.Load<Effect>("Effect\\ToonEffect");
            textureName = texName;
            effect.Parameters["ToonTexture"].SetValue(thisEntity.ThisEntityManager.thisGame.Content.Load<Texture2D>("Effect\\ToonTexture"));
        }

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            //回転行列
            Matrix rotationX = Matrix.CreateRotationX(thisEntity.Rotation.X);
            Matrix rotationY = Matrix.CreateRotationY(thisEntity.Rotation.Y);
            Matrix rotationZ = Matrix.CreateRotationZ(thisEntity.Rotation.Z);
            Matrix rotationMatrix = rotationX * rotationY * rotationZ;
            //大きさ
            Matrix scaleMatrix = Matrix.CreateScale(thisEntity.Scale);
            //最終的な行列
            world = rotationMatrix * scaleMatrix * Matrix.CreateTranslation(thisEntity.position);
        }

        #endregion

        #region 描画

        public override void Draw(Camera camera, GameTime gameTime)
        {
            effect.Parameters["MyTexture"].SetValue(thisEntity.ThisEntityManager.thisGame.Content.Load<Texture2D>(textureName));
            foreach (ModelMesh mesh in model.Meshes)
            {
                effect.Parameters["World"].SetValue(transform[mesh.ParentBone.Index] * world);
                effect.Parameters["WorldViewProjection"].SetValue(world * camera.view * camera.projection);
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (EffectTechnique technique in meshPart.Effect.Techniques)
                    {
                        thisEntity.ThisEntityManager.thisGame.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                        thisEntity.ThisEntityManager.thisGame.GraphicsDevice.Indices = meshPart.IndexBuffer;

                        RasterizerState rasterizerState2 = RasterizerState.CullClockwise;
                        thisEntity.ThisEntityManager.thisGame.GraphicsDevice.RasterizerState = rasterizerState2;

                        effect.CurrentTechnique.Passes[0].Apply();

                        thisEntity.ThisEntityManager.thisGame.GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            meshPart.VertexOffset,
                            0,
                            meshPart.NumVertices,
                            meshPart.StartIndex,
                            meshPart.PrimitiveCount
                            );

                        RasterizerState rasterizerState1 = RasterizerState.CullCounterClockwise;
                        thisEntity.ThisEntityManager.thisGame.GraphicsDevice.RasterizerState = rasterizerState1;

                        effect.CurrentTechnique.Passes[1].Apply();

                        thisEntity.ThisEntityManager.thisGame.GraphicsDevice.DrawIndexedPrimitives(
                            PrimitiveType.TriangleList,
                            meshPart.VertexOffset,
                            0,
                            meshPart.NumVertices,
                            meshPart.StartIndex,
                            meshPart.PrimitiveCount
                            );
                    }
                }
            }
        }

        #endregion
    }

    class EntityStatus_Model_Motion : EntityStatus_Model
    {
        #region フィールド

        SkinningData skinningData;
        AnimationPlayer animationPlayer;
        /// <summary>
        /// クリップ名配列のインデックス
        /// </summary>
        string[] clipNames = { "Take 001" };
        /// <summary>
        /// クリップ名配列
        /// これはこのサンプルで使用しているC_Skinman.fbxに組み込まれたいるものです。
        /// </summary>
        int clipIndex;
        /// <summary>
        /// アニメーションのループ再生フラグ
        /// </summary>
        public bool LoopEnable
        {
            get { return loopEnable; }
            set { loopEnable = value; }
        }
        bool loopEnable;
        /// <summary>
        /// アニメーションの一時停止フラグ
        /// </summary>
        bool pauseEnable;
        /// <summary>
        /// アニメーションのスローモーション再生速度
        /// １より大きくなるにしたがって再生速度が遅くなります。
        /// </summary>
        int slowMotionOrder;
        int slowMotionCount;

        #endregion

        public EntityStatus_Model_Motion(Entity entity, Model m, string texName)
            : base(entity, m)//,texName)
        {
            loopEnable = true;
            LoadSkinnedModel();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateAnimation(gameTime, true, world);
        }

        /// <summary>
        /// スキンモデルの読み込み処理
        /// </summary>
        private void LoadSkinnedModel()
        {
            // SkinningDataを取得
            skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // AnimationPlayerを作成
            animationPlayer = new AnimationPlayer(skinningData);

            // アニメーションを再生する
            ChangeAnimationClip(clipNames[clipIndex], loopEnable, 0.0f);
        }

        /// <summary>
        /// アニメーションの更新
        /// </summary>
        private void UpdateAnimation(GameTime gameTime, bool relativeToCurrentTime, Matrix transform)
        {
            // 一時停止状態でないか？
            if (pauseEnable)
                return;

            // スローモーションが有効か？
            if (slowMotionOrder > 0)
            {
                if (slowMotionCount > 0)
                {
                    slowMotionCount--;
                    return;
                }
                slowMotionCount = slowMotionOrder;
            }

            // アニメーションの更新
            animationPlayer.Update(gameTime.ElapsedGameTime, true, transform);
        }
        
        /// <summary>
        /// アニメーションの切替処理
        /// </summary>
        public void ChangeAnimationClip(string clipName, bool loop, float weight)
        {
            // クリップ名からAnimationClipを取得して再生する
            AnimationClip clip = skinningData.AnimationClips[clipNames[clipIndex]];

            animationPlayer.StartClip(clip, loop, weight);
        }

        #region 描画

        public override void Draw(Camera camera, GameTime gameTime)
        {
            Matrix[] bones = animationPlayer.GetSkinTransforms();
            // モデルを描画
            foreach (ModelMesh mesh in model.Meshes)
            {
                string name = mesh.Name;
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                }

                mesh.Draw();
            }
        }

        #endregion
    }

    class EntityStatus_Model_ToonMotion : EntityStatus_Model
    {
        #region

        CustomSkinnedEffect custam;

        SkinningData skinningData;
        AnimationPlayer animationPlayer;
        /// <summary>
        /// クリップ名配列のインデックス
        /// </summary>
        string[] clipNames;
        /// <summary>
        /// クリップ名配列
        /// </summary>
        public int ClipIndex
        {
            get { return clipIndex; }
            set { clipIndex = value; }
        }
        int clipIndex;
        /// <summary>
        /// アニメーションのループ再生フラグ
        /// </summary>
        public bool LoopEnable
        {
            get { return loopEnable; }
            set { loopEnable = value; }
        }
        bool loopEnable;
        /// <summary>
        /// アニメーションの一時停止フラグ
        /// </summary>
        bool pauseEnable;
        /// <summary>
        /// アニメーションのスローモーション再生速度
        /// １より大きくなるにしたがって再生速度が遅くなります。
        /// </summary>
        protected int slowMotionOrder;
        int slowMotionCount;
        
        #endregion

        public EntityStatus_Model_ToonMotion(Entity entity, Model m, string texName,params string[] motionKey)
            : base(entity, m)
        {
            clipNames = motionKey;   
            loopEnable = true;
            LoadSkinnedModel();

            // Load our custom effect
            Effect customEffect = thisEntity.ThisEntityManager.thisGame.Content.Load<Effect>("Effect/SkinnedToonEffect");
            customEffect.Parameters["ToonTexture"].SetValue(thisEntity.ThisEntityManager.thisGame.Content.Load<Texture2D>("Effect\\ToonTexture"));
            customEffect.Parameters["MyTexture"].SetValue(thisEntity.ThisEntityManager.thisGame.Content.Load<Texture2D>(texName));

            // Apply it to the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    SkinnedEffect skinnedEffect = part.Effect as SkinnedEffect;
                    if (skinnedEffect != null)
                    {
                        // Create new custom skinned effect from our base effect
                        CustomSkinnedEffect custom = new CustomSkinnedEffect(customEffect);
                        custom.CopyFromSkinnedEffect(skinnedEffect);

                        part.Effect = custom;
                    }
                }
            }

            // Look up our custom skinning information.
            SkinningData skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");
            
        }

        public override void Update(GameTime gameTime)
        {
            //base.Update(gameTime);

            // 範囲を超えたら初期化
            if (clipIndex >= clipNames.Length)
                clipIndex = clipNames.Length - 1;
            if (clipIndex < 0)
                clipIndex = 0;

            // クリップに変更があったか？
            if (animationPlayer.CurrentClip.Name.CompareTo(clipNames[clipIndex]) != 0)
                // クリップを切り替える
                ChangeAnimationClip(clipNames[clipIndex], loopEnable, 0.0f);

            UpdateAnimation(gameTime, true, world);
        }

        /// <summary>
        /// スキンモデルの読み込み処理
        /// </summary>
        private void LoadSkinnedModel()
        {
            // SkinningDataを取得
            skinningData = model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // AnimationPlayerを作成
            animationPlayer = new AnimationPlayer(skinningData);

            // アニメーションを再生する
            ChangeAnimationClip(clipNames[clipIndex], loopEnable, 0.0f);
        }

        /// <summary>
        /// アニメーションの更新
        /// </summary>
        private void UpdateAnimation(GameTime gameTime, bool relativeToCurrentTime, Matrix transform)
        {
            // 一時停止状態でないか？
            if (pauseEnable)
                return;

            // スローモーションが有効か？
            if (slowMotionOrder > 0)
            {
                if (slowMotionCount > 0)
                {
                    slowMotionCount--;
                    return;
                }
                slowMotionCount = slowMotionOrder;
            }

            // アニメーションの更新
            animationPlayer.Update(gameTime.ElapsedGameTime, true, transform);
        }

        /// <summary>
        /// アニメーションの切替処理
        /// </summary>
        public void ChangeAnimationClip(string clipName, bool loop, float weight)
        {
            // クリップ名からAnimationClipを取得して再生する
            AnimationClip clip = skinningData.AnimationClips[clipNames[clipIndex]];

            animationPlayer.StartClip(clip, loop, weight);
        }

        #region 描画

        public override void Draw(Camera camera, GameTime gameTime)
        {
            Matrix[] bones = animationPlayer.GetSkinTransforms();

            // モデルを描画
            foreach (ModelMesh mesh in model.Meshes)
            {
                string name = mesh.Name;
                foreach (CustomSkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);
                    effect.View = camera.view;
                    effect.Projection = camera.projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
        }

        #endregion
    }

    #endregion

    #region あたり判定クラス

    /// <summary>
    /// Entity内のEntityStatus_Hitに代入すると円形のあたり判定を付与します
    /// 当たった時の反応は両者反発です
    /// </summary>
    class EntityStatus_Hit_Ball_Repulsion : EntityStatus_Hit_Ball
    {
        #region フィールド

        /// <summary>
        /// 衝突時の反発力
        /// </summary>
        public float Resilience
        {
            get { return resilience; }
        }
        protected float resilience;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ステータスを付与したいEntityの参照と半径の大きさを入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="a"></param>
        public EntityStatus_Hit_Ball_Repulsion(Entity entity, float r, HitPriorityGroup priority)
            : base(entity, r,priority)
        {
            hitGroup = HitPriorityGroup.player;
        }

        /// <summary>
        /// ステータスを付与したいEntityの参照と半径の大きさと抵抗値を入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="a"></param>
        public EntityStatus_Hit_Ball_Repulsion(Entity entity, float r, float reg, HitPriorityGroup priority)
            : base(entity, r,priority)
        {
            resilience = reg;
        }

        /// <summary>
        /// ステータスを付与したいEntityの参照と半径の大きさとあたり判定の優先度を入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="a"></param>
        public EntityStatus_Hit_Ball_Repulsion(Entity entity, float r, HitPriorityGroup grope, HitPriorityGroup priority)
            : base(entity, r,priority)
        {
            hitGroup = grope;
        }

        #endregion
        
        #region 判定

        protected override bool Hitjudg_Ball(EntityStatus_Hit entity)
        {
            //キャストしてBallの半径を取り出せるようにします
            Shape_Ball entityR = (Shape_Ball)entity;

            //あたり判定をします
            Vector3 vec;
            vec.X = entity.thisEntity.position.X - thisEntity.position.X;
            vec.Y = entity.thisEntity.position.Y - thisEntity.position.Y;
            vec.Z = entity.thisEntity.position.Z - thisEntity.position.Z;
            float r = entityR.Radius + radius;

            if ((vec.X * vec.X) + (vec.Y * vec.Y) + (vec.Z * vec.Z) <= r * r)
            {
                Vector3 reboundDirection = MyUtility.NormalizeVector3(vec);
                if (thisEntity.MoveStatus != null)
                    thisEntity.MoveStatus.moveVec += -reboundDirection * resilience;
                if (entity.thisEntity.MoveStatus != null)
                    entity.thisEntity.MoveStatus.moveVec += reboundDirection * resilience;

                return true;
            }
            return false;
        }

        #endregion
    }
    
    /// <summary>
    /// Entity内のEntityStatus_Hitに代入すると箱型のあたり判定を付与します
    /// 当たった時の反応は対象の進行不可です
    /// </summary>
    class EntityStatus_Hit_Box_Bloc : EntityStatus_Hit_Box
    {
        #region コンストラクタ

        /// <summary>
        /// ステータスを付与したいEntityの参照と箱を構成する頂点2つを入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="a"></param>
        public EntityStatus_Hit_Box_Bloc(Entity entity, Vector3 posMin, Vector3 posMax, HitPriorityGroup priority)
            : base(entity, posMin, posMax,priority)
        {
        }

        #endregion

        #region 判定

        protected override bool Hitjudg_Ball(EntityStatus_Hit entity)
        {
            //Ballの要素を取り出せるようにする
            Shape_Ball entityR = (Shape_Ball)entity;

            //X,Z軸が一致していた場合
            if (HitBall(BoxPointMax, BoxPointMin, entity.thisEntity.position, entityR.Radius))
            {
                switch (FaceHitBall(BoxPointMax, BoxPointMin, entity.thisEntity.position, entity.thisEntity.MoveStatus.OldPosition, entityR.Radius))
                {
                    case HitFace.Up:
                        if (entity.thisEntity is Player)
                        { }
                        entity.thisEntity.position.Y = BoxPointMax.Y + entityR.Radius;
                        if (entity.thisEntity.MoveStatus != null)
                        {
                            entity.thisEntity.MoveStatus.moveVec.Y = 0;
                            entity.thisEntity.MoveStatus.landing = true;
                            if (entity.thisEntity is Hitogata)
                            {
                                int a=0;
                                a++;
                            }
                        }
                        return true;

                    case HitFace.Doun:
                    entity.thisEntity.position.Y = BoxPointMin.Y - entityR.Radius;
                    return true;

                    case HitFace.Left:
                    entity.thisEntity.position.X = BoxPointMin.X - entityR.Radius;
                    entity.thisEntity.MoveStatus.moveVec.X = 0;
                    return true;

                    case HitFace.Right:
                    entity.thisEntity.position.X = BoxPointMax.X + entityR.Radius;
                    entity.thisEntity.MoveStatus.moveVec.X = 0;
                    return true;

                    case HitFace.Back:
                    entity.thisEntity.position.Z = BoxPointMin.Z - entityR.Radius;
                    entity.thisEntity.MoveStatus.moveVec.Z = 0;
                    return true;

                    case HitFace.flont:
                    entity.thisEntity.position.Z = BoxPointMax.Z + entityR.Radius;
                    entity.thisEntity.MoveStatus.moveVec.Z = 0;
                    return true;
                }                
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// Entity内のEntityStatus_Hitに代入すると線形のあたり判定を付与します
    /// 当たった時の反応は対象の進行不可です
    /// </summary>
    class EntityStatus_Hit_PlaneLine_Bloc : EntityStatus_Hit_PlaneLine
    {
        #region コンストラクタ

        /// <summary>
        /// ステータスを付与したいEntityの参照と線分を構成する頂点3つを入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="a"></param>
        public EntityStatus_Hit_PlaneLine_Bloc(Entity entity, HitPriorityGroup priority, params Vector2[] point)
            : base(entity,priority, point)
        {
            hitGroup = HitPriorityGroup.terrain;
            for (int i = 0; i < thisEntity.ThisEntityManager.hitList.Count; i++)
            {
                if (thisEntity.ThisEntityManager.hitList[i] is EntityStatus_Hit_PlaneLine_Bloc)
                    Hitjudg_PlaneLine_Bloc(thisEntity.ThisEntityManager.hitList[i]);
            }
        }

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion

        #region 判定
        
        /// <summary>
        /// 線分の壁と交差した際、鋭角に入り込んでめり込むことを防ぐ
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="index"></param>
        protected void Hitjudg_PlaneLine_Bloc(EntityStatus_Hit entity)
        {
            Shape_Line2D entityLine = (Shape_Line2D)entity;
            for (int i = 0; i < Line.Length; i++)
            {
                for (int j = 0; j < entityLine.Line.Length; j++)
                {
                    Vector2 vecA1 = new Vector2(entityLine.Line[j].X, entityLine.Line[j].Y);
                    Vector2 vecA2 = new Vector2(entityLine.Line[(j + 1) % entityLine.Line.Length].X, entityLine.Line[(j + 1) % entityLine.Line.Length].Y);
                    if (CrossingLine(vecA1, vecA2, Line[i], Line[(i + 1) % Line.LongLength]))
                    {
                        Vector2 pos = CrossingPoint(vecA1, vecA2, Line[i], Line[(i + 1) % Line.LongLength]);

                        EntityStatus_Hit_CrossingPointBloc crossingPoint = new EntityStatus_Hit_CrossingPointBloc(thisEntity, new Vector3(pos.X, 30f, pos.Y), 1f);
                        thisEntity.HitStatus.Add(crossingPoint);
                    }
                }
            }
        }

        protected override bool Hitjudg_Ball(EntityStatus_Hit entity)
        {
            for (int i = 0; i < Line.Length; i++)
            {
                Vector2 vecA1 = new Vector2(entity.thisEntity.MoveStatus.OldPosition.X, entity.thisEntity.MoveStatus.OldPosition.Z);
                Vector2 vecA2 = new Vector2(entity.thisEntity.position.X, entity.thisEntity.position.Z);
                if (CrossingLine(vecA1, vecA2, Line[i], Line[(i + 1) % Line.LongLength]))
                {
                    Vector2 vec = ProjectionVector(vecA1, vecA2, Line[i], Line[(i + 1) % Line.LongLength]);

                    float rag = (float)Math.Atan2(entity.thisEntity.position.Z - entity.thisEntity.MoveStatus.OldPosition.Z, entity.thisEntity.position.X - entity.thisEntity.MoveStatus.OldPosition.X);

                    entity.thisEntity.position.X = vec.X - (float)Math.Cos(rag) * 1;
                    entity.thisEntity.position.Z = vec.Y - (float)Math.Sin(rag) * 1;
                }
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// 線分の交差点に設置される判定
    /// 当たった時の反応は進行不可です
    /// </summary>
    class EntityStatus_Hit_CrossingPointBloc : EntityStatus_Hit_Ball
    {
        #region フィールド

        Vector3 position;

        #endregion

        #region コンストラクタ

        public EntityStatus_Hit_CrossingPointBloc(Entity entity, Vector3 pos, float r)
            : base(entity, r,HitPriorityGroup.terrain)
        {
            hitGroup = HitPriorityGroup.terrain;
            position = pos;
        }

        #endregion

        #region 判定

        protected override bool Hitjudg_Ball(EntityStatus_Hit entity)
        {
            Shape_Ball entityBall=(Shape_Ball)entity;

            if (entity.thisEntity.MoveStatus != null)
                radius = MyUtility.Vector2Size(new Vector2(entity.thisEntity.MoveStatus.moveVec.X, entity.thisEntity.MoveStatus.moveVec.Z)) * 1.1f;

            if (CircleHit(entity.thisEntity.position, entityBall.Radius, position, Radius))
            {
                float rag = (float)Math.Atan2(entity.thisEntity.MoveStatus.OldPosition.Z - position.Z, entity.thisEntity.MoveStatus.OldPosition.X - position.X);
                entity.thisEntity.position.X = position.X + (float)Math.Cos(rag) * Radius;
                entity.thisEntity.position.Z = position.Z + (float)Math.Sin(rag) * Radius;
                return true;
            }
            return false;
        }

        #endregion
    }

    #endregion
}