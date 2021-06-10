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
    //class GrebeShadowObject:Enemy
    //{
    //    float fieldView;
    //    bool discovery;

    //    public GrebeShadowObject(Vector3 pos, GameDate date, ScoreData score)
    //        : base(pos, date, score)
    //    {
    //        fieldView = MathHelper.ToRadians(30);
    //    }

    //    protected override void Control(GameTime gameTime)
    //    {
    //        float pRota = MathHelper.ToDegrees(MyUtility.OverRadianReset(thisEntityManager.player.Rotation.Y));
    //        float myRata = MathHelper.ToDegrees(MyUtility.OverRadianReset(rotation.Y));
    //        float rota = pRota - myRata;
    //        MyUtility.OverRadianReset(rota);

    //        Vector3 vec = thisEntityManager.player.position - position;
    //        float rag = (float)Math.Atan2(vec.Z, vec.X);
    //        if (Math.Abs(rota) <= fieldView)
    //        {
    //            discovery = true;
    //            goalAngle = rag + (float)Math.PI;
    //            rotation.Y = goalAngle;
    //        }
    //        else
    //            discovery = false;

    //        if (discovery)
    //        {
    //            moveStatus.moveVec.X += (float)Math.Cos(rag) * moveSpeed;
    //            moveStatus.moveVec.Z += (float)Math.Sin(rag) * moveSpeed;
    //        }
    //    }

    //    public override void Lord(EntityManager entityManager)
    //    {
    //        thisEntityManager = entityManager;
    //        modelStatus = new EntityStatus_Model(this, entityManager.LordModel("GameModel\\EnemyHakaKari1"));
    //        moveStatus = new EntityStatus_Move(this);
    //        hitStatus = new List<EntityStatus_Hit>();
    //        hitStatus.Add(new EntityStatus_Hit_Ball_Enemy_Repulsion(this, 100f, 1.0f));
    //        base.Lord(entityManager);
    //    }
    //}

    class GrebeShadowObject : Enemy
    {
        public bool search;

        public GrebeShadowObject(Vector3 pos, GameDate date, ScoreData score)
            : base(pos, date, score)
        {
            moveSpeed = 1.5f;
            maxSpeed = 5;
        }
        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = new EntityStatus_Model_GrebeShadow(this, entityManager);
            moveStatus = new EntityStatus_Move(this, true);
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball_Enemy_Repulsion(this, 50,1));
            hitStatus.Add(new EntityStatus_Hit_Search(this));

            base.Lord(entityManager);
        }

        protected override void Control(GameTime gameTime)
        {
            if (search)
            {
                Vector3 vec = thisEntityManager.player.position - position;
                goalAngle = (float)Math.Atan2(vec.Z, -vec.X);

                if (MyUtility.Vector2Size(new Vector2(moveStatus.moveVec.X, moveStatus.moveVec.Z)) < maxSpeed)
                    moveStatus.moveVec -= new Vector3((float)Math.Cos(rotation.Y), 0, -(float)Math.Sin(rotation.Y)) * moveSpeed;

                base.Control(gameTime);
            }
        }

        class EntityStatus_Hit_Search : EntityStatus_Hit_Ball
        {
            GrebeShadowObject grebeShadow;
            public EntityStatus_Hit_Search(GrebeShadowObject entity)
                : base(entity, float.Parse(ConfigurationManager.AppSettings["GrebeShadowObject_search"]), HitPriorityGroup.attack)
            {
                float a = float.Parse(ConfigurationManager.AppSettings["GrebeShadowObject_search"]);
                grebeShadow = entity;
            }

            public override void HitJudgShapeSelect(EntityStatus_Hit entity)
            {
                if (entity.thisEntity is Player && entity is Shape_Ball)
                    Hitjudg_Player(entity);
                else
                    base.HitJudgShapeSelect(entity);
            }

            protected bool Hitjudg_Player(EntityStatus_Hit entity)
            {
                Shape_Ball ball = (Shape_Ball)entity;
                if (BallHit(thisEntity.position, radius, entity.thisEntity.position, ball.Radius))
                {
                    grebeShadow.search = true;
                    return true;
                }
                else
                {
                    grebeShadow.search = false;
                    return false;
                }
            }
        }

        class EntityStatus_Model_GrebeShadow:EntityStatus_Model_ToonMotion
        {
            public EntityStatus_Model_GrebeShadow(Entity entity, EntityManager entityManager)
                : base(entity, entityManager.LordModel("GameModel\\EnemyHaka"), "GameModel\\enemyhakaUV", "move")
            { }
            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
                //回転行列
                Matrix rotationX = Matrix.CreateRotationX(thisEntity.Rotation.X);
                Matrix rotationY = Matrix.CreateRotationY(thisEntity.Rotation.Y - (float)Math.PI/2);
                Matrix rotationZ = Matrix.CreateRotationZ(thisEntity.Rotation.Z);
                Matrix rotationMatrix = rotationX * rotationY * rotationZ;
                //大きさ
                Matrix scaleMatrix = Matrix.CreateScale(thisEntity.Scale);
                //最終的な行列
                Vector3 pos = thisEntity.position; pos.Y -= 20f;
                world = rotationMatrix * scaleMatrix * Matrix.CreateTranslation(pos);
            }
        }
    }
}