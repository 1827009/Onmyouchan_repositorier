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
    /// 指定した座標の中心にオリジナルの数字を表示する
    /// </summary>
    class Figure:Ui
    {
        #region フィールド

        /// <summary>
        /// 数字のサイズ
        /// </summary>
        public float FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }
        float fontSize;

        /// <summary>
        /// 表示される数字
        /// </summary>
        public int No
        {
            get { return no; }
            set { no = value;
                updateFlag = true; }
        }
        int no;

        /// <summary>
        /// 数字変動しなければ更新しない
        /// </summary>
        bool updateFlag = true;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 設置したい位置を1~100の値で
        /// </summary>
        /// <param name="pos"></param>
        public Figure(Vector2 pos, int n, float size)
        {
            Position = pos;
            no = n;
            fontSize = size;
        }

        #endregion

        #region 更新

        /// <summary>
        /// 数字の桁反映
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            if (updateFlag)
            {
                images.Clear();

                int digit = 0;
                int n = no;
                //桁を数える
                while (n!=0)
                {
                    ImageStatus num = new ImageStatus(thisUiManager.TextureLord("Image\\GameUI\\No\\" + n % 10));
                    images.Add(num);
                    n /= 10;
                    digit++;
                }
                //0なら0分の桁
                if (digit == 0)
                {
                    ImageStatus num = new ImageStatus(thisUiManager.TextureLord("Image\\GameUI\\No\\" + 0));
                    digit++;
                    images.Add(num);
                }
                n = no;
                for (int i = 0; i < images.Count; i++)
                {
                    images[i].Scale = fontSize;
                    Vector2 size = new Vector2(images[i].Image.Width, images[i].Image.Height) * fontSize;
                    images[i].CenterPosition = new Vector2((i - images.Count * 0.5f) + 1f, 0.5f);
                    n /= 10;
                }
                updateFlag = false;
            }
        }

        #endregion

        #region メソッド
        
        public override void Lord(UiManager uiManager)
        {
            base.Lord(uiManager);
        }

        #endregion
    }
}
