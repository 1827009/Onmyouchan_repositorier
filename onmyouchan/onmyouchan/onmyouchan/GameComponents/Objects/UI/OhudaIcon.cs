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
    class OhudaIcon : Ui
    {
        #region フィールド

        int ohudaNumber = 3;

        Texture2D ohudaImage;
        Texture2D ohudaFrameImage;

        ImageStatus[] ohuda;
        ImageStatus[] frame;

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
                thisUiManager.ThisGame1.spriteBatch.Draw(frame[i].image, frame[i].position, Color.White);
                thisUiManager.ThisGame1.spriteBatch.Draw(ohuda[i].image, ohuda[i].position, new Rectangle(0, 0, 64, 64), Color.White);
            }
        }

        #endregion

        #region メソッド

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);

            ohudaImage = thisUiManager.ThisGame1.Content.Load<Texture2D>("GameUI\\OhudaIcon");
            ohudaFrameImage = thisUiManager.ThisGame1.Content.Load<Texture2D>("GameUI\\OhudaIconFrame");

            ohuda = new ImageStatus[ohudaNumber];
            frame = new ImageStatus[ohudaNumber];

            for (int i = 0; i < ohudaNumber; i++)
            {
                ohuda[i].image = ohudaImage;
                frame[i].image = ohudaFrameImage;

                ohuda[i].position.Y = thisUiManager.ThisGame1.graphics.PreferredBackBufferHeight - 64;
                frame[i].position.Y = ohuda[i].position.Y;

                ohuda[i].position.X = (thisUiManager.ThisGame1.graphics.PreferredBackBufferWidth / 2 - 32 * ohudaNumber) + i * 64;
                frame[i].position.X = ohuda[i].position.X;
            }
        }

        #endregion
    }
}
