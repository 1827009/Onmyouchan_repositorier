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
    class PauseBackUi:Ui
    {
        ImageStatus frame;
        ImageStatus back;

        public SineWaveProduction uiProduction;

        /// <summary>
        /// 巻物のスクロール
        /// </summary>
        public float Turn
        {
            get { return turn; }
            set
            {
                if (turn < 1) turn = (turn + value) % 1;
                else turn = value;
            }
        }
        float turn = 0;

        public override void Update(GameTime time)
        {
            uiProduction.Update();
        }

        public override void Draw()
        {
            Rectangle r = images[0].DrawRectangle();
            r.X += (int)Position.X; r.Y += (int)Position.Y;
            Rectangle scroll = new Rectangle((int)(turn * images[0].Image.Width) / 2, 0, images[0].Image.Width / 2, images[0].Image.Height);
            thisUiManager.ThisGame1.spriteBatch.Draw(images[0].Image, r, scroll, images[0].ImageColor);

            r = images[1].DrawRectangle();
            r.X += (int)Position.X; r.Y += (int)Position.Y;
            thisUiManager.ThisGame1.spriteBatch.Draw(images[1].Image, r, images[1].ImageColor);
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);

            back = new ImageStatus(uiManager.TextureLord("Image\\GameUI\\GameMenu"));
            back.SetImageRectangle(uiManager.setPosition(1, 1));
            images.Add(back);

            frame = new ImageStatus(uiManager.TextureLord("Image\\GameUI\\GameMenuFrame"));
            frame.SetImageRectangle(uiManager.setPosition(0.1f, 1));
            frame.CenterPosition = new Vector2(1, 0);
            images.Add(frame);

            Position = thisUiManager.setPosition(new Vector2(1f, 0));

            uiProduction = new SineWaveProduction(this);
            uiProduction.moveSpeed = 0.3f;
        }
    }
}
