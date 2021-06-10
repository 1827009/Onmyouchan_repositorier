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
    class ItemUi:Ui
    {
        Player player;
        ImageStatus[] image;
        ImageStatus[] image2;

        public ItemUi(Player p)
            : base()
        {
            player = p;
            image = new ImageStatus[2];
            image[0] = new ImageStatus();
            image[1] = new ImageStatus();
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            Position = uiManager.setPosition(new Vector2(0, 1));
            image[0].Image = uiManager.TextureLord("GameUi\\item flame");
            image[1].Image = uiManager.TextureLord("GameUi\\Ghost gather");
            image[0].Position = new Vector2(0, -image[0].Image.Height);
            image[1].Position = new Vector2(0, -image[1].Image.Height);
        }

        public override void Draw()
        {
            thisUiManager.ThisGame1.spriteBatch.Draw(image[0].Image, image[0].Position + Position, Color.White);
            if (player.Item1 > 0)
                thisUiManager.ThisGame1.spriteBatch.Draw(image[1].Image, image[1].Position + Position, Color.White);
        }
    }
    class Item2Ui : Ui
    {
        Player player;
        ImageStatus[] image2;

        public Item2Ui(Player p)
            : base()
        {
            player = p;
            image2 = new ImageStatus[2];
            image2[0] = new ImageStatus();
            image2[1] = new ImageStatus();
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            Position = uiManager.setPosition(new Vector2(1, 1));

            image2[0].Image = uiManager.TextureLord("GameUi\\item flame");
            image2[1].Image = uiManager.TextureLord("GameUi\\Ghost bar");
            image2[0].Position = new Vector2(-image2[0].Image.Height, -image2[0].Image.Height);
            image2[1].Position = new Vector2(-image2[1].Image.Height, -image2[1].Image.Height);
        }

        public override void Draw()
        {
            thisUiManager.ThisGame1.spriteBatch.Draw(image2[0].Image, image2[0].Position + Position, Color.White);
            if (player.Item2 > 0)
                thisUiManager.ThisGame1.spriteBatch.Draw(image2[1].Image, image2[1].Position + Position, Color.White);
        }
    }
}
