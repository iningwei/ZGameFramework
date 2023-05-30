using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZGame.TimerTween
{
    class TimerManager : SingletonMonoBehaviour<TimerManager>
    {
        public List<Timer> timers = new List<Timer>();

        public List<Timer> timersToAdd = new List<Timer>();


        public void RegisterTimer(Timer timer)
        {
            TimerGlobal.Counter++;
            if (GetTimer(TimerGlobal.Counter) != null)
            {
                //duplicated id, then continue call RegisterTimer can auto add id
                this.RegisterTimer(timer);

            }
            else
            {
                timer.SetId(TimerGlobal.Counter);
                this.timersToAdd.Add(timer);
            }
        }

        public void CancelAllTimers()
        {
            foreach (Timer timer in this.timers)
            {
                timer.Cancel();
            }

            this.timers = new List<Timer>();
            this.timersToAdd = new List<Timer>();
        }

        public void PauseAllTimers()
        {
            foreach (Timer timer in timers)
            {
                timer.Pause();
            }
        }

        public void ResumeAllTimers()
        {
            foreach (Timer timer in timers)
            {
                timer.Resume();
            }
        }

        public void CancelTimer(Timer timer)
        {
            if (this.timersToAdd.Contains(timer))
            {
                this.timersToAdd.Remove(timer);
            }
            if (this.timers.Contains(timer))
            {
                this.timers.Remove(timer);
            }
            timer?.Cancel();
        }

        public Timer GetTimer(int id)
        {
            foreach (Timer timer in this.timersToAdd)
            {
                if (timer.ID == id)
                {

                    return timer;
                }
            }
            foreach (Timer timer in this.timers)
            {
                if (timer.ID == id)
                {
                    return timer;
                }
            }
            return null;
        }

        public void CancelTimer(int id)
        {
            Timer timer = GetTimer(id);
            CancelTimer(timer);
        }

        private void Update()
        {
            this.updateAllTimers();
        }
        private void FixedUpdate()
        {
            this.fixedUpdateTimers();
        }


        int timerCount = 0;
        void updateAllTimers()
        {
            if (this.timersToAdd.Count > 0)
            {
                this.timers.AddRange(this.timersToAdd);
                this.timersToAdd.Clear();
            }

            timerCount = timers.Count;
            if (timerCount > 0)
            {
                //Debug.Log("timers count:" + timers.Count);

                for (int i = timerCount - 1; i >= 0; i--)
                {
                    if (timers[i] != null)
                    {
                        timers[i].Update();
                    }
                }




                this.timers.RemoveAll(t => t.IsDone);

            }

        }

        void fixedUpdateTimers()
        {
            if (timers.Count > 0)
            {
                try
                {
                    foreach (Timer timer in timers)
                    {
                        try
                        {
                            if (timer.IsNeedDoRealFixedUpdate)
                            {
                                timer.FixedUpdate();
                            }

                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("fixedUpdate timer ex:" + ex.ToString() + ", we remove it! please check, tag:" + timer.Tag);
                            timer.Cancel();
                        }
                    }
                }
                catch (Exception ex1)
                {
                    Debug.LogError("fixedUpdate timer ex1:" + ex1.ToString());
                }



            }
        }
    }
}
