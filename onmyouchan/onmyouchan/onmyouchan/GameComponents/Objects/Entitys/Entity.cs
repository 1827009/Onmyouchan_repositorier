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

namespace onmyouchan
{
    #region 列挙体

    /// <summary>
    /// Entityの種別
    /// </summary>
    enum EntityType
    {
        etc,
        camera,
        terrain,
        player,
        enemy,
        attack
    }

    #endregion

    /// <summary>
    /// フィールドに存在するオブジェクトの原型です
    /// 宣言する際はentityManagerクラスのSignUpEntityの引数に宣言してください
    /// </summary>
    class Entity
    {
        #region フィールド

        /// <summary>
        /// 所属しているentityManagerの参照
        /// </summary>
        public EntityManager ThisEntityManager
        {
            get { return thisEntityManager; }
        }
        protected EntityManager thisEntityManager;

        /// <summary>
        /// 中心座標
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// 回転
        /// </summary>
        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        protected Vector3 rotation;

        /// <summary>
        /// モデルの大きさ倍率
        /// </summary>
        public float Scale
        {
            get { return scale; }
        }
        protected float scale = 1.0f;

        /// <summary>
        /// 種別
        /// </summary>
        public EntityType Attribute
        {
            get { return attribute; }
        }
        EntityType attribute;

        /// <summary>
        /// モデルステータス
        /// </summary>
        public EntityStatus_Model ModelStatus
        {
            get { return modelStatus; }
        }
        protected EntityStatus_Model modelStatus;

        /// <summary>
        /// 物理ステータス
        /// </summary>
        public EntityStatus_Move MoveStatus
        {
            get { return moveStatus; }
        }
        protected EntityStatus_Move moveStatus;

        /// <summary>
        /// あたり判定ステータス
        /// </summary>
        public List<EntityStatus_Hit> HitStatus
        {
            get { return hitStatus; }
        }
        protected List<EntityStatus_Hit> hitStatus;
        
        #endregion

        #region コンストラクタ
        
        /// <summary>
        /// Entityの種別を設定します
        /// </summary>
        /// <param name="type"></param>
        public Entity(EntityType type)
        {
            attribute = type;
        }
        
        #endregion

        #region 更新

        /// <summary>
        /// baseで実行しないでください
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            thisEntityManager.entityUpdateDelegate -= Update;
        }

        #endregion

        #region メソッド

        bool moveUpdateFlag = false;
        bool hitUpdateFlag = false;
        bool modelUpdateFlag = false;
        /// <summary>
        /// 継承先はbaseで実行してください
        /// </summary>
        /// <param name="entityManager"></param>
        public virtual void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;

