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

namespace onmyouchan
{
    /// <summary>
    /// プレイヤーや敵等の原型
    /// </summary>
    class Character : Entity
    {
        #region フィールド


        /// <summary>
        /// 移動速度
        /// </summary>
        protected float moveSpeed = 1.3f;


        /// <summary>
        /// 最高速度
        /// </summary>
        protected float maxSpeed = 10;

        /// <summary>
        /// 能動的動作不能時間
        /// </summary>
        protected int controlWeit;

        #endregion

        #region コンストラクタ

        public Character(EntityType type, Vector3 pos)
            : base(type)
        {
            position = pos;
        }

        #endregion

        #region 更新

        /// <summary>
        /// baseで実行して下さい
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (controlWeit > 0)
                controlWeit--;

            NatureRotation();
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 回転速度
        /// </summary>
        private const float RotationSpeed = 0.2f;
        /// <summary>
        /// 自然な回転の目標角度
        /// </summary>
        protected float goalAngle = 0;
        /// <summary>
        /// 自然な角度変更
        /// </summary>
        private void NatureRotation()
        {
            if (goalAngle != rotation.Y)
            {
                rotation.Y = MyUtility.OverRadianReset(rotation.Y);
                //回転量が近ければそこに向く
                if (Math.Abs(rotation.Y - goalAngle) <= RotationSpeed)
                {                    
                    rotation.Y = goalAngle;
                }
                else
                {
                    float rota = MathHelper.ToDegrees(rotation.Y);
                    float gRota = MathHelper.ToDegrees(goalAngle);

                    if (rota > 0)
                    {
                        if (rota > gRota && rota - 180 < gRota)
                            rotation.Y -= RotationSpeed;
                        else
                            rotation.Y += RotationSpeed;
                    }
                    else
                    {
                        if (rota < gRota && rota + 180 > gRota)
                            rotation.Y += RotationSpeed;
                        else
                            rotation.Y -= RotationSpeed;
                    }
                }
            }
        }

        /// <summary>
        /// 自身へのダメージ
        /// </summary>
        public virtual void TakenDamage()
        { }

        #endregion
    }
        
    /// <summary>
    /// 地形
    /// </summary>
    class SampleTerrain : Entity
    {
        #region コンストラクタ

        public SampleTerrain(Vector3 pos)
            : base(EntityType.terrain)
        {
            position = pos;
            scale = 2f;
        }

        #endregion

        #region メソッド

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = new EntityStatus_Model_Normal(this, entityManager.LordModel("GameModel\\hakakari2"));
            moveStatus = null;
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Box_Bloc(this, new Vector3(-thisEntityManager.StageSize / 2, -50, -thisEntityManager.StageSize / 2), new Vector3(thisEntityManager.StageSize / 2, 0, thisEntityManager.StageSize / 2)));

            base.Lord(entityManager);
        }

        #endregion
    }

    /// <summary>
    /// 何の効果もない目印用置物
    /// </summary>
    class SamplePoint:Entity
    {
        public SamplePoint(Vector3 pos)
            : base(EntityType.etc)
        { position = pos; }
        public override void Lord(EntityManager entityManager)
        {
            modelStatus = new EntityStatus_Model_SampleShader(this, entityManager.LordModel("O_Cube1"));
            base.Lord(entityManager);
        } 
    }
    class EntityStatus_Model_SampleShader : EntityStatus_Model_Normal
    {
        public EntityStatus_Model_SampleShader(Entity entity,Model model)
            : base(entity, model)
        { }

        public override void Draw(Camera camera, GameTime gameTime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.PreferPerPixelLighting = true;

                    effect.EnableDefaultLighting();

                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.World = world;

                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight1.Enabled = false;
                    effect.DirectionalLight2.Enabled = false;
                }
                mesh.Draw();
            }
        }
    }
}
