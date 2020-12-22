using UnityEngine;

namespace ZGame.Ress
{
    [System.Serializable]
    public class TextureRes : ImgRes
    {
        string name;
        Texture tex;

        public TextureRes(string resName, Texture resObj) : base(resName, resObj)
        {
            this.name = resName;
            this.tex = resObj;
        }

        public override T GetRes<T>(string name)
        {
            T result = default(T);
            if (!(typeof(T).Equals(typeof(Texture))))
            {
                Debug.LogError("类型不匹配Texture");
            }

            if (this.resName != name)
            {
                Debug.LogError("error,name不匹配");
            }

            result = (T)(object)this.tex;

            if (result == null)
            {
                Debug.LogError("error, get res fail,texture is null");
            }

            return result;
        }
    }

}