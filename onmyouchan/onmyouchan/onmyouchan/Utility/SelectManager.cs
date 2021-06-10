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
    /// <summary>
    /// メニューの選択操作の使いまわし用です
    /// </summary>
    class SelectManager
    {
        public Cue decideSe;
        public Cue selectSe;

        public Vector2 MaxSelect
        {
            get { return maxSelect; }
            set
            {
                maxSelect = value;
                if (maxSelect.X <= 0)
                    maxSelect.X = 1;
                if (maxSelect.Y <= 0)
                    maxSelect.Y = 1;
            }
        }
        Vector2 maxSelect;

        public Vector2 NowSelect
        {
            get { return nowSelect; }
            set
            {
                if (nowSelect != value)
                    nowSelect = value;
                else
                    return;

                bool seFrag=false;

                if (nowSelect.X < 0)
                    nowSelect.X = 0;
                else if (nowSelect.X >= maxSelect.X)
                    nowSelect.X = maxSelect.X - 1;
                else
                    seFrag = true;

                if (nowSelect.Y < 0)
                    nowSelect.Y = 0;
                else if (nowSelect.Y >= maxSelect.Y)
                    nowSelect.Y = maxSelect.Y - 1;
                else
                    seFrag = true;
                if (seFrag)
                    MyUtility.SEPlay(selectSe, "CursorMove");
            }
        }
        Vector2 nowSelect;

        public bool selectOn;

        public SelectManager(int x,int y)
        {
            maxSelect.X = x;
            maxSelect.Y = y;
        }

        public void Initialize()
        {
            selectOn = false;
            nowSelect = Vector2.Zero;
        }

        public void Update()
        {
            NowSelect += ControllManager.SelectKey();
            if (ControllManager.KeyDecide())
            {
                if (!selectOn)
                {
                    selectOn = true;
                    MyUtility.SEPlay(decideSe, "clappers01");
                }
            }
            else
                selectOn = false;
        }
    }
}
