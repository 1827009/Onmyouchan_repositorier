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
    class StartMessage:SimpleFont
    {
        SineWave sinWave;
        public StartMessage(SpriteFont f, string s, Vector2 pos)
            : base(f, s, pos)
        {
            sinWave = new SineWave();
            sinWave.RotaSpeed = 1.5f;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            color.A = (byte)(Math.Abs(sinWave.Sin(gameTime)) * 255);
        }
    }
}
