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

using onmyouchan.Entity;

namespace onmyouchan.UI
{
    class OhudaIcon : Ui
    {
        #region フィールド

        /// <summary>
        /// 所属しているplayer
        /// </summary>
        Player player;

        int ohudaNumber = 3;

        ImageStatus[] ohuda;
        ImageStatus[] frame;

        #endregion

        #region コンストラクタ

        public OhudaIcon(Player p)
        {
            player = p;
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
            for (int i = 0; i < ohudaNumber; i++)
            {
                thisUiManager.ThisGame1.spriteBatch.Draw(frame[i].Image, frame[i].Position+Position, Color.White);
                if (player.ohudaStock >= i + 1)
                    thisUiManager.ThisGame1.spriteBatch.Draw(ohuda[i].Image, ohuda[i].Position+Position, new Rectangle(0, 0, 64, 64), Color.White);
            }
        }

        #endregion

        #region メソッド

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            
            ohuda = new ImageStatus[ohudaNumber];
            for (int i = 0; i < ohudaNumber; i++)
                ohuda[i] = new ImageStatus();
            frame = new ImageStatus[ohudaNumber];
            for (int i = 0; i < ohudaNumber; i++)
                frame[i] = new ImageStatus();

            Position = new Vector2(thisUiManager.ThisGame1.graphics.PreferredBackBufferWidth / 2, thisUiManager.ThisGame1.graphics.PreferredBackBufferHeight);
            for (int i = 0; i < ohudaNumber; i++)
            {
                ohuda[i].Image = thisUiManager.TextureLord("GameUI\\OhudaIcon");
                frame[i].Image = thisUiManager.TextureLord("GameUI\\OhudaIconFrame");

                ohuda[i].Position = new Vector2((i - ohuda.Length * 0.5f) * ohuda[i].Image.Width, -ohuda[i].Image.Height);
                frame[i].Position = ohuda[i].Position;
            }
        }

        #endregion
    }
}
