using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using onmyouchan.Entity;
using onmyouchan.UI;

namespace onmyouchan
{
    /// <summary>
    /// IUpdateable インターフェイスを実装したゲーム コンポーネントです。
    /// </summary>
    class GamePauseMenuConponents : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        UiManager uiManager;
        SelectManager selectManager;
        GameDate gameDate;

        PauseBackUi back;
        SelectCursor cursor;

        public GamePauseMenuConponents(Game1 game,GameDate date)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;
            uiManager = new UiManager(game1);
            selectManager = new SelectManager();
            gameDate = date;

            //オブジェクト
            back = new PauseBackUi();
            cursor = new SelectCursor();
        }

        /// <summary>
        /// ゲーム コンポーネントの初期化を行います。
        /// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
        /// </summary>
        public override void Initialize()
        {
            // TODO: ここに初期化のコードを追加します。
            uiManager.Initialize();
            selectManager.Initialize();
            selectManager.MaxSelect = new Vector2(4, 0);

            uiManager.LordUi(back);
            back.Position = uiManager.setPosition(new Vector2(1f, 0f));
            back.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(0.5f, 0f));

            cursor.Initialize();
            uiManager.LordUi(cursor);
            cursor.Position = uiManager.setPosition(new Vector2(1f, 0.12f));
                        
            base.Initialize();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。
            selectManager.Update();
            uiManager.Update(gameTime);

            if (!gameDate.PauseFrag)
            {
                back.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(1, 0));
                cursor.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(1f, 0.12f));
                if (back.Position == back.uiProduction.GoalPoint)
                {
                    game1.Components.Remove(this);
                    Initialize();
                }
            }
            else
            {
                switch ((int)selectManager.NowSelect.X)
                {
                    case 0:
                        //ゲームに戻るの位置
                        cursor.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(0.60f, 0.12f));
                        if (selectManager.selectOn)
                        {
                            gameDate.PauseFrag = false;
                        }

                        break;

                    case 1:
                        //やり直すの位置
                        cursor.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(0.70f, 0.12f));
                        if (selectManager.selectOn)
                        {
                            game1.gameComponent.InitializeFlag = true;
                        }

                        break;

                    case 2:
                        //設定の位置
                        cursor.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(0.80f, 0.12f));
                        break;

                    case 3:
                        //ステージ選択に戻るの位置
                        cursor.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(0.90f, 0.12f));
                        if (selectManager.selectOn)
                        {
                            game1.Scene = Scenes.Menu;
                        }

                        break;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // TODO: ここに描画コードを追加します。
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            uiManager.Draw();

            base.Draw(gameTime);
        }
    }
}
