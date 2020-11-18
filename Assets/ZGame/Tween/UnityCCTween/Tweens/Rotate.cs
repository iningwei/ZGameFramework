using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.cc
{
    /// <summary>
    /// Rotate is driven from an InfiniteTimeTween
    /// </summary>
    public class Rotate : InfiniteTimeTween
    {
        float xValue;
        float yValue;
        float zValue;
        Space relativeSpace;

        /// <summary>
        /// Applies a rotation of zAngle degrees around the z axis, xAngle degrees around
        /// the x axis, and yAngle degrees around the y axis (in that order) each seconds.        
        /// </summary>
        /// <param name="xValue"></param>
        /// <param name="yValue"></param>
        /// <param name="zValue"></param>
        /// <param name="relativeSpace"></param>
        public Rotate(float xValue, float yValue, float zValue, Space relativeSpace)
        {
            this.xValue = xValue;
            this.yValue = yValue;
            this.zValue = zValue;

            this.relativeSpace = relativeSpace;
            this.SetTweenName("Rotate");
        }

        public override event EventHandler<TweenFinishedEventArgs> TweenFinished;

     

        public override void Finish()
        {
            this.isDone = true;
            this.TweenFinished?.Invoke(this, new TweenFinishedEventArgs(this.GetHolder(), this));
        }

    
        public override void Run()
        {

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
            this.doRotate();
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

        private void doRotate()
        {
            if (this.IsDone())
            {
                return;
            }
            this.holder.transform.Rotate(xValue * Time.deltaTime, yValue * Time.deltaTime, zValue * Time.deltaTime, this.relativeSpace);

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
            this.isPause = false;
            this.totalPausedTime += (this.GetTime() - this.lastPausedTime);
        }

       

        public override Tween OnUpdate(Action<float> callback)
        {
            this.updateCallback = callback;
            return this;
        }
        public override Tween OnComplete(Action<object[]> callback, params object[] param)
        {
            this.completeCallback = callback;
            return this;
        }
    }
}