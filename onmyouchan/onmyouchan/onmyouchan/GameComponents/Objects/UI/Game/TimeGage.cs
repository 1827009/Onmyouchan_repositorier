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

namespace onmyouchan.UI
{
    class TimeGage:Ui
    {
        #region フィールド

        GameDate gameDate;

        /// <summary>
        /// ゲージ枠のデータ
        /// </summary>
        ImageStatus frame;
        /// <summary>
        /// ゲージのデータ
        /// </summary>
        ImageStatus gage;

        #endregion

        #region コンストラクタ

        public TimeGage(GameDate date)
        {
            gameDate = date;
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
            int length = (int)(((float)gameDate.RemainingTime / (float)gameDate.MaxRemainingTime) * gage.Image.Width);

            thisUiManager.ThisGame1.spriteBatch.Draw(frame.Image, frame.Position + Position, Color.White);
            thisUiManager.ThisGame1.spriteBatch.Draw(gage.Image, gage.Position + Position, new Rectangle(0, 0, length, gage.Image.Height), Color.White);
        }

        #endregion

        #region メソッド

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            frame = new ImageStatus();
            gage = new ImageStatus();

            gage.Image = thisUiManager.TextureLord("GameUI\\timeGage");
            frame.Image = thisUiManager.TextureLord("GameUI\\timeGageFrame");

            Position = uiManager.setPosition(new Vector2(0.5f, 0));

            frame.Position = new Vector2(-frame.Image.Width * 0.5f, 0);
            gage.Position = new Vector2(-gage.Image.Width * 0.5f, 0);
        }

        #endregion
    }
}
