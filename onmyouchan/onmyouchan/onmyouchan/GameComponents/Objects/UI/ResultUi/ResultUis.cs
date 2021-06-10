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
    class ResultBack:Ui
    {
        ImageStatus imageState;

        public override void Draw()
        {
            thisUiManager.ThisGame1.spriteBatch.Draw(imageState.Image, imageState.Position, Color.White);
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            imageState = new ImageStatus();
            imageState.Image = uiManager.TextureLord("GameUI\\Result");
        }
    }
}
