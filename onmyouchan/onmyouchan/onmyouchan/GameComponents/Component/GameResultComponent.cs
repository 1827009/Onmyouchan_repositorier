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
    /// </summary>
    class GameResultComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        GameDate gameDate;
        ScoreData scoreDate;

        UiManager uiManager;

        List<int> ranking;
        const int rankingMemory = 3;

        public GameResultComponent(Game1 game,GameDate date,ScoreData score)
            : base(game)
        {
            // TODO: Construct any child components here
            this.game1 = game;
            gameDate = date;
            scoreDate = score;
            uiManager = new UiManager(game1);

            ranking = new List<int>();
        }

        protected override void LoadContent()
        {
            uiManager.TextureLords("GameUi\\No\\0", "GameUi\\No\\1", "GameUi\\No\\0", "GameUi\\No\\2", "GameUi\\No\\3", "GameUi\\No\\4", "GameUi\\No\\5", "GameUi\\No\\6", "GameUi\\No\\7", "GameUi\\No\\8", "GameUi\\No\\9");        
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            #region スコア計算

            scoreDate.RemainingTime = gameDate.MaxRemainingTime - gameDate.RemainingTime;
            int overall = 18000;
            //制限時間の反映
            overall -= (int)scoreDate.RemainingTime;
            //結界の生成数の反映
            overall -= scoreDate.BorderCount * 100;
            //アイテムの使用数の反映
            overall -= scoreDate.ItemCount * 500;
            /*
            if (17880 <= overall)
                uiManager.LordUi(new Figure(new Vector2(73, 35), "S"));
            else if (17450 <= overall)
                uiManager.LordUi(new Figure(new Vector2(73, 35), "A"));
            else if (15870 <= overall)
                uiManager.LordUi(new Figure(new Vector2(73, 35), "B"));
            else
                uiManager.LordUi(new Figure(new Vector2(73, 35), "C"));
            */

            ranking.Add(overall);
            ranking.Sort();
            if (ranking.Count >= rankingMemory)
                ranking.RemoveRange(rankingMemory, ranking.Count - rankingMemory);

            #endregion
                        
            uiManager.LordUi(new ResultBack());
            uiManager.LordUi(new Figure(uiManager.setPosition(new Vector2(0.16f, 0.44f)), scoreDate.BorderCount, 0.17f));
            uiManager.LordUi(new Figure(uiManager.setPosition(new Vector2(0.50f, 0.43f)), (int)scoreDate.RemainingTime, 0.17f));
            uiManager.LordUi(new Figure(uiManager.setPosition(new Vector2(0.50f, 0.78f)), scoreDate.ItemCount, 0.17f));

            for (int i = 0; i < ranking.Count; i++)
            {
                uiManager.LordUi(new Figure(uiManager.setPosition(new Vector2(0.83f, 0.45f + i * 0.18f)), ranking[i], 0.17f));
            }

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
