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
    enum MenuScenes
    {
        Title,
        Help,
        StageSelect
    }
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// ゲームのSceneをのぞいたSceneの制御をしています
    /// </summary>
    class MenuComponent : Microsoft.Xna.Framework.GameComponent
    {
        Game1 game1;
        public Cue bgm;

        TitleComponent titleComponent;
        HelpComponent helpComponent;
        StageSelectComponent stageSelectComponent;

        public MenuScenes Scene
        {
            get { return scene; }
            set { scene = value; }
        }
        MenuScenes scene;

        public MenuComponent(Game1 game)
            : base(game)
        {
            // TODO: Construct any child components here
            game1 = game;

            titleComponent = new TitleComponent(game1, this);
            helpComponent = new HelpComponent(game1);
            stageSelectComponent = new StageSelectComponent(game1,this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            switch (scene)
            {
                case MenuScenes.Title:
                    game1.fade.Fade(false, 7);

                    if (!game1.Components.Contains(titleComponent))
                    {
                        game1.Components.Remove(stageSelectComponent);
                        game1.Components.Add(titleComponent);
                        stageSelectComponent.Initialize();
                        if (bgm != null)
                            bgm.Stop(AudioStopOptions.AsAuthored);
                        bgm = Game1.sound.GetCue("TitleBGM2");
                        bgm.Play();
                    }
                    
                    break;

                case MenuScenes.Help:
                    if (!game1.Components.Contains(helpComponent))
                    {
                        if (game1.fade.Fade(true, 7))
                        {
                            game1.Components.Add(helpComponent);
                            helpComponent.Initialize();
                        }
                    }
                    break;

                case MenuScenes.StageSelect:
                    if (!game1.Components.Contains(stageSelectComponent))
                    {
                        game1.Components.Remove(titleComponent);
                        game1.Components.Add(stageSelectComponent);
                        titleComponent.Initialize();
                        if (bgm != null)
                            bgm.Stop(AudioStopOptions.AsAuthored);
                        bgm = Game1.sound.GetCue("Menu1");
                        bgm.Play();
                    }
                    break;
            }

            base.Update(gameTime);
        }
    }
}
