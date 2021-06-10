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
    class WispObject:Enemy
    {
        bool getaway;

        bool cool;
        float stamina;
        float coolTime;


        public WispObject(Vector3 pos, GameDate date, ScoreData score)
            : base(pos, date, score)
        {
            moveSpeed = float.Parse(ConfigurationManager.AppSettings["WispMove_AccelerationSpeed"]);
            maxSpeed = float.Parse(ConfigurationManager.AppSettings["WispMove_MaxSpeed"]);
            coolTime = 1;

            attack = 100;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
         
        protected override void Control(GameTime gameTime)
        {
            if (MyUtility.Vector2Size(new Vector2(moveStatus.moveVec.X, moveStatus.moveVec.Z)) < maxSpeed)
            {
                if (cool)
                {
                    stamina += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (stamina >= coolTime)
                    {
                        cool = false;
                        Vector3 vec = thisEntityManager.player.position - position;

                        if (getaway)
                            rotation.Y = (float)Math.Atan2(vec.Z, vec.X) + (float)Math.PI;
                        else
                            rotation.Y = (float)Math.Atan2(vec.Z, vec.X);
                        goalAngle = rotation.Y;
                    }
                }
                else
                {
                    stamina -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    moveStatus.moveVec += new Vector3((float)Math.Cos(rotation.Y), 0, (float)Math.Sin(rotation.Y)) * moveSpeed;

                    if (stamina <= 0)
                        cool = true;
                }
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
}
