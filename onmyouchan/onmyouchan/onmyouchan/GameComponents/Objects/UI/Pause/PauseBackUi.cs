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

        public UiProduction uiProduction;

        /// <summary>
        /// 巻物のスクロール
        /// </summary>
        public int Turn
        {
            get { return turn; }
            set { if (turn > 0 && turn < back.Image.Width / 2) turn = value; }
        }
        public int turn = 0;

        public override void Draw()
        {
            thisUiManager.ThisGame1.spriteBatch.Draw(back.Image, back.Position + Position, new Rectangle(Turn, 0, Turn + back.Image.Width / 2, back.Image.Height), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            thisUiManager.ThisGame1.spriteBatch.Draw(frame.Image, frame.Position + Position, Color.Wheat);
        }

        public override void Update(GameTime time)
        {
            uiProduction.Update();
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            back = new ImageStatus();
            back.Image = uiManager.TextureLord("GameUI\\GameMenu");

            frame = new ImageStatus();
            frame.Image = uiManager.TextureLord("GameUI\\GameMenuFrame");
            frame.Position = new Vector2(-frame.Image.Width * 0.5f, 0);

            Position = thisUiManager.setPosition(new Vector2(1f, 0));
            uiProduction = new UiProduction(this);
            uiProduction.moveSpeed = 0.3f;
        }
    }
}
