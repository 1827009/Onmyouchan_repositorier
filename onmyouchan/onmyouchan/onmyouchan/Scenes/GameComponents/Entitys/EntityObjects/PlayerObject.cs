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
using System.Configuration;

namespace onmyouchan.Entity
{
    enum ActionStatus
    {
        idle,
        walk,
        slip,
        setOhuda,
        attack,
        item1,
        item2
    }
    delegate bool Activate();

    #region プレーヤークラス

    /// <summary>
    /// プレイヤー
    /// </summary>
    class Player : Character
    {
        #region フィールド

        ScoreData scoreDate;

        /// <summary>
        /// 状態
        /// </summary>
        public ActionStatus action;
        /// <summary>
        /// ひるみ時間
        /// </summary>
        public float knockbackTime;
        public float invincibleTime;

        Activate OhudaInstallationAction;
        Activate OhudaAttackAction;
        Activate HitogataThrowAction;
        Activate GoheiShakeAction;

        /// <summary>
        /// お札の最大所持数
        /// </summary>
        int maxOhudaStock = 3;
        /// <summary>
        /// お札の所持数
        /// </summary>
        public int ohudaStock
        {
            get { return maxOhudaStock - ohudaList.Count; }
        }

        /// <summary>
        /// 引き寄せアイテムの数
        /// </summary>
        public int Item1
        {
            get { return item1; }
            set { item1 = value; }
        }
        int item1 = 0;
        /// <summary>
        /// 結界解除アイテムの数
        /// </summary>
        public int Item2
        {
            get { return item2; }
            set { item2 = value; }
        }
        int item2 = 0;
        
        /// <summary>
        /// 設置したお札への参照
        /// </summary>
        public List<Ohuda> ohudaList;

        float stopItemTime = 7;
        public float damage = 0;

        #endregion

        #region コンストラクタ

        public Player(Vector3 pos,ScoreData score)
            : base(EntityType.player, pos)
        {
            scoreDate = score;

            ohudaList = new List<Ohuda>();
            scale = 1f;

            maxSpeed = float.Parse(ConfigurationManager.AppSettings["PlayerMove_MaxSpeed"]);
            moveSpeed = float.Parse(ConfigurationManager.AppSettings["PlayerMove_AccelerationSpeed"]);

            OhudaInstallationAction += ControllManager.KeyDecide;
            OhudaAttackAction += ControllManager.LeftTrigger;
            HitogataThrowAction += ControllManager.KeyCancel;
            GoheiShakeAction += ControllManager.RightTrigger;
        }

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            knockback(gameTime);
            base.Update(gameTime);
        }

        public void knockback(GameTime gameTime)
        {
            if (knockbackTime > 0)
            {
                knockbackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                controlWeit = 0.1f;
                action = ActionStatus.slip;
            }
            if (invincibleTime > 0)
            {
                invincibleTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        #endregion

        #region メソッド

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = new EntityStatus_Model_Player(this);
            moveStatus = new EntityStatus_Move(this);
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball_Repulsion(this, 50, 0.3f,HitPriorityGroup.player));

            base.Lord(entityManager);
        }

        /// <summary>
        /// 操作
        /// </summary>
        protected override void Control(GameTime gameTime)
        {
            action = ActionStatus.idle;
            switch (action)
            {
                case ActionStatus.idle:
                case ActionStatus.walk:
                    //移動
                    Move();

                    //お札設置
                    OhudaInstallation();

                    //お札なげ
                    OhudaAttack();

                    //引き寄せアイテム
                    HitogataThrow();

                    //結界消去アイテム
                    GoheiShake();
                    break;
            }
        }

