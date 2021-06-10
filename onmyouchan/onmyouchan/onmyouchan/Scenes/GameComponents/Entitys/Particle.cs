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
    class Particle:Entity
    {
        Entity BelongsEntity;

        List<SingleParticle> particles;
        Texture2D texture;

        Random rand;
        
        /// <summary>
        /// 範囲
        /// </summary>
        float range;
        int rangeRand;
        /// <summary>
        /// 頻度
        /// </summary>
        float frequency;
        /// <summary>
        /// 1桁で
        /// </summary>
        int frequencyRand;
        float spawnTime;

        float particleTime;

        Vector3 moveVector;

        /// <summary>
        /// パーティクルを発生させるEntity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="tex"></param>
        /// <param name="size"></param>
        /// <param name="r"></param>
        /// <param name="rRand"></param>
        /// <param name="fr"></param>
        /// <param name="frRand"></param>
        public Particle(Entity entity, float lifeTime, Texture2D tex, float size, float r, int rRand, float fr, int frRand)
            : base(EntityType.etc)
        {
            rand = new Random();
            particles = new List<SingleParticle>();

            BelongsEntity = entity;
            particleTime = lifeTime;
            texture = tex;
            scale = size;
            range = r;
            rangeRand = rRand;
            frequency = fr;
            frequencyRand = frRand;
        }

        public override void Update(GameTime gameTime)
        {
            if (BelongsEntity.UnloadFlag)
            {
                this.UnloadFlag = true;
                return;
            }

            if (BelongsEntity.position != position)
                position = BelongsEntity.position;

            if (spawnTime > 0)
            {
                spawnTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                float ragXY = MathHelper.ToRadians(rand.Next(360));
                float ragXZ = MathHelper.ToRadians(rand.Next(360));

                Vector3 pos;
                pos.X = (float)Math.Cos(ragXY) * (range + rand.Next(rangeRand)) + position.X;
                pos.Y = (float)Math.Sin(ragXY) * (range + rand.Next(rangeRand)) + position.Y;
                pos.Z = (float)Math.Sin(ragXZ) * (range + rand.Next(rangeRand)) + position.Z;

                particles.Add(new SingleParticle(particleTime, pos, scale, moveVector, texture, new BasicEffect(thisEntityManager.thisGame.GraphicsDevice)));
            }
            else
            {
                spawnTime = frequency + (rand.Next(frequencyRand) * 0.1f);
            }

            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime);
                if (particles[i].Life > 0)
                    particles.Remove(particles[i]);
            }

            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(thisEntityManager.thisGame.GraphicsDevice);
            }
        }
    }

    class SingleParticle
    {
        VertexPositionTexture[] vertex;
        public float Life
        { get { return life; } }
        float life;
        Effect effect;
        Texture2D texture;
        Vector3 moveVector;

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        float scale;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector3 position;

        public SingleParticle(float lifeTime,Vector3 pos, float size,Vector3 move, Texture2D tex, Effect ef)
        {
            effect = ef;
            texture = tex;
            moveVector = move;
            life = lifeTime;

            vertex = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(1,1,0)*size+pos,new Vector2(1,1)),
                new VertexPositionTexture(new Vector3(-1,1,0)*size+pos,new Vector2(1,0)),
                new VertexPositionTexture(new Vector3(-1,-1,0)*size+pos,new Vector2(0,0)),
                
                new VertexPositionTexture(new Vector3(1,1,0)*size+pos,new Vector2(1,1)),
                new VertexPositionTexture(new Vector3(1,-1,0)*size+pos,new Vector2(0,1)),
                new VertexPositionTexture(new Vector3(-1,-1,0)*size+pos,new Vector2(0,0)),
            };
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < vertex.Length; i++)
            {
                vertex[i].Position += moveVector;
            }
            life -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
        }

        public void Draw(GraphicsDevice graphics)
        {
            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertex, 0, vertex.Length / 3);
            }
        }
    }
}
