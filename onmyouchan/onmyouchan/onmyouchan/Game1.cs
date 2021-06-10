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
    public enum Scenes
    {
        Menu,
        Game
    }
    delegate void GameDelegate();
    /// <summary>
    /// 基底 Game クラスから派生した、ゲームのメイン クラスです。
    /// </summary>
    class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public SoundEffect soundEffect;
        public SoundEffectInstance soundInstance;
        public void SoundPlay(string soundName)
        {
            if (soundInstance != null)
                soundInstance.Stop();
            soundEffect=Content.Load<SoundEffect>(soundName);
            soundInstance = soundEffect.CreateInstance();
            soundInstance.Play();
        }
                
        /// <summary>
        /// 画像表示管理
        /// </summary>
        public UiManager uiManager;
        public FadeProduction fade;
        public FadeProduction2 fade2;

        /// <summary>
        /// 一定時間更新を完全停止する
        /// </summary>
        public WeitTime weitTime;

        public Scenes Scene
        {
            get { return scene; }
            set { scene = value; }
        }
        Scenes scene;

        public GameModeComponent gameComponent;
        public MenuComponent menuComponent;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //ウィンドウサイズ
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            
        }

        /// <summary>
        /// ゲームが実行を開始する前に必要な初期化を行います。
        /// ここで、必要なサービスを照会して、関連するグラフィック以外のコンテンツを
        /// 読み込むことができます。base.Initialize を呼び出すと、使用するすべての
        /// コンポーネントが列挙されるとともに、初期化されます。
        /// </summary>
        protected override void Initialize()
        {
            // TODO: ここに初期化ロジックを追加します。
            InputManager.Initialize();

            scene = Scenes.Menu;

            gameComponent = new GameModeComponent(this);
            menuComponent = new MenuComponent(this);

            if (!Components.Contains(menuComponent))
                Components.Add(menuComponent);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent はゲームごとに 1 回呼び出され、ここですべてのコンテンツを
        /// 読み込みます
        /// </summary>
        protected override void LoadContent()
        {
            // 新規の SpriteBatch を作成します。これはテクスチャーの描画に使用できます。
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // TODO: this.Content クラスを使用して、ゲームのコンテンツを読み込みます。
            uiManager = new UiManager(this);
            fade = new FadeProduction(this, true);
            fade2 = new FadeProduction2();
            weitTime = new WeitTime();

            uiManager.LordUi(fade);
            uiManager.LordUi(fade2);
            fade.Fade(false, 5);
        }

        /// <summary>
        /// UnloadContent はゲームごとに 1 回呼び出され、ここですべてのコンテンツを
        /// アンロードします。
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: ここで ContentManager 以外のすべてのコンテンツをアンロードします。
        }

        /// <summary>
        /// ワールドの更新、衝突判定、入力値の取得、オーディオの再生などの
        /// ゲーム ロジックを、実行します。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        protected override void Update(GameTime gameTime)
        {
            // ゲームの終了条件をチェックします。
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: ここにゲームのアップデート ロジックを追加します。
            if (weitTime.Update(gameTime))
                return;

            if (!fade.On)
                InputManager.Update();
            uiManager.Update(gameTime);

            switch (scene)
            {
                case Scenes.Menu:
                    ChangeComponents(menuComponent);
                    break;

                case Scenes.Game:
                    ChangeComponents(gameComponent);
                    break;
            }

            
            base.Update(gameTime);
        }

        /// <summary>
        /// ゲームが自身を描画するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // TODO: ここに描画コードを追加します。

            base.Draw(gameTime);
            uiManager.Draw();
        }

        private void ChangeComponents(GameComponent comp)
        {
            if (!Components.Contains(comp))
            {
                Components.Clear();
                Components.Add(comp);
                comp.Initialize();
            }
        }

        private void StartEvent()
        {

        }
    }

    class WeitTime
    {
        public float Weit
        {
            get{return weit;}
            set{weit=value;}
        }
        float weit;
        public bool Update(GameTime gameTime)
        {
            weit -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            return weit > 0;
        }
    }
}
