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
        public RotateTo(float duration, Vector3 targetAngle, Space relativeSpace)
        {
            if (duration < 0)
            {
                Debug.LogError("error,RotateTo duration should >=0");
                return;
            }
            this.SetDuration(duration);
            this.targetAngle = targetAngle;
            this.relativeSpace = relativeSpace;
            this.SetTweenName("RotateTo");
        }

        public override event EventHandler<TweenFinishedEventArgs> TweenFinished;



        public override FiniteTimeTween Delay(float time)
        {
            return new Sequence(new DelayTime(time), this);
        }

        public override TweenInterval Easing(Ease ease)
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

        public override int GetRepeatTimes()
        {
            return this.repeatTimes;
        }

        public override FiniteTimeTween OnComplete(Action<object[]> callback, params object[] param)
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
            this.lastPausedTime = Time.time;
        }

        public override void Resume()
        {
            if (!this.isPause)
            {
                return;
            }
            this.totalPausedTime += (Time.time - this.lastPausedTime);
        }

        public override void Reverse()
        {
            throw new NotImplementedException();
        }

        public override void Run()
        {
            this.isDone = false;
            if (this.repeatedTimes == 0)
            {
                this.startAngle = this.relativeSpace == Space.Self ? this.holder.transform.localEulerAngles : this.holder.transform.eulerAngles;
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

            this.startTime = Time.time - this.GetTotalPausedTime();
            this.truePartialRunTime = 0f;
        }

        public override void SetDuration(float time)
        {
            this.duration = time;
        }

        public override FiniteTimeTween SetRepeatTimes(int times)
        {
            this.repeatTimes = times;
            return this;
        }

        public override FiniteTimeTween SetRepeatType(RepeatType repeatType)
        {
            this.repeatType = repeatType;
            return this;
        }

        public override FiniteTimeTween SetTag(int tag)
        {
            this.tag = tag;
            return this;
        }

        public override void SetHolder(GameObject target)
        {
            this.holder = target;
        }

        public override FiniteTimeTween SetTweenName(string name)
        {
            this.tweenName = name;
            return this;
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

            this.truePartialRunTime = Time.time - startTime - this.GetTotalPausedTime();
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
    }
}