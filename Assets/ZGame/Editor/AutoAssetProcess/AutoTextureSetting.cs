using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoTextureSetting : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture2D)
    {
        var importer = assetImporter as TextureImporter;
        //importer.alphaSource = TextureImporterAlphaSource.FromInput;
        //importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;

        importer.sRGBTexture = false;//经测试开启了sRGB会影响Texture的mipmapCount参数的值。。。unity bug???
        importer.isReadable = false;
        importer.npotScale = TextureImporterNPOTScale.None;

        //////    TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
        //////androidSettings.maxTextureSize = 2048;
        //////androidSettings.format = TextureImporterFormat.ASTC_6x6;
        //////   androidSettings.name = "Android";
        //////androidSettings.overridden = true;
        //////    importer.SetPlatformTextureSettings(androidSettings);


        TextureImporterPlatformSettings iosSettings = new TextureImporterPlatformSettings();
        //iosSettings.maxTextureSize = 2048;
        iosSettings.format = TextureImporterFormat.ASTC_6x6;
        iosSettings.name = "iPhone";
        iosSettings.overridden = true;

        importer.SetPlatformTextureSettings(iosSettings);
    }

    void OnPreprocessAnimation()
    {

        ModelImporter modelImporter = assetImporter as ModelImporter;
        modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;



    }

    void OnPreprocessAudio()
    {
        //TODO:后续自动把双通道改成单通道
        //实测双通道改单通道，音质变化太大。作废

        //if (assetPath.Contains("mono"))
        //{

        //}

        //AudioImporter audioImporter = assetImporter as AudioImporter;
        //audioImporter.forceToMono = true;
    }
}
