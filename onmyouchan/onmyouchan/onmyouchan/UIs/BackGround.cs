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
    class BackGround : Ui
    {
        public BackGround(Texture2D tex)
        {
            images.Add(new ImageStatus(tex));
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            images[0].SetImageRectangle(thisUiManager.setPosition(1,1));
        }
    }
}
