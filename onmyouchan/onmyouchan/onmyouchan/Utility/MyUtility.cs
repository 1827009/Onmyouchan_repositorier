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
    /// <summary>
    /// 変数の変換メソッドとかいつでも使いたいときに入れときたいもののクラスです
    /// </summary>
    static class MyUtility
    {
        public const float SecondFrame = 60f;

        /// <summary>
        /// int型でaをb乗します
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int IntPow(int a, int b)
        {
            int aCopy = a;
            if (b == 0)
                return 1;
            for (int i = 1; i < b; i++)
            {
                a *= aCopy;
            }
            return a;
        }

        /// <summary>
        /// float型でaをb乗します
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float FloatPow(float a, int b)
        {
            float aCopy = a;
            if (b == 0)
                return 1;
            for (int i = 1; i < b; i++)
            {
                a *= aCopy;
            }
            return a;
        }

        /// <summary>
        /// Vector2の小数点をカットします
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static void RoundOff(ref float a)
        {
            if (a - Math.Floor(a) > 0.45f)
                a = (float)Math.Floor(a);// + 1f;
            else
                a = (float)Math.Floor(a);
        }
        public static void RoundOff2(ref float a)
        {
            if (a - Math.Floor(a) > 0.45f)
                a = (float)Math.Floor(a) + 1f;
            else
                a = (float)Math.Floor(a) + 1f;
        }

        /// <summary>
        /// ベクトルを正規化する
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 NormalizeVector3(Vector3 vec)
        {
            Vector3 a;
            float sqrt = (float)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
            if (sqrt != 0.0f)
            {
                a.X = vec.X / sqrt;
                a.Y = vec.Y / sqrt;
                a.Z = vec.Z / sqrt;
            }
            else
            {
                return Vector3.Zero;
            }

            return a;
        }

        /// <summary>
        /// ラジアンをPI~-PIまでの値にリセットする
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        public static float OverRadianReset(float rota)
        {
            if (rota > (float)Math.PI)
                rota = (float)-Math.PI * 2 + rota;
            else if (rota < (float)-Math.PI)
                rota = (float)Math.PI * 2 + rota;

            return rota;
        }

        /// <summary>
        /// ベクトルの大きさ
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static float Vector2Size(Vector2 vec)
        {
            return (float)Math.Sqrt(FloatPow(vec.X, 2) + FloatPow(vec.Y, 2));
        }
        /// <summary>
        /// ベクトルの大きさ
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static float Vector2Size(float x,float y)
        {
            return (float)Math.Sqrt(FloatPow(x, 2) + FloatPow(y, 2));
        }

        public static void SEPlay(Cue cue,string se)
        {
            cue = Game1.seSound.GetCue(se);
            if (cue.IsPrepared)
            cue.Play();
        }
    }

    class SineWave
    {
        public float Radian
        {
            get { return rag; }
            set { rag = value; }
        }
        float rag;
        float oldRag;

        /// <summary>
        /// 回転速度(半周までの秒)
        /// </summary>
        public float RotaSpeed
        {
            get { return rotaSpeed; }
            set { rotaSpeed = (float)Math.PI / value; }
        }
        float rotaSpeed = 1;
        /// <summary>
        /// 回転のsinの値を返す
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public float Sin(GameTime gameTime)
        {
            oldRag = rag;
            rag += rotaSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            rag = MyUtility.OverRadianReset(rag);
            float a = (float)Math.Sin(rag);
            return (float)Math.Sin(rag);
        }
        /// <summary>
        /// 回転が反対なら"PI",0地点なら"Zero",該当なしなら"None"を返す 
        /// </summary>
        /// <returns></returns>
        public string Point(GameTime gameTime)
        {
            if (oldRag < 0 && rag >= 0)//Math.Abs(rag) > Math.PI - (rotaSpeed*0.999f) * (float)gameTime.ElapsedGameTime.TotalSeconds)
            {
                //if (rag < 0)
                //    rag = (float)Math.PI - rotaSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //else
                //    rag = (float)Math.PI + rotaSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //rag = MyUtility.OverRadianReset(rag);
                if (rotaSpeed >= 0)
                    return "Zero";
                else
                    return "PI";
            }
            if (oldRag >= 0 && rag < 0)//Math.Abs(rag) < (rotaSpeed*0.999f) * (float)gameTime.ElapsedGameTime.TotalSeconds)
            {
                //if (rag < 0)
                //    rag = -rotaSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //else
                //    rag = rotaSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //rag = MyUtility.OverRadianReset(rag);
                if (rotaSpeed >= 0)
                    return "PI";
                else
                    return "Zero";
            }
            return "None";
        }
        /// <summary>
        /// 回転をリセットする
        /// </summary>
        public void Reset()
        {
            rag = 0;
        }
    }
}