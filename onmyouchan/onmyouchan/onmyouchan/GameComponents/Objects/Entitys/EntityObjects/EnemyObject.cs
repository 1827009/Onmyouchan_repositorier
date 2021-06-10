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
    /// 敵の基礎クラス
    /// </summary>
    class Enemy : Character
    {
        #region フィールド

        GameDate gameDate;
        ScoreData scoreDate;

        /// <summary>
        /// 体力値
        /// </summary>
        public int HitPoint
        { get { return hitPoint; } set { hitPoint = value; } }
        public int hitPoint = 1;

        /// <summary>
        /// 攻撃力
        /// </summary>
        public float Attack
        { get { return attack; } }
        protected float attack = 1;

        #endregion

        #region コンストラクタ

        public Enemy(Vector3 pos,GameDate date,ScoreData score)
            : base(EntityType.enemy, pos)
        {
            gameDate = date;
            scoreDate = score;
        }

        #endregion

        #region 更新

        /// <summary>
        /// baseで実行
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (hitPoint <= 0)
                UnloadFlag = true;
            base.Update(gameTime);
        }

        #endregion

        #region メソッド

        public override void Unload()
        {
            gameDate.KillCount++;
            base.Unload();
        }

        /// <summary>
        /// 攻撃判定を受けたときのイベント
        /// </summary>
        public override void TakenDamage()
        {
            HitPoint--;
        }

        public override void Lord(EntityManager entityManager)
        {
            base.Lord(entityManager);
        }

        #endregion
    }

#region EntityStatus

    /// <summary>
    /// Entity内のEntityStatus_Hitに代入すると円形のあたり判定を付与します
    /// 当たった時の反応は両者反発です
    /// </summary>
    class EntityStatus_Hit_Ball_Enemy_Repulsion : EntityStatus_Hit_Ball
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
        public EntityStatus_Hit_Ball_Enemy_Repulsion(Entity entity, float r)
            : base(entity, r,HitPriorityGroup.enemy)
        {
        }

        /// <summary>
        /// ステータスを付与したいEntityの参照と半径の大きさと抵抗値を入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="a"></param>
        public EntityStatus_Hit_Ball_Enemy_Repulsion(Entity entity, float r, float reg)
            : base(entity, r,HitPriorityGroup.enemy)
        {
            resilience = reg;
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
                if (entity.thisEntity is Player)
                {
                    try
                    {
                        Enemy enemy = (Enemy)thisEntity;
                        thisEntity.ThisEntityManager.player.damage += enemy.Attack;
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("enemy専用のステータスが別のオブジェクトに反映されています");
                    }
                }

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

#endregion
}
