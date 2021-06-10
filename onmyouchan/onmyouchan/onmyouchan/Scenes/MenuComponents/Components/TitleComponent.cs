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
    /// タイトルのSceneです
    /// </summary>
    class TitleComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region フィールド

        Game1 game1;
        UiManager uiManager;
        SelectManager selectManager;
        MenuComponent menuComponent;

        int titlePhase;

        StartMessage startMessage;
        BackGround backGround;
        TitleLogo titleLogo;

        MakimonoFrame freame;
        HorizontalCursor cursor;
        SimpleFont startButton;
        SimpleFont helpButton;
        SimpleFont optionButton;
        SimpleFont exitButton;

        #endregion

        public TitleComponent(Game1 game, MenuComponent menu)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;
            menuComponent = menu;

            uiManager = new UiManager(game1);
            selectManager = new SelectManager(0, 4);
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

            backGround = new BackGround(uiManager.TextureLord("Image\\BackGround\\title_background"));
            titleLogo = new TitleLogo(uiManager.TextureLord("Image\\Item\\title_logo"));
            startMessage = new StartMessage(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "ボタンを押してね", uiManager.setPosition(new Vector2(0.475f, 0.37f)));
            startMessage.FontColor = new Color(0, 0, 0, 0);
            uiManager.LordUi(backGround);
            uiManager.LordUi(titleLogo);

            freame = new MakimonoFrame();
            cursor = new HorizontalCursor();
            cursor.Position = uiManager.setPosition(1f, 0.05f);
            cursor.SinProduction.GoalPoint = uiManager.setPosition(1f, 0.03f);
            cursor.SinProduction.moveSpeed = 0.5f;
            startButton = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "はじめる", uiManager.setPosition(new Vector2(1f, 0.05f)));
            startButton.SinProduction.moveSpeed = 0.1f;
            helpButton = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "あそびかた", uiManager.setPosition(new Vector2(1f, 0.15f)));
            helpButton.SinProduction.moveSpeed = 0.1f;
            optionButton = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "設定", uiManager.setPosition(new Vector2(1f, 0.25f)));
            optionButton.SinProduction.moveSpeed = 0.1f;
            exitButton = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), "おわる", uiManager.setPosition(new Vector2(1f, 0.35f)));
            exitButton.SinProduction.moveSpeed = 0.1f;

            titlePhase = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。
            uiManager.Update(gameTime);

            switch (titlePhase)
            {
                case 0:
                    titlePhase++;
                    break;
                case 1:
                    if (ControllManager.KeyDecide() || !titleLogo.Production(gameTime, true))
                    {
                        uiManager.LordUi(startMessage);
                        titleLogo.Production(true);
                        game1.fade.Fade(false, 5);
                        titlePhase++;
                    }
                    break;
                case 2:
                    if (ControllManager.KeyDecide())
                    {
                        startMessage.UnlordFrag = true;
                        titlePhase++;
                    }
                    break;
                case 3:
                    if (!titleLogo.Production(gameTime, false))
                        titleLogo.UnlordFrag = true;
                    if (!startMessage.TransparentProduction(gameTime, -2))
                        startMessage.UnlordFrag = true;

                    uiManager.LordUi(freame);
                    uiManager.LordUi(cursor);
                    uiManager.LordUi(startButton);
                    uiManager.LordUi(helpButton);
                    uiManager.LordUi(optionButton);
                    uiManager.LordUi(exitButton);
                    titlePhase++;
                    break;
                case 4:
                    titleLogo.UnlordFrag = true;
                    startMessage.UnlordFrag = true;
                    startButton.SinProduction.GoalPoint = uiManager.setPosition(0.68f, 0.05f);
                    helpButton.SinProduction.GoalPoint = uiManager.setPosition(0.68f, 0.15f);
                    optionButton.SinProduction.GoalPoint = uiManager.setPosition(0.68f, 0.25f);
                    exitButton.SinProduction.GoalPoint = uiManager.setPosition(0.68f, 0.35f);
                    if (exitButton.SinProduction.MoveOn)
                    {
                        SelectMenu();
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

        private void SelectMenu()
        {
            if (!game1.Components.Contains(game1.optionComponent) && menuComponent.Scene!= MenuScenes.Help)
            {
                selectManager.Update();
                switch ((int)selectManager.NowSelect.Y)
                {
                    case 0:
                        if (selectManager.selectOn)
                            StartEvent();
                        if (cursor.SinProduction.MoveOn)
                            cursor.SinProduction.GoalPoint = uiManager.setPosition(0.65f, 0.055f);
                        break;

                    case 1:
                        if (selectManager.selectOn)
                            HelpEvent();
                        if (cursor.SinProduction.MoveOn)
                            cursor.SinProduction.GoalPoint = uiManager.setPosition(0.65f, 0.155f);
                        break;

                    case 2:
                        if (selectManager.selectOn)
                            OptionEvent();
                        if (cursor.SinProduction.MoveOn)
                            cursor.SinProduction.GoalPoint = uiManager.setPosition(0.65f, 0.255f);
                        break;

                    case 3:
                        if (selectManager.selectOn)
                            ExitEvent();
                        if (cursor.SinProduction.MoveOn)
                            cursor.SinProduction.GoalPoint = uiManager.setPosition(0.65f, 0.355f);
                        break;
                }
            }
        }

        private void StartEvent()
        {
            if (game1.fade.Fade(true, 5))
                menuComponent.Scene = MenuScenes.StageSelect;
        }
        private void HelpEvent()
        {
            menuComponent.Scene = MenuScenes.Help;
        }
        private void OptionEvent()
        {
            if (!game1.Components.Contains(game1.optionComponent))
            {
                game1.optionComponent = new OptionComponent(game1);
                game1.Components.Add(game1.optionComponent);
            }
        }
        private void ExitEvent()
        {
            if (game1.fade.Fade(true, 5))
                game1.Exit();
        }
    }
}
