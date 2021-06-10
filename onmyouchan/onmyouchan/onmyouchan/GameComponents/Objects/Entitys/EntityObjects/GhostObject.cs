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
using System.Configuration;

namespace onmyouchan.Entity
{
    class GhostObject:Enemy
    {
        public GhostObject(Vector3 pos, GameDate date, ScoreData score)
            : base(pos ,date, score)
        {
            moveSpeed = float.Parse(ConfigurationManager.AppSettings["GhostMove_AccelerationSpeed"]);
            maxSpeed = float.Parse(ConfigurationManager.AppSettings["GhostMove_MaxSpeed"]);
        }

        public override void Update(GameTime gameTime)
        {
            rotation.Y -= (float)MathHelper.ToRadians(0.6f);
            goalAngle = rotation.Y;

            base.Update(gameTime);
        }

        protected override void Control(GameTime gameTime)
        {
            if (MyUtility.Vector2Size(new Vector2(moveStatus.moveVec.X, moveStatus.moveVec.Z)) < maxSpeed)
            {
                moveStatus.moveVec.X -= (float)Math.Cos(this.rotation.Y) * moveSpeed;
                //moveStatus.moveVec.Z += (float)Math.Sin(this.rotation.Y) * moveSpeed;
            }
        }

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = new EntityStatus_Model_Ghost(this, entityManager.LordModel("GameModel\\obake\\obakebone3"));
            moveStatus = new EntityStatus_Move(this);
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball_Enemy_Repulsion(this, 50f, 1.0f));
            base.Lord(entityManager);
        }
    }

    class EntityStatus_Model_Ghost : EntityStatus_Model_ToonMotion
    {
        public EntityStatus_Model_Ghost(Entity entity, Model model)
            : base(entity, model, "GameModel\\obakeUVkari","Take 001")
        { }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //回転行列
            Matrix rotationX = Matrix.CreateRotationX(thisEntity.Rotation.X);
            Matrix rotationY = Matrix.CreateRotationY(thisEntity.Rotation.Y);
            Matrix rotationZ = Matrix.CreateRotationZ(thisEntity.Rotation.Z);
            Matrix rotationMatrix = rotationX * rotationY * rotationZ;
            //大きさ
            Matrix scaleMatrix = Matrix.CreateScale(thisEntity.Scale);
            //最終的な行列
            Vector3 pos = thisEntity.position; pos.Y += 50f;
            world = rotationMatrix * scaleMatrix * Matrix.CreateTranslation(pos);   
        }
    }
}
