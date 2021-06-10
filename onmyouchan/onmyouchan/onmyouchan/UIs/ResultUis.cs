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
    class ResultBack:Ui
    {
        ImageStatus imageState;

        public override void Draw()
        {
            thisUiManager.ThisGame1.spriteBatch.Draw(imageState.image, imageState.position, Color.White);
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            imageState.image = uiManager.TextureLord("GameUI\\Result");
        }
    }
}