        /// <summary>
        /// 移動
        /// </summary>
        private void Move()
        {
            //スティックを操作していれば
            if (ControllManager.MoveKey() != Vector2.Zero)
            {
                float angle = (float)Math.Atan2(ControllManager.MoveKey().Y, ControllManager.MoveKey().X) + thisEntityManager.mainCamera.Rotation.Y;
                Vector2 stick = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                //最高速度を超えていなければ
                if ((moveStatus.moveVec.X * moveStatus.moveVec.X) + (moveStatus.moveVec.Z * moveStatus.moveVec.Z) <= maxSpeed * maxSpeed)
                {
                    moveStatus.moveVec.X += stick.X * moveSpeed;
                    moveStatus.moveVec.Z -= stick.Y * moveSpeed;
                }

                angle = MyUtility.OverRadianReset(angle);
                goalAngle = angle;

                action = ActionStatus.walk;
            }
        }
        /// <summary>
        /// キャラクターからどれくらい前にお札を設置するか
        /// </summary>
        const float installationPosition = 50;
        /// <summary>
        /// キャラクターの前にお札を設置
        /// </summary>
        private void OhudaInstallation()
        {
            if (OhudaInstallationAction())
            {
                action = ActionStatus.setOhuda;
                if (ohudaStock == 1)
                {
                    Vector2 pos1 = new Vector2((ohudaList[1].position.X - ohudaList[0].position.X) * 0.5f, (ohudaList[1].position.Z - ohudaList[0].position.Z) * 0.5f);
                    Vector2 pos2 = new Vector2(pos1.X + ohudaList[0].position.X, pos1.Y + ohudaList[0].position.Z);
                    float rag = (float)Math.Atan2(position.X - pos2.X, position.Z - pos2.Y);
                    goalAngle = rag + (float)Math.PI / 2;
                    rotation.Y = rag + (float)Math.PI / 2;
                }
                Vector3 pos = position;
                pos.X += (float)Math.Cos(rotation.Y) * installationPosition;
                pos.Z -= (float)Math.Sin(rotation.Y) * installationPosition;
                pos.Y += 20;
                Ohuda ohuda = new Ohuda(this, pos, rotation);
                ohudaList.Add(ohuda);
                thisEntityManager.LordEntity(ohuda);

                //3つ目なら結界を作る
                if (ohudaList.Count == 3)
                {
                    thisEntityManager.LordEntity(new OhudaBorder(ohudaList[0].position, ohudaList[1].position, ohudaList[2].position));
                    for (int i = 0; i < ohudaList.Count; i++)
                    {
                        ohudaList[i].borderOn = true;
                    }
                    ohudaList.Clear();
                    scoreDate.BorderCount++;
                }
            }
        }

        /// <summary>
        /// 引き寄せの使用
        /// </summary>
        private void HitogataThrow()
        {
            if (HitogataThrowAction())
            {
                controlWeit = 1.4f;
                scoreDate.ItemCount++;
                Hitogata hitogata = new Hitogata(position, rotation.Y);
                thisEntityManager.LordEntity(hitogata);
                action = ActionStatus.item1;
            }
        }

        /// <summary>
        /// 止めるアイテム
        /// </summary>
        private void OhudaAttack()
        {
            #region 旧
            /*if (OhudaAttackAction())
            {
                controlWeit = 0.5f;
                Vector3 pos = position;

                pos.X += (float)Math.Cos(rotation.Y) * installationPosition;
                pos.Z -= (float)Math.Sin(rotation.Y) * installationPosition;
                pos.Y += 50;

                OhudaAttack ohuda = new OhudaAttack(pos);
                thisEntityManager.LordEntity(ohuda);
                ohuda.MoveStatus.moveVec.X = (float)Math.Cos(rotation.Y) * float.Parse(ConfigurationManager.AppSettings["PlayerAction_AttackVector"]);
                ohuda.MoveStatus.moveVec.Z -= (float)Math.Sin(rotation.Y) * float.Parse(ConfigurationManager.AppSettings["PlayerAction_AttackVector"]);
                ohuda.MoveStatus.PhysicalStatus = new PhysicalParameter(0, 0);

                action = ActionStatus.attack;
            }*/
            #endregion
            if (OhudaAttackAction() && item1 > 0)
            {
                item1--;
                controlWeit = 0.8f;
                scoreDate.ItemCount++;
                for (int i = 0; i < thisEntityManager.entityList.Count; i++)
                {
                    if (thisEntityManager.entityList[i] is Enemy)
                    {
                        Enemy e = (Enemy)thisEntityManager.entityList[i];
                        e.StopItem(stopItemTime);
                    }
                }
                action = ActionStatus.item1;
            }
        }
        /// <summary>
        /// 結界消去アイテムの使用
        /// </summary>
        private void GoheiShake()
        {
            if (GoheiShakeAction() && item2 > 0)
            {
                item2--;
                controlWeit = 1f;
                scoreDate.ItemCount++;
                thisEntityManager.LordEntity(new Oharai(this));
                for(int i=0;i<thisEntityManager.entityList.Count;i++)
                {
                    if (thisEntityManager.entityList[i] is OhudaBorder || thisEntityManager.entityList[i] is Ohuda)
                        thisEntityManager.entityList[i].UnloadFlag = true;
                }
                action = ActionStatus.item2;
            }
        }

