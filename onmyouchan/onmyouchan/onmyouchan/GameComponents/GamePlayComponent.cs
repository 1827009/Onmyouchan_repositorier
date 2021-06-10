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
    class GamePlayComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        GameDate gameDate;
        ScoreData scoreDate;

        UiManager uiManager;
        
        public EntityManager entityManager;

        public GamePlayComponent(Game1 game,GameDate date,ScoreData score)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            this.game1 = game;
            gameDate = date;
            scoreDate = score;
        }

        /// <summary>
        /// ゲーム コンポーネントの初期化を行います。
        /// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
        /// </summary>
        public override void Initialize()
        {
            // TODO: ここに初期化のコードを追加します。
            entityManager = new EntityManager(game1);
            uiManager = new UiManager(game1);

            #region 仮初期化
            
            entityManager.LordCamera(new Camera());
            entityManager.LordPlayer(new Player(new Vector3(0,100,0),scoreDate));
            entityManager.LordEntity(new SampleTerrain(Vector3.Zero));
            entityManager.mainCamera.BelongsEntity = entityManager.player;
            uiManager.LordUi(new TimeGage(gameDate));
            uiManager.LordUi(new OhudaIcon(entityManager.player));

            entityManager.LordEntity(new GhostObject(new Vector3(-170, 100, 200), gameDate, scoreDate));
            entityManager.LordEntity(new GhostObject(new Vector3(570, 100, 50), gameDate, scoreDate));
            entityManager.LordEntity(new GhostObject(new Vector3(90, 100, -167), gameDate, scoreDate));
            entityManager.LordEntity(new GhostObject(new Vector3(-1700, 100, 250), gameDate, scoreDate));
            entityManager.LordEntity(new GhostObject(new Vector3(50, 100, 750), gameDate, scoreDate));
            entityManager.LordEntity(new GhostObject(new Vector3(80, 100, -1067), gameDate, scoreDate));

            #endregion
                                 
            base.Initialize();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。
            if (gameDate.PauseFrag)
                return;

            uiManager.Update(gameTime);
            entityManager.Update(gameTime);

            TimeMinus(gameTime);

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
                gameDate.RemainingTime -= 0.1f + entityManager.player.damage;
                entityManager.player.damage = 0;
            }
        }
    }

}
