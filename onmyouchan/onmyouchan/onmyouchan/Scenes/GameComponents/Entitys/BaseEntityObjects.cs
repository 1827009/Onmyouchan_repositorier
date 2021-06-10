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
        protected float controlWeit;

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
                controlWeit -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                Control(gameTime);
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
        public float goalAngle = 0;
        /// <summary>
        /// 自然な角度変更
        /// </summary>
        private void NatureRotation()
        {
            if (goalAngle != rotation.Y)
            {
                //回転量が近ければそこに向く
                if (Math.Abs(rotation.Y - goalAngle) <= RotationSpeed)
                {
                    rotation.Y = goalAngle;
                }
                else
                {
                    rotation.Y = MyUtility.OverRadianReset(rotation.Y);
                    goalAngle = MyUtility.OverRadianReset(goalAngle);

                    if (rotation.Y > 0)
                    {
                        if (rotation.Y > goalAngle && rotation.Y - (float)Math.PI < goalAngle)
                            rotation.Y -= RotationSpeed;
                        else
                            rotation.Y += RotationSpeed;
                    }
                    else
                    {
                        if (rotation.Y < goalAngle && rotation.Y + (float)Math.PI > goalAngle)
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

        protected virtual void Control(GameTime gameTime)
        { }

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
            thisEntityManager = entityManager;

            modelStatus = new EntityStatus_Model_Toon(this, entityManager.LordModel("Sample\\O_Cube1"), "Sample\\O_Cube1_tx");
            base.Lord(entityManager);
        }
    }
}
