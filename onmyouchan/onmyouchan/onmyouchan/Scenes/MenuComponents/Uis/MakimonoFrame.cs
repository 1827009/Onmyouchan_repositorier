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
    class MakimonoFrame:Ui
    {
        SineWaveProduction sineWave;

        public MakimonoFrame()
        {
            sineWave = new SineWaveProduction(this);
        }

        public override void Update(GameTime time)
        {
            sineWave.Update();
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            images.Add(new ImageStatus(uiManager.TextureLord("Image\\GameUI\\GameMenu")));
            images[0].SetImageRectangle(uiManager.setPosition(0.5f, 0.5f));
            images.Add(new ImageStatus(uiManager.TextureLord("Image\\GameUI\\GameMenuFrame")));
            images[1].SetImageRectangle(uiManager.setPosition(0.05f, 0.5f));

            Position = uiManager.setPosition(1, 0);
            sineWave.GoalPoint = uiManager.setPosition(0.6f, 0);
        }
    }

    class HorizontalCursor:Ui
    {
        public SineWaveProduction SinProduction
        {
            get { return sineWave; }
            set { sineWave = value; }
        }
        SineWaveProduction sineWave;

        float drawWeight;
        Rectangle rangeRectangle;
        const float productionSpeed = 1800;

        public HorizontalCursor()
        {
            sineWave = new SineWaveProduction(this);
        }

        public override void Update(GameTime gameTime)
        {
            sineWave.Update();
            Production(gameTime);
        }

        public void Production(GameTime gameTime)
        {
            if (!sineWave.MoveOn)
            {
                rangeRectangle.Height = images[0].Image.Height;
                rangeRectangle.Width = (int)(images[0].Image.Height * 0.8);
                drawWeight = images[0].Image.Height;
            }
            else if (rangeRectangle.Width < images[0].Image.Width)
            {
                drawWeight += (float)gameTime.ElapsedGameTime.TotalSeconds * productionSpeed;
                rangeRectangle.Width = (int)drawWeight;
            }                
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            images.Add(new ImageStatus(uiManager.TextureLord("Image\\GameUI\\HorizontallyCursor")));
        }

        public override void Draw()
        {
            Rectangle r = rangeRectangle;
            r.X += (int)Position.X; r.Y += (int)Position.Y;
            thisUiManager.ThisGame1.spriteBatch.Draw(images[0].Image, r, rangeRectangle, Color.White);          
        }
    }
}
