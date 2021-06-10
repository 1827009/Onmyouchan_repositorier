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
    delegate bool GemePlayEvent(GameTime gameTime);
    /// <summary>
    /// IUpdateable インターフェイスを実装したゲーム コンポーネントです。
    /// ゲーム中のSceneです。細かいオブジェクトの制御などを行う各Managerクラスの更新などが行われています
    /// </summary>
    class GamePlayComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        GameDate gameDate;
        ScoreData scoreDate;
        GamePauseMenuConponents gamePauseMenuConponents;

        UiManager uiManager;
        public EntityManager entityManager;
        public Stage stage;

        Figure no;
        event GemePlayEvent Event;
        private bool OnGamePlayEvent(GameTime gameTime)
        {
            if (Event != null)
                return Event(gameTime);
            return false;
        }

        public GamePlayComponent(Game1 game, GameDate date, ScoreData score)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            this.game1 = game;
            gameDate = date;
            scoreDate = score;
            gamePauseMenuConponents = new GamePauseMenuConponents(game1, gameDate);
            entityManager = new EntityManager(game1);
            uiManager = new UiManager(game1);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// ゲーム コンポーネントの初期化を行います。
        /// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
        /// </summary>
        public override void Initialize()
        {
            // TODO: ここに初期化のコードを追加します。
            gamePauseMenuConponents.Initialize();
            if (game1.Components.Contains(gamePauseMenuConponents))
                game1.Components.Remove(gamePauseMenuConponents);
            entityManager.Initialize();
            uiManager.Initialize();
            stage = new Stage(game1, entityManager, new Vector2(2750, 1100), gameDate, scoreDate);

            StartCount = 4;
            entityManager.ControlFlag = false;
            no = new Figure(uiManager.setPosition(new Vector2(0.5f, 0.5f)), (int)0, 0.4f);
            uiManager.LordUi(no);

            entityManager.LordMainCamera(new MainCamera());
            entityManager.LordPlayer(new Player(new Vector3(500, 100, 0), scoreDate));
            entityManager.mainCamera.BelongsEntity = entityManager.player;

            if (game1.gameComponent.bgm != null)
                game1.gameComponent.bgm.Stop(AudioStopOptions.AsAuthored);
            stage.SetStage(game1.gameComponent.difficulty, ref game1.gameComponent.bgm);
            
            uiManager.LordUi(new TimeGage(gameDate));
            uiManager.LordUi(new OhudaIcon(entityManager.player));
            uiManager.LordUi(new ItemUi(entityManager.player));
            uiManager.LordUi(new Item2Ui(entityManager.player));

            Event = new GemePlayEvent(StartCountEvent);

            base.Initialize();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。
            if (ControllManager.KeyStart())
            {
                if (!game1.Components.Contains(gamePauseMenuConponents))
                {
                    game1.Components.Add(gamePauseMenuConponents);
                    gameDate.PauseFrag = true;
                }
            }
            if (gameDate.PauseFrag)
                return;

            if (OnGamePlayEvent(gameTime))
                TimeMinus(gameTime);

            uiManager.Update(gameTime);
            entityManager.Update(gameTime);
            stage.Update();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // TODO: ここに描画コードを追加します。
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            entityManager.Draw(gameTime);
            uiManager.Draw();

            base.Draw(gameTime);
        }

        /// <summary>
        /// 制限時間の更新
        /// </summary>
        /// <param name="damage"></param>
        public void TimeMinus(GameTime gameTime)
        {
            if (gameDate.RemainingTime > 0)
            {
                gameDate.RemainingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds + entityManager.player.damage * (float)gameTime.ElapsedGameTime.TotalSeconds;
                entityManager.player.damage = 0;
            }
        }

        float StartCount;
        /// <summary>
        /// スタート時のカウントダウン
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private bool StartCountEvent(GameTime gameTime)
        {
            if (StartCount > 0)
            {
                StartCount -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                no.No = (int)StartCount;
                return false;
            }
            else
            {
                entityManager.ControlFlag = true;
                uiManager.UiList.Remove(no);
                Event -= this.StartCountEvent;
                Event += this.FinishCountEvent;
                return true;
            }
        }
        /// <summary>
        /// 終了前のカウントダウン
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private bool FinishCountEvent(GameTime gameTime)
        {
            if ((int)gameDate.RemainingTime < 3 && gameDate.RemainingTime > 0)
            {
                if (!uiManager.UiList.Contains(no))
                    uiManager.LordUi(no);
                no.No = (int)gameDate.RemainingTime + 1;
            }
            else if (gameDate.RemainingTime < 0)
            {
                Event -= this.FinishCountEvent;
                game1.gameComponent.ClearFlag = true;
            }
            return true;
        }
    }

}