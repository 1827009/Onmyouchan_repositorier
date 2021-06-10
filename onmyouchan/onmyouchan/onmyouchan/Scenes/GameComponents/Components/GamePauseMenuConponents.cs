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
    /// ゲームScene中のメニューです。各ボタンの機能(設定の内部除く)含めてここで実装されています。
    /// </summary>
    class GamePauseMenuConponents : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        UiManager uiManager;
        SelectManager selectManager;
        GameDate gameDate;

        SimpleFont returnMessage;
        SimpleFont retryMessage;
        SimpleFont optionMessage;
        SimpleFont exitMessage;

        PauseBackUi back;
        HorizontalCursor cursor;

        public GamePauseMenuConponents(Game1 game,GameDate date)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;
            uiManager = new UiManager(game1);
            selectManager = new SelectManager(0,0);
            gameDate = date;

            //オブジェクト
            back = new PauseBackUi();
            cursor = new HorizontalCursor();
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
            selectManager.MaxSelect = new Vector2(0, 4);

            returnMessage = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "ゲームに戻る", uiManager.setPosition(new Vector2(1, 0.12f)));
            retryMessage = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "やりなおす", uiManager.setPosition(new Vector2(1, 0.22f)));
            optionMessage = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "設定", uiManager.setPosition(new Vector2(1, 0.32f)));
            exitMessage = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "ステージ選択に戻る", uiManager.setPosition(new Vector2(1, 0.42f)));

            uiManager.LordUi(back);
            back.Position = uiManager.setPosition(new Vector2(1f, 0f));
            back.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(0.5f, 0f));

            uiManager.LordUi(cursor);
            cursor.Position = uiManager.setPosition(new Vector2(1f, 0.12f));
            cursor.SinProduction.moveSpeed = 0.5f;

            returnMessage.SinProduction.GoalPoint = uiManager.setPosition(0.55f, 0.12f);
            retryMessage.SinProduction.GoalPoint = uiManager.setPosition(0.55f, 0.22f);
            optionMessage.SinProduction.GoalPoint = uiManager.setPosition(0.55f, 0.32f);
            exitMessage.SinProduction.GoalPoint = uiManager.setPosition(0.55f, 0.42f);
            uiManager.LordUi(returnMessage);
            uiManager.LordUi(retryMessage);
            uiManager.LordUi(optionMessage);
            uiManager.LordUi(exitMessage);
                        
            base.Initialize();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。
            uiManager.Update(gameTime);

            if (!gameDate.PauseFrag)
            {
                back.uiProduction.GoalPoint = uiManager.setPosition(new Vector2(1, 0));
                cursor.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(1f, 0.12f));
                returnMessage.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(1f, 0.12f));
                retryMessage.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(1f, 0.22f));
                optionMessage.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(1f, 0.32f));
                exitMessage.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(1f, 0.42f));
                if (back.Position == back.uiProduction.GoalPoint)
                {
                    game1.Components.Remove(this);
                    Initialize();
                }
            }
            else if(!game1.Components.Contains(game1.optionComponent))
            {
                if (ControllManager.KeyStart())
                    gameDate.PauseFrag = false;
                selectManager.Update();
                switch ((int)selectManager.NowSelect.Y)
                {
                    case 0:
                        //ゲームに戻るの位置
                        cursor.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(0.50f, 0.12f));
                        if (selectManager.selectOn)
                        {
                            gameDate.PauseFrag = false;
                        }

                        break;

                    case 1:
                        //やり直すの位置
                        cursor.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(0.50f, 0.22f));
                        if (selectManager.selectOn)
                        {
                            game1.gameComponent.InitializeFlag = true;
                        }

                        break;

                    case 2:
                        //設定の位置
                        cursor.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(0.50f, 0.32f));
                        if (selectManager.selectOn)
                        {
                            if (!game1.Components.Contains(game1.optionComponent))
                            {
                                game1.optionComponent = new OptionComponent(game1, back);
                                game1.Components.Add(game1.optionComponent);
                            }
                        }
                        
                        break;

                    case 3:
                        //ステージ選択に戻るの位置
                        cursor.SinProduction.GoalPoint = uiManager.setPosition(new Vector2(0.50f, 0.42f));
                        if (selectManager.selectOn)
                        {
                            if (game1.fade.Fade(true, 4))
                            {
                                game1.gameComponent.bgm.Stop(AudioStopOptions.AsAuthored);
                                game1.Scene = Scenes.Menu;
                            }
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
