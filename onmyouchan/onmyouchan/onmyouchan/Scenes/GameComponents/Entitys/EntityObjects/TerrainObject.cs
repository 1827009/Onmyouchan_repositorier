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

namespace onmyouchan.Entity
{
    /// <summary>
    /// 地形
    /// </summary>
    class SampleTerrain : Entity
    {
        #region コンストラクタ

        public Vector2 size;

        public SampleTerrain(Vector3 pos,Vector2 s)
            : base(EntityType.terrain)
        {
            position = pos;
            scale = 4f;
            this.size = s;
        }

        #endregion

        #region メソッド

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = new EntityStatus_Model(this, entityManager.LordModel("GameModel\\hakakari4"));
            moveStatus = null;
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Box_Bloc(this, new Vector3(position.X - size.X, -50, position.Z - size.Y), new Vector3(position.X + size.X, 0, position.Z + size.Y), HitPriorityGroup.attack));
            hitStatus.Add(new EntityStatus_Hit_PlaneLine_Bloc(this, HitPriorityGroup.terrain,
                new Vector2(position.X - size.X, position.Z - size.Y),
                new Vector2(position.X + size.X, position.Z - size.Y),
                new Vector2(position.X + size.X, position.Z + size.Y),
                new Vector2(position.X - size.X, position.Z + size.Y)));

            base.Lord(entityManager);
        }

        #endregion
    }

    class EntityStatus_Hit_Box_Cage : EntityStatus_Hit_Box
    {
        public EntityStatus_Hit_Box_Cage(Entity entity, Vector3 posMin, Vector3 posMax)
            : base(entity, posMin, posMax, HitPriorityGroup.terrain)
        { }

        protected override bool Hitjudg_Ball(EntityStatus_Hit entity)
        {
            Shape_Ball ball = (Shape_Ball)entity;
            if (!base.HitBall(BoxPointMax, BoxPointMin, entity.thisEntity.position, ball.Radius))
            {
                switch (FaceHitBall(BoxPointMax, BoxPointMin, entity.thisEntity.position, entity.thisEntity.MoveStatus.OldPosition, ball.Radius))
                {
                    case HitFace.Up:
                        entity.thisEntity.position.Y = BoxPointMax.Y - ball.Radius;
                        return true;

                    case HitFace.Doun:
                        entity.thisEntity.position.Y = BoxPointMin.Y + ball.Radius;
                        if (entity.thisEntity.MoveStatus != null)
                        {
                            entity.thisEntity.MoveStatus.moveVec.Y = 0;
                            entity.thisEntity.MoveStatus.landing = true;
                        }
                        return true;

                    case HitFace.Left:
                        entity.thisEntity.position.X = BoxPointMin.X + ball.Radius;
                        entity.thisEntity.MoveStatus.moveVec.X = 0;
                        return true;

                    case HitFace.Right:
                        entity.thisEntity.position.X = BoxPointMax.X - ball.Radius;
                        entity.thisEntity.MoveStatus.moveVec.X = 0;
                        return true;

                    case HitFace.Back:
                        entity.thisEntity.position.Z = BoxPointMin.Z + ball.Radius;
                        entity.thisEntity.MoveStatus.moveVec.Z = 0;
                        return true;

                    case HitFace.flont:
                        entity.thisEntity.position.Z = BoxPointMax.Z - ball.Radius;
                        entity.thisEntity.MoveStatus.moveVec.Z = 0;
                        return true;
                }
            }
            return false;
        }
    }

    class EntityStatus_Model_Shadow : EntityStatus_Model
    {
        Effect shadowEffect;
        RenderTarget2D shadowMap;

        public EntityStatus_Model_Shadow(Entity entity,Model model)
            : base(entity,model)
        {
            shadowEffect = entity.ThisEntityManager.thisGame.Content.Load<Effect>("Effect\\ShadowEffect");
            shadowEffect.Parameters["ShadowMapView"].SetValue(thisEntity.ThisEntityManager.shadowCamera.view);
            shadowEffect.Parameters["ShadowMapProjection"].SetValue(thisEntity.ThisEntityManager.shadowCamera.projection);
            shadowEffect.Parameters["View"].SetValue(thisEntity.ThisEntityManager.mainCamera.view);
            shadowEffect.Parameters["Projection"].SetValue(thisEntity.ThisEntityManager.mainCamera.projection);

            shadowMap = new RenderTarget2D(
                    thisEntity.ThisEntityManager.thisGame.GraphicsDevice,
                    thisEntity.ThisEntityManager.thisGame.graphics.PreferredBackBufferWidth,
                    thisEntity.ThisEntityManager.thisGame.graphics.PreferredBackBufferHeight,
                    true,
                    SurfaceFormat.Single,DepthFormat.None
                );
        }
        public override void Draw(Camera camera, GameTime gameTime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                shadowEffect.Parameters["World"].SetValue(transform[mesh.ParentBone.Index] * world);
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    foreach (EffectTechnique technique in meshPart.Effect.Techniques)
                    {
                        thisEntity.ThisEntityManager.thisGame.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);
                        thisEntity.ThisEntityManager.thisGame.GraphicsDevice.Indices = meshPart.IndexBuffer;

                        shadowEffect.CurrentTechnique.Passes[0].Apply();

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

        private void initShadowMap()
        {
            //thisEntity.ThisEntityManager.thisGame.GraphicsDevice.SetRenderTarget(shadowMap);
            //thisEntity.ThisEntityManager.thisGame.GraphicsDevice.Clear(Color.White);
            
            //foreach (Matrix triangleTransform in triangles)
            //{
            //    shadowMapCreator.Parameters["World"].SetValue(triangleTransform);

            //    shadowMapCreator.Begin();
            //    shadowMapCreator.CurrentTechnique.Passes[0].Begin();

            //    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //        PrimitiveType.TriangleList,
            //        triangleVertices,
            //        0,
            //        1
            //        );

            //    shadowMapCreator.CurrentTechnique.Passes[0].End();
            //    shadowMapCreator.End();
            //}

            //GraphicsDevice.SetRenderTarget(0, null);

        }
    }
}
