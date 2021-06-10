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
using System.Configuration;

namespace onmyouchan.UI
{
    class ResultBack : Ui
    {
        ImageStatus imageState;

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            imageState = new ImageStatus(uiManager.TextureLord("Image\\Item\\Result"));
            //imageState.SetImageRectangle(uiManager.setPosition(0.6f, 1.4f));
            imageState.CenterPosition = new Vector2(0.5f, 0.5f);
            Position = uiManager.setPosition(0.65f, 0.5f);
            imageState.Scale = 2;
            images.Add(imageState);
        }
    }

    class ScoreStamp:Ui
    {
        SineWave sin;

        ImageStatus KaStamp;
        ImageStatus RyoStamp;
        ImageStatus YuStamp;
        ImageStatus SyuStamp;

        int score;
        float alpha;

        public ScoreStamp(int s)
        {
            score = s;
            sin = new SineWave();
        }

        public override void Update(GameTime gameTime)
        {
            if (alpha < 1)
            {
                images[0].Scale -= (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f;
                alpha += (float)gameTime.ElapsedGameTime.TotalSeconds;
                images[0].ImageColor = new Color(255, 255, 255, alpha);
            }
            else
            {
                images[0].Scale = 0.5f
                    ;
                alpha = 255;
            }
            base.Update(gameTime);
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            KaStamp = new ImageStatus(uiManager.TextureLord("Image\\Item\\KaStamp"));
            RyoStamp = new ImageStatus(uiManager.TextureLord("Image\\Item\\RyoStamp"));
            YuStamp = new ImageStatus(uiManager.TextureLord("Image\\Item\\YuStamp"));
            SyuStamp = new ImageStatus(uiManager.TextureLord("Image\\Item\\SyuStamp"));

            switch (uiManager.ThisGame1.gameComponent.difficulty)
            {
                case 0:
                    if (score >= float.Parse(ConfigurationManager.AppSettings["Score1_Syu"]))
                        images.Add(SyuStamp);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score1_Yu"]))
                        images.Add(YuStamp);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score1_Ryo"]))
                        images.Add(RyoStamp);
                    else
                        images.Add(KaStamp);
                    break;
                case 1:
                    if (score >= float.Parse(ConfigurationManager.AppSettings["Score2_Syu"]))
                        images.Add(SyuStamp);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score2_Yu"]))
                        images.Add(YuStamp);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score2_Ryo"]))
                        images.Add(RyoStamp);
                    else
                        images.Add(KaStamp);
                    break;
                case 2:
                    if (score >= float.Parse(ConfigurationManager.AppSettings["Score3_Syu"]))
                        images.Add(SyuStamp);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score3_Yu"]))
                        images.Add(YuStamp);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score3_Ryo"]))
                        images.Add(RyoStamp);
                    else
                        images.Add(KaStamp);
                    break;
            }

            images[0].CenterPosition = new Vector2(0.5f, 0.5f);
            images[0].Scale = 2;
        }
    }
    class ScoreCharacter : Ui
    {
        SineWave sin;

        ImageStatus Ka;
        ImageStatus Ryo;
        ImageStatus Yu;
        ImageStatus Syu;

        int score;

        public ScoreCharacter(int s)
        {
            score = s;
            sin = new SineWave();
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            Ka = new ImageStatus(uiManager.TextureLord("Image\\Item\\ResultCharacter1"));
            Ryo = new ImageStatus(uiManager.TextureLord("Image\\Item\\ResultCharacter2"));
            Yu = new ImageStatus(uiManager.TextureLord("Image\\Item\\ResultCharacter3"));
            Syu = new ImageStatus(uiManager.TextureLord("Image\\Item\\ResultCharacter4"));

            switch (uiManager.ThisGame1.gameComponent.difficulty)
            {
                case 0:
                    if (score >= float.Parse(ConfigurationManager.AppSettings["Score1_Syu"]))
                        images.Add(Syu);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score1_Yu"]))
                        images.Add(Yu);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score1_Ryo"]))
                        images.Add(Ryo);
                    else
                        images.Add(Ka);
                    break;
                case 1:
                    if (score >= float.Parse(ConfigurationManager.AppSettings["Score2_Syu"]))
                        images.Add(Syu);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score2_Yu"]))
                        images.Add(Yu);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score2_Ryo"]))
                        images.Add(Ryo);
                    else
                        images.Add(Ka);
                    break;
                case 2:
                    if (score >= float.Parse(ConfigurationManager.AppSettings["Score3_Syu"]))
                        images.Add(Syu);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score3_Yu"]))
                        images.Add(Yu);
                    else if (score >= float.Parse(ConfigurationManager.AppSettings["Score3_Ryo"]))
                        images.Add(Ryo);
                    else
                        images.Add(Ka);
                    break;
            }
            images[0].CenterPosition = new Vector2(0.5f, 0.5f);
        }
    }
}
