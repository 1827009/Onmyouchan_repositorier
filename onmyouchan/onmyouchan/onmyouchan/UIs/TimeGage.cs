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

namespace onmyouchan
{
    class TimeGage:Ui
    {
        #region フィールド

        OnmyouchanSystem thisSystem;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector2 Position
        {
            get { return frame.position; }
            set
            {
                frame.position = value;
                gage.position = value;
            }
        }
        /// <summary>
        /// ゲージ枠のデータ
        /// </summary>
        ImageStatus frame;
        /// <summary>
        /// ゲージのデータ
        /// </summary>
        ImageStatus gage;

        /// <summary>
        /// ゲージの長さピクセル
        /// </summary>
        const int timeGageLength = 352;

        #endregion

        #region コンストラクタ

        public TimeGage(OnmyouchanSystem system)
        {
            thisSystem = system;
        }

        #endregion

        #region 更新

        public override void Update(GameTime time)
        {
        }

        #endregion

        #region 描画

        public override void Draw()
        {
            int length = (int)(((float)thisSystem.RemainingTime / (float)thisSystem.MaxRemainingTime) * timeGageLength);

            thisUiManager.ThisGame1.spriteBatch.Draw(frame.image, frame.position, Color.White);
            thisUiManager.ThisGame1.spriteBatch.Draw(gage.image, gage.position, new Rectangle(0, 0, length, 45), Color.White);
        }

        #endregion

        #region メソッド

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);

            frame.image = thisUiManager.ThisGame1.Content.Load<Texture2D>("GameUI\\timeGageFrame");
            gage.image = thisUiManager.ThisGame1.Content.Load<Texture2D>("GameUI\\timeGage");

            frame.position.X = thisUiManager.ThisGame1.graphics.PreferredBackBufferWidth / 2 - 314;
            gage.position.X = thisUiManager.ThisGame1.graphics.PreferredBackBufferWidth / 2 - 192;
            gage.position.Y = 35;
        }

        #endregion
    }
}
