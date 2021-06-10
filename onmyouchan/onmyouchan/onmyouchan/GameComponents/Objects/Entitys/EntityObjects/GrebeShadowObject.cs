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
    class GrebeShadowObject:Enemy
    {
        float fieldView;
        bool discovery;

        public GrebeShadowObject(Vector3 pos, GameDate date, ScoreData score)
            : base(pos, date, score)
        {
            fieldView = MathHelper.ToRadians(30);
        }

        protected override void Control(GameTime gameTime)
        {
            float pRota = MathHelper.ToDegrees(MyUtility.OverRadianReset(thisEntityManager.player.Rotation.Y));
            float myRata = MathHelper.ToDegrees(MyUtility.OverRadianReset(rotation.Y));
            float rota = pRota - myRata;
            MyUtility.OverRadianReset(rota);

            Vector3 vec = thisEntityManager.player.position - position;
            float rag = (float)Math.Atan2(vec.Z, vec.X);
            if (Math.Abs(rota) <= fieldView)
            {
                discovery = true;
                goalAngle = rag + (float)Math.PI;
                rotation.Y = goalAngle;
            }
            else
                discovery = false;

            if (discovery)
            {
                moveStatus.moveVec.X += (float)Math.Cos(rag) * moveSpeed;
                moveStatus.moveVec.Z += (float)Math.Sin(rag) * moveSpeed;
            }
        }

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = new EntityStatus_Model_Toon(this, entityManager.LordModel("GameModel\\obakekari1"), "GameModel\\obakeUVkari");
            moveStatus = new EntityStatus_Move(this);
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball_Enemy_Repulsion(this, 50f, 1.0f));
            base.Lord(entityManager);
        }
    }
}
