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
    /// カメラのオブジェクト
    /// </summary>
    class Camera : Entity
    {
        #region フィールド

        /// <summary>
        /// 注視点
        /// </summary>
        public Vector3 TargetPosition
        {
            get { return targetPosition; }
            set { targetPosition = value; }
        }
        protected Vector3 targetPosition;

        /// <summary>
        /// 注視点からの距離
        /// </summary>
        protected Vector3 distance;

        protected Matrix rotationMatrix;
        public Matrix view;
        public Matrix projection;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// GraphicsDeviceを入れてください
        /// </summary>
        /// <param name="graphics"></param>
        public Camera()
            : base(EntityType.camera)
        {
            rotation.X = MathHelper.ToRadians(-35);
        }

        #endregion

        #region 初期化

        public virtual void Initialize()
        {
            position = Vector3.Zero;
            distance.Z = 1000f;
            distance.Y = 100;

            rotation.Y = MathHelper.ToRadians(45);

            float fieldOfView;
            float aspectRatio;
            float nearPlaneDistance;
            float farPlaneDistance;
            //視野
            fieldOfView = MathHelper.ToRadians(45.0f);
            //縦横比
            aspectRatio = (float)thisEntityManager.thisGame.GraphicsDevice.Viewport.Width / (float)thisEntityManager.thisGame.GraphicsDevice.Viewport.Height;
            //MIN描写距離
            nearPlaneDistance = 1.0f;
            //MAX描写距離
            farPlaneDistance = 50000.0f;

            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        #endregion

        #region アップデート

        public override void Update(GameTime gameTime)
        {
            view = Matrix.CreateLookAt(position, targetPosition, rotationMatrix.Up);
            SetPosition();
        }
        /// <summary>
        /// カメラの位置を設定する
        /// </summary>
        private void SetPosition()
        {
            //回転を行列化
            rotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
            position = targetPosition + Vector3.Transform(distance, rotationMatrix);
        }

        #endregion

        #region メソッド

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;

            Initialize();
            base.Lord(entityManager);
        }

        #endregion
    }
    /// <summary>
    /// 操作を含めたカメラ
    /// </summary>
    class MainCamera : Camera
    {
        #region フィールド

        /// <summary>
        /// 所属しているEntity
        /// </summary>
        public Entity BelongsEntity
        {
            set { belongsEntity = value; }
        }
        Entity belongsEntity;
        
        /// <summary>
        /// 回転速度
        /// </summary>
        float rotationSpead;

        #endregion

        #region 初期化

        public override void Initialize()
        {
            position = Vector3.Zero;
            //rotationSpead = Option.CameraOption_Speed;
            distance.Z = 1000f;
            distance.Y = 100;

            rotation.Y = MathHelper.ToRadians(45);

            float fieldOfView;
            float aspectRatio;
            float nearPlaneDistance;
            float farPlaneDistance;
            //視野
            fieldOfView = MathHelper.ToRadians(45.0f);
            //縦横比
            aspectRatio = (float)thisEntityManager.thisGame.GraphicsDevice.Viewport.Width / (float)thisEntityManager.thisGame.GraphicsDevice.Viewport.Height;
            //MIN描写距離
            nearPlaneDistance = 1.0f;
            //MAX描写距離
            farPlaneDistance = 50000.0f;

            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        #endregion

        #region アップデート

        public override void Update(GameTime gameTime)
        {
            view = Matrix.CreateLookAt(position, targetPosition, rotationMatrix.Up);
            Control();
            SetPosition();
        }
        /// <summary>
        /// カメラの位置を設定する
        /// </summary>
        private void SetPosition()
        {
            //回転を行列化
            rotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);

            if (belongsEntity != null)
            {
                Vector3 pos = belongsEntity.position;

                position = pos + Vector3.Transform(distance, rotationMatrix);
                targetPosition = belongsEntity.MoveStatus.OldPosition;
                targetPosition.Y += 100;
            }
            else
                position = targetPosition + Vector3.Transform(distance, rotationMatrix);
        }

        #endregion

        #region 操作

        private void Control()
        {
            if (ControllManager.CameraKey(false) != Vector2.Zero)
            {
                //距離調整

                distance.Z -= ControllManager.ZoomKey() * 10;

                //下限値、上限値
                if (distance.Z > 5000)
                    distance.Z = 5000;
                else if (distance.Z < 10)
                    distance.Z = 10;

                //回転
                if (Option.CameraOption_HorizontalReverse)
                    rotation.Y += ControllManager.CameraKey(false).X * (Option.CameraOption_Speed * 0.007f);
                else
                    rotation.Y -= ControllManager.CameraKey(false).X * (Option.CameraOption_Speed * 0.007f);
                if (Option.CameraOption_VerticalReverse)
                    rotation.X += ControllManager.CameraKey(false).Y * (Option.CameraOption_Speed * 0.007f);
                else
                    rotation.X -= ControllManager.CameraKey(false).Y * (Option.CameraOption_Speed * 0.007f);

                if (rotation.X > Math.PI / 2)
                    rotation.X = (float)Math.PI / 2;
                else if (rotation.X < -Math.PI / 2)
                    rotation.X = (float)-Math.PI / 2;
                rotation.Y %= (float)Math.PI * 2;
                rotation.X %= (float)Math.PI * 2;
            }
        }

        #endregion

        #region メソッド

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = null;
            moveStatus = new EntityStatus_Move_Camera(this);
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new Entity_EntityStatus_Hit_Camera(this));

            Initialize();
            base.Lord(entityManager);
        }

        #endregion
    }

    #region Status
    /// <summary>
    /// カメラの移動Status
    /// </summary>
    class EntityStatus_Move_Camera : EntityStatus_Move
    {
        Camera thisCamera;
        public EntityStatus_Move_Camera(Camera camera)
            : base(camera, false)
        {
            thisCamera = camera;
        }

        public override void Update(GameTime gameTime)
        {
            oldPosition = thisCamera.TargetPosition;
            Physical();
        }
    }
    /// <summary>
    /// カメラのあたり判定Status
    /// </summary>
    class Entity_EntityStatus_Hit_Camera : EntityStatus_Hit_Ball
    {
        public Entity_EntityStatus_Hit_Camera(Camera camera):base(camera,30,HitPriorityGroup.camera)
        {
        }

        protected override bool Hitjudg_Box(EntityStatus_Hit entity)
        {
            Shape_Box box = (Shape_Box)entity;

            if (thisEntity.position.Y - radius<box.BoxPointMax.Y)
            {
                thisEntity.position.Y = box.BoxPointMax.Y + radius;
                return true;
            }
            return false;
        }
    }

#endregion
}
