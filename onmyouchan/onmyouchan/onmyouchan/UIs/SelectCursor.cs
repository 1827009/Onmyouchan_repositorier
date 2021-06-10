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
    class SelectCursor:Ui
    {
        ImageStatus image;

        public SineWaveProduction uiProduction;

        public SelectCursor()
        {
            uiProduction = new SineWaveProduction(this);
        }

        public void Initialize()
        {
            uiProduction.Initialize();
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            image = new ImageStatus(thisUiManager.TextureLord("Image\\GameUI\\Cursor"));
        }

        public override void Draw()
        {
            thisUiManager.ThisGame1.spriteBatch.Draw(image.Image, Position, Color.White);
        }


        public override void Update(GameTime time)
        {
            uiProduction.Update();
        }

    }
}
