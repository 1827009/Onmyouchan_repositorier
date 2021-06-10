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
        float actionRota;
        public float MoveRange
        {
            get { return sinwWave.RotaSpeed; }
            set { sinwWave.RotaSpeed = value; }
        }
        SineWave sinwWave;

        public GhostObject(Vector3 pos, GameDate date, ScoreData score,float r)
            : base(pos ,date, score)
        {
            sinwWave = new SineWave();
            moveSpeed = float.Parse(ConfigurationManager.AppSettings["GhostMove_AccelerationSpeed"]);
            maxSpeed = float.Parse(ConfigurationManager.AppSettings["GhostMove_MaxSpeed"]);
            actionRota = MathHelper.ToRadians(r);
        }
        
        protected override void Control(GameTime gameTime)
        {
            float speed = moveSpeed * (float)Math.Cos(Math.Abs(sinwWave.Sin(gameTime)));
            if (sinwWave.Point(gameTime) == "Zero")
            {
                //if (rotation.Y > 0)
                    goalAngle = rotation.Y + actionRota;
                //else
                //    goalAngle = rotation.Y - actionRota;
            }
            if (MyUtility.Vector2Size(new Vector2(moveStatus.moveVec.X, moveStatus.moveVec.Z)) < maxSpeed)
            {
                moveStatus.moveVec.X -= (float)Math.Cos(this.rotation.Y) * speed;
                moveStatus.moveVec.Z += (float)Math.Sin(this.rotation.Y) * speed;
            }
        }

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = new EntityStatus_Model_Ghost(this, entityManager.LordModel("GameModel\\obakebone3"));
            moveStatus = new EntityStatus_Move(this);
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball_Enemy_Repulsion(this, 50f, 1.0f));
            base.Lord(entityManager);
        }
    }

    class EntityStatus_Model_Ghost : EntityStatus_Model_ToonMotion
    {
        public EntityStatus_Model_Ghost(Entity entity, Model model)
            : base(entity, model, "GameModel\\obakeUV1", "Take 001")
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
            Vector3 pos = thisEntity.position; pos.Y += 10f;
            world = rotationMatrix * scaleMatrix * Matrix.CreateTranslation(pos);   
        }
    }
}
