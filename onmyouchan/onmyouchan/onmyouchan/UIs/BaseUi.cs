using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace onmyouchan.UI
{
    /// <summary>
    /// 画像格納と表示をサポートするクラス
    /// </summary>
    class ImageStatus
    {
        /// <summary>
        /// テクスチャ
        /// </summary>
        public Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }
        Texture2D image;
        
        /// <summary>
        /// カラー
        /// </summary>
        public Color ImageColor
        {
            get { return color; }
            set { color = value; }
        }
        Color color;
        
        /// <summary>
        /// 0~1の範囲で中心座標
        /// </summary>
        public Vector2 CenterPosition
        {
            get { return centerPosition; }
            set { centerPosition = value; }
        }
        public Vector2 centerPosition;
        /// <summary>
        /// 短形
        /// </summary>
        public Rectangle ImageRectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }
        /// <summary>
        /// 短形
        /// </summary>
        public void SetImageRectangle(float x, float y)
        {
            rectangle.Width = (int)x;
            rectangle.Height = (int)y;
        }
        public void SetImageRectangle(Vector2 pos)
        {
            rectangle.Width = (int)pos.X;
            rectangle.Height = (int)pos.Y;
        }
        Rectangle rectangle;
        /// <summary>
        /// 大きさ
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        float scale = 1;

        public ImageStatus(Texture2D tex)
        {
            image=tex;
            rectangle.Width = image.Width;
            rectangle.Height = image.Height;
            color = Color.White;
        }

        /// <summary>
        /// 各ステータスを反映した短径
        /// </summary>
        /// <param name="uiManeger"></param>
        /// <returns></returns>
        public Rectangle DrawRectangle()
        {
            Rectangle output;
            output.Width = (int)(rectangle.Width * scale); output.Height = (int)(rectangle.Height * scale);
            output.X = (int)(-output.Width * centerPosition.X); output.Y = (int)(-output.Height * centerPosition.Y);

            return output;
        }
    }
    
    /// <summary>
    /// GameUiでメソッドを呼ぶための原型クラス
    /// </summary>
    class Ui
    {
        #region フィールド

        /// <summary>
        /// 所属しているUiManagerの参照
        /// </summary>
        protected UiManager thisUiManager;

        /// <summary>
        /// 画像や位置
        /// </summary>
        protected List<ImageStatus> images;

        /// <summary>
        /// 基本位置
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        /// <summary>
        /// 安全にEntityManagerの登録を解除するフラグ
        /// </summary>
        public bool UnlordFrag
        {
            get { return unlordFrag; }
            set { unlordFrag = value; }
        }
        bool unlordFrag;

        #endregion

        #region コンストラクタ

        public Ui()
        {
            images = new List<ImageStatus>();
        }

        #endregion

        #region 更新

        public virtual void Update(GameTime gameTime) { }

        #endregion

        #region 描画

        public virtual void Draw() 
        {
            for (int i = 0; i < images.Count; i++)
            {
                Rectangle r = images[i].DrawRectangle();
                r.X += (int)position.X; r.Y += (int)position.Y;
                thisUiManager.ThisGame1.spriteBatch.Draw(images[i].Image, r, images[i].ImageColor);
            }
        }

        #endregion

        #region 読み込み

        /// <summary>
        /// 継承したクラスはbaseを実行した後に処理をしてください
        /// </summary>
        /// <param name="uiManager"></param>
        public virtual void Lord(UiManager uiManager)
        {
            thisUiManager = uiManager;
        }

        /// <summary>
        /// EntityManagerの登録を解除します
        /// </summary>
        public virtual void Unlord()
        {
            unlordFrag = false;
        }

        #endregion

        #region メソッド
        
        /// <summary>
        /// nを0~255までの数値にTime秒かけて変化させる。動作中かを返す
        /// </summary>
        /// <param name="inOut"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        protected static bool StandOut(ref float n, float time, GameTime gameTime)
        {
            if (n <= 0 && time < 0)
            {
                n = 0;
                return false;
            }
            else if (n >= 255 && time >= 0)
            {
                n = 255;
                return false;
            }
            n += ((float)gameTime.ElapsedGameTime.TotalSeconds * 255) / time;
            return true;
        }
        
        #endregion
    }

    /// <summary>
    /// 演出付き移動
    /// Uiの中に宣言して使う。Updateが必要
    /// </summary>
    class SineWaveProduction
    {
        #region フィールド

        /// <summary>
        /// 所属しているUi
        /// </summary>
        Ui thisUi;

        /// <summary>
        /// 目標座標
        /// </summary>
        Vector2 goalPoint;
        /// <summary>
        /// 現座標と目標の中間地点
        /// </summary>
        Vector2 center;
        /// <summary>
        /// 円形運動の半径
        /// </summary>
        float radius;
        /// <summary>
        /// 円形運動の角度
        /// </summary>
        float radian;
        /// <summary>
        /// 目標への角度
        /// </summary>
        float goalRadian;
        public float moveSpeed = 0.15f;

        /// <summary>
        /// 動作中ではないか
        /// </summary>
        public bool MoveOn
        {
            get { return thisUi.Position == goalPoint; }
        }

        #endregion

        #region コンストラクタ

        public SineWaveProduction(Ui ui)
        {
            thisUi = ui;
            goalPoint = ui.Position;
        }

        #endregion

        #region 初期化

        public void Initialize()
        {
            center = goalPoint - ((goalPoint - thisUi.Position) * 0.5f);
            radius = MyUtility.Vector2Size((goalPoint - thisUi.Position) * 0.5f);
            goalRadian = (float)Math.Atan2(goalPoint.Y - thisUi.Position.Y, goalPoint.X - thisUi.Position.X);
            radian = (float)Math.PI;
        }

        #endregion

        #region 更新

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (thisUi.Position != goalPoint)
            {
                float vec;
                radian -= moveSpeed;
                if (radian <= 0)
                {
                    radian = 0;
                    thisUi.Position = goalPoint;
                    return;
                }
                vec = radius * (float)Math.Cos(radian);
                thisUi.Position = new Vector2(center.X + (float)Math.Cos(goalRadian) * vec, center.Y + (float)Math.Sin(goalRadian) * vec);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 指定した座標にサイン波移動
        /// </summary>
        /// <param name="goal"></param>
        public Vector2 GoalPoint
        {
            get { return goalPoint; }
            set
            {
                //移動先を指定されたら勝手にセッティングもする
                if (GoalPoint != value)
                {
                    goalPoint = value;
                    center = goalPoint - ((goalPoint - thisUi.Position) * 0.5f);
                    radius = MyUtility.Vector2Size((goalPoint - thisUi.Position) * 0.5f);
                    goalRadian = (float)Math.Atan2(goalPoint.Y - thisUi.Position.Y, goalPoint.X - thisUi.Position.X);
                    radian = (float)Math.PI;
                }
            }
        }

        /// <summary>
        /// 移動強制完了
        /// </summary>
        public void Arrival()
        {
            thisUi.Position = goalPoint;
        }

        #endregion
    }
}
