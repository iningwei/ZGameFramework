﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.cc
{
    /*  
     * 每个 TweenComp 在执行了Run之后不允许加入新的Tween，有需要的话，可以通过为target添加新的TweenComp来解决
     * 
     */
    public class TweenComp : MonoBehaviour
    {

        [SerializeField]
        int tweenId;

        public string tweenName;
        public bool isFinished = false;

        Tween tween = null;
        public void AddTween(Tween tween)
        {
            if (this.tween != null)
            {
                Debug.LogError("该TweenComp已经运行，无法中途插入补间");
                return;
            }

            this.tweenName = tween.GetTweenName();
            this.tween = tween;
            tweenId = this.tween.GetId();

            this.tween.Run();
        }

        public Tween GetTween()
        {
            return this.tween;
        }

        float pausedTime = 0;
        bool isPaused = false;
        public void PauseTween()
        {
            this.isPaused = true;
            this.pausedTime = 0;
        }
        public void ResumeTween()
        {
            this.isPaused = false;
        }

        void Update()
        {
            if (this.tween != null)
            {
                if (isPaused)
                {
                    this.pausedTime += Time.deltaTime;
                    return;
                }

                if (this.tween.Update())
                {
                    //Debug.Log(this.gameObject.name + " 上的补间播完了 ");
                    this.tween = null;
                    this.isFinished = true;
                }
            }
        }


        //Recycle Tween if it is not null while destroy!!!
        private void OnDestroy()
        {
            if (this.tween != null)
            {
                TweenManager.Instance.RemoveTween(this.gameObject, this.tween);
            }
        }
    }
}
