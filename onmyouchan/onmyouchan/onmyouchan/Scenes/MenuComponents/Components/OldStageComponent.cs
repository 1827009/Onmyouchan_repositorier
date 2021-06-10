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


namespace onmyouchan.Old
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class StageComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;

        bool selected;

        public enum STAGE
        {
            brown,
            green,
            grave,
            None
        }

        GameDelegate gameEvent;
        private void OnGameEvent()
        {
            if (gameEvent != null)
                gameEvent();
        }

        static int stageNum = 3;

        Texture2D stageSelectBackTexture;

        public STAGE choice;

        private SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        Texture2D[] stageTexture = new Texture2D[stageNum];
        Vector2[] postion = new Vector2[stageNum];

        public StageComponent(Game1 game)
            : base(game)
        {
            // TODO: Construct any child components here
            game1=game;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Game.Content.Load<SpriteFont>(@"item/SpriteFont1");

            stageSelectBackTexture = Game.Content.Load<Texture2D>("BackGround/stage_select");

            stageTexture[0] = Game.Content.Load<Texture2D>("stage/bg_chiheisen_brown");
            stageTexture[1] = Game.Content.Load<Texture2D>("stage/bg_chiheisen_green");
            stageTexture[2] = Game.Content.Load<Texture2D>("stage/halloween_grave");

                base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            gameEvent += StartEvent;

            selected = false;
            choice = STAGE.brown;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            OnGameEvent();

            if (ControllManager.Camera_LeftKey(true) && choice > STAGE.brown)
                choice--;

            if (ControllManager.Camera_RightKey(true) && choice < STAGE.None - 1)
                choice++;

            if(ControllManager.KeyCancel())
                Game.Components.Remove(this);

            if (ControllManager.KeyDecide())
            {
                if (choice >= STAGE.brown && choice < STAGE.grave)
                {
                    game1.Scene = Scenes.Game;
                    //if (Game.Components.Contains(this))
                    //    Game.Components.Remove(this);
                    selected = true;
                }
            }

            base.Update(gameTime);
        }

        public bool IsSelected()
        {
            return selected;
        }

        public STAGE selectedStage()
        {
            if (!IsSelected())
                return STAGE.None;
            else
                return choice;

        }

        public override void Draw(GameTime gameTime)
        {
                Vector2 WindowSize = new Vector2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
                Vector2 textPos = new Vector2(60, WindowSize.Y - 60);

                float[] scale = new float[stageNum];

                for (int i = 0; i < stageNum; i++)
                    scale[i] = 0.4f;

                scale[(int)choice] = 0.6f;

                postion[0] = new Vector2(2 * WindowSize.X / 9, 3 * WindowSize.Y / 5);
                postion[1] = new Vector2(WindowSize.X / 2, WindowSize.Y / 3);
                postion[2] = new Vector2(7 * WindowSize.X / 9, 3 * WindowSize.Y / 5);

                GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin();

                spriteBatch.Draw(stageSelectBackTexture, new Rectangle(0, 0, (int)WindowSize.X, (int)WindowSize.Y), Color.White);

                switch (choice)
                {
                    case STAGE.brown:
                        spriteBatch.DrawString(spriteFont, "No.1\n\nGround", textPos, Color.Black);
                        break;

                    case STAGE.green:
                        spriteBatch.DrawString(spriteFont, "No.2\n\nWWWWWWWWWW", textPos, Color.Black);
                        break;

                    default:
                        spriteBatch.DrawString(spriteFont, "Please wait.", textPos, Color.Black);
                        break;
                }

                for (int i = 0; i < stageNum; i++)
                    spriteBatch.Draw(stageTexture[i], new Rectangle((int)(postion[i].X - stageTexture[i].Width * scale[i] / 2), (int)(postion[i].Y - stageTexture[i].Height * scale[i] / 2), (int)(stageTexture[i].Width * scale[i]), (int)(stageTexture[i].Height * scale[i])), Color.White);

                spriteBatch.End();

                base.Draw(gameTime);

        }

        private void StartEvent()
        {
            game1.SoundPlay("Sound\\Music\\Menu1");

            gameEvent -= StartEvent;
        }
    }
}
