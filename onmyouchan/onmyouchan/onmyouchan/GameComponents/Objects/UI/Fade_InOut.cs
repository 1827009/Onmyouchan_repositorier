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
    class Fade_InOut:Ui
    {
        #region フィールド

        Game1 thisGame1;

        Texture2D fadeTexture;

        Color color;
        float fadeTime;
        public float FadeTime
        { set { fadeTime = (255f / value) / MyUtility.SecondFrame; } }

        bool fade;
        /// <summary>
        /// 動作中か
        /// </summary>
        public bool On
        {
            get { return on; }
            set { on = value; }
        }
        bool on;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// trueにすると初期が黒画面になります
        /// </summary>
        /// <param name="game"></param>
        /// <param name="totalTime"></param>
        /// <param name="inOrOut"></param>
        public Fade_InOut(Game1 game, bool blackOut)
        {
            thisGame1 = game;

            fade = blackOut;

            color = new Color(0f, 0f, 0f, 255f);
            fadeTexture = new Texture2D(game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            fadeTexture.SetData<Color>(new Color[] { color });

            if (!blackOut)
                color.A = 0;
        }

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            if (!fade)
            {
                if (fadeTime >= Math.Abs(color.A))
                {
                    color.A = (byte)0;
                    on = false;
                }
                else
                {
                    on = true;
                    color.A -= (byte)fadeTime;
                }
            }
            else
            {
                if (255f - fadeTime <= Math.Abs(color.A))
                {
                    color.A = (byte)255;
                    on = false;
                }
                else
                {
                    on = true;
                    color.A += (byte)fadeTime;
                }
            }
        }

        #endregion

        #region 描画

        public override void Draw()
        {
            thisGame1.spriteBatch.Draw(fadeTexture, new Rectangle(0, 0, thisGame1.GraphicsDevice.Viewport.Width, thisGame1.GraphicsDevice.Viewport.Height), color);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// trueでフェードアウト、falseフェードインします
        /// </summary>
        /// <param name="inAndOut"></param>
        public void Fade(bool inAndOut,float time)
        {
            if (!on)
            {
                fadeTime = time;
                fade = inAndOut;
            }
        }

        #endregion
    }
}
