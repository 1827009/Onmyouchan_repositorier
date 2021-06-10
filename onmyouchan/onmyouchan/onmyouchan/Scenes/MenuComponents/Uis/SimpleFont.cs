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
    class SimpleFont:Ui
    {
        SpriteFont font;
        /// <summary>
        /// 文字
        /// </summary>
        public String Str
        {
            get { return str; }
            set { str = value; }
        }
        string str;

        public SineWaveProduction SinProduction
        {
            get { return sinProduction; }
            set { sinProduction = value; }
        }
        SineWaveProduction sinProduction;
        Vector2 centerPoint;
        public float Size
        {
            get { return size; }
            set { size = value; }
        }
        float size = 1;
        float rotation;
        public Color FontColor
        {
            get { return color; }
            set
            {
                color = value;
                alpha = color.A;
            }
        }
        protected Color color=Color.Black;
        float alpha;
        
        public SimpleFont(SpriteFont f,string s,Vector2 pos)
        {
            font = f;
            str = s;
            Position = pos;
            sinProduction = new SineWaveProduction(this);
            sinProduction.GoalPoint = pos;
        }

        public override void Update(GameTime time)
        {
            sinProduction.Update();
        }
        
        public override void Draw()
        {
            thisUiManager.ThisGame1.spriteBatch.DrawString(font, str, Position, color, rotation, centerPoint, size, SpriteEffects.None, 0);
        }

        /// <summary>
        /// 浮き出る演出
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool TransparentProduction(GameTime time, float fadeTime)
        {
            color.A = (byte)alpha;
            return StandOut(ref alpha, fadeTime, time);
        }
        /// <summary>
        /// 演出強制完了
        /// </summary>
        /// <param name="completion"></param>
        /// <returns></returns>
        public bool TransparentProduction(bool completion)
        {
            if (completion)
            {
                color = Color.Black;
                return false;
            }
            else
            {
                color = new Color(0, 0, 0, 0);
                return false;
            }
        }
    }
}
