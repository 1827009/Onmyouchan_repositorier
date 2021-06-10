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
    /// <summary>
    /// 黒い画面のフェードイン、アウト
    /// </summary>
    class FadeProduction:Ui
    {
        #region フィールド

        Texture2D fadeTexture;

        Color color;
        float alpha;
        float fadeTime;
        public float FadeTime
        { set { fadeTime = (255f / value) / MyUtility.SecondFrame; } }

        /// <summary>
        /// 状態
        /// </summary>
        bool inOut;

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
        public FadeProduction(Game1 game, bool blackOut)
        {
            inOut = blackOut;

            color = new Color(0f, 0f, 0f, 255f);
            fadeTexture = new Texture2D(game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            fadeTexture.SetData<Color>(new Color[] { color });

            if (!blackOut)
            {
                color.A = 0;
            }
        }

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            if (!inOut)
            {
                if (fadeTime >= Math.Abs(color.A))
                {
                    color.A = (byte)0;
                    on = false;
                }
                else
                {
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
                    color.A += (byte)fadeTime;
                }
            }
        }

        #endregion

        #region 描画

        public override void Draw()
        {            
            thisUiManager.ThisGame1.spriteBatch.Draw(fadeTexture, new Rectangle(0, 0, thisUiManager.ThisGame1.GraphicsDevice.Viewport.Width, thisUiManager.ThisGame1.GraphicsDevice.Viewport.Height), color);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// trueでフェードアウト、falseフェードイン。完了している場合にtrueを返す
        /// </summary>
        /// <param name="inAndOut"></param>
        public bool Fade(bool inOrOut,float time)
        {
            if (!on)
            {
                fadeTime = time;
                inOut = inOrOut;
                on = true;
            }
            if (inOrOut && color.A == 255)
            {
                on = false;
                return true;
            }
            else if (!inOrOut && color.A == 0)
            {
                on = false;
                return true;
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// 扉が閉じるフェードイン、アウト
    /// </summary>
    class FadeProduction2 : Ui
    {
        #region フィールド

        ImageStatus fadeTextureRight;
        ImageStatus fadeTextureLeft;
        const int texIndex = 2;

        SineWaveProduction uiProduction;
        
        /// <summary>
        /// 状態
        /// </summary>
        bool inOut;

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

        public FadeProduction2()
        {
            uiProduction = new SineWaveProduction(this);
        }

        #endregion

        #region 更新

        public override void Update(GameTime time)
        {
            if (!inOut)
            {
                uiProduction.GoalPoint = Vector2.Zero;
                if (Vector2.Zero == Position)
                {
                    on = false;
                }
                else
                {
                    on = true;
                }
            }
            else
            {
                uiProduction.GoalPoint = thisUiManager.setPosition(new Vector2(0.5f, 0));
                if (thisUiManager.setPosition(new Vector2(0.5f, 0)) == Position)
                {
                    on = false;
                }
                else
                {
                    on = true;
                }
            }

            uiProduction.Update();
        }

        #endregion

        #region 描画

        public override void Draw()
        {
            Rectangle r = images[0].DrawRectangle();
            r.X += (int)Position.X; r.Y += (int)Position.Y;
            thisUiManager.ThisGame1.spriteBatch.Draw(images[0].Image, r, images[0].ImageColor);

            r = images[1].DrawRectangle();
            r.X += (int)(thisUiManager.ThisGame1.graphics.PreferredBackBufferWidth - Position.X); r.Y += (int)Position.Y;
            thisUiManager.ThisGame1.spriteBatch.Draw(images[1].Image, r, images[1].ImageColor);
        }

        #endregion

        #region メソッド

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);

            fadeTextureLeft = new ImageStatus(thisUiManager.TextureLord("Image\\Item\\Fade1L"));
            fadeTextureLeft.centerPosition.X = 1;
            fadeTextureLeft.ImageRectangle = new Rectangle(fadeTextureLeft.ImageRectangle.X, fadeTextureLeft.ImageRectangle.Y, (int)(uiManager.ThisGame1.graphics.PreferredBackBufferWidth * 0.5f), (int)(uiManager.ThisGame1.graphics.PreferredBackBufferHeight));

            fadeTextureRight = new ImageStatus(thisUiManager.TextureLord("Image\\Item\\Fade1R"));
            fadeTextureRight.ImageRectangle = new Rectangle(fadeTextureRight.ImageRectangle.X, fadeTextureRight.ImageRectangle.Y, (int)(uiManager.ThisGame1.graphics.PreferredBackBufferWidth * 0.5f), (int)(uiManager.ThisGame1.graphics.PreferredBackBufferHeight));

            images.Add(fadeTextureLeft);
            images.Add(fadeTextureRight);
        }

        /// <summary>
        /// trueでフェードアウト、falseフェードイン。完了している場合にtrueを返す
        /// </summary>
        /// <param name="inAndOut"></param>
        public bool Fade(bool inOrOut, float time)
        {
            if (!on)
            {
                inOut = inOrOut;
                on = true;
            }
            if (inOrOut && Position.X == thisUiManager.ThisGame1.graphics.PreferredBackBufferWidth / 2)
            {
                on = false;
                return true;
            }
            else if (!inOrOut && Position.X == 0)
            {
                on = false;
                return true;
            }

            return false;
        }

        #endregion
    }
}
