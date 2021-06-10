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
    class UiProduction
    {
        GameTime gameTime;

        /// <summary>
        /// uiの移動
        /// </summary>
        public Vector2 Speed
        {
            get { return speed; }
        }
        Vector2 speed;
        Vector2 goalPosition;
        public void moveSpeed(Vector2 pos, Vector2 goalPos, float goalTime)
        {
            speed = (goalPos - pos) / (goalTime * (float)gameTime.ElapsedGameTime.TotalHours);
        }    
    }
}
