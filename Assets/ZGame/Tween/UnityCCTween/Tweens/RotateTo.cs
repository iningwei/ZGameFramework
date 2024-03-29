using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.cc
{
    public class RotateTo : TweenInterval
    {
        Vector3 startAngle;
        Vector3 targetAngle;
        Space relativeSpace;
        bool ignoreClosest = false;
        public RotateTo(float duration, Vector3 targetAngle, Space relativeSpace, bool ignoreClosest = false)
        {
            if (duration < 0)
            {
                Debug.LogError("error,RotateTo duration should >=0");
                return;
            }
            this.SetDuration(duration);
            this.targetAngle = targetAngle;
            //Debug.LogError("targetAngle:" + this.targetAngle);
            this.relativeSpace = relativeSpace;
            this.ignoreClosest = ignoreClosest;
            this.SetTweenName("RotateTo");
        }

        public override event EventHandler<TweenFinishedEventArgs> TweenFinished;



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
            if (this.TweenFinished != null)
            {
                this.TweenFinished(this, new TweenFinishedEventArgs(this.GetHolder(), this));
            }
        }



        public override Tween OnComplete(Action<object[]> callback, params object[] param)
        {
            this.completeCallback = callback;
            this.completeCallbackParams = param;
            return this;
        }

        public override Tween OnUpdate(Action<float> callback)
        {
            this.updateCallback = callback;
            return this;
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
            if (!this.isPause)
            {
                return;
            }
            this.totalPausedTime += (this.GetTime() - this.lastPausedTime);
        }



        public override void Run()
        {
            this.isDone = false;
            if (this.repeatedTimes == 0)
            {

                this.startAngle = this.relativeSpace == Space.Self ? this.holder.transform.localEulerAngles : this.holder.transform.eulerAngles;



                if (this.ignoreClosest == false)
                {
                    //caculate more suitable targetAngle
                    //if not
                    //Such as statAngle x=300, targetAngle x=10, then it will rotate from 300 to 10 like 290��270��260...10. It rotated 290 degree.In fact we need it rotate 70 degree in xAxis

                    Vector3 suitTargetAngle = new Vector3(closestRot(this.startAngle.x, this.targetAngle.x), closestRot(this.startAngle.y, this.targetAngle.y), closestRot(this.startAngle.z, this.targetAngle.z));
                    //Debug.LogError($"startAngle:{startAngle}, targetAngle:{targetAngle},suitTargetAngle:{suitTargetAngle}");

                    this.targetAngle = suitTargetAngle;
                }
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
            this.doRotateTo();
            this.doUpdateCallback();

            return this.IsDone();
        }

        private void doRotateTo()
        {
            if (this.IsDone())
            {
                return;
            }

            float t = this.truePartialRunTime / this.duration;
            t = t > 1 ? 1 : t;
            var dir = this.tweenDiretion == 1 ? (this.targetAngle - this.startAngle) : (this.startAngle - this.targetAngle);
            var desAngle = (this.tweenDiretion == 1 ? this.startAngle : this.targetAngle) + dir * (this.easeFunc(t));
            if (this.relativeSpace == Space.Self)
            {
                this.holder.transform.localEulerAngles = desAngle;
            }
            else
            {
                this.holder.transform.eulerAngles = desAngle;
            }
        }

        private void doUpdateCallback()
        {
            if (this.IsDone() || this.updateCallback == null)
            {
                return;
            }
            this.updateCallback(this.truePartialRunTime);
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



        //this code from LeanTween
        float closestRot(float from, float to)
        {
            float minusWhole = 0 - (360 - to);
            float plusWhole = 360 + to;
            float toDiffAbs = Mathf.Abs(to - from);
            float minusDiff = Mathf.Abs(minusWhole - from);
            float plusDiff = Mathf.Abs(plusWhole - from);
            if (toDiffAbs < minusDiff && toDiffAbs < plusDiff)
            {
                return to;
            }
            else
            {
                if (minusDiff < plusDiff)
                {
                    return minusWhole;
                }
                else
                {
                    return plusWhole;
                }
            }
        }
    }
}