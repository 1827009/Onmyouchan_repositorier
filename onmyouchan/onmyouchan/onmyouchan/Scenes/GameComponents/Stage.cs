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
    /// <summary>
    /// ステージのギミック、敵の配置など
    /// </summary>
    class Stage
    {
        Game1 game1;

        public Vector2 size;
        GameDate gameDate;
        ScoreData scoreDate;
        EntityManager entityManager;
        int clearCount;

        //すでにアイテムがスポーンしたか
        bool spornItem1;
        bool spornItem2;

        int difficulty;

        public Stage(Game1 game,EntityManager eManager, Vector2 stageSize, GameDate date, ScoreData score)
        {
            game1 = game;
            clearCount = 0;

            entityManager = eManager;
            size = stageSize;
            gameDate = date;
            scoreDate = score;

            eManager.shadowCamera = new Camera();
            eManager.shadowCamera.position.Y = 1000;
        }

        public void Update()
        {
            //ステージ外に出ない
            for (int i = 0; i < entityManager.entityList.Count; i++)
            {
                if (entityManager.entityList[i].Attribute != EntityType.camera &&
                    entityManager.entityList[i].Attribute != EntityType.terrain)
                {
                    if (entityManager.entityList[i].position.X > size.X)
                        entityManager.entityList[i].position.X = size.X;
                    else if (entityManager.entityList[i].position.X < -size.X)
                        entityManager.entityList[i].position.X = -size.X;

                    if (entityManager.entityList[i].position.Z > size.Y)
                        entityManager.entityList[i].position.Z = size.Y;
                    if (entityManager.entityList[i].position.Z < -size.Y)
                        entityManager.entityList[i].position.Z = -size.Y;

                    if (Math.Abs(entityManager.entityList[i].position.Y) < -1000)
                        entityManager.entityList[i].position.Y = 100;
                }
            }

            //アイテムのスポーン
            if (scoreDate.BorderCount == 5 && gameDate.RemainingTime < gameDate.MaxRemainingTime * 0.5f && !spornItem2)
            {
                spornItem2 = true;
                Random rand = new Random();
                Item2Object item2 = new Item2Object(new Vector3(0, 150, 0));
                entityManager.LordEntity(item2);
                item2.ItemSet(size);
            }
            if (gameDate.RemainingTime < gameDate.MaxRemainingTime * 0.3f && !spornItem1)
            {
                spornItem1 = true;
                Random rand = new Random();
                Item1Object item1 = new Item1Object(new Vector3(0, 150, 0));
                entityManager.LordEntity(item1);
                item1.ItemSet(size);
            }

            if (clearCount <= gameDate.KillCount)
                entityManager.thisGame.gameComponent.ClearFlag = true;
        }

        /// <summary>
        /// ステージの設定
        /// </summary>
        /// <param name="difficulty"></param>
        public void SetStage(int difficulty,ref Cue bgm)
        {
            this.difficulty = difficulty;
            if (game1.gameComponent.bgm != null)
                game1.gameComponent.bgm.Stop(AudioStopOptions.AsAuthored);
            //ステージごと
            switch(difficulty)
            {
                case 0:
                    entityManager.LordEntity(new SampleTerrain(Vector3.Zero, size));
                    bgm = Game1.sound.GetCue("Game1");

                    float rag=(float)Math.Atan2(size.Y,size.X);

                    GhostObject g1 = new GhostObject(new Vector3(size.X / -0.9f, 100, size.Y * -0.9f), gameDate, scoreDate, 180);
                    g1.MoveRange = 3;
                    g1.Rotation = new Vector3(g1.Rotation.X, -rag+(float)Math.PI, g1.Rotation.Z);
                    g1.goalAngle = g1.Rotation.Y;

                    GhostObject g2 = new GhostObject(new Vector3(size.X / -0.9f, 100, size.Y * 0.9f), gameDate, scoreDate, 180);
                    g2.MoveRange = 3;
                    g2.Rotation = new Vector3(g2.Rotation.X, rag + (float)Math.PI, g2.Rotation.Z);
                    g2.goalAngle = g2.Rotation.Y;

                    GhostObject g3 = new GhostObject(new Vector3(size.X, 100, 0), gameDate, scoreDate, 180);
                    g3.MoveRange = 3;
                    g3.Rotation = new Vector3(g3.Rotation.X, -g3.Rotation.Y, g3.Rotation.Z);
                    g3.goalAngle = g3.Rotation.Y;

                    entityManager.LordEntity(g1);
                    entityManager.LordEntity(g2);
                    entityManager.LordEntity(g3);
                    clearCount = 3;
                    break;

                case 1:
                    entityManager.LordEntity(new SampleTerrain(Vector3.Zero, size));
                    bgm = Game1.sound.GetCue("Game2");

                    float rag2=(float)Math.Atan2(size.Y,size.X);

                    GhostObject g2_1 = new GhostObject(new Vector3(size.X / -0.9f, 100, size.Y * 0.9f), gameDate, scoreDate, 180);
                    g2_1.MoveRange = 3;
                    g2_1.Rotation = new Vector3(g2_1.Rotation.X, g2_1.Rotation.Y + (float)Math.PI, g2_1.Rotation.Z);
                    g2_1.goalAngle = g2_1.Rotation.Y;

                    GhostObject g2_2 = new GhostObject(new Vector3(size.X / 0.9f, 100, size.Y * 0.5f), gameDate, scoreDate, 180);
                    g2_2.MoveRange = 3;
                    g2_2.Rotation = new Vector3(g2_2.Rotation.X, g2_2.Rotation.Y, g2_2.Rotation.Z);
                    g2_2.goalAngle = g2_2.Rotation.Y;

                    GhostObject g2_3 = new GhostObject(new Vector3(size.X / 0.9f, 100, size.Y * -0.5f), gameDate, scoreDate, 180);
                    g2_3.MoveRange = 3;
                    g2_3.Rotation = new Vector3(g2_3.Rotation.X, g2_3.Rotation.Y, g2_3.Rotation.Z);
                    g2_3.goalAngle = g2_3.Rotation.Y;

                    GhostObject g2_4 = new GhostObject(new Vector3(size.X / -0.9f, 100, size.Y * -0.9f), gameDate, scoreDate, 180);
                    g2_4.MoveRange = 3;
                    g2_4.Rotation = new Vector3(g2_4.Rotation.X, g2_4.Rotation.Y + (float)Math.PI, g2_4.Rotation.Z);
                    g2_4.goalAngle = g2_4.Rotation.Y;

                    GrebeShadowObject g2_5 = new GrebeShadowObject(new Vector3(0, 100, 0), gameDate, scoreDate);
                    g2_5.Rotation = new Vector3(g2_5.Rotation.X, g2_5.Rotation.Y + (float)Math.PI, g2_5.Rotation.Z);
                    g2_5.goalAngle = g2_5.Rotation.Y;
                    
                    WispObject g2_6 = new WispObject(new Vector3(size.Y * -0.6f, 100, size.Y * 0f), gameDate, scoreDate);
                    g2_6.Rotation = new Vector3(g2_6.Rotation.X, g2_6.Rotation.Y, g2_6.Rotation.Z);
                    g2_6.goalAngle = g2_6.Rotation.Y;

                    entityManager.LordEntity(g2_1);
                    entityManager.LordEntity(g2_2);
                    entityManager.LordEntity(g2_3);
                    entityManager.LordEntity(g2_4);
                    entityManager.LordEntity(g2_5);
                    entityManager.LordEntity(g2_6);
                    clearCount = 6;
                    break;

                case 2:
                    entityManager.LordEntity(new SampleTerrain(Vector3.Zero, size));
                    bgm = Game1.sound.GetCue("Game3");

                    float rag3=(float)Math.Atan2(size.Y,size.X);

                    GhostObject g3_1 = new GhostObject(new Vector3(size.Y * -0.8f, 100, size.Y * 0.8f), gameDate, scoreDate, 90);
                    g3_1.MoveRange = 1;
                    g3_1.Rotation = new Vector3(g3_1.Rotation.X, g3_1.Rotation.Y + (float)Math.PI, g3_1.Rotation.Z);
                    g3_1.goalAngle = g3_1.Rotation.Y;

                    GhostObject g3_2 = new GhostObject(new Vector3(size.Y * 0.8f, 100, size.Y * -0.8f), gameDate, scoreDate, 90);
                    g3_2.MoveRange = 1;
                    g3_2.Rotation = new Vector3(g3_2.Rotation.X, g3_2.Rotation.Y, g3_2.Rotation.Z);
                    g3_2.goalAngle = g3_2.Rotation.Y;

                    GhostObject g3_3 = new GhostObject(new Vector3(size.Y * 0.1f, 100, size.Y * -0.25f), gameDate, scoreDate, 90);
                    g3_3.MoveRange = 0.5f;
                    g3_3.Rotation = new Vector3(g3_3.Rotation.X, g3_3.Rotation.Y, g3_3.Rotation.Z);
                    g3_3.goalAngle = g3_3.Rotation.Y;

                    GhostObject g3_4 = new GhostObject(new Vector3(size.Y * -0.1f, 100, size.Y * 0.25f), gameDate, scoreDate, 90);
                    g3_4.MoveRange = 0.5f;
                    g3_4.Rotation = new Vector3(g3_4.Rotation.X, g3_4.Rotation.Y + (float)Math.PI, g3_4.Rotation.Z);
                    g3_4.goalAngle = g3_4.Rotation.Y;

                    GhostObject g3_5 = new GhostObject(new Vector3(size.X * -0.1f, 100, size.Y * -0.1f), gameDate, scoreDate, 90);
                    g3_5.MoveRange = 0.5f;
                    g3_5.Rotation = new Vector3(g3_5.Rotation.X, g3_5.Rotation.Y + (float)Math.PI, g3_5.Rotation.Z);
                    g3_5.goalAngle = g3_5.Rotation.Y;

                    GrebeShadowObject g3_6 = new GrebeShadowObject(new Vector3(size.Y * 0.8f, 100, size.Y * 0.8f), gameDate, scoreDate);
                    g3_6.Rotation = new Vector3(g3_6.Rotation.X, g3_6.Rotation.Y + (float)Math.PI, g3_6.Rotation.Z);
                    g3_6.goalAngle = g3_6.Rotation.Y;

                    GrebeShadowObject g3_7 = new GrebeShadowObject(new Vector3(size.Y * -0.8f, 100, size.Y * -0.8f), gameDate, scoreDate);
                    g3_7.Rotation = new Vector3(g3_7.Rotation.X, g3_7.Rotation.Y, g3_7.Rotation.Z);
                    g3_7.goalAngle = g3_7.Rotation.Y;

                    WispObject g3_9 = new WispObject(new Vector3(size.Y * 0.6f, 100, size.Y * 0.6f), gameDate, scoreDate);
                    g3_9.Rotation = new Vector3(g3_9.Rotation.X, g3_9.Rotation.Y, g3_9.Rotation.Z);
                    g3_9.goalAngle = g3_9.Rotation.Y;

                    WispObject g3_10 = new WispObject(new Vector3(size.Y * -0.6f, 100, size.Y * 0f), gameDate, scoreDate);
                    g3_10.Rotation = new Vector3(g3_10.Rotation.X, g3_10.Rotation.Y, g3_10.Rotation.Z);
                    g3_10.goalAngle = g3_10.Rotation.Y;

                    entityManager.LordEntity(g3_1);
                    entityManager.LordEntity(g3_2);
                    entityManager.LordEntity(g3_3);
                    entityManager.LordEntity(g3_4);
                    entityManager.LordEntity(g3_5);
                    entityManager.LordEntity(g3_6);
                    entityManager.LordEntity(g3_7);
                    entityManager.LordEntity(g3_9);
                    entityManager.LordEntity(g3_10);
                    clearCount = 9;
                    break;
            }
            bgm.Play();
        }
    }
}
