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
    class GameModeComponent : Microsoft.Xna.Framework.GameComponent
    {        
        Game1 game1;

        #region コンポーネント

        GamePlayComponent gamePlayComponent;
        GamePauseMenuConponents gamePauseMenuConponents;
        GameResultComponent gameResultComponent;

        #endregion

        /// <summary>
        /// ゲームの状態
        /// </summary>
        public GamePhase Phase
        {
            get { return phase; }
        }
        GamePhase phase;
        /// <summary>
        /// スコアデータ
        /// </summary>
        public ScoreData scoreDate;

        public GameDate gameDate;

        public GameModeComponent(Game1 game)
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
            scoreDate = new ScoreData();
            gameDate = new GameDate();

            gamePlayComponent = new GamePlayComponent(game1, gameDate, scoreDate);
            gamePauseMenuConponents = new GamePauseMenuConponents(game1);
            gameResultComponent = new GameResultComponent(game1, gameDate, scoreDate);

            phase = GamePhase.start;

            gameDate.MaxRemainingTime = 1000;
            gameDate.RemainingTime = 1000;
            gameDate.GoalKillCount = 2;
            
            base.Initialize();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。
            switch (phase)
            {
                case GamePhase.start:
                    if (!game1.Components.Contains(gamePlayComponent))
                        game1.Components.Add(gamePlayComponent);

                    phase = GamePhase.nowGame;
                    break;

                case GamePhase.nowGame:
                    if (InputManager.IsJustKeyDown(Keys.Escape))
                    {
                        if (!gameDate.PauseFrag)
                        {
                            gameDate.PauseFrag = true;
                            if (!game1.Components.Contains(gamePauseMenuConponents))
                                game1.Components.Add(gamePauseMenuConponents);
                        }
                        else
                        {
                            gameDate.PauseFrag = false;
                            if (game1.Components.Contains(gamePauseMenuConponents))
                                game1.Components.Remove(gamePauseMenuConponents);
                        }
                    }

                    if (gameDate.KillCount > gameDate.GoalKillCount || gameDate.RemainingTime <= 0)
                    {
                        game1.fade_InOut.Fade(true, 4);
                        phase = GamePhase.GameFinish;
                    }
                    break;

                case GamePhase.GameFinish:
                    if (!game1.fade_InOut.On)
                    {
                        if (game1.Components.Contains(gamePlayComponent))
                            game1.Components.Remove(gamePlayComponent);
                        phase = GamePhase.Result;
                    }
                    break;

                case GamePhase.Result:
                    if (!game1.Components.Contains(gameResultComponent))
                        game1.Components.Add(gameResultComponent);
                    game1.fade_InOut.Fade(false, 4);
                    if (ControllManager.KeyDecide())
                        phase = GamePhase.Finish;
                    break;

                case GamePhase.Finish:
                    game1.fade_InOut.Fade(false, 4);        
                    #region コンポーネントの除外・有効化

                    if (!game1.Components.Contains(game1.menuComponent))
                        game1.Components.Add(game1.menuComponent);

                    if (game1.Components.Contains(gamePlayComponent))
                    {
                        gamePlayComponent.Initialize();
                        game1.Components.Remove(gamePlayComponent);
                    }
                    if (game1.Components.Contains(gameResultComponent))
                    {
                        gameResultComponent.Initialize();
                        game1.Components.Remove(gameResultComponent);
                    }
                    if (game1.Components.Contains(this))
                    {
                        this.Initialize();
                        game1.Components.Remove(this);
                    }

                    #endregion
                    phase = GamePhase.Result;

                    break;
            }

            base.Update(gameTime);
        }
    }

    /// <summary>
    /// ゲームのフェイズ
    /// </summary>
    public enum GamePhase
    {
        start,
        nowGame,
        GameFinish,
        Result,
        Finish
    }

    /// <summary>
    /// ゲームのコンポーネント間で受け渡すデータ
    /// </summary>
    class GameDate
    {
        /// <summary>
        /// ポーズなう
        /// </summary>
        public bool PauseFrag
        {
            get { return pauseFrag; }
            set { pauseFrag = value; }
        }
        bool pauseFrag;

        /// <summary>
        /// 討伐数
        /// </summary>
        public int KillCount
        {
            get { return killCount; }
            set { killCount = value; }
        }
        int killCount;
        public int GoalKillCount
        {
            get { return goalKillCount; }
            set { goalKillCount = value; }
        }
        int goalKillCount;
        
        /// <summary>
        /// 残り時間
        /// </summary>
        public float RemainingTime
        {
            get { return remainingTime; }
            set { remainingTime = value; }
        }
        float remainingTime;
        /// <summary>
        /// 初期時間制限
        /// </summary>
        public float MaxRemainingTime
        {
            get { return maxRemainingTime; }
            set { maxRemainingTime = value; }
        }
        float maxRemainingTime;

        public void Initialize()
        {
            killCount = 0;
            remainingTime = 0;
            maxRemainingTime = 0;
        }
    }

    /// <summary>
    /// スコア計算に使うデータのセット
    /// </summary>
    class ScoreData
    {
        public int BorderCount
        {
            get { return borderCount; }
            set { borderCount = value; }
        }
        int borderCount;

        public float RemainingTime
        {
            get { return remainingTime; }
            set { remainingTime = value; }
        }
        float remainingTime;

        public int ItemCount
        {
            get { return itemCount; }
            set { itemCount = value; }
        }
        int itemCount;
    }
}
