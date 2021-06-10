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
using onmyouchan.Entity;

namespace onmyouchan
{
    class Stage
    {
        public Vector2 size;
        GameDate gameDate;
        ScoreData scoreDate;
        EntityManager entityManager;

        //すでにアイテムがスポーンしたか
        bool spornItem1;
        bool spornItem2;

        public Stage(EntityManager eManager,Vector2 stageSize, GameDate date,ScoreData score)
        {
            entityManager = eManager;
            size = stageSize;
            gameDate = date;
            scoreDate = score;
        }

        public void Update()
        {
            //アイテムのスポーン
            if (scoreDate.BorderCount == 3&&!spornItem2)
            {
                spornItem2 = true;
                Random rand = new Random();
                entityManager.LordEntity(new Item2Object(new Vector3(0,150,0)));
            }
            if (gameDate.RemainingTime < gameDate.MaxRemainingTime * 0.5f && gameDate.KillCount < gameDate.GoalKillCount * 0.5f && !spornItem1)
            {
                spornItem1 = true;
                Random rand = new Random();
                entityManager.LordEntity(new ItemObject(new Vector3(0, 150, 0)));
            }
        }
    }
}