            if (moveStatus != null)
            {
                thisEntityManager.moveUpdateDelegate += moveStatus.Update;
                moveUpdateFlag = true;
            }
            if (HitStatus != null)
            {
                thisEntityManager.hitUpdateDelegate += HitUpdate;
                hitUpdateFlag = true;
            }
            if (modelStatus != null)
            {
                thisEntityManager.modelUpdateDelegate += modelStatus.Update;
                modelUpdateFlag = true;
            }
            thisEntityManager.entityUpdateDelegate += Update;
        }

        /// <summary>
        /// EntityManagerの登録を解除します
        /// 継承先はbaseで実行してください
        /// </summary>
        public virtual void Unload()
        {
            thisEntityManager.entityList.Remove(this);

            if (moveUpdateFlag)
                thisEntityManager.moveUpdateDelegate -= moveStatus.Update;
            if (hitUpdateFlag)
                thisEntityManager.hitUpdateDelegate -= HitUpdate;
            if (modelUpdateFlag)
                thisEntityManager.modelUpdateDelegate -= modelStatus.Update;
            thisEntityManager.entityUpdateDelegate -= Update;
        }

        /// <summary>
        /// hitStatusリストすべての更新
        /// </summary>
        /// <param name="gameTime"></param>
        public void HitUpdate(GameTime gameTime)
        {
            for (int i = 0; i < hitStatus.Count; i++)
            {
                hitStatus[i].Update(gameTime);
            }
        }

        #endregion
    }

    #region モデル表示のステータス

    /// <summary>
    /// モデルを扱うクラスの原型
    /// </summary>
    class EntityStatus_Model
    {
        /// <summary>
        /// 所属しているEntityの参照
        /// </summary>
        protected Entity thisEntity;

        public EntityStatus_Model(Entity entity)
        {
            thisEntity = entity;
        }

        public virtual void Update(GameTime gameTime)
        { }
        public virtual void Draw(Camera camera, GameTime gameTime)
        { }
    }

    #endregion

    #region あたり判定のステータス
    /*
     * 説明
     * あたり判定はentityManagerクラスの中でオブジェクト同士をマッチングさせ、EntityStatus_HitのメンバであるhitGroupの値の大きいほうのメソッドを呼び出します
     * その際、EntityStatus_HitクラスのHitJudgShapeSelectを呼び出し、対象の形状を選択して適した判定のメソッドを呼び出します
     * 
     * 注意点
     * 新しく判定の形状を作る際はコンストラクタなどでhitShapeの指定、列挙体HitShapeTypeへの登録、HitJudgShapeSelectのswitch、すべてのクラスに対応した判定メソッドの追加を忘れないでください
     */

    #region 列挙体

    /// <summary>
    /// あたり判定の優先度グループ
    /// </summary>
    enum HitPriorityGroup
    {
        note,
        player,
        attack,
        terrain
    }

    #endregion

    #region あたり判定の形状インターフェース
    //あたり判定のメソッドを読み込むため、ここにある形状データを継承してください

    /// <summary>
    /// 球型のあたり判定の要素
    /// </summary>
    interface Shape_Ball
    {
        /// <summary>
        /// 半径
        /// </summary>
        float Radius
        {
            get;
        }
    }

    /// <summary>
    /// 箱型のあたり判定の要素
    /// </summary>
    interface Shape_Box
    {
        /// <summary>
        /// 箱を構成する頂点1
        /// </summary>
        Vector3 BoxPointMin
        {
            get;
        }
        /// <summary>
        /// 箱を構成する頂点2
        /// </summary>
        Vector3 BoxPointMax
        {
            get;
        }
    }

    /// <summary>
    /// 三角形(平面)のあたり判定の要素
    /// </summary>
    interface Shape_Triangle
    {
        /// <summary>
        /// 三角形を構成する頂点1
        /// </summary>
        Vector2 TrianglePoint1
        {
            get;
        }

        /// <summary>
        /// 三角形を構成する頂点2
        /// </summary>
        Vector2 TrianglePoint2
        {
            get;
        }

        /// <summary>
        /// 三角形を構成する頂点3
        /// </summary>
        Vector2 TrianglePoint3
        {
            get;
        }
    }

    interface Shape_Line2D
    {
        /// <summary>
        /// 線形の判定
        /// </summary>
        Vector2[] Line
        { get; }
    }

    #endregion

    #region 原型のクラス

    /// <summary>
    /// あたり判定を付与するクラスの原型です
    /// </summary>
    class EntityStatus_Hit
    {
        #region フィールド

        /// <summary>
        /// ステータスを付与したいEntityの参照
        /// </summary>
        protected Entity thisEntity;

        /// <summary>
        /// 判定の優先度
        /// </summary>
        public HitPriorityGroup HitGroup
        {
            get { return hitGroup; }
        }
        protected HitPriorityGroup hitGroup = HitPriorityGroup.note;

        /// <summary>
        /// あたり判定空間レベル
        /// </summary>
        protected int judgSpaceLevel;
        /// <summary>
        /// あたり判定空間番号
        /// </summary>
        protected int judgSpaceNo;

        #endregion

        #region コンストラクタ

        public EntityStatus_Hit(Entity entity)
        {
            thisEntity = entity;
        }

        #endregion

        #region 更新

        public virtual void Update(GameTime gameTime)
        { }

        #endregion

        #region メソッド

        /// <summary>
        /// 対象の形状に対して適切な判定メソッドを呼び出します
        /// </summary>
        /// <param name="entity"></param>
        public virtual void HitJudgShapeSelect(Entity entity, int index)
        {
            if (HitGroup == HitPriorityGroup.terrain && entity.HitStatus[index].HitGroup == HitPriorityGroup.terrain)
                return;

            if (entity.HitStatus[index] is EntityStatus_Hit_Ball)
                Hitjudg_Ball(entity, index);

            else if (entity.HitStatus[index] is EntityStatus_Hit_Box)
                Hitjudg_Box(entity, index);

            else if (entity.HitStatus[index] is EntityStatus_Hit_Triangle)
                Hitjudg_triangle(entity, index);

            else if (entity.HitStatus[index] is EntityStatus_Hit_PlaneLine)
                Hitjudg_PlaneLine(entity, index);
        }

        /// <summary>
        /// 所属あたり判定空間をセットします
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        public virtual void SetJudgSpase(Vector2 pos1,Vector2 pos2)
        {/*
            //既に所属している空間があれば抜ける
            if (thisEntity.ThisEntityManager.hitList[(int)(MyUtility.IntPow(4, judgSpaceLevel) / 3)].Contains(thisEntity))
                thisEntity.ThisEntityManager.hitList[(int)(MyUtility.IntPow(4, judgSpaceLevel) / 3)].Remove(thisEntity);

            pos1.X += thisEntity.Position.X;
            pos1.Y += thisEntity.Position.Z;
            pos2.X += thisEntity.Position.X;
            pos2.Y += thisEntity.Position.Z;

            int p1 = MyBitSeting(pos1.X) | (MyBitSeting(pos1.Y) << 1);//判定の角の所属空間を調べます
            int p2 = MyBitSeting(pos2.X) | (MyBitSeting(pos2.Y) << 1);//反対の角の所属空間を調べます
            int a = p1 ^ p2;

            for (int i = 0; i < thisEntity.ThisEntityManager.DivisionNumber + 1; i++)
            {
                if (a <= MyUtility.IntPow(2, i * 2) - 1)
                {
                    //空間レベルをセットします
                    judgSpaceLevel = thisEntity.ThisEntityManager.DivisionNumber - i;
                    break;
                }
            }
            //所属空間をセットします
            judgSpaceNo = p2 >> (judgSpaceLevel * 2);

            //あたり判定空間に登録
            thisEntity.ThisEntityManager.hitList[MyUtility.IntPow(4, judgSpaceLevel) / 3].Add(thisEntity);*/

            thisEntity.ThisEntityManager.hitList.Add(thisEntity);
        }

        /// <summary>
        /// bitを1つ開けにします
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private int MyBitSeting(float p)
        {
            int m;
            m = (int)(p / thisEntity.ThisEntityManager.SpaceSize);
            m = (m | (m << 8)) & 0x00ff00ff;
            m = (m | (m << 4)) & 0x0f0f0f0f;
            m = (m | (m << 2)) & 0x33333333;
            m = (m | (m << 1)) & 0x55555555;
            return m;
        }

        #endregion

        #region 判定

        /// <summary>
        /// 継承したクラスに対象(円型)との判定でオーバーライドしてください
        /// </summary>
        /// <param name="entity"></param>
        protected virtual bool Hitjudg_Ball(Entity entity, int index)
        { return false; }

        /// <summary>
        /// 継承したクラスに対象(箱型)との判定でオーバーライドしてください
        /// </summary>
        /// <param name="entity"></param>
        protected virtual bool Hitjudg_Box(Entity entity, int index)
        { return false; }
        
        /// <summary>
        /// 継承したクラスに対象(三角形(平面))との判定でオーバーライドしてください
        /// </summary>
        /// <param name="entity"></param>
        protected virtual bool Hitjudg_triangle(Entity entity, int index)
        { return false; }
        
        /// <summary>
        /// 継承したクラスに対象(線分(平面))との判定でオーバーライドしてください
        /// </summary>
        /// <param name="entity"></param>
        protected virtual bool Hitjudg_PlaneLine(Entity entity, int index)
        { return false; }

        #endregion
    }

    /// <summary>
    /// 球型のあたり判定の原型です
    /// </summary>
    class EntityStatus_Hit_Ball : EntityStatus_Hit, Shape_Ball
    {
        #region フィールド

        /// <summary>
        /// 半径
        /// </summary>
        public float Radius
        {
            get { return radius; }
        }
        protected float radius;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ステータスを付与したいEntityの参照と半径の大きさを入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="a"></param>
        public EntityStatus_Hit_Ball(Entity entity, float r)
            : base(entity)
        {
            radius = r;
        }

        #endregion

        #region 更新

        /// <summary>
        /// あたり判定空間レベルとあたり判定空間をセットします。
        /// baseで実行してください
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            SetJudgSpase(new Vector2(radius, radius), new Vector2(-radius, -radius));
        }

        #endregion

        #region 判定

        /// <summary>
        /// 球形同士が当たっているかどうか
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="rA"></param>
        /// <param name="vecB"></param>
        /// <param name="rB"></param>
        /// <returns></returns>
        protected bool BallHit(Vector3 vecA, float rA, Vector3 vecB, float rB)
        {
            if ((vecB.X - vecA.X * vecB.X - vecA.X) + (vecB.Y - vecA.Y * vecB.Y - vecA.Y) + (vecB.Z - vecA.Z * vecB.Z - vecA.Z) <= rA * rB)
                return true;
            return false;
        }

        /// <summary>
        /// 円形同士が当たっているかどうか
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="rA"></param>
        /// <param name="vecB"></param>
        /// <param name="rB"></param>
        /// <returns></returns>
        protected bool CircleHit(Vector3 vecA, float rA, Vector3 vecB, float rB)
        {
            Vector3 a = vecA - vecB;
            return ((a.X * a.X) + (a.Z * a.Z)) <= rA + rB;
        }

        #endregion
    }

    /// <summary>
    /// 箱形のあたり判定の原型です
    /// </summary>
    class EntityStatus_Hit_Box : EntityStatus_Hit, Shape_Box
    {
        #region フィールド

        /// <summary>
        /// 箱を構成する頂点1(中心座標加算済み)
        /// </summary>
        public Vector3 BoxPointMin
        {
            get { return boxPointMin + thisEntity.position; }
        }
        Vector3 boxPointMin;

        /// <summary>
        /// 箱を構成する頂点2(中心座標加算済み)
        /// </summary>
        public Vector3 BoxPointMax
        {
            get { return boxPointMax + thisEntity.position; }
        }
        Vector3 boxPointMax;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ステータスを付与したいEntityの参照と箱を構成する頂点2つを指定してください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="posMin"></param>
        /// <param name="posMax"></param>
        public EntityStatus_Hit_Box(Entity entity, Vector3 posMin, Vector3 posMax)
            : base(entity)
        {
            boxPointMin = posMin;
            boxPointMax = posMax;
        }

        #endregion 

        #region 更新

        /// <summary>
        /// あたり判定空間レベルとあたり判定空間をセットします。
        /// baseで実行してください
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            SetJudgSpase(new Vector2(boxPointMax.X, boxPointMax.Z), new Vector2(boxPointMin.X, boxPointMin.Z));
        }

        #endregion
    }

    /// <summary>
    /// 三角形(平面)のあたり判定の原型です
    /// </summary>
    class EntityStatus_Hit_Triangle : EntityStatus_Hit, Shape_Triangle
    {
        #region フィールド

        /// <summary>
        /// 三角形を構成する頂点
        /// </summary>
        public Vector2 TrianglePoint1
        {
            get { return trianglePoint1; }
        }
        Vector2 trianglePoint1;

        /// <summary>
        /// 三角形を構成する頂点2
        /// </summary>
        public Vector2 TrianglePoint2
        {
            get { return trianglePoint2; }
        }
        Vector2 trianglePoint2;

        /// <summary>
        /// 三角形を構成する頂点3
        /// </summary>
        public Vector2 TrianglePoint3
        {
            get { return trianglePoint3; }
        }
        Vector2 trianglePoint3;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ステータスを付与したいEntityの参照と三角形を構成する頂点3つを指定してください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="posMin"></param>
        /// <param name="posMax"></param>
        public EntityStatus_Hit_Triangle(Entity entity, Vector2 pos1, Vector2 pos2, Vector2 pos3)
            : base(entity)
        {
            trianglePoint1 = pos1;
            trianglePoint2 = pos2;
            trianglePoint3 = pos3;
        }
        public EntityStatus_Hit_Triangle(Entity entity, Vector3 pos1, Vector3 pos2, Vector3 pos3)
            : base(entity)
        {
            trianglePoint1.X = pos1.X;
            trianglePoint1.Y = pos1.Z;
            trianglePoint2.X = pos2.X;
            trianglePoint2.Y = pos2.Z;
            trianglePoint3.X = pos3.X;
            trianglePoint3.Y = pos3.Z;
        }

        #endregion

        #region 更新

        /// <summary>
        /// あたり判定空間レベルとあたり判定空間をセットします。
        /// baseで実行してください
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            Vector2 max;
            max.X = trianglePoint1.X;
            if (max.X < trianglePoint2.X)
                max.X = trianglePoint2.X;
            if (max.X < trianglePoint3.X)
                max.X = trianglePoint3.X;

            max.Y = trianglePoint1.Y;
            if (max.Y > trianglePoint2.Y)
                max.Y = trianglePoint2.Y;
            if (max.Y > trianglePoint3.Y)
                max.Y = trianglePoint3.Y;

            Vector2 min;
            min.X = trianglePoint1.X;
            if (min.X < trianglePoint2.X)
                min.X = trianglePoint2.X;
            if (min.X < trianglePoint3.X)
                min.X = trianglePoint3.X;

            min.Y = trianglePoint1.Y;
            if (min.Y > trianglePoint2.Y)
                min.Y = trianglePoint2.Y;
            if (min.Y > trianglePoint3.Y)
                min.Y = trianglePoint3.Y;

            SetJudgSpase(min, max);
        }

        #endregion
    }
        
    /// <summary>
    /// 線分(平面)のあたり判定の原型です
    /// </summary>
    class EntityStatus_Hit_PlaneLine : EntityStatus_Hit, Shape_Line2D
    {
        #region フィールド

        public Vector2[] Line
        { get { return line; } }
        Vector2[] line;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ステータスを付与したいEntityの参照と線分を構成する頂点を入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="point"></param>
        public EntityStatus_Hit_PlaneLine(Entity entity,params Vector2[] point)
            : base(entity)
        {
            line = point; 
        }

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            Vector2 max=line[0];
            for (int i = 0; i < line.Length; i++)
            {
                if (max.X < line[i].X)
                    max.X = line[i].X;
                if (max.Y < line[i].Y)
                    max.Y = line[i].Y;
            }

            Vector2 min = line[0];
            for (int i = 0; i < line.Length; i++)
            {
                if (min.X < line[i].X)
                    min.X = line[i].X;
                if (min.Y < line[i].Y)
                    min.Y = line[i].Y;
            }

            SetJudgSpase(min, max);
        }

        #endregion

        #region 判定

        /// <summary>
        /// 線分の交差判定
        /// </summary>
        /// <param name="vecA1"></param>
        /// <param name="vecA2"></param>
        /// <param name="vecB1"></param>
        /// <param name="vecB2"></param>
        /// <returns></returns>
        protected static bool CrossingLine(Vector2 vecA1, Vector2 vecA2, Vector2 vecB1, Vector2 vecB2)
        {
            float ta = (vecB1.X - vecB2.X) * (vecA1.Y - vecB1.Y) + (vecB1.Y - vecB2.Y) * (vecB1.X - vecA1.X);
            float tb = (vecB1.X - vecB2.X) * (vecA2.Y - vecB1.Y) + (vecB1.Y - vecB2.Y) * (vecB1.X - vecA2.X);

            float tc = (vecA1.X - vecA2.X) * (vecB1.Y - vecA1.Y) + (vecA1.Y - vecA2.Y) * (vecA1.X - vecB1.X);
            float td = (vecA1.X - vecA2.X) * (vecB2.Y - vecA1.Y) + (vecA1.Y - vecA2.Y) * (vecA1.X - vecB2.X);

            return tc * td < 0 && ta * tb < 0;
        }

        /// <summary>
        /// 交差していることを前提に交差点を返す
        /// </summary>
        /// <param name="vecA1"></param>
        /// <param name="vecA2"></param>
        /// <param name="vecB1"></param>
        /// <param name="vecB2"></param>
        /// <returns></returns>
        protected Vector2 CrossingPoint(Vector2 vecA1, Vector2 vecA2, Vector2 vecB1, Vector2 vecB2)
        {
            Vector2 vecA; vecA.X = vecA2.X - vecA1.X; vecA.Y = vecA2.Y - vecA1.Y;
            Vector2 vecB; vecB.X = vecB2.X - vecB1.X; vecB.Y = vecB2.Y - vecB1.Y;

            float a1b1X = vecB1.X - vecA1.X;
            float a1b1Y = vecB1.Y - vecA1.Y;
            float bunbo = (vecA2.X - vecA1.X) * (vecB2.Y - vecB1.Y) - (vecA2.Y - vecA1.Y) * (vecB2.X - vecB1.X);

            float ap = ((vecB2.Y - vecB1.Y) * a1b1X - (vecB2.X - vecB1.X) * a1b1Y) / bunbo;
            float bp = ((vecA2.Y - vecA1.Y) * a1b1X - (vecA2.X - vecA1.X) * a1b1Y) / bunbo;

            vecA.X = vecA.X * ap + vecA1.X;
            vecA.Y = vecA.Y * ap + vecA1.Y;
            return vecA;
        }
        /// <summary>
        /// vecBにvecAを投影した座標を返す
        /// </summary>
        /// <param name="vecA1"></param>
        /// <param name="vecA2"></param>
        /// <param name="vecB1"></param>
        /// <param name="vecB2"></param>
        /// <returns></returns>
        protected Vector2 ProjectionVector(Vector2 vecA1, Vector2 vecA2, Vector2 vecB1, Vector2 vecB2)
        {
            float vecAsize = MyUtility.Vector2Size(vecA2.X - vecA1.X, vecA2.Y - vecA1.Y);

            Vector2 vecC = CrossingPoint(vecA1, vecA2, vecB1, vecB2);
            Vector2 vecA = new Vector2(vecA2.X - vecC.X, vecA2.Y - vecC.Y);
            Vector2 vecB = new Vector2(vecB2.X - vecC.X, vecB2.Y - vecC.Y);

            float ragB = (float)Math.Atan2(vecB.Y, vecB.X);
            float ragAB = (float)Math.Atan2(vecA.Y, vecA.X) - ragB;
            float Asize = MyUtility.Vector2Size(vecA) * (float)Math.Cos(ragAB);

            return new Vector2((vecC.X + Asize * (float)Math.Cos(ragB)), vecC.Y + Asize * (float)Math.Sin(ragB));
        }


        #endregion
    }

    #endregion

    #endregion

    #region 物理挙動のステータス

    /// <summary>
    /// Entity内のEntityStatus_Moveに代入すると重力、慣性を付与します
    /// </summary>
    class EntityStatus_Move
    {
        #region フィールド

        /// <summary>
        /// ステータスを付与したいMyModelの参照
        /// </summary>
        Entity thisEntity;

        /// <summary>
        /// 重力の影響を受けるか
        /// </summary>
        public bool gravtyON = true;

        /// <summary>
        /// 重さや空気などの抵抗パラメータ
        /// </summary>
        PhysicalParameter physicalParameter = new PhysicalParameter(0.5f, 0.5f);

        /// <summary>
        /// 着地しているか
        /// </summary>
        public bool landing;

        /// <summary>
        /// 各方向への勢い
        /// </summary>
        public Vector3 moveVec;

        /// <summary>
        /// 前フレームでの座標
        /// </summary>
        public Vector3 OldPosition { get { return oldPosition; } }
        Vector3 oldPosition;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// Entityの参照を入れます
        /// </summary>
        public EntityStatus_Move(Entity entity)
        {
            thisEntity = entity;
        }
        public EntityStatus_Move(Entity entity, bool gravty)
        {
            thisEntity = entity;
            gravtyON = gravty;
        }

        #endregion

        #region アップデート

        public void Update(GameTime gameTime)
        {
            if (thisEntity.position != oldPosition)
                oldPosition = thisEntity.position;
            Physical();
        }

        #region Updateの中身

        private void Physical()
        {
            float resistance;
            //地面についていたら抵抗を上げる
            if (landing)
                resistance = physicalParameter.resistance * 2;
            else
                resistance = physicalParameter.resistance;

            //yの移動
            if (gravtyON && !landing)
            {
                //y移動の反映
                thisEntity.position.Y += moveVec.Y;

                //重力
                moveVec.Y -= physicalParameter.weight;
            }

            //xzの移動
            if (moveVec != Vector3.Zero)
            {
                //xz移動
                thisEntity.position.X += moveVec.X;
                thisEntity.position.Z += moveVec.Z;

                //xz方向の抵抗減衰値を出す
                float moveRag = (float)Math.Atan2(moveVec.Z, moveVec.X);
                float moveX = (float)Math.Cos(moveRag) * resistance;
                float moveZ = (float)Math.Sin(moveRag) * resistance;

                //抵抗の減衰
                if (Math.Abs(moveVec.X) <= Math.Abs(moveX))
                    moveVec.X = 0f;
                else if (moveVec.X > 0)
                    moveVec.X -= Math.Abs(moveX);
                else if (moveVec.X < 0)
                    moveVec.X += Math.Abs(moveX);

                if (Math.Abs(moveVec.Z) <= Math.Abs(moveZ))
                    moveVec.Z = 0f;
                else if (moveVec.Z > 0)
                    moveVec.Z -= Math.Abs(moveZ);
                else if (moveVec.Z < 0)
                    moveVec.Z += Math.Abs(moveZ);
            }

            landing = false;
        }

        #endregion

        #endregion
    }

    #region 重さとか抵抗の構造体

    /// <summary>
    /// 重さとか抵抗のパラメータ
    /// </summary>
    struct PhysicalParameter
    {
        /// <summary>
        /// 重さや重力の類
        /// </summary>
        public float weight;
        /// <summary>
        /// 空気抵抗とか横への抵抗
        /// </summary>
        public float resistance;

        public PhysicalParameter(float w, float r)
        {
            weight = w;
            resistance = r;
        }
    }

    #endregion

    #endregion
}