using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

namespace ZGame.Pack
{
    public class PackConfig
    {
        public static bool ABResEncrypt;
        public static bool ABNameConfuse;
        static PackConfig()
        {
            var asset = Resources.Load("PackConfig", typeof(TextAsset)) as TextAsset;

            var dic = Json.Deserialize(asset.text) as Dictionary<string, object>;
            ABResEncrypt = (bool)dic["ABResEncrypt"];
            ABNameConfuse = (bool)dic["ABNameConfuse"];
        }

    }
}