using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using onmyouchan.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace onmyouchan.UI
{
    class OptionGage:Ui
    {
        public SineWaveProduction sineProduction;
        /// <summary>
        /// アイコンの数
        /// </summary>
        public int Step
        {
            get { return step; }
            set
            {
                if (step != value)
                {
                    step = value;
                    updateFrag = true;
                }
            }
        }
        int step;
        bool updateFrag;

        public OptionGage(int n,Vector2 pos)
        {
            Step = n;
            sineProduction = new SineWaveProduction(this);
            Position = pos;
            sineProduction.GoalPoint=pos;
        }

        public override void Update(GameTime gameTime)
        {
            sineProduction.Update();
            if (updateFrag)
            {
                images.Clear();
                for (int i = 0; i < step; i++)
                {
                    ImageStatus image = new ImageStatus(thisUiManager.TextureLord("Image\\GameUI\\OhudaIcon"));
                    image.CenterPosition = new Vector2(-i, 0);
                    images.Add(image);
                }
            }
            base.Update(gameTime);
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
                
        }
    }
}
