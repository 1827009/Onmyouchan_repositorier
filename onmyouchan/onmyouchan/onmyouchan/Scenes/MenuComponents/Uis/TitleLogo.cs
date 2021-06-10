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
    class TitleLogo:Ui
    {
        float alpha;

        #region コンストラクタ

        public TitleLogo(Texture2D tex)
        {
            images.Add(new ImageStatus(tex));
            images[0].ImageColor = new Color(0, 0, 0, 0);
        }

        #endregion
        
        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
            Position = uiManager.setPosition(0.45f, 0.05f);
            images[0].SetImageRectangle(uiManager.setPosition(0.5f, 0.3f));
        }

        /// <summary>
        /// 浮き出る演出
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Production(GameTime time,bool onOff)
        {
            bool output;   
            if (onOff)
                output= StandOut(ref alpha, 3f, time);
            else
                output= StandOut(ref alpha, -1f, time);
            float a = alpha / 255;
            images[0].ImageColor = new Color(new Vector4(a));
            return output;
        }
        /// <summary>
        /// 演出強制完了
        /// </summary>
        /// <param name="completion"></param>
        /// <returns></returns>
        public bool Production(bool completion)
        {
            if (completion)
            {
                images[0].ImageColor = Color.White;
                return false;
            }
            return true;
        }
    }
}
