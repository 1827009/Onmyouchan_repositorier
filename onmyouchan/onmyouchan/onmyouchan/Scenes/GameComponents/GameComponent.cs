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
    public class GameComponent : Microsoft.Xna.Framework.GameComponent
    {        
        Game1 game1;

        GamePlayComponent gamePlayComponent;
        GameResultComponent gameResultComponent;

        OnmyouchanSystem system;

        public GameComponent(Game1 game)
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
            system = new OnmyouchanSystem(game1);

            gamePlayComponent = new GamePlayComponent(game1,system);
            gameResultComponent = new GameResultComponent(game1,system);
            
            base.Initialize();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。
            switch (system.Phase)
            {
                case GamePhase.start:
                    if (!game1.Components.Contains(gamePlayComponent))
                        game1.Components.Add(gamePlayComponent);
                    system.StartEvent(gamePlayComponent.entityManager,gamePlayComponent.uiManager);
                    break;

                case GamePhase.nowGame:
                    system.GameProcessing(gameTime);
                    break;

                case GamePhase.GameFinish:
                    if (game1.Components.Contains(gamePlayComponent))
                        game1.Components.Remove(gamePlayComponent);
                    if (!game1.Components.Contains(gameResultComponent))
                        game1.Components.Add(gameResultComponent);

                    system.SetUiManager = gameResultComponent.uiManager;
                    system.FinishEvent(gameTime);
                    break;

                case GamePhase.Result:
                    system.ResultEvent(gameTime);
                    break;

                case GamePhase.Finish:
                    if (!game1.Components.Contains(game1.menuComponent))
                        game1.Components.Add(game1.menuComponent);

                    if (game1.Components.Contains(gamePlayComponent))
                        game1.Components.Remove(gamePlayComponent);
                    if (game1.Components.Contains(gameResultComponent))
                        game1.Components.Remove(gameResultComponent);
                    if (game1.Components.Contains(this))
                        game1.Components.Remove(this);
                    break;
            }

            base.Update(gameTime);
        }

        public GamePhase gameState()
        {
            return system.Phase;
        }
    }
}