        public override void TakenDamage()
        {
            action = ActionStatus.slip;
            base.TakenDamage();
        }

        #endregion
    }
    class Oharai:Entity
    {
        Player player;
        public Oharai(Player p):base(EntityType.etc)
        {
            player=p;
        }
        public override void Update(GameTime gameTime)
        {
            position = player.position; position.Y += 150;
            if (player.action != ActionStatus.item2)
                UnloadFlag = true;
            base.Update(gameTime);
        }
        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            //modelStatus = new EntityStatus_Model_Toon(this, entityManager.LordModel("GameModel\\oharai1"), "GameModel\\oharaiUV");
            modelStatus = new EntityStatus_Model_Oharai(this);
            rotation.X = (float)Math.PI;
            position = player.position; position.Y += 150;
            base.Lord(entityManager);
        }
    }

    #region 設置お札

    /// <summary>
    /// お札
    /// </summary>
    class Ohuda : Entity
    {
        #region フィールド

        /// <summary>
        /// 設置したプレイヤーへの参照
        /// </summary>
        Player thisPlayer;

        /// <summary>
        /// 生存時間
        /// </summary>
        public float life;

        /// <summary>
        /// 結界が起動しているか
        /// </summary>
        public bool borderOn;

        #endregion

        #region コンストラクタ

        public Ohuda(Player player, Vector3 pos, Vector3 rota)
            : base(EntityType.etc)
        {
            thisPlayer = player;

            rotation = rota;
            rotation.X = MathHelper.ToRadians(75);
            position = pos;

            life = float.Parse(ConfigurationManager.AppSettings["PlayerAction_OhudaLifeTime"]);
        }

        #endregion

        #region 更新

        public override void Update(GameTime gameTime)
        {
            //持続時間が無くなったら消滅
            if (life <= 0)
            {
                UnloadFlag = true;
            }
            //lifeがまだあれば減らし、結界化してれば何もしない
            else if (!borderOn)
            {
                rotation.Y += 0.05f;
                life -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
                rotation.Y += 0.1f;
        }

        #endregion

        #region メソッド

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            modelStatus = new EntityStatus_Model_Toon(this, entityManager.LordModel("GameModel\\fuda1"),"GameModel\\fudauv1");
            moveStatus = null;
            hitStatus = null;

            //thisEntityManager.LordEntity(new Particle(this, 1, thisEntityManager.thisGame.Content.Load<Texture2D>("GameModel\\fuda1"),
            //    5,
            //    10,
            //    10,
            //    1,
            //    1));

            base.Lord(entityManager);
        }

        public override void Unload()
        {
            thisEntityManager.entityList.Remove(this);
            if (thisPlayer.ohudaList.Contains(this))
                thisPlayer.ohudaList.Remove(this);
        }

        #endregion
    }
    /// <summary>
    /// お札による結界
    /// </summary>
    class OhudaBorder : Entity
    {
        #region フィールド

        Vector3 trianglePos1;
        Vector3 trianglePos2;
        Vector3 trianglePos3;

        #endregion

        #region コンストラクタ

        public OhudaBorder(Vector3 pos1, Vector3 pos2, Vector3 pos3)
            : base(EntityType.terrain)
        {
            trianglePos1 = pos1;
            trianglePos2 = pos2;
            trianglePos3 = pos3;
        }

        #endregion

        #region メソッド

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;

            modelStatus = new EntityStatus_Model_Triangle(this, trianglePos1, trianglePos2, trianglePos3);
            moveStatus = null;
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_PlaneLine_Bloc(this,HitPriorityGroup.terrain, new Vector2(trianglePos1.X, trianglePos1.Z), new Vector2(trianglePos2.X, trianglePos2.Z), new Vector2(trianglePos3.X, trianglePos3.Z)));
            hitStatus.Add(new EntityStatus_Hit_Triangle_Note(this, new Vector2(trianglePos1.X, trianglePos1.Z), new Vector2(trianglePos2.X, trianglePos2.Z), new Vector2(trianglePos3.X, trianglePos3.Z)));

            base.Lord(entityManager);
        }

        #endregion
    }

    #endregion

    #region 投げるお札

    class OhudaAttack : Entity
    {
        float life;
        public OhudaAttack(Vector3 pos)
            : base(EntityType.attack)
        {
            position = pos;
            life = float.Parse(ConfigurationManager.AppSettings["PlayerAction_AttackLifeTime"]);
        }

        public override void Update(GameTime gameTime)
        {
            life-=(float)gameTime.ElapsedGameTime.TotalSeconds;
            if (life == 0)
            {
                scale = 3;
                hitStatus.Add(new EntityStatus_Hit_Ball_Repulsion_OhudaAttack(this, 150, 40));
            }
            else if (life < 0)
                UnloadFlag = true;
        }

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;

            hitStatus=new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball_Repulsion_OhudaAttack(this, 20, 70));
            moveStatus = new EntityStatus_Move(this, false);
            modelStatus = new EntityStatus_Model(this, thisEntityManager.LordModel("GameModel\\fuda1"));
            base.Lord(entityManager);
        }
    }
    class EntityStatus_Hit_Ball_Repulsion_OhudaAttack:EntityStatus_Hit_Ball_Repulsion
    {
        bool hit;
        public EntityStatus_Hit_Ball_Repulsion_OhudaAttack(Entity entity, float r, float reg)
            : base(entity,r,reg,HitPriorityGroup.attack)
        { }

        public override void HitJudgShapeSelect(EntityStatus_Hit entity)
        {
            if (entity.thisEntity.Attribute == EntityType.enemy)
                base.HitJudgShapeSelect(entity);
        }

        protected override bool Hitjudg_Ball(EntityStatus_Hit entity)
        {
            if (!hit)
            {
                //キャストしてBallの半径を取り出せるようにします
                Shape_Ball entityR = (Shape_Ball)entity;

                //あたり判定をします
                Vector3 vec;
                vec.X = entity.thisEntity.position.X - thisEntity.position.X;
                vec.Y = entity.thisEntity.position.Y - thisEntity.position.Y;
                vec.Z = entity.thisEntity.position.Z - thisEntity.position.Z;
                float r = entityR.Radius + radius;

                if (BallHit(thisEntity.position, radius, entity.thisEntity.position, entityR.Radius))
                {
                    hit = true;
                    Vector3 reboundDirection = MyUtility.NormalizeVector3(vec);
                    if (entity.thisEntity.MoveStatus != null)
                        entity.thisEntity.MoveStatus.moveVec += reboundDirection * resilience;

                    return true;
                }
            }
            return false;
        }
    }

    #endregion

    #region アイテム

    class Hitogata:Entity
    {
        float speed = 7;
        EntityStatus_Hit_Ball_Item attack;
        /// <summary>
        /// 生存時間
        /// </summary>
        float life = 2;

        public Hitogata(Vector3 pos,float rota)
            : base(EntityType.attack)
        {
            position = pos;
            rotation.Y = rota + (float)Math.PI * 0.5f;
            attack = new EntityStatus_Hit_Ball_Item(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (moveStatus.landing)
            {
                if (!hitStatus.Contains(attack))
                    hitStatus.Add(attack);
                moveStatus.moveVec = Vector3.Zero;
                life -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (life < 0)
                    UnloadFlag = true;
            }
            base.Update(gameTime);
        }

        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;

            moveStatus = new EntityStatus_Move(this, true);
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball(this,40,HitPriorityGroup.attack));
            modelStatus = new EntityStatus_Model_Toon(this, entityManager.LordModel("GameModel\\hikiyose1"), "GameModel\\hikiyoseUV");

            moveStatus.PhysicalStatus = new PhysicalParameter(0.4f, 0.0f);
            moveStatus.moveVec = new Vector3((float)Math.Sin(rotation.Y) * speed, 10, (float)Math.Cos(rotation.Y) * speed);

            base.Lord(entityManager);
        }
    }

    class EntityStatus_Hit_Ball_Item:EntityStatus_Hit_Ball
    {
        const int rang = 500;

        float Suction;

        public EntityStatus_Hit_Ball_Item(Entity entity)
            : base(entity, rang, HitPriorityGroup.attack)
        {
            Suction = 1f;
            hitGroup = HitPriorityGroup.attack;
        }

        public override void HitJudgShapeSelect(EntityStatus_Hit entity)
        {
            if (entity.thisEntity.Attribute == EntityType.enemy)
                base.HitJudgShapeSelect(entity);
        }

        protected override bool Hitjudg_Ball(EntityStatus_Hit entity)
        {
            //キャストしてBallの半径を取り出せるようにします
            Shape_Ball entityR = (Shape_Ball)entity;

            //あたり判定をします
            Vector3 vec;
            vec.X = entity.thisEntity.position.X - thisEntity.position.X;
            vec.Y = entity.thisEntity.position.Y - thisEntity.position.Y;
            vec.Z = entity.thisEntity.position.Z - thisEntity.position.Z;

            if (BallHit(thisEntity.position, Radius, entity.thisEntity.position, entityR.Radius))
            {
                Vector3 reboundDirection = MyUtility.NormalizeVector3(vec);
                if (entity.thisEntity.MoveStatus != null)
                {
                    entity.thisEntity.MoveStatus.moveVec -= reboundDirection * Suction;
                }

                return true;
            }
            return false;
        }
    }
    class ItemObject : Entity
    {
        public ItemObject(Vector3 pos)
            : base(EntityType.attack)
        {
            position = pos;
        }
        public void ItemSet(Vector2 size)
        {
            Random rand=new Random();
            position.X = rand.Next((int)size.X * 2) - size.X;
            position.Z = rand.Next((int)size.Y * 2) - size.Y;
            moveStatus.OldPosition = position;
        }
    }

    class Item1Object : ItemObject
    {
        public Item1Object(Vector3 pos):base(pos)
        {
        }
        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball_ItemObject(this));
            modelStatus = new EntityStatus_Model_Toon(this, entityManager.LordModel("GameModel\\timestop1"), "GameModel\\timestopUV");
            moveStatus = new EntityStatus_Move(this);

            rotation.X = -(float)Math.PI/2;
            base.Lord(entityManager);
        }
    }
    class Item2Object : ItemObject
    {
        public Item2Object(Vector3 pos)
            : base(pos)
        {
        }
        public override void Lord(EntityManager entityManager)
        {
            thisEntityManager = entityManager;
            hitStatus = new List<EntityStatus_Hit>();
            hitStatus.Add(new EntityStatus_Hit_Ball_Item2Object(this));
            modelStatus = new EntityStatus_Model_Toon(this, entityManager.LordModel("GameModel\\oharai1"), "GameModel\\oharaiUV");
            moveStatus = new EntityStatus_Move(this);

            rotation.X = (float)Math.PI/2;
            base.Lord(entityManager);
        }
    }
    class EntityStatus_Hit_Ball_ItemObject : EntityStatus_Hit_Ball
    {
        ItemObject item;
        public EntityStatus_Hit_Ball_ItemObject(ItemObject entity)
            : base(entity, 30, HitPriorityGroup.attack)
        {
            item = entity;
        }

        public override void HitJudgShapeSelect(EntityStatus_Hit entity)
        {
            if (entity.thisEntity is Player)
                Hitjudg_Player((Player)entity.thisEntity);
        }
        protected bool Hitjudg_Player(Player entity)
        {
            Shape_Ball ball = (Shape_Ball)entity.HitStatus[0];
            if (BallHit(thisEntity.position, radius, entity.position,ball.Radius))
            {
                entity.Item1++;
                thisEntity.UnloadFlag = true;
                thisEntity.HitStatus.Remove(this);
                return true;
            }
            return false;
        }
    }
    class EntityStatus_Hit_Ball_Item2Object : EntityStatus_Hit_Ball
    {
        ItemObject item;
        public EntityStatus_Hit_Ball_Item2Object(ItemObject entity)
            : base(entity, 30, HitPriorityGroup.attack)
        {
            item = entity;
        }

        public override void HitJudgShapeSelect(EntityStatus_Hit entity)
        {
            if (entity.thisEntity is Player)
                Hitjudg_Player((Player)entity.thisEntity);
        }
        protected bool Hitjudg_Player(Player entity)
        {
            Shape_Ball ball = (Shape_Ball)entity.HitStatus[0];
            if (BallHit(thisEntity.position, radius, entity.position, ball.Radius))
            {
                entity.Item2++;
                thisEntity.UnloadFlag = true;
                thisEntity.HitStatus.Remove(this);
                return true;
            }
            return false;
        }

    }

    #endregion

    #endregion

    #region EntityStatus

    /// <summary>
    /// プレイヤー用角度調整を含むステータス
    /// </summary>
    class EntityStatus_Model_Player : EntityStatus_Model_ToonMotion
    {
        Player thisPlayer;
        float motionTime;

        #region コンストラクタ

        public EntityStatus_Model_Player(Player entity)
            : base(entity, entity.ThisEntityManager.LordModel("GameModel\\moderuB1"), "GameModel\\uv2", "Stand", "wark", "Knock back", "Hudaoki", "Item motion1", "Item motion2")
        {
            thisPlayer = entity;
        }

        #endregion

        #region 更新

        /// <summary>
        /// モデルの向き調整を含んでる
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (motionTime <= 0)
            {
                switch (thisPlayer.action)
                {
                    case ActionStatus.idle:
                        ClipIndex = 0;
                        LoopEnable = true;
                        break;
                    case ActionStatus.walk:
                        ClipIndex = 1;
                        LoopEnable = true;
                        break;
                    case ActionStatus.slip:
                        ClipIndex = 2;
                        LoopEnable = false;
                        break;
                    case ActionStatus.setOhuda:
                        ClipIndex = 3;
                        motionTime = 0.7f;
                        LoopEnable = false;
                        break;
                    case ActionStatus.item2:
                        ClipIndex = 4;
                        LoopEnable = false;
                        break;
                    case ActionStatus.item1:
                        ClipIndex = 5;
                        LoopEnable = false;
                        break;
                }
            }
            else
                motionTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
            //回転行列
            Matrix rotationX = Matrix.CreateRotationX(thisEntity.Rotation.X);
            Matrix rotationY = Matrix.CreateRotationY(thisEntity.Rotation.Y);
            Matrix rotationZ = Matrix.CreateRotationZ(thisEntity.Rotation.Z + (float)Math.PI / 2.5f);
            Matrix rotationMatrix = rotationZ*rotationX * rotationY;
            //大きさ
            Matrix scaleMatrix = Matrix.CreateScale(thisEntity.Scale);
            //最終的な行列
            Vector3 pos = thisEntity.position; pos.Y += 25f;
            world = rotationMatrix * scaleMatrix * Matrix.CreateTranslation(pos);
        }

        #endregion
    }

    class EntityStatus_Model_Oharai : EntityStatus_Model_ToonMotion
    {
        #region コンストラクタ

        public EntityStatus_Model_Oharai(Entity entity)
            : base(entity, entity.ThisEntityManager.LordModel("GameModel\\oharai1"), "GameModel\\oharaiUV", "furifuri")
        {
        }

        #endregion

        #region 更新

        /// <summary>
        /// モデルの向き調整を含んでる
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //回転行列
            Matrix rotationX = Matrix.CreateRotationX(thisEntity.Rotation.X);
            Matrix rotationY = Matrix.CreateRotationY(thisEntity.Rotation.Y);
            Matrix rotationZ = Matrix.CreateRotationZ(thisEntity.Rotation.Z + (float)Math.PI / 2.5f);
            Matrix rotationMatrix = rotationZ * rotationX * rotationY;
            //大きさ
            Matrix scaleMatrix = Matrix.CreateScale(thisEntity.Scale);
            //最終的な行列
            Vector3 pos = thisEntity.position; pos.Y += 25f;
            world = rotationMatrix * scaleMatrix * Matrix.CreateTranslation(pos);
        }

        #endregion
    }

    /// <summary>
    /// 自作Effectによる三角形描画
    /// </summary>
    class EntityStatus_Model_Triangle : EntityStatus_Model
    {
        #region フィールド

        /// <summary>
        /// 頂点数
        /// </summary>
        const int points = 3;

        /// <summary>
        /// 頂点、色
        /// </summary>
        VertexPositionTexture[] vertices;

        Effect effect;

        #endregion

        #region コンストラクタ

        public EntityStatus_Model_Triangle(Entity entity, Vector3 pos1, Vector3 pos2, Vector3 pos3)
            : base(entity)
        {
            vertices = new VertexPositionTexture[points];
            vertices[0] = new VertexPositionTexture(pos1, new Vector2(0.5f, 0));
            vertices[1] = new VertexPositionTexture(pos2, new Vector2(1f, 1f));
            vertices[2] = new VertexPositionTexture(pos3, new Vector2(0f, 1f));

            effect = thisEntity.ThisEntityManager.thisGame.Content.Load<Effect>("Effect\\TriangleEffect");
            effect.Parameters["MyTexture"].SetValue(thisEntity.ThisEntityManager.thisGame.Content.Load<Texture2D>("GameModel\\IMG_2302"));
        }

        #endregion

        #region 表示

        public override void Draw(Camera camera, GameTime gameTime)
        {
            effect.Parameters["View"].SetValue(camera.view);
            effect.Parameters["Projection"].SetValue(camera.projection);
            
            RasterizerState rasterizer = RasterizerState.CullNone;
            thisEntity.ThisEntityManager.thisGame.GraphicsDevice.RasterizerState = rasterizer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //thisEntity.ThisEntityManager.thisGame.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
                thisEntity.ThisEntityManager.thisGame.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                    PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 頂点の位置をセットする
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="pos3"></param>
        public void SetPosition(Vector3 pos1, Vector3 pos2, Vector3 pos3)
        {
            vertices[0].Position = pos1;
            vertices[1].Position = pos2;
            vertices[2].Position = pos3;
        }

        #endregion
    }

    /// <summary>
    /// Entity内のEntityStatus_Hitに代入すると三角形のあたり判定を付与します
    /// 当たった時の反応は敵へのダメージです
    /// </summary>
    class EntityStatus_Hit_Triangle_Note : EntityStatus_Hit_Triangle
    {
        #region コンストラクタ

        /// <summary>
        /// ステータスを付与したいEntityの参照と三角形を構成する頂点3つを入れてください
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="a"></param>
        public EntityStatus_Hit_Triangle_Note(Entity entity, Vector2 pos1, Vector2 pos2, Vector2 pos3)
            : base(entity, pos1, pos2, pos3,HitPriorityGroup.attack2)
        {
        }
        public EntityStatus_Hit_Triangle_Note(Entity entity, Vector3 pos1, Vector3 pos2, Vector3 pos3)
            : base(entity, pos1, pos2, pos3,HitPriorityGroup.attack2)
        {
        }

        #endregion

        #region 判定

        protected override bool Hitjudg_Ball(EntityStatus_Hit entity)
        {
            if (entity.thisEntity is Enemy||entity.thisEntity is ItemObject)
            {
                //Ballの要素を取り出せるようにする
                Shape_Ball entityR = (Shape_Ball)entity;

                //対象の未来座標を計算
                Vector3 futurePos = entity.thisEntity.position + entity.thisEntity.MoveStatus.moveVec;

                Vector2 pos12 = TrianglePoint2 - TrianglePoint1;
                Vector2 pos2p;
                pos2p.X = entity.thisEntity.position.X - TrianglePoint2.X;
                pos2p.Y = entity.thisEntity.position.Z - TrianglePoint2.Y;

                Vector2 pos23 = TrianglePoint3 - TrianglePoint2;
                Vector2 pos3p;
                pos3p.X = entity.thisEntity.position.X - TrianglePoint3.X;
                pos3p.Y = entity.thisEntity.position.Z - TrianglePoint3.Y;

                Vector2 pos31 = TrianglePoint1 - TrianglePoint3;
                Vector2 pos1p;
                pos1p.X = entity.thisEntity.position.X - TrianglePoint1.X;
                pos1p.Y = entity.thisEntity.position.Z - TrianglePoint1.Y;

                float a = pos12.X * pos2p.Y - pos12.Y * pos2p.X;
                float b = pos23.X * pos3p.Y - pos23.Y * pos3p.X;
                float c = pos31.X * pos1p.Y - pos31.Y * pos1p.X;

                if ((a > 0 && b > 0 && c > 0) || (a < 0 && b < 0 && c < 0))
                {
                    if (entity.thisEntity is ItemObject)
                    {
                        ItemObject item = (ItemObject)entity.thisEntity;
                        item.ItemSet(thisEntity.ThisEntityManager.thisGame.gameComponent.gamePlayComponent.stage.size);
                    }
                    else
                    {
                        Enemy enemy = (Enemy)entity.thisEntity;
                        enemy.TakenDamage();
                    }
                    return true;
                }
            }

            return false;
        }

        #endregion
    }

    #endregion
}

