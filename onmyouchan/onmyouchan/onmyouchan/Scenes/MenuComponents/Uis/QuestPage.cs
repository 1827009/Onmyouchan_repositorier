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
    class QuestPage:Ui
    {
        ImageStatus easyPage;
        ImageStatus normalPage;
        ImageStatus hardPage;

        int Radius = 700;
        float radian;
        float moveSpeed;
        float moveRag;
        float goalRadian;
        float easyPointRag;
        float normalPointRag;
        float hardPointRag;

        public int Select
        {
            get { return select; }
            set
            {
                if (goalRadian == radian)
                {
                    select = value % 3;
                    if (0 > select)
                        select = 3;
                    switch (select)
                    {
                        case 0:
                            goalRadian = easyPointRag + (float)Math.PI;
                            break;

                        case 1:
                            goalRadian = normalPointRag + (float)Math.PI;
                            break;

                        case 2:
                            goalRadian = hardPointRag + (float)Math.PI;
                            break;
                    }
                    moveRag = (goalRadian - radian) / moveSpeed;
                }
            }
        }
        int select;

        public QuestPage()
        {
            moveSpeed = 0.1f;
            Select = 0;
            easyPointRag = (float)MathHelper.ToRadians(40);
            normalPointRag = (float)MathHelper.ToRadians(20);
            hardPointRag = (float)MathHelper.ToRadians(0);
        }

        public override void Update(GameTime gameTime)
        {
            if (goalRadian != radian)
            {
                float rag = moveRag * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if ((moveRag >= 0 && radian >= goalRadian - rag && radian <= goalRadian + rag) ||
                    (moveRag < 0 && radian >= goalRadian + rag && radian <= goalRadian - rag))
                    radian = goalRadian;
                else
                    radian += rag;
            }

            base.Update(gameTime);
        }

        public override void Draw()
        {
            Rectangle easy = easyPage.DrawRectangle();
            easy.X = (int)(easy.X + Position.X + (Math.Cos(radian - easyPointRag) * Radius));
            easy.Y = (int)(easy.Y + Position.Y + (Math.Sin(radian - easyPointRag) * Radius));

            Rectangle normal = normalPage.DrawRectangle();
            normal.X = (int)(normal.X + Position.X + (Math.Cos(radian - normalPointRag) * Radius));
            normal.Y = (int)(normal.Y + Position.Y + (Math.Sin(radian - normalPointRag) * Radius));

            Rectangle hard = hardPage.DrawRectangle();
            hard.X = (int)(hard.X + Position.X + (Math.Cos(radian - hardPointRag) * Radius));
            hard.Y = (int)(hard.Y + Position.Y + (Math.Sin(radian - hardPointRag) * Radius));

            switch (select)
            {
                case 0:
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[2].Image, hard, Color.White);
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[1].Image, normal, Color.White);
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[0].Image, easy, Color.White);
                    break;
                case 1:
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[0].Image, easy, Color.White);
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[2].Image, hard, Color.White);
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[1].Image, normal, Color.White);
                    break;
                case 2:
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[0].Image, easy, Color.White);
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[1].Image, normal, Color.White);
                    thisUiManager.ThisGame1.spriteBatch.Draw(images[2].Image, hard, Color.White);
                    break;
            }
        }
            
        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);

            Position = uiManager.setPosition(1, 0.5f);

            easyPage = new ImageStatus(uiManager.TextureLord("Image\\Item\\Quest1"));
            easyPage.CenterPosition = new Vector2(0.0f, 0.5f);
            images.Add(easyPage);

            normalPage = new ImageStatus(uiManager.TextureLord("Image\\Item\\Quest2"));
            normalPage.CenterPosition = new Vector2(0.0f, 0.5f);
            images.Add(normalPage);

            hardPage = new ImageStatus(uiManager.TextureLord("Image\\Item\\Quest3"));
            hardPage.CenterPosition = new Vector2(0.0f, 0.5f);
            images.Add(hardPage);
        }
    }

    class QuestPage_v2 : Ui
    {
        Quest easyPage;
        Quest normalPage;
        Quest hardPage;
        Color color;

        public int Select
        {
            get { return select; }
            set
            {
                if (select != value)
                {
                    select = value;
                    updateFlag = true;
                }
            }
        }
        int select;
        bool updateFlag;

        public QuestPage_v2()
        {
            Select = 0;
            updateFlag = true;
            color = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            if (updateFlag)
            {
                updateFlag = false;
                switch (select)
                {
                    case 0:
                        easyPage.MovePhase = 0;
                        normalPage.MovePhase = 1;
                        hardPage.MovePhase = 2;
                        break;
                    case 1:
                        easyPage.MovePhase = 2;
                        normalPage.MovePhase = 0;
                        hardPage.MovePhase = 1;
                        break;
                    case 2:
                        easyPage.MovePhase = 1;
                        normalPage.MovePhase = 2;
                        hardPage.MovePhase = 0;
                        break;
                }
            }
            if (easyPage.Replacement || normalPage.Replacement || hardPage.Replacement)
            {
                thisUiManager.UiList.Remove(easyPage);
                thisUiManager.UiList.Remove(normalPage);
                thisUiManager.UiList.Remove(hardPage);
                switch (select)
                {
                    case 0:
                        thisUiManager.UiList.Add(hardPage);
                        thisUiManager.UiList.Add(normalPage);
                        thisUiManager.UiList.Add(easyPage);
                        break;
                    case 1:
                        thisUiManager.UiList.Add(easyPage);
                        thisUiManager.UiList.Add(hardPage);
                        thisUiManager.UiList.Add(normalPage);
                        break;
                    case 2:
                        thisUiManager.UiList.Add(normalPage);
                        thisUiManager.UiList.Add(easyPage);
                        thisUiManager.UiList.Add(hardPage);
                        break;
                }
            }
            base.Update(gameTime);
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);

            Position = uiManager.setPosition(1, 0.5f);

            hardPage = new Quest(uiManager.TextureLord("Image\\Item\\Quest3"), 2);
            uiManager.LordUi(hardPage);

            normalPage = new Quest(uiManager.TextureLord("Image\\Item\\Quest2"), 1);
            uiManager.LordUi(normalPage);

            easyPage = new Quest(uiManager.TextureLord("Image\\Item\\Quest1"), 0);
            uiManager.LordUi(easyPage);
        }
    }

    class Quest:Ui
    {
        public SineWaveProduction SinProduction
        {
            get { return sinProduction; }
            set { sinProduction = value; }
        }
        SineWaveProduction sinProduction;

        public int MovePhase
        {
            get { return movePhase; }
            set
            {
                if ((movePhase == 0 && value == 2) || (movePhase == 2 && value == 0))
                    turnFrag = true;
                movePhase = value; 
            }
        }
        int movePhase;
        bool turnFrag;

        public bool Replacement { get { return replacement; } }
        bool replacement;

        public Quest(Texture2D tex, int posNo)
        {
            images.Add(new ImageStatus(tex));
            sinProduction = new SineWaveProduction(this);
            sinProduction.moveSpeed = 0.25f;
            turnFrag = false;
            movePhase = posNo;
        }

        public override void Update(GameTime gameTime)
        {
            sinProduction.Update();
            if (turnFrag)
            {
                sinProduction.GoalPoint = thisUiManager.setPosition(1.5f, 0.3f);
                if (sinProduction.MoveOn)
                {
                    turnFrag = false;
                    replacement = true;
                }
            }
            else
            {
                replacement = false;
                switch (movePhase)
                {
                    case 0:
                        sinProduction.GoalPoint = thisUiManager.setPosition(0.75f, 0.5f);
                        break;
                    case 1:
                        sinProduction.GoalPoint = thisUiManager.setPosition(0.77f, 0.48f);
                        break;
                    case 2:
                        sinProduction.GoalPoint = thisUiManager.setPosition(0.79f, 0.46f);
                        break;
                }
            }
            base.Update(gameTime);
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            images[0].CenterPosition = new Vector2(0.5f, 0.5f);
            images[0].Scale = 1.5f;
            switch (movePhase)
            {
                case 0:
                    sinProduction.GoalPoint = thisUiManager.setPosition(0.75f, 0.5f);
                    break;
                case 1:
                    sinProduction.GoalPoint = thisUiManager.setPosition(0.77f, 0.48f);
                    break;
                case 2:
                    sinProduction.GoalPoint = thisUiManager.setPosition(0.79f, 0.46f);
                    break;
            }
            sinProduction.Arrival();
        }
    }
}
