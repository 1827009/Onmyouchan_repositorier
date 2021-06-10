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
    class Page:Ui
    {
        ImageStatus back;
        ImageStatus leftPage;
        ImageStatus rightPage;
        ImageStatus moveBack;
        ImageStatus movePage;

        public int PageNumber
        {
            get { return page; }
            set
            {
                TurnPage(value);
            }
        }
        int page;

        float moveSpeed = 0.5f;
        bool pageChange;
        
        public override void Update(GameTime gameTime)
        {
            if (images.Contains(movePage))
            {
                if (!pageChange)
                {
                    if (movePage.ImageRectangle.Width > 0)
                    {
                        movePage.ImageRectangle = new Rectangle(movePage.ImageRectangle.X, movePage.ImageRectangle.Y,
                            movePage.ImageRectangle.Width - (int)((movePage.Image.Width * gameTime.ElapsedGameTime.TotalSeconds) / moveSpeed),
                            movePage.ImageRectangle.Height);
                    }
                    else
                    {
                        movePage.Image = thisUiManager.TextureLord("Image\\Item\\Page1");
                        movePage.ImageRectangle = new Rectangle(movePage.Image.Width, 0, movePage.Image.Width, movePage.Image.Height);
                        movePage.CenterPosition = new Vector2(1.0f, 0.0f);

                        moveBack.ImageRectangle = movePage.ImageRectangle;
                        moveBack.CenterPosition = movePage.CenterPosition;
                        images.Insert(3, moveBack);

                        pageChange = true;
                    }
                }
                else
                {
                    if (movePage.ImageRectangle.X > 0)
                    {
                        movePage.ImageRectangle = new Rectangle((int)(movePage.ImageRectangle.X - (int)((movePage.Image.Width * gameTime.ElapsedGameTime.TotalSeconds) / moveSpeed)),
                            movePage.ImageRectangle.Y,
                            movePage.Image.Width - movePage.ImageRectangle.X,
                            movePage.ImageRectangle.Height);

                        moveBack.ImageRectangle = movePage.ImageRectangle;
                    }
                    else
                    {
                        pageChange = false;
                        images.Remove(moveBack);
                        images.Remove(movePage);
                    }
                }
            }
            base.Update(gameTime);
        }

        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);

            back = new ImageStatus(thisUiManager.TextureLord("Image\\Item\\Page1"));
            back.CenterPosition = new Vector2(1, 0);
            images.Add(back);

            leftPage = new ImageStatus(uiManager.TextureLord("Image\\Item\\Page1"));
            images.Add(leftPage);

            rightPage = new ImageStatus(uiManager.TextureLord("Image\\Item\\Page1"));
            rightPage.CenterPosition = new Vector2(1, 0);
            images.Add(rightPage);
            
            Position = uiManager.setPosition(0.5f, 0);
        }

        /// <summary>
        /// ページをめくる演出スイッチ
        /// </summary>
        /// <param name="n"></param>
        private void TurnPage(int n)
        {
            if (!images.Contains(moveBack))
            {
                switch (n)
                {
                    default:
                        moveBack = new ImageStatus(thisUiManager.TextureLord("Image\\GameUI\\Page1_1"));
                        movePage = new ImageStatus(thisUiManager.TextureLord("Image\\GameUI\\Page2"));
                        images.Add(movePage);
                        break;
                }
            }
        }
    }
}
