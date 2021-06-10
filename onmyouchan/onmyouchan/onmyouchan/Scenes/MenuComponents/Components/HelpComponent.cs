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
using onmyouchan.UI;

namespace onmyouchan
{
    /// <summary>
    /// IUpdateable インターフェイスを実装したゲーム コンポーネントです。
    /// ヘルプの画面を表示します。
    /// </summary>
    class HelpComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        UiManager uiManager;
        SelectManager selectManager;

        HelpUi helpUi1;
        HelpUi helpUi2;
        HelpUi helpUi3;
        HelpUi helpUi4;

        bool endFrag;

        public HelpComponent(Game1 game)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;
            uiManager = new UiManager(game1);
            selectManager = new SelectManager(4, 0);
        }

        /// <summary>
        /// ゲーム コンポーネントの初期化を行います。
        /// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
        /// </summary>
        public override void Initialize()
        {
            // TODO: ここに初期化のコードを追加します。
            uiManager.Initialize();
            selectManager.Initialize();
            selectManager.NowSelect = new Vector2(3, 0);
            endFrag = false;

            helpUi1 = new HelpUi(uiManager.TextureLord("Image\\Item\\op"));
            helpUi2 = new HelpUi(uiManager.TextureLord("Image\\Item\\howtogame"));
            helpUi3 = new HelpUi(uiManager.TextureLord("Image\\Item\\howtoplay2"));
            helpUi4 = new HelpUi(uiManager.TextureLord("Image\\Item\\howtoplayt3"));
            helpUi1.production.GoalPoint = uiManager.setPosition(0, 0);
            helpUi2.production.GoalPoint = uiManager.setPosition(-1, 0);
            helpUi3.production.GoalPoint = uiManager.setPosition(-2, 0);
            helpUi4.production.GoalPoint = uiManager.setPosition(-3, 0);
            helpUi1.Position = helpUi1.production.GoalPoint;
            helpUi2.Position = helpUi2.production.GoalPoint;
            helpUi3.Position = helpUi3.production.GoalPoint;
            helpUi4.Position = helpUi4.production.GoalPoint;

            uiManager.LordUi(helpUi1);
            uiManager.LordUi(helpUi2);
            uiManager.LordUi(helpUi3);
            uiManager.LordUi(helpUi4);

            game1.fade.Fade(false, 7);

            base.Initialize();
        }

        /// <summary>
        /// ゲーム コンポーネントが自身を更新するためのメソッドです。
        /// </summary>
        /// <param name="gameTime">ゲームの瞬間的なタイミング情報</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: ここにアップデートのコードを追加します。
            uiManager.Update(gameTime);
            selectManager.Update();

            if (ControllManager.KeyCancel() || ControllManager.KeyDecide())
            {
                endFrag = true;
            }
            if (endFrag)
            {
                if (game1.fade.Fade(true, 7))
                {
                    game1.menuComponent.Scene = MenuScenes.Title;
                    game1.Components.Remove(this);
                }
            }
            else
            {
                switch ((int)selectManager.NowSelect.X)
                {
                    case 3:
                        helpUi1.production.GoalPoint = uiManager.setPosition(0, 0);
                        helpUi2.production.GoalPoint = uiManager.setPosition(-1, 0);
                        helpUi3.production.GoalPoint = uiManager.setPosition(-2, 0);
                        helpUi4.production.GoalPoint = uiManager.setPosition(-3, 0);
                        break;
                    case 2:
                        helpUi1.production.GoalPoint = uiManager.setPosition(1, 0);
                        helpUi2.production.GoalPoint = uiManager.setPosition(0, 0);
                        helpUi3.production.GoalPoint = uiManager.setPosition(-1, 0);
                        helpUi4.production.GoalPoint = uiManager.setPosition(-2, 0);
                        break;
                    case 1:
                        helpUi1.production.GoalPoint = uiManager.setPosition(2, 0);
                        helpUi2.production.GoalPoint = uiManager.setPosition(1, 0);
                        helpUi3.production.GoalPoint = uiManager.setPosition(0, 0);
                        helpUi4.production.GoalPoint = uiManager.setPosition(-1, 0);
                        break;
                    case 0:
                        helpUi1.production.GoalPoint = uiManager.setPosition(3, 0);
                        helpUi2.production.GoalPoint = uiManager.setPosition(2, 0);
                        helpUi3.production.GoalPoint = uiManager.setPosition(1, 0);
                        helpUi4.production.GoalPoint = uiManager.setPosition(0, 0);
                        break;
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            uiManager.Draw();
            base.Draw(gameTime);
        }
    }
}
