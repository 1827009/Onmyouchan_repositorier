#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace onmyouchan
{
    /// <summary>
    /// 細かい入力を制御するクラスです。
    /// </summary>
    public class ControllManager
    {
        #region フィールド
        private static Keys decideKey = Keys.Enter;
        private static Keys cancelKey = Keys.Space;
        private static Keys move_upKey = Keys.W;
        private static Keys move_downKey = Keys.S;
        private static Keys move_leftKey = Keys.A;
        private static Keys move_rightKey = Keys.D;
        private static Keys camera_upKey = Keys.Up;
        private static Keys camera_downKey = Keys.Down;
        private static Keys camera_leftKey = Keys.Left;
        private static Keys camera_rightKey = Keys.Right;

        private static Keys zoomInKey = Keys.I;
        private static Keys zoomOutKey = Keys.O;

        private static Buttons decideButton = Buttons.B;
        private static Buttons cancelButton = Buttons.A;

        private static Buttons zoomInButton = Buttons.DPadUp;
        private static Buttons zoomOutButton = Buttons.DPadDown;

        //PADの移動キー、カメラキーについては各場所を変更してください。
        
        #endregion

        #region キー

        /// <summary>
        /// 決定キーが押されたか？
        /// </summary>
        public static bool KeyDecide()
        {
            bool key = false;

            if (InputManager.IsJustKeyDown(decideKey))
                key = true;

            if (InputManager.IsConnected(PlayerIndex.One) && InputManager.IsJustButtonDown(PlayerIndex.One, decideButton))
                key = true;

            return key;
        }

        /// <summary>
        /// キャンセルキーが押されたか？
        /// </summary>
        public static bool KeyCancel()
        {
            bool key = false;

            if (InputManager.IsJustKeyDown(cancelKey))
                key = true;

            if (InputManager.IsConnected(PlayerIndex.One) && InputManager.IsJustButtonDown(PlayerIndex.One, cancelButton))
                key = true;

            return key;
        }

        /// <summary>
        /// スタートボタンが押されたか？
        /// </summary>
        /// <returns></returns>
        public static bool KeyStart()
        {
            if (InputManager.IsJustButtonDown(PlayerIndex.One,Buttons.Start)||InputManager.IsJustKeyDown(Keys.Escape))
                return true;
            return false;
        }

        public static bool LeftTrigger()
        {
            if (InputManager.IsJustLeftTriggerDown(PlayerIndex.One, 0.3f) || InputManager.IsJustKeyDown(Keys.Q))
                return true;
            return false;
        }
        public static bool RightTrigger()
        {
            if (InputManager.IsJustRightTriggerDown(PlayerIndex.One, 0.3f) || InputManager.IsJustKeyDown(Keys.E))
                return true;
            return false;
        }

        static bool oldLeftTrigger;
        public static bool JustLeftTrigger()
        {
            
            if ((InputManager.IsJustLeftTriggerDown(PlayerIndex.One, 0.3f)&&!oldLeftTrigger) || InputManager.IsJustKeyDown(Keys.Q))
            {
                oldLeftTrigger = true;
                return true;
            }
            oldLeftTrigger = false;
            return false;
        }
        static bool oldRightTrigger;
        public static bool JustRightTrigger()
        {
            if ((InputManager.IsJustRightTriggerDown(PlayerIndex.One, 0.3f) && !oldRightTrigger) || InputManager.IsJustKeyDown(Keys.E))
            {
                oldRightTrigger = true;
                return true;
            }
            oldRightTrigger = false;
            return false;
        }

        #endregion

        #region 移動キー

        /// <summary>
        /// 移動キーが押されているか？(return Vector2D)
        /// </summary>
        public static Vector2 MoveKey()
        {
            Vector2 crossKey = Vector2.Zero;

            if (InputManager.IsKeyDown(move_leftKey))
                crossKey.X -= 1.0f;
            if (InputManager.IsKeyDown(move_rightKey))
                crossKey.X += 1.0f;
            if (InputManager.IsKeyDown(move_upKey))
                crossKey.Y += 1.0f;
            if (InputManager.IsKeyDown(move_downKey))
                crossKey.Y -= 1.0f;

            if (InputManager.IsConnected(PlayerIndex.One))
            {
                crossKey.X += InputManager.GetThumbSticksLeft(PlayerIndex.One).X;
                crossKey.Y += InputManager.GetThumbSticksLeft(PlayerIndex.One).Y;
            }

            return crossKey;
        }

        /// <summary>
        /// 移動キー(左)が押されているか？
        /// </summary>
        public static bool Move_LeftKey()
        {
            bool key = false;

            if (MoveKey().X < 0)
                key = true;

            return key;
        }

        /// <summary>
        /// 移動キー(右)が押されているか？
        /// </summary>
        public static bool Move_RightKey()
        {
            bool key = false;

            if (MoveKey().X > 0)
                key = true;

            return key;
        }

        /// <summary>
        /// 移動キー(上)が押されているか？
        /// </summary>
        public static bool Move_UpKey()
        {
            bool key = false;

            if (MoveKey().Y < 0)
                key = true;

            return key;
        }

        /// <summary>
        /// 移動キー(下が押されているか？)
        /// </summary>
        public static bool Move_DownKey()
        {
            bool key = false;

            if (MoveKey().Y > 0)
                key = true;

            return key;
        }

        #endregion

        #region 十字、カメラキー

        private static float longPush = 0;
        /// <summary>
        /// 選択の上下左右出力
        /// </summary>
        /// <returns></returns>
        public static Vector2 SelectKey()
        {
            Vector2 output = Vector2.Zero;

            if (InputManager.GetThumbSticksRight(PlayerIndex.One).X > 0.3f)
            {
                output.X = 1;
            }
            else if (InputManager.GetThumbSticksRight(PlayerIndex.One).X < -0.3f)
            {
                output.X = -1;
            }
            else if (InputManager.GetThumbSticksLeft(PlayerIndex.One).X > 0.3f)
            {
                output.X = 1;
            }
            else if (InputManager.GetThumbSticksLeft(PlayerIndex.One).X < -0.3f)
            {
                output.X = -1;
            }
            else if (InputManager.IsButtonDown(PlayerIndex.One, Buttons.DPadRight))
                output.X = 1;
            else if (InputManager.IsButtonDown(PlayerIndex.One, Buttons.DPadLeft))
                output.X = -1;
            else if (InputManager.IsKeyDown(Keys.Right))
                output.X = 1;
            else if (InputManager.IsKeyDown(Keys.Left))
                output.X = -1;

            if (InputManager.GetThumbSticksRight(PlayerIndex.One).Y > 0.3f)
            {
                output.Y = -1;
            }
            else if (InputManager.GetThumbSticksRight(PlayerIndex.One).Y < -0.3f)
            {
                output.Y = 1;
            }
            else if (InputManager.GetThumbSticksLeft(PlayerIndex.One).Y > 0.3f)
            {
                output.Y = -1;
            }
            else if (InputManager.GetThumbSticksLeft(PlayerIndex.One).Y < -0.3f)
            {
                output.Y = 1;
            }
            else if (InputManager.IsButtonDown(PlayerIndex.One, Buttons.DPadUp))
                output.Y = -1;
            else if (InputManager.IsButtonDown(PlayerIndex.One, Buttons.DPadDown))
                output.Y = 1;
            else if (InputManager.IsKeyDown(Keys.Up))
                output.Y = -1;
            else if (InputManager.IsKeyDown(Keys.Down))
                output.Y = 1;

            if (output != Vector2.Zero)
                longPush++;
            else
                longPush = 0;

            if (longPush == 1 || longPush > 20)
                return output;

            return Vector2.Zero;
        }

        /// <summary>
        /// 十字、カメラキーが
        /// (true : 押されたか？、false : 押されているか？)(return Vector2D)
        /// </summary>
        public static Vector2 CameraKey(bool just)
        {
            Vector2 crossKey = Vector2.Zero;

            if (just)
            {
                if (InputManager.IsJustKeyDown(camera_leftKey))
                    crossKey.X -= 1.0f;
                if (InputManager.IsJustKeyDown(camera_rightKey))
                    crossKey.X += 1.0f;
                if (InputManager.IsJustKeyDown(camera_upKey))
                    crossKey.Y -= 1.0f;
                if (InputManager.IsJustKeyDown(camera_downKey))
                    crossKey.Y += 1.0f;

                if (InputManager.IsConnected(PlayerIndex.One))
                {
                    if(!InputManager.GetRightStickOld(PlayerIndex.One))
                    {
                            crossKey.X += InputManager.GetThumbSticksRight(PlayerIndex.One).X;
                            crossKey.Y -= InputManager.GetThumbSticksRight(PlayerIndex.One).Y;
                    }
                }
            }
            else
            {
                if (InputManager.IsKeyDown(camera_leftKey))
                    crossKey.X -= 1.0f;
                if (InputManager.IsKeyDown(camera_rightKey))
                    crossKey.X += 1.0f;
                if (InputManager.IsKeyDown(camera_upKey))
                    crossKey.Y -= 1.0f;
                if (InputManager.IsKeyDown(camera_downKey))
                    crossKey.Y += 1.0f;

                if (InputManager.IsConnected(PlayerIndex.One))
                {
                    crossKey.X += InputManager.GetThumbSticksRight(PlayerIndex.One).X;
                    crossKey.Y -= InputManager.GetThumbSticksRight(PlayerIndex.One).Y;
                }
            }

            return crossKey;
        }

        /// <summary>
        /// 十字、カメラキー(左)が
        /// (true : 押されたか？、false : 押されているか？)
        /// </summary>
        public static bool Camera_LeftKey(bool just)
        {
            bool key = false;

            if (CameraKey(just).X < 0)
                key = true;

            return key;
        }

        /// <summary>
        /// 十字、カメラキー(右)が
        /// (true : 押されたか？、false : 押されているか？)
        /// </summary>
        public static bool Camera_RightKey(bool just)
        {
            bool key = false;

            if (CameraKey(just).X > 0)
                key = true;

            return key;
        }

        /// <summary>
        /// 十字、カメラキー(上)が
        /// (true : 押されたか？、false : 押されているか？)
        /// </summary>
        public static bool Camera_UpKey(bool just)
        {
            bool key = false;

            if (CameraKey(just).Y < 0)
                key = true;

            return key;
        }

        /// <summary>
        /// 十字、カメラキー(下)が
        /// (true : 押されたか？、false : 押されているか？)
        /// </summary>
        public static bool Camera_DownKey(bool just)
        {
            bool key = false;

            if (CameraKey(just).Y > 0)
                key = true;

            return key;
        }

        /// <summary>
        /// ズームキーが押されているか？
        /// </summary>
        public static float ZoomKey()
        { 
            float key = 0f;
            
            if (InputManager.IsKeyDown(zoomInKey) || InputManager.IsButtonDown(PlayerIndex.One, zoomInButton))
                key += 1.0f;

            if (InputManager.IsKeyDown(zoomOutKey) || InputManager.IsButtonDown(PlayerIndex.One, zoomOutButton))
                key -= 1.0f;


            return key;
        }


        #endregion
    }
}