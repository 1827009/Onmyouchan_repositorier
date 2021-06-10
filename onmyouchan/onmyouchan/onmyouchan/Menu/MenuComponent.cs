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

namespace onmyouchan
{
    /// <summary>
    /// IUpdateable インターフェイスを実装したゲーム コンポーネントです。
    /// </summary>
    class MenuComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        
        private SpriteBatch spriteBatch;

        private Texture2D titleLogoTexture;
        private Texture2D menuStartTexture;
        private Texture2D menuOptionTexture;
        private Texture2D menuExitTexture;

        private Texture2D backGroundTexture;

        StageComponent stageComponent;
        OptionComponent optionComponent;

        public enum Menu
        { 
            Start,
            Option,
            Exit
        }

        GameDelegate gameEvent;
        private void OnGameEvent()
        {
            if (gameEvent != null)
                gameEvent();
        }

        Menu menu = Menu.Start;
        bool selected;
        
        public MenuComponent(Game1 game)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;
            stageComponent = new StageComponent(game1);
            optionComponent = new OptionComponent(game1);
        }

        /// <summary>
        /// ゲーム コンポーネントの初期化を行います。
        /// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
        /// </summary>
        public override void Initialize()
        {
            // TODO: ここに初期化のコードを追加します。
            
            stageComponent.Initialize();
            optionComponent.Initialize();

            gameEvent = new GameDelegate(StartEvent);

            menu = Menu.Start;
            selected = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            backGroundTexture = Game.Content.Load<Texture2D>("BackGround/title_background");
            titleLogoTexture = Game.Content.Load<Texture2D>("item/title_logo");

            menuStartTexture = Game.Content.Load<Texture2D>("item/start_button");
            menuOptionTexture = Game.Content.Load<Texture2D>("item/help_button");
            menuExitTexture = Game.Content.Load<Texture2D>("item/exit_white");

            //base.LoadContent();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。

            OnGameEvent();

            if (stageComponent.selectedStage() != StageComponent.STAGE.None)
            {
                Game.Components.Remove(stageComponent);
                Game.Components.Remove(this);
            }
            
            if (IsSelected())
            {
                switch (menu)
                {
                    case Menu.Start:
                        if (!Game.Components.Contains(stageComponent))
                            Game.Components.Add(stageComponent);
                            break;

                    case Menu.Option:
                        if (!Game.Components.Contains(optionComponent))
                            Game.Components.Add(optionComponent);
                        break;

                    case Menu.Exit:
                        Game.Exit();
                        break;
                }
                selected = false;
            }
            else
            {
                if (!Game.Components.Contains(stageComponent) && !Game.Components.Contains(optionComponent))
                {
                    if (ControllManager.Camera_UpKey(true))
                    {
                        if (Menu.Start < menu)
                            menu--;
                    }
                    else if (ControllManager.Camera_DownKey(true))
                    {
                        if (Menu.Exit > menu)
                            menu++;
                    }
                    else if (ControllManager.KeyDecide())
                    {
                        selected = true;
                    }
                    else if (ControllManager.KeyCancel())
                        menu = Menu.Exit;
                }
                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 WindowSize = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);

            float titleScale = 0.5f;
            float startScale = 0.25f;
            float optionScale = 0.25f;
            float exitScale = 0.5f;

            float selectSpace = 100f;

            GraphicsDevice.Clear(Color.CornflowerBlue);            

            spriteBatch.Begin();
            spriteBatch.Draw(backGroundTexture, new Rectangle(0, 0, (int)WindowSize.X,(int)WindowSize.Y), Color.White);
            //spriteBatch.Draw(titleLogoTexture, new Rectangle((int)(WindowSize.X / 2 - titleLogoTexture.Width * titleScale / 2), 0, (int)(titleLogoTexture.Width * titleScale), (int)(titleLogoTexture.Height * titleScale)), Color.White);
            spriteBatch.Draw(titleLogoTexture, new Rectangle((int)WindowSize.X / 3, 0, (int)(titleLogoTexture.Width * titleScale), (int)(titleLogoTexture.Height * titleScale)), Color.White);
            if (!IsSelected())
            {
                switch (menu)
                {
                    case Menu.Start:
                        startScale = 0.5f;
                        break;

                    case Menu.Option:
                        optionScale = 0.5f;
                        break;

                    case Menu.Exit:
                        exitScale = 1.0f;
                        break;
                }
                spriteBatch.Draw(menuStartTexture,new Rectangle((int)(WindowSize.X / 2 - menuStartTexture.Width * startScale / 2), (int)(WindowSize.Y / 2 - menuStartTexture.Height * startScale / 2), (int)(menuStartTexture.Width * startScale), (int)(menuStartTexture.Height * startScale)),Color.White);
                spriteBatch.Draw(menuOptionTexture, new Rectangle((int)(WindowSize.X / 2 - menuOptionTexture.Width * optionScale / 2),  (int)(WindowSize.Y / 2 - menuOptionTexture.Height * optionScale / 2 + selectSpace), (int)(menuOptionTexture.Width * optionScale), (int)(menuOptionTexture.Height * optionScale)), Color.White);
                spriteBatch.Draw(menuExitTexture, new Rectangle((int)(WindowSize.X / 2 - menuExitTexture.Width * exitScale / 2), (int)(WindowSize.Y / 2 - menuExitTexture.Height * exitScale / 2 + 2 * selectSpace), (int)(menuExitTexture.Width * exitScale), (int)(menuExitTexture.Height * exitScale)), Color.White);            
            }
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        public StageComponent.STAGE stageSelect()
        {
            return stageComponent.selectedStage();
        }

        public Menu selectedMenu 
        {
            get { return menu; }
        }

        public bool IsSelected()
        {
            return selected;
        }

        private void StartEvent()
        {
            game1.SoundPlay("Sound\\Music\\TitleBGM1");
            
            gameEvent -= StartEvent;
        }
    }
}
