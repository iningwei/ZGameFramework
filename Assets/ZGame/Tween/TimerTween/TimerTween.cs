﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZGame.TimerTween
{
    public class TimerTween
    {
        public static Timer ValueByInterval(float from, float to, float duration, float interval, Action<float> updateCallback, Action complteCallback)
        {
            float lastTime = 0;
            float gap = to - from;
            float offset = gap / duration;
            int tickedCount = 0;
            Timer timer = new Timer(duration,
                onComplete: () =>
            {
                //UnityEngine.Debug.Log("tickedCount:" + tickedCount);
                lastTime = 0;
                tickedCount = 0;
                if (complteCallback != null)
                {
                    complteCallback();
                }
            },
                onUpdate: (t) =>
            {
                //UnityEngine.Debug.Log("  t:" + t);
                if (Math.Abs(t - lastTime) >= interval)
                {
                    tickedCount++;
                    lastTime = t;
                    float v = from + t * duration * offset;
                    //UnityEngine.Debug.Log("v:" + v + ", t:" + t + ", duration:" + duration + ", offset:" + offset + ", from:" + from);
                    if (updateCallback != null)
                    {
                        updateCallback(v);
                    }
                }
            });
            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer Value(float from, float to, float duration, Action<float> updateCallback, Action complteCallback)
        {
            float gap = to - from;
            float offset = gap / duration;
            int tickedCount = 0;
            //t由于要进行ease变换，故限制了范围为[0,1]
            Timer timer = new Timer(duration, onComplete: () =>
            {
                //UnityEngine.Debug.Log("tickedCount:" + tickedCount);
                tickedCount = 0;
                if (complteCallback != null)
                {
                    complteCallback();
                }
            }, onUpdate: (t) =>
            {
                tickedCount++;
                float v = from + t * duration * offset;
                //UnityEngine.Debug.Log("v:" + v + ", t:" + t + ", duration:" + duration + ", offset:" + offset + ", from:" + from);
                if (updateCallback != null)
                {
                    updateCallback(v);
                }
            });

            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer Delay(float delayTime, Action call)
        {
            Timer timer = new Timer(delayTime, onComplete: () =>
            {
                if (call != null)
                {
                    call();
                }
            });
            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer Repeat(float interval, Func<bool> repeatCallback)
        {
            Timer timer = new Timer(interval);
            timer.SetLoop(0);
            timer.SetOnComplete(() =>
            {
                if (repeatCallback != null)
                {
                    bool r = repeatCallback();
                    if (!r)
                    {
                        timer.Cancel();
                    }
                }
            });

            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static Timer RepeatCount(float interval, int count, Func<bool> repeatCallback)
        {
            if (count < 0)
            {
                UnityEngine.Debug.LogError("error, count<0");
                return null;
            }
            Timer timer = new Timer(interval);
            timer.SetLoop(count);
            timer.SetOnComplete(() =>
            {
                if (repeatCallback != null)
                {
                    bool r = repeatCallback();
                    if (!r)
                    {
                        timer.Cancel();
                    }
                }
            });

            TimerManager.Instance.RegisterTimer(timer);
            return timer;
        }

        public static void Cancel(Timer timer)
        {
            TimerManager.Instance.CancelTimer(timer);
        }

        public static void Pause(Timer timer)
        {
            timer?.Pause();
        }

        public static void Resume(Timer timer)
        {
            timer?.Resume();
        }
    }
}
