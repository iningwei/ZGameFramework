using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class RSA
{
    /// <summary>
    /// 生成RSA私钥 公钥
    /// </summary>
    /// <param name="privateKey"></param>
    /// <param name="publicKey"></param>
    public static void RSAGenerateKey(ref string privateKey, ref string publicKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        privateKey = rsa.ToXmlString(true);
        publicKey = rsa.ToXmlString(false);
    }

    /// <summary>
    /// 用RSA公钥 加密
    /// </summary>
    /// <param name="data"></param>
    /// <param name="publicKey">xml格式</param>
    /// <returns></returns>
    public static byte[] RSAEncrypt(byte[] data, string publicKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(publicKey);
        byte[] encryptData = rsa.Encrypt(data, false);
        return encryptData;
    }

    /// <summary>
    /// 用RSA私钥 解密
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static byte[] RSADecrypt(byte[] data, string privateKey)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(privateKey);
        byte[] decryptData = rsa.Decrypt(data, false);
        return decryptData;
    }

 
}
