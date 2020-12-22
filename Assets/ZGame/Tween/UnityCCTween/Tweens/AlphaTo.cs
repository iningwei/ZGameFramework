﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.cc
{
    public class AlphaTo : TweenInterval
    {
        float targetAlpha;

        Dictionary<Material, float> allMaterials = new Dictionary<Material, float>();
        Dictionary<UnityEngine.UI.Image, float> allImages = new Dictionary<UnityEngine.UI.Image, float>();
        Dictionary<UnityEngine.UI.RawImage, float> allRawImages = new Dictionary<UnityEngine.UI.RawImage, float>();
        Dictionary<UnityEngine.UI.Text, float> allTexts = new Dictionary<UnityEngine.UI.Text, float>();



        bool includeChilds = false;
        bool includeInactive = false;

        public override event EventHandler<TweenFinishedEventArgs> TweenFinished;

        public AlphaTo(float duration, float targetAlpha, bool includeChilds = false, bool includeInactive = false)
        {
            if (duration < 0)
            {
                Debug.LogError("error,AlphaTo duration should >=0");
                return;
            }
            this.SetDuration(duration);
            this.targetAlpha = targetAlpha;
            this.includeChilds = includeChilds;
            this.includeInactive = includeInactive;
            this.SetTweenName("AlphaTo");
        }
        public override Tween Delay(float time)
        {
            return new Sequence(new DelayTime(time), this);
        }

        public override Tween Easing(Ease ease)
        {
            this.easeFunc = EaseTool.Get(ease);
            return this;
        }

        public override void Finish()
        {
            this.isDone = true;
            this.repeatedTimes = 0;
            if (this.completeCallback != null)
            {
                this.completeCallback(this.completeCallbackParams);
            }
            this.TweenFinished?.Invoke(this, new TweenFinishedEventArgs(this.GetHolder(), this));
        }


        public override Tween OnComplete(Action<object[]> callback, params object[] param)
        {
            this.completeCallback = callback;
            this.completeCallbackParams = param;
            return this;
        }



        public override void Run()
        {
            this.isDone = false;

            if (this.repeatedTimes == 0)
            {
                //TODO:Set From Alpha
                #region 3D物体或者Unity2D物体
                Renderer[] allRenderers;
                if (includeChilds)
                {
                    allRenderers = this.holder.GetComponentsInChildren<Renderer>(includeInactive);
                }
                else
                {
                    allRenderers = this.holder.GetComponents<Renderer>();
                }

                for (int i = 0; i < allRenderers.Length; i++)
                {
                    var mats = allRenderers[i].GetMaterials();
                    for (int j = 0; j < mats.Length; j++)
                    {
                        if (!allMaterials.ContainsKey(mats[j]))
                        {
                            //若shader不支持透明，那么即使如下设置，也不会生效
                            if (mats[j].HasProperty("_Color"))
                            {
                                allMaterials.Add(mats[j], mats[j].color.a);

                            }
                            else if (mats[j].HasProperty("_TintColor"))
                            {
                                allMaterials.Add(mats[j], mats[j].GetColor("_TintColor").a);
                            }
                        }
                    }
                }
                #endregion

                #region UGUI物体

                if (includeChilds)
                {
                    var imgs = this.GetHolder().GetComponentsInChildren<UnityEngine.UI.Image>(includeInactive);
                    for (int i = 0; i < imgs.Length; i++)
                    {
                        this.allImages.Add(imgs[i], imgs[i].color.a);
                    }

                    var rawImgs = this.GetHolder().GetComponentsInChildren<UnityEngine.UI.RawImage>(includeInactive);
                    for (int i = 0; i < rawImgs.Length; i++)
                    {
                        this.allRawImages.Add(rawImgs[i], rawImgs[i].color.a);
                    }

                    var texts = this.GetHolder().GetComponentsInChildren<UnityEngine.UI.Text>(includeInactive);
                    for (int i = 0; i < texts.Length; i++)
                    {
                        this.allTexts.Add(texts[i], texts[i].color.a);
                    }
                }
                else
                {
                    var img = this.GetHolder().GetComponent<UnityEngine.UI.Image>();
                    this.allImages.Add(img, img.color.a);

                    var rawImg = this.GetHolder().GetComponent<UnityEngine.UI.RawImage>();
                    this.allRawImages.Add(rawImg, rawImg.color.a);

                    var text = this.GetHolder().GetComponent<UnityEngine.UI.Text>();
                    this.allTexts.Add(text, text.color.a);
                }


                #endregion

            }
            else
            {
                if (this.GetRepeatType() == RepeatType.Clamp)
                {
                    this.tweenDiretion = 1;
                }
                else
                {
                    this.tweenDiretion = -this.tweenDiretion;
                }
            }

            //Debug.Log(this.GetTarget() + "  AlphaTo 相关mat个数：" + allMaterials.Count);
            this.startTime = this.GetTime() - this.GetTotalPausedTime();
            this.truePartialRunTime = 0f;
        }





        public override bool Update()
        {
            if (this.IsDone())
            {
                return true;
            }
            if (this.IsPause())
            {
                return false;
            }

            this.truePartialRunTime = this.GetTime() - startTime - this.GetTotalPausedTime();
            if (this.truePartialRunTime > this.duration)
            {
                this.OnPartialTweenFinished();
            }

            this.doAlphaTo();
            this.doUpdateCallback();

            return this.IsDone();
        }

        private void doUpdateCallback()
        {
            if (this.IsDone() || this.updateCallback == null)
            {
                return;
            }

            this.updateCallback(this.truePartialRunTime);
        }



        private void doAlphaTo()
        {
            if (this.IsDone())
            {
                return;
            }

            float t = this.truePartialRunTime / this.duration;
            t = t > 1 ? 1 : t;

            //Unity3D obj or Unity2D obj
            foreach (var item in this.allMaterials)
            {
                var dir = this.tweenDiretion == 1 ? (this.targetAlpha - item.Value) : (item.Value - this.targetAlpha);
                var desAlpha = (this.tweenDiretion == 1 ? item.Value : this.targetAlpha) + dir * (this.easeFunc(t));


                item.Key.color = new Color(item.Key.color.r, item.Key.color.g, item.Key.color.b, desAlpha);
            }


            //UGUI obj
            foreach (var item in this.allImages)
            {
                var dir = this.tweenDiretion == 1 ? (this.targetAlpha - item.Value) : (item.Value - this.targetAlpha);
                var desAlpha = (this.tweenDiretion == 1 ? item.Value : this.targetAlpha) + dir * (this.easeFunc(t));
                item.Key.color = new Color(item.Key.color.r, item.Key.color.g, item.Key.color.b, desAlpha);
            }

            foreach (var item in this.allRawImages)
            {
                var dir = this.tweenDiretion == 1 ? (this.targetAlpha - item.Value) : (item.Value - this.targetAlpha);
                var desAlpha = (this.tweenDiretion == 1 ? item.Value : this.targetAlpha) + dir * (this.easeFunc(t));
                item.Key.color = new Color(item.Key.color.r, item.Key.color.g, item.Key.color.b, desAlpha);
            }

            foreach (var item in this.allTexts)
            {
                var dir = this.tweenDiretion == 1 ? (this.targetAlpha - item.Value) : (item.Value - this.targetAlpha);
                var desAlpha = (this.tweenDiretion == 1 ? item.Value : this.targetAlpha) + dir * (this.easeFunc(t));
                item.Key.color = new Color(item.Key.color.r, item.Key.color.g, item.Key.color.b, desAlpha);
            }

        }

        protected override void OnPartialTweenFinished()
        {
            this.repeatedTimes++;
            if (this.repeatedTimes == this.repeatTimes)
            {
                this.Finish();
            }
            else
            {
                this.Run();
            }
        }



        public override void Pause()
        {
            if (this.isPause)
            {
                return;
            }

            this.isPause = true;
            this.lastPausedTime = this.GetTime();
        }

        public override void Resume()
        {
            if (this.isPause == false)
            {
                return;
            }

            this.totalPausedTime += (this.GetTime() - this.lastPausedTime);
        }



        public override Tween OnUpdate(Action<float> callback)
        {
            this.updateCallback = callback;
            return this;
        }

    }
}