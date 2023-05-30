﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//图形示例：http://hosted.zeh.com.br/tweener/docs/en-us/
//该部分源码来自：https://github.com/setchi/EasingCore
namespace ZGame
{
    public enum Ease
    {
        Linear,//
        InBack,
        InBounce,
        InCirc,
        InCubic,
        InElastic,
        InExpo,
        InQuad,
        InQuart,
        InQuint,
        InSine,//
        OutBack,
        OutBounce,
        OutCirc,
        OutCubic,
        OutElastic,
        OutExpo,
        OutQuad,
        OutQuart,
        OutQuint,
        OutSine,//
        InOutBack,
        InOutBounce,
        InOutCirc,
        InOutCubic,
        InOutElastic,
        InOutExpo,
        InOutQuad,
        InOutQuart,
        InOutQuint,
        InOutSine,//
    }




    public static class EaseTool
    {
        /// <summary>
        /// Gets the easing function
        /// </summary>
        /// <param name="type">ease type</param>
        /// <returns>easing function</returns>
        public static Func<float, float> Get(Ease type)
        {
            switch (type)
            {
                case Ease.Linear: return Linear;
                case Ease.InBack: return InBack;
                case Ease.InBounce: return InBounce;
                case Ease.InCirc: return InCirc;
                case Ease.InCubic: return InCubic;
                case Ease.InElastic: return InElastic;
                case Ease.InExpo: return InExpo;
                case Ease.InQuad: return InQuad;
                case Ease.InQuart: return InQuart;
                case Ease.InQuint: return InQuint;
                case Ease.InSine: return InSine;
                case Ease.OutBack: return OutBack;
                case Ease.OutBounce: return OutBounce;
                case Ease.OutCirc: return OutCirc;
                case Ease.OutCubic: return OutCubic;
                case Ease.OutElastic: return OutElastic;
                case Ease.OutExpo: return OutExpo;
                case Ease.OutQuad: return OutQuad;
                case Ease.OutQuart: return OutQuart;
                case Ease.OutQuint: return OutQuint;
                case Ease.OutSine: return OutSine;
                case Ease.InOutBack: return InOutBack;
                case Ease.InOutBounce: return InOutBounce;
                case Ease.InOutCirc: return InOutCirc;
                case Ease.InOutCubic: return InOutCubic;
                case Ease.InOutElastic: return InOutElastic;
                case Ease.InOutExpo: return InOutExpo;
                case Ease.InOutQuad: return InOutQuad;
                case Ease.InOutQuart: return InOutQuart;
                case Ease.InOutQuint: return InOutQuint;
                case Ease.InOutSine: return InOutSine;
                default: return Linear;
            }
        }

        static float Linear(float t) => t;

        static float InBack(float t) => t * t * t - t * Mathf.Sin(t * Mathf.PI);

        static float OutBack(float t) => 1f - InBack(1f - t);

        static float InOutBack(float t) =>
            t < 0.5f
                ? 0.5f * InBack(2f * t)
                : 0.5f * OutBack(2f * t - 1f) + 0.5f;

        static float InBounce(float t) => 1f - OutBounce(1f - t);

        static float OutBounce(float t) =>
            t < 4f / 11.0f ?
                (121f * t * t) / 16.0f :
            t < 8f / 11.0f ?
                (363f / 40.0f * t * t) - (99f / 10.0f * t) + 17f / 5.0f :
            t < 9f / 10.0f ?
                (4356f / 361.0f * t * t) - (35442f / 1805.0f * t) + 16061f / 1805.0f :
                (54f / 5.0f * t * t) - (513f / 25.0f * t) + 268f / 25.0f;

        static float InOutBounce(float t) =>
            t < 0.5f
                ? 0.5f * InBounce(2f * t)
                : 0.5f * OutBounce(2f * t - 1f) + 0.5f;

        static float InCirc(float t) => 1f - Mathf.Sqrt(1f - (t * t));

        static float OutCirc(float t) => Mathf.Sqrt((2f - t) * t);

        static float InOutCirc(float t) =>
            t < 0.5f
                ? 0.5f * (1 - Mathf.Sqrt(1f - 4f * (t * t)))
                : 0.5f * (Mathf.Sqrt(-((2f * t) - 3f) * ((2f * t) - 1f)) + 1f);

        static float InCubic(float t) => t * t * t;

        static float OutCubic(float t) => InCubic(t - 1f) + 1f;

        static float InOutCubic(float t) =>
            t < 0.5f
                ? 4f * t * t * t
                : 0.5f * InCubic(2f * t - 2f) + 1f;

        static float InElastic(float t) => Mathf.Sin(13f * (Mathf.PI * 0.5f) * t) * Mathf.Pow(2f, 10f * (t - 1f));

        static float OutElastic(float t) => Mathf.Sin(-13f * (Mathf.PI * 0.5f) * (t + 1)) * Mathf.Pow(2f, -10f * t) + 1f;

        static float InOutElastic(float t) =>
            t < 0.5f
                ? 0.5f * Mathf.Sin(13f * (Mathf.PI * 0.5f) * (2f * t)) * Mathf.Pow(2f, 10f * ((2f * t) - 1f))
                : 0.5f * (Mathf.Sin(-13f * (Mathf.PI * 0.5f) * ((2f * t - 1f) + 1f)) * Mathf.Pow(2f, -10f * (2f * t - 1f)) + 2f);

        static float InExpo(float t) => Mathf.Approximately(0.0f, t) ? t : Mathf.Pow(2f, 10f * (t - 1f));

        static float OutExpo(float t) => Mathf.Approximately(1.0f, t) ? t : 1f - Mathf.Pow(2f, -10f * t);

        static float InOutExpo(float v) =>
            Mathf.Approximately(0.0f, v) || Mathf.Approximately(1.0f, v)
                ? v
                : v < 0.5f
                    ? 0.5f * Mathf.Pow(2f, (20f * v) - 10f)
                    : -0.5f * Mathf.Pow(2f, (-20f * v) + 10f) + 1f;

        static float InQuad(float t) => t * t;

        static float OutQuad(float t) => -t * (t - 2f);

        static float InOutQuad(float t) =>
            t < 0.5f
                ? 2f * t * t
                : -2f * t * t + 4f * t - 1f;

        static float InQuart(float t) => t * t * t * t;

        static float OutQuart(float t)
        {
            var u = t - 1f;
            return u * u * u * (1f - t) + 1f;
        }

        static float InOutQuart(float t) =>
            t < 0.5f
                ? 8f * InQuart(t)
                : -8f * InQuart(t - 1f) + 1f;

        static float InQuint(float t) => t * t * t * t * t;

        static float OutQuint(float t) => InQuint(t - 1f) + 1f;

        static float InOutQuint(float t) =>
            t < 0.5f
                ? 16f * InQuint(t)
                : 0.5f * InQuint(2f * t - 2f) + 1f;

        static float InSine(float t) => Mathf.Sin((t - 1f) * (Mathf.PI * 0.5f)) + 1f;

        static float OutSine(float t) => Mathf.Sin(t * (Mathf.PI * 0.5f));

        static float InOutSine(float t) => 0.5f * (1f - Mathf.Cos(t * Mathf.PI));
    }
}
