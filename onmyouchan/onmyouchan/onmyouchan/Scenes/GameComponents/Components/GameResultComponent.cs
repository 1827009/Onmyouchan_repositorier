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
    /// This is a game component that implements IUpdateable.
    /// リザルトのSceneです。ここでそのままスコアを集計しています
    /// </summary>
    class GameResultComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        GameDate gameDate;
        ScoreData scoreDate;

        UiManager uiManager;

        ResultBack qestPaper;
        SimpleFont timeMessage;
        SimpleFont itemMessage;
        SimpleFont barrierMessage;
        ScoreStamp stamp;

        ScoreCharacter character;

        int overall;

        /// <summary>
        /// ゲームの状態
        /// </summary>
        public int Phase
        {
            get { return phase; }
        }
        int phase;

        public GameResultComponent(Game1 game,GameDate date,ScoreData score)
            : base(game)
        {
            // TODO: Construct any child components here
            this.game1 = game;
            gameDate = date;
            scoreDate = score;
            uiManager = new UiManager(game1);
        }

        protected override void LoadContent()
        {
            uiManager.TextureLords("Image\\GameUI\\No\\0", "Image\\GameUI\\No\\1", "Image\\GameUI\\No\\0", "Image\\GameUI\\No\\2", "Image\\GameUI\\No\\3", "Image\\GameUI\\No\\4", "Image\\GameUI\\No\\5", "Image\\GameUI\\No\\6", "Image\\GameUI\\No\\7", "Image\\GameUI\\No\\8", "Image\\GameUI\\No\\9");        
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            phase = 0;
            qestPaper = new ResultBack();
            timeMessage = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), gameDate.RemainingTime.ToString(), uiManager.setPosition(new Vector2(0.67f, 0.23f)));
            itemMessage = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), scoreDate.ItemCount.ToString(), uiManager.setPosition(new Vector2(0.67f, 0.31f)));
            barrierMessage = new SimpleFont(game1.Content.Load<SpriteFont>("Font\\SpriteFont"), scoreDate.BorderCount.ToString(), uiManager.setPosition(new Vector2(0.67f, 0.39f)));

            #region スコア計算

            overall = gameDate.KillCount * 300 + (int)gameDate.RemainingTime + scoreDate.ItemCount * -10 + scoreDate.BorderCount * -50;

            #endregion

            stamp = new ScoreStamp(overall);
            character = new ScoreCharacter(overall);
            stamp.Position = uiManager.setPosition(0.75f, 0.7f);
            character.Position = uiManager.setPosition(0.22f, 0.6f);
                        
            uiManager.LordUi(new BackGround(uiManager.TextureLord("Image\\BackGround\\ResultBack")));
            uiManager.LordUi(qestPaper);
            uiManager.LordUi(timeMessage);
            uiManager.LordUi(itemMessage);
            uiManager.LordUi(barrierMessage);
            uiManager.LordUi(stamp);
            uiManager.LordUi(character);

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            uiManager.Update(gameTime);

            switch (phase)
            {
                case 0:
                    game1.gameComponent.bgm = Game1.sound.GetCue("Result1");
                    game1.gameComponent.se = Game1.seSound.GetCue("dram");
                    game1.gameComponent.bgm.Play();
                    MyUtility.SEPlay(game1.gameComponent.se, "dram");
                    game1.fade2.Fade(false, 5);
                    phase = 1;
                    break;
                case 1:
                    int time = (int)gameDate.RemainingTime;
                    timeMessage.Str = time.ToString();
                    itemMessage.Str = scoreDate.ItemCount.ToString();
                    barrierMessage.Str = scoreDate.BorderCount.ToString();
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
