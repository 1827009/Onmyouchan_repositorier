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
    class GamePauseMenuConponents : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        UiManager uiManager;
        SelectManager selectManager;

        PauseBackUi back;
        SelectCursor cursor;

        SelectCursor cursor2;
        SelectCursor cursor3;
        SelectCursor cursor4;

        public GamePauseMenuConponents(Game1 game)
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
            uiManager = new UiManager(game1);
            selectManager = new SelectManager(new Vector2(4, 0));

            back = new PauseBackUi();
            uiManager.LordUi(back);
            cursor = new SelectCursor(uiManager.setPosition(new Vector2(0.58f, 0.1f)));
            uiManager.LordUi(cursor);
                        
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

            switch ((int)selectManager.NowSelect.X)
            {
                case 0:
                    cursor.MoveCursor(uiManager.setPosition(new Vector2(0.58f, 0.1f)));
                    break;
                case 1:
                    cursor.MoveCursor(uiManager.setPosition(new Vector2(0.68f, 0.1f)));
                    break;
                case 2:
                    cursor.MoveCursor(uiManager.setPosition(new Vector2(0.78f, 0.1f)));
                    break;
                case 3:
                    cursor.MoveCursor(uiManager.setPosition(new Vector2(0.88f, 0.1f)));
                    break;
                case 4:
                    cursor.MoveCursor(uiManager.setPosition(new Vector2(0.98f, 0.1f)));
                    break;
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
