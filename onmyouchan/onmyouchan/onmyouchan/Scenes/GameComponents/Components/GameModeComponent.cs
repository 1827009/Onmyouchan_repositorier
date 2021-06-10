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
    /// <summary>
    /// IUpdateable インターフェイスを実装したゲーム コンポーネントです。
    /// ゲームのシーンを制御しています
    /// </summary>
    class GameModeComponent : Microsoft.Xna.Framework.GameComponent
    {
        Game1 game1;
        public Cue bgm;
        public Cue se;

        #region コンポーネント

        public GamePlayComponent gamePlayComponent;
        public GameResultComponent gameResultComponent;

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
        /// <summary>
        /// 難易度
        /// </summary>
        public int difficulty;
               
        public GameModeComponent(Game1 game)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;

            gameDate = new GameDate();
            scoreDate = new ScoreData();

            gamePlayComponent = new GamePlayComponent(game1, gameDate, scoreDate);
            gameResultComponent = new GameResultComponent(game1, gameDate, scoreDate);
        }
        
        /// <summary>
        /// ゲーム コンポーネントの初期化を行います。
        /// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
        /// </summary>
        public override void Initialize()
        {
            // TODO: ここに初期化のコードを追加します。
            gamePlayComponent.Initialize();
            if (game1.Components.Contains(gamePlayComponent))
                game1.Components.Remove(gamePlayComponent);

            gameResultComponent.Initialize();
            if (game1.Components.Contains(gameResultComponent))
                game1.Components.Remove(gameResultComponent);

            gameDate.Initialize();
            scoreDate.Initialize();
            
            phase = GamePhase.start;

            gameDate.MaxRemainingTime = 180;
            gameDate.RemainingTime = 180;
            gameDate.GoalKillCount = 6;

            initializeFlag = false;
            clearFlag = false;

            base.Initialize();
        }

        public bool InitializeFlag
        {
            get { return initializeFlag; }
            set { initializeFlag = value; }
        }
        bool initializeFlag;
        public bool ClearFlag
        {
            get { return clearFlag; }
            set { clearFlag = value; }
        }
        bool clearFlag;
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
                    if (!game1.fade.On)
                    {
                        if (!game1.Components.Contains(gamePlayComponent))
                            game1.Components.Add(gamePlayComponent);

                        if (game1.fade.Fade(false, 4))
                            phase = GamePhase.nowGame;
                    }
                    break;

                case GamePhase.nowGame:
                    if (ClearFlag)
                    {
                        if (game1.fade2.Fade(true, 50))
                            phase = GamePhase.GameFinish;
                    }
                    else if (initializeFlag)
                    {
                        if (game1.fade.Fade(true, 4))
                            this.Initialize();
                    }
                    break;

                case GamePhase.GameFinish:
                    if (!game1.fade2.On)
                    {
                        bgm.Stop(AudioStopOptions.AsAuthored);
                        if (game1.Components.Contains(gamePlayComponent))
                            game1.Components.Remove(gamePlayComponent);
                        if (!game1.Components.Contains(gameResultComponent))
                            game1.Components.Add(gameResultComponent);
                        phase = GamePhase.Result;
                        game1.weitTime.Weit = 0.7f;
                    }
                    break;

                case GamePhase.Result:
                    if (ControllManager.KeyDecide())
                    {
                        phase = GamePhase.Finish;
                    }
                    break;

                case GamePhase.Finish:
                    if (game1.fade2.Fade(true, 5))
                    {
                        game1.weitTime.Weit = 0.7f;
                        game1.Components.Remove(gameResultComponent);
                        bgm.Stop(AudioStopOptions.AsAuthored);
                        game1.Scene = Scenes.Menu;
                    }
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
            pauseFrag = false;
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

        public int ItemCount
        {
            get { return itemCount; }
            set { itemCount = value; }
        }
        int itemCount;

        public void Initialize()
        {
            borderCount = 0;
            itemCount = 0;
        }
    }
}
