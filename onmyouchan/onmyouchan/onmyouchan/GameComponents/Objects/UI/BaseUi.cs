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
    /// 画像と位置
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
        /// 位置
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;
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
        /// 基本位置
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        #endregion
        
        #region 更新

        public virtual void Update(GameTime time) { }

        #endregion

        #region 描画
        
        public virtual void Draw() { }

        #endregion

        #region メソッド

        /// <summary>
        /// baseを実行した後に処理をしてください
        /// </summary>
        /// <param name="uiManager"></param>
        public virtual void Lord(UiManager uiManager)
        {
            thisUiManager = uiManager;
        }
        
        #endregion
    }

    /// <summary>
    /// 演出付き移動(Uiの中に宣言して使うStatus)
    /// </summary>
    class UiProduction
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

        #endregion

        public UiProduction(Ui ui)
        {
            thisUi = ui;
            goalPoint = ui.Position;
        }

        public void Initialize()
        {
            center = goalPoint - ((goalPoint - thisUi.Position) * 0.5f);
            radius = MyUtility.Vector2Size((goalPoint - thisUi.Position) * 0.5f);
            goalRadian = (float)Math.Atan2(goalPoint.Y - thisUi.Position.Y, goalPoint.X - thisUi.Position.X);
            radian = (float)Math.PI;
        }

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
    }
}
