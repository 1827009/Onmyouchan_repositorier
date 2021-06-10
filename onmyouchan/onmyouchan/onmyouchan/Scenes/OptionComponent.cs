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
    /// ゲームの設定のSceneです
    /// </summary>
    class OptionComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Game1 game1;
        UiManager uiManager;
        SineWave sineWave;
        SelectManager selectManager;

        SelectManager cameraSpeedSelectManager;

        PauseBackUi back;
        HorizontalCursor cursor;
        SimpleFont cameraOptionMassage;
        SimpleFont cameraReverse1Massage;
        SimpleFont cameraHorizontalReverseMassage;
        SimpleFont cameraReverse2Massage;
        SimpleFont cameraVerticalReverseMassage;
        SimpleFont cameraSpeedMassage;
        OptionGage cameraSpeedGage;
        SimpleFont quitMassage;

        int phase = 0;
        bool pause;
        bool cameraSpeedSelect;
        bool quitFrag;

        public OptionComponent(Game1 game)
            : base(game)
        {
            // TODO: ここで子コンポーネントを作成します。
            game1 = game;
            uiManager = new UiManager(game);
            back = new PauseBackUi();
        }
        public OptionComponent(Game1 game,PauseBackUi b)
            : base(game)
        {
            game1 = game;
            uiManager = new UiManager(game);
            back = b;
            pause = true;
        }

        /// <summary>
        /// ゲーム コンポーネントの初期化を行います。
        /// ここで、必要なサービスを照会して、使用するコンテンツを読み込むことができます。
        /// </summary>
        public override void Initialize()
        {
            // TODO: ここに初期化のコードを追加します。
            uiManager.Initialize();
            uiManager.LordUi(back);
            selectManager = new SelectManager(1, 4);
            
            cursor = new HorizontalCursor();
            cursor.Position = uiManager.setPosition(1, 0.2f);
            cursor.SinProduction.moveSpeed = 0.5f;
            cameraOptionMassage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "～カメラ設定～", uiManager.setPosition(1, 0.1f));
            cameraReverse1Massage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "縦操作：", uiManager.setPosition(1, 0.2f));
            if (Option.CameraOption_HorizontalReverse)
                cameraHorizontalReverseMassage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "反転", uiManager.setPosition(1.2f, 0.2f));
            else
                cameraHorizontalReverseMassage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "通常", uiManager.setPosition(1.2f, 0.2f));

            cameraReverse2Massage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "横操作：", uiManager.setPosition(1, 0.3f));
            if (Option.CameraOption_HorizontalReverse)
                cameraVerticalReverseMassage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "反転", uiManager.setPosition(1.2f, 0.2f));
            else
                cameraVerticalReverseMassage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "通常", uiManager.setPosition(1.2f, 0.2f));
            cameraSpeedMassage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "速度", uiManager.setPosition(1, 0.4f));
            cameraSpeedGage = new OptionGage(Option.CameraOption_Speed, uiManager.setPosition(1.1f, 0.4f));
            quitMassage = new SimpleFont(uiManager.ThisGame1.Content.Load<SpriteFont>("Font\\SpriteFont"), "戻る", uiManager.setPosition(1f, 0.6f));

            uiManager.LordUi(cursor);
            uiManager.LordUi(cameraOptionMassage);
            uiManager.LordUi(cameraReverse1Massage);
            uiManager.LordUi(cameraHorizontalReverseMassage);
            uiManager.LordUi(cameraVerticalReverseMassage);
            uiManager.LordUi(cameraReverse2Massage);
            uiManager.LordUi(cameraSpeedMassage);
            uiManager.LordUi(cameraSpeedGage);
            uiManager.LordUi(quitMassage);
            back.uiProduction.GoalPoint = uiManager.setPosition(0, 0);

            sineWave = new SineWave();
            sineWave.RotaSpeed = 1;

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
            switch(phase)
            {
                case 0:
                    StartEvent(gameTime);
                    break;
                case 1:
                    if (!cameraSpeedSelect)
                        SelectEvent();
                    else
                        CameraSpeedEvent();
                    break;
            }

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            uiManager.Draw();
            base.Draw(gameTime);
        }

        private void StartEvent(GameTime gameTime)
        {
            if (sineWave.Radian <= 0.9f)
            {
                back.Turn = (float)Math.Abs(sineWave.Sin(gameTime));
                cursor.SinProduction.GoalPoint = uiManager.setPosition(0.15f, 0.2f);
                cameraOptionMassage.SinProduction.GoalPoint = uiManager.setPosition(0.1f, 0.1f);
                cameraReverse1Massage.SinProduction.GoalPoint = uiManager.setPosition(0.15f, 0.2f);
                cameraHorizontalReverseMassage.SinProduction.GoalPoint = uiManager.setPosition(0.5f, 0.2f);
                cameraReverse2Massage.SinProduction.GoalPoint = uiManager.setPosition(0.15f, 0.3f);
                cameraVerticalReverseMassage.SinProduction.GoalPoint = uiManager.setPosition(0.5f, 0.3f);
                cameraSpeedMassage.SinProduction.GoalPoint = uiManager.setPosition(0.15f, 0.4f);
                cameraSpeedGage.sineProduction.GoalPoint = uiManager.setPosition(0.3f, 0.4f);
                quitMassage.SinProduction.GoalPoint = uiManager.setPosition(0.15f, 0.5f);
            }
            else
            {
                back.Turn = 1;
                phase = 1;
            }
        }
        private void SelectEvent()
        {
            selectManager.Update();
            switch ((int)selectManager.NowSelect.Y)
            {
                case 0:
                    cursor.SinProduction.GoalPoint = uiManager.setPosition(0.1f, 0.2f);
                    if (selectManager.selectOn)
                    {
                        selectManager.selectOn = false;
                        if (Option.CameraOption_HorizontalReverse)
                        {
                            cameraHorizontalReverseMassage.Str = "通常";
                            Option.CameraOption_HorizontalReverse = false;
                        }
                        else
                        {
                            cameraHorizontalReverseMassage.Str = "反転";
                            Option.CameraOption_HorizontalReverse = true;
                        }
                    }
                    break;
                case 1:
                    cursor.SinProduction.GoalPoint = uiManager.setPosition(0.1f, 0.3f);
                    if (selectManager.selectOn)
                    {
                        selectManager.selectOn = false;
                        if (Option.CameraOption_VerticalReverse)
                        {
                            cameraVerticalReverseMassage.Str = "通常";
                            Option.CameraOption_VerticalReverse = false;
                        }
                        else
                        {
                            cameraVerticalReverseMassage.Str = "反転";
                            Option.CameraOption_VerticalReverse = true;
                        }
                    }
                    break;
                case 2:
                    cursor.SinProduction.GoalPoint = uiManager.setPosition(0.1f, 0.4f);
                    if (selectManager.selectOn)
                    {
                        selectManager.selectOn = false;
                        cameraSpeedSelect = true;
                        cameraSpeedSelectManager = new SelectManager(10, 1);
                        cameraSpeedSelectManager.NowSelect = new Vector2(Option.CameraOption_Speed, 0);
                    }
                    break;
                case 3:
                    cursor.SinProduction.GoalPoint = uiManager.setPosition(0.1f, 0.5f);
                    if (selectManager.selectOn)
                    {
                        quitFrag = true;
                    }
                    break;
            }
            if (ControllManager.KeyCancel())
                quitFrag = true;
            if (quitFrag)
                QuitEvent();
        }
        private void CameraSpeedEvent()
        {
            cameraSpeedSelectManager.Update();
            cameraSpeedGage.Step = (int)cameraSpeedSelectManager.NowSelect.X;
            if (cameraSpeedSelectManager.selectOn)
            {
                cameraSpeedSelectManager.selectOn = false;
                cameraSpeedSelect = false;
                Option.CameraOption_Speed = (int)cameraSpeedSelectManager.NowSelect.X;
            }
        }
        private void QuitEvent()
        {
            if (pause)
                back.uiProduction.GoalPoint = uiManager.setPosition(0.5f, 0);
            else
                back.uiProduction.GoalPoint = uiManager.setPosition(1.1f, 0);

            cursor.SinProduction.GoalPoint = uiManager.setPosition(1, 0.2f);
            cameraOptionMassage.SinProduction.GoalPoint = uiManager.setPosition(1.3f, 0.1f);
            cameraReverse1Massage.SinProduction.GoalPoint = uiManager.setPosition(1.3f, 0.2f);
            cameraHorizontalReverseMassage.SinProduction.GoalPoint = uiManager.setPosition(1.3f, 0.2f);
            cameraReverse2Massage.SinProduction.GoalPoint = uiManager.setPosition(1.3f, 0.3f);
            cameraVerticalReverseMassage.SinProduction.GoalPoint = uiManager.setPosition(1.3f, 0.3f);
            cameraSpeedMassage.SinProduction.GoalPoint = uiManager.setPosition(1.3f, 0.4f);
            cameraSpeedGage.sineProduction.GoalPoint = uiManager.setPosition(1.3f, 0.4f);
            quitMassage.SinProduction.GoalPoint = uiManager.setPosition(1.3f, 0.5f);
            if (back.uiProduction.MoveOn)
            {
                quitFrag = false;
                game1.Components.Remove(this);
            }
        }
    }
}
