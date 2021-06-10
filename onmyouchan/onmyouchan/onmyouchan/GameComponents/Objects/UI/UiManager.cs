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
    #region 構造体、インターフェースとかベースUi
    
    /// <summary>
    /// 画像インデックスのデータ
    /// </summary>
    struct TextureIndexStatus
    {
        public string idName;
        public Texture2D texture;
    }

    #endregion

    /// <summary>
    /// プレイ中の2D表示UIを管理するクラス
    /// </summary>
    class UiManager
    {
        #region フィールド

        /// <summary>
        /// 所属しているゲームループの参照
        /// </summary>
        public Game1 ThisGame1
        {
            get { return thisGame1; }
        }
        Game1 thisGame1;

        /// <summary>
        /// Uiのリスト
        /// </summary>
        public List<Ui> UiList
        { get { return uiList; } }
        List<Ui> uiList;

        /// <summary>
        /// テクスチャデータ
        /// </summary>
        public List<TextureIndexStatus> textureList;

        #endregion

        #region コンストラクタ

        public UiManager(Game1 game)
        {
            thisGame1 = game;

            uiList = new List<Ui>();
            textureList = new List<TextureIndexStatus>();
        }

        #endregion

        #region 初期化

        /// <summary>
        /// 全てをクリア
        /// </summary>
        public void Initialize()
        {
            uiList.Clear();
        }

        #endregion

        #region 更新

        public void Update(GameTime time)
        {
            //登録されてるUiを更新
            for (int i = 0; i < uiList.Count; i++)
            {
                uiList[i].Update(time);
            }
        }

        #endregion

        #region 描画

        public void Draw()
        {
            thisGame1.spriteBatch.Begin();

            for (int i = 0; i < uiList.Count; i++)
            {
                uiList[i].Draw();
            }
            thisGame1.spriteBatch.End();
        }

        #endregion

        #region メソッド

        public void LordUi(Ui ui)
        {
            uiList.Add(ui);
            ui.Lord(this);
        }
        
        /// <summary>
        /// 複数の画像をロードします
        /// </summary>
        /// <param name="texName"></param>
        public void TextureLords(params string[] texName)
        {
            for (int i = 0; i < texName.Length; i++)
            {
                if (!textureList.Exists(x => x.idName == texName[i]))
                {
                    TextureIndexStatus tex;
                    tex.idName = texName[i];
                    tex.texture = thisGame1.Content.Load<Texture2D>(texName[i]);
                    textureList.Add(tex);
                }
            }
        }
        /// <summary>
        /// 画像がインデックスに無ければ追加し、その後参照を返します
        /// </summary>
        /// <param name="texName"></param>
        /// <returns></returns>
        public Texture2D TextureLord(string texName)
        {
            if (!textureList.Exists(x => x.idName == texName))
            {
                TextureIndexStatus tex;
                tex.idName = texName;
                tex.texture = thisGame1.Content.Load<Texture2D>(texName);
                textureList.Add(tex);
            }
            return textureList.Find(x => x.idName == texName).texture;
        }

        /// <summary>
        /// 表示する位置を0~1の割合でセットする
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2 setPosition(Vector2 pos)
        {
            Vector2 output;
            output.Y = pos.Y * ThisGame1.graphics.PreferredBackBufferHeight;
            output.X = pos.X * ThisGame1.graphics.PreferredBackBufferWidth;
            return output;
        }

        #endregion
    }
}
