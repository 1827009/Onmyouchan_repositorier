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

namespace onmyouchan.Entity
{
    #region 構造体

    struct ModelIndexStatus
    {
        public string idName;
        public Model model;
    }

    #endregion

    /// <summary>
    /// ゲーム上のオブジェクトを管理するクラス
    /// </summary>
    class EntityManager
    {
        #region フィールド

        #region 参照

        /// <summary>
        /// 所属しているゲームループの参照
        /// </summary>
        public Game thisGame;

        #endregion

        #region 4分空間

        /// <summary>
        /// 縦横のステージ幅(正方形)
        /// </summary>
        public float StageSize
        {
            get { return stageSize; }
            set { stageSize = value; }
        }
        private float stageSize = 5000;

        /// <summary>
        /// あたり判定空間を何段階4分割をしているか
        /// </summary>
        public int DivisionNumber
        { 
            get { return divisionNumber; } 
        }
        private int divisionNumber = 3;

        /// <summary>
        /// あたり判定空間分割1スペースの広さ
        /// </summary>
        public float SpaceSize
        {
            get { return spaceSize; }
        }
        private float spaceSize;

        #endregion

        #region オブジェクト

        /// <summary>
        /// プレイヤー
        /// </summary>
        public Player player;

        /// <summary>
        /// カメラ
        /// </summary>
        public Camera mainCamera;

        /// <summary>
        /// 各Entityはここからモデルを参照
        /// </summary>
        public List<ModelIndexStatus> modelIndex;

        /// <summary>
        /// 登録されているEntityのリスト
        /// </summary>
        public List<Entity> entityList;

        /// <summary>
        /// あたり判定のリスト
        /// </summary>
        public List<EntityStatus_Hit> hitList;

        //Entity固有の動きを停止する
        public bool ControlFlag
        {
            get { return controlFlag; }
            set { controlFlag = value; }
        }
        bool controlFlag;

        #endregion

        #endregion

        #region コンストラクタ

        public EntityManager(Game game)
        {
            thisGame = game;

            entityList = new List<Entity>();
            hitList = new List<EntityStatus_Hit>();
            modelIndex = new List<ModelIndexStatus>();
        }

        #endregion

        #region 初期化

        /// <summary>
        /// 登録されているEntityをすべてクリアします
        /// </summary>
        public void Initialize()
        {
            entityList.Clear();
            hitList.Clear();
            player = null;
            mainCamera = null;
            controlFlag = true;

            /*あたり判定空間の１空間の大きさをセットします
            spaceSize = stageSize / MyUtility.IntPow(2, divisionNumber);
            
            //空間の数分の所属空間を確保します
            int a = 0;
            for (int i = 0; i < divisionNumber; i++)
                a += MyUtility.IntPow(2, i * (i + 1));
            hitList = new List<Entity>[a];
            for (int i = 0; i < a; i++)
                hitList[i] = new List<Entity>();*/
        }

        #endregion

        #region 更新

        /// <summary>
        /// 登録されたオブジェクトのUpdateを行います
        /// </summary>
        public void Update(GameTime gameTime) 
        {
            HitListJudg();
            EntityListUpdate(gameTime);
        }

        /// <summary>
        /// 登録されているEntityの更新をします
        /// </summary>
        private void EntityListUpdate(GameTime gameTime)
        {
            for (int i = 0; i < entityList.Count; i++)
            {
                if (entityList[i].HitStatus != null)
                    entityList[i].HitUpdate(gameTime);
            }
            for (int i = 0; i < entityList.Count; i++)
            {

                if (controlFlag || entityList[i].Attribute == EntityType.camera)
                    entityList[i].Update(gameTime);

                if (entityList[i].MoveStatus != null)
                    entityList[i].MoveStatus.Update(gameTime);

                if (entityList[i].ModelStatus != null)
                    entityList[i].ModelStatus.Update(gameTime);

                if (entityList[i].UnloadFlag)
                    UnlordEntity(entityList[i]);
            }
        }

        /// <summary>
        /// 登録されているEntityのあたり判定を呼びます
        /// </summary>
        public void HitListJudg()
        {
            for (int i = 0; i < hitList.Count; i++)
            {
                for (int j = i; j < hitList.Count; j++)
                {
                    if (i != j)
                        HitJudgCall(hitList[i], hitList[j]);
                }
            }
            hitList.Clear();
        }
        //対象同士の優先度に応じたあたり判定メソッドを呼びます
        public void HitJudgCall(EntityStatus_Hit entity1, EntityStatus_Hit entity2)
        {
            if (entity1 != entity2)
            {
                if (entity1.HitGroup >= entity2.HitGroup)
                    entity1.HitJudgShapeSelect(entity2);
                else
                    entity2.HitJudgShapeSelect(entity1);
            }
        }

        #endregion

        #region 表示

        /// <summary>
        /// 登録されたEntityの表示を行います
        /// </summary>
        /// <param name="camera"></param>
        public void Draw(GameTime gameTime)
        {
            foreach (Entity entity in entityList)
            {
                if (entity.ModelStatus != null)
                    entity.ModelStatus.Draw(mainCamera,gameTime);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// メインカメラを登録します
        /// </summary>
        /// <param name="camera"></param>
        public void LordCamera(Camera camera)
        {
            mainCamera = camera;
            entityList.Add(camera);
            camera.Lord(this);
        }

        /// <summary>
        /// プレイヤーオブジェクトを登録します
        /// </summary>
        /// <param name="entity"></param>
        public void LordPlayer(Player p)
        {
            player = p;
            entityList.Insert(0, p);
            p.Lord(this);
        }

        /// <summary>
        /// Entityオブジェクトを登録します
        /// </summary>
        /// <param name="entity"></param>
        public void LordEntity(Entity entity)
        {
            entityList.Add(entity);
            entity.Lord(this);
        }

        /// <summary>
        /// Entityオブジェクトを削除します
        /// </summary>
        /// <param name="entity"></param>
        public void UnlordEntity(Entity entity)
        {
            entityList.Remove(entity);
            entity.Unload();
        }

        /// <summary>
        /// モデルをロードします
        /// </summary>
        /// <param name="model"></param>
        public void LordModels(params string[] modelName)
        {
            for (int i = 0; i < modelName.Length; i++)
            {
                if (!modelIndex.Exists(x => x.idName == modelName[i]))
                {
                    ModelIndexStatus modelIndexStatus;
                    modelIndexStatus.idName = modelName[i];
                    modelIndexStatus.model = thisGame.Content.Load<Model>(modelName[i]);
                    modelIndex.Add(modelIndexStatus);
                }
            }
        }
        /// <summary>
        /// モデルがインデックスに無ければ追加し、その後参照を返します
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public Model LordModel(string modelName)
        {
            if (!modelIndex.Exists(x => x.idName == modelName))
            {
                ModelIndexStatus modelIndexStatus;
                modelIndexStatus.idName = modelName;
                modelIndexStatus.model = thisGame.Content.Load<Model>(modelName);
                modelIndex.Add(modelIndexStatus);
            }
            return modelIndex.Find(x => x.idName == modelName).model;
        }

        #endregion
    }
}
