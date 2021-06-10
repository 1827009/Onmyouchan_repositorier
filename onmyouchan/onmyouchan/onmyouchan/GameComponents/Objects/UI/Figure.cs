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
        
        List<ImageStatus> textureStatus;

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
        public Figure(Vector2 pos,int n,float size)
        {
            textureStatus = new List<ImageStatus>();
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
                int digit = 0;
                int n = no;
                while (n!=0)
                {
                    n /= 10;
                    digit++;
                }
                if (digit == 0)
                    digit++;
                n = no;
                for (int i = 0; i < digit; i++)
                {
                    if (textureStatus.Count < digit)
                        textureStatus.Add(new ImageStatus());

                    textureStatus[i].Image = thisUiManager.TextureLord("GameUi\\No\\" + n % 10);
                    Vector2 size = new Vector2(textureStatus[i].Image.Width, textureStatus[i].Image.Height) * fontSize;
                    textureStatus[i].Position = new Vector2(Position.X - size.X * (i - digit * 0.5f+1), Position.Y - size.Y * 0.5f);
                    n /= 10;
                }
                updateFlag = false;
            }
        }

        #endregion

        #region 描画

        public override void Draw()
        {
            for (int i = 0; i < textureStatus.Count; i++)
            {
                thisUiManager.ThisGame1.spriteBatch.Draw(textureStatus[i].Image, textureStatus[i].Position, null, Color.White, 0, Vector2.Zero, fontSize, SpriteEffects.None, 0);
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
