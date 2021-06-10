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
    class OptionComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;

        private SpriteBatch spriteBatch;

        private Texture2D ruluBackTexture;
        private Texture2D optionTexture;

        public OptionComponent(Game1 game)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;
        }

        /// <summary>
        /// ゲーム コンポーネントの初期化を行います。
        /// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
        /// </summary>
        public override void Initialize()
        {
            // TODO: ここに初期化のコードを追加します。

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            optionTexture = Game.Content.Load<Texture2D>("item/op");
            ruluBackTexture = Game.Content.Load<Texture2D>("BackGround/rule_background");

            base.LoadContent();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。


            if(ControllManager.KeyCancel())
            {
                Game.Components.Remove(this);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 WindowSize = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            float optionScale = 0.6f;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.Draw(ruluBackTexture,new Rectangle(0,0,(int)WindowSize.X,(int)WindowSize.Y),Color.White);
            spriteBatch.Draw(optionTexture, new Rectangle((int)(WindowSize.X / 2 - optionTexture.Width * optionScale / 2), (int)(WindowSize.Y / 2 - optionTexture.Height * optionScale / 2), (int)(optionTexture.Width * optionScale), (int)(optionTexture.Height * optionScale)), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    
    }
}
