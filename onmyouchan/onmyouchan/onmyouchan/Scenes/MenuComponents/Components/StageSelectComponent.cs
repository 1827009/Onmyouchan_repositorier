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

using onmyouchan.UI;

namespace onmyouchan
{
    /// <summary>
    /// IUpdateable インターフェイスを実装したゲーム コンポーネントです。
    /// ステージセレクトのSceneです
    /// </summary>
    class StageSelectComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        MenuComponent menuComponent;
        UiManager uiManager;
        SelectManager selectManager;

        BackGround back;
        QuestPage_v2 questPage;
        HorizontalCursor cursor;

        SimpleFont easy;
        SimpleFont normal;
        SimpleFont hard;
        SimpleFont returnMenu;

        int stageSelectPhase;

        public StageSelectComponent(Game1 game,MenuComponent menu)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;
            menuComponent = menu;
            uiManager = new UiManager(game);
            selectManager = new SelectManager(1, 4);
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

            back = new BackGround(uiManager.TextureLord("Image\\BackGround\\StageSelect"));
            questPage = new QuestPage_v2();
            cursor = new HorizontalCursor();
            cursor.SinProduction.moveSpeed = 0.5f;

            easy = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "下級", uiManager.setPosition(new Vector2(0.15f, 0.15f)));
            easy.Size = 1.3f;
            normal = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "中級", uiManager.setPosition(new Vector2(0.15f, 0.3f)));
            normal.Size = 1.3f;
            hard = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "上級", uiManager.setPosition(new Vector2(0.15f, 0.45f)));
            hard.Size = 1.3f;
            returnMenu = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "戻る", uiManager.setPosition(new Vector2(0.05f, 0.8f)));

            stageSelectPhase = 0;

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

            switch (stageSelectPhase)
            {
                case 0:
                    uiManager.LordUi(back);
                    uiManager.LordUi(questPage);
                    uiManager.LordUi(cursor);
                    uiManager.LordUi(easy);
                    uiManager.LordUi(normal);
                    uiManager.LordUi(hard);
                    uiManager.LordUi(returnMenu);
                    stageSelectPhase++;
                    selectManager.NowSelect = new Vector2(0, 0);
                    game1.fade.Fade(false, 5);
                    break;
                case 1:
                    if (game1.fade2.Fade(false, 5))
                    {
                        selectManager.Update();
                        SelectStage();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            uiManager.Draw();
            base.Draw(gameTime);
        }

        private void SelectStage()
        {
            if (ControllManager.KeyCancel())
                selectManager.NowSelect = new Vector2(0,3);

            if (selectManager.NowSelect.Y != 3)
                questPage.Select = (int)selectManager.NowSelect.Y;

            switch ((int)selectManager.NowSelect.Y)
            {
                case 0:
                    if (selectManager.selectOn)
                        EasySelectEvent();
                    if (cursor.SinProduction.MoveOn)
                        cursor.SinProduction.GoalPoint = uiManager.setPosition(0.1f, 0.18f);
                    break;

                case 1:
                    if (selectManager.selectOn)
                        NormalSelectEvent();
                    if (cursor.SinProduction.MoveOn)
                        cursor.SinProduction.GoalPoint = uiManager.setPosition(0.1f, 0.33f);
                    break;

                case 2:
                    if (selectManager.selectOn)
                        HardSelectEvent();
                    if (cursor.SinProduction.MoveOn)
                        cursor.SinProduction.GoalPoint = uiManager.setPosition(0.1f, 0.48f);
                    break;

                case 3:
                    if (selectManager.selectOn)
                        ReturnEvent();
                    if (cursor.SinProduction.MoveOn)
                        cursor.SinProduction.GoalPoint = uiManager.setPosition(0.02f, 0.81f);
                    break;
            }
        }

        private void EasySelectEvent()
        {
            if (game1.fade.Fade(true, 5))
                game1.Scene = Scenes.Game;
            game1.gameComponent.difficulty = 0;
            game1.menuComponent.bgm.Stop(AudioStopOptions.AsAuthored);
        }
        private void NormalSelectEvent()
        {
            if (game1.fade.Fade(true, 5))
                game1.Scene = Scenes.Game;
            game1.gameComponent.difficulty = 1;
            game1.menuComponent.bgm.Stop(AudioStopOptions.AsAuthored);
        }
        private void HardSelectEvent()
        {
            if (game1.fade.Fade(true, 5))
                game1.Scene = Scenes.Game;
            game1.gameComponent.difficulty = 2;
            game1.menuComponent.bgm.Stop(AudioStopOptions.AsAuthored);
        }
        private void ReturnEvent()
        {
            if (game1.fade.Fade(true, 5))
                menuComponent.Scene = MenuScenes.Title;
            game1.menuComponent.bgm.Stop(AudioStopOptions.AsAuthored);
        }
    }
}

