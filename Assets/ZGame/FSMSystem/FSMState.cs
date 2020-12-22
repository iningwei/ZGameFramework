using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.FSM
{
    public class FSMState
    {
        Dictionary<TransitionType, StateType> map = new Dictionary<TransitionType, StateType>();

        public StateType StateType
        {
            get;
            set;
        }

        public FSMState()
        {
            InitTransitions();
        }

        public virtual void InitTransitions()
        {

        }
        public void AddTransition(TransitionType trans, StateType state)
        {
            if (trans == TransitionType.None)
            {
                Debug.LogError("error, not allow add TransitionType None");
                return;
            }
            if (state == StateType.None)
            {
                Debug.LogError("error, not allow add StateType None");
                return;
            }
            if (this.map.ContainsKey(trans))
            {
                Debug.LogError("error,already exist transition:" + trans);
                return;
            }
            this.map.Add(trans, state);
        }


        public void DeleteTransition(TransitionType trans)
        {
            if (trans == TransitionType.None)
            {
                Debug.LogError("DeleteTransition None is not allowed");
                return;
            }

            if (this.map.ContainsKey(trans))
            {
                this.map.Remove(trans);
                return;
            }

            Debug.LogError($"error,Transition:{trans.ToString()} not exist");
        }


        public StateType GetOutputState(TransitionType trans)
        {
            if (this.map.ContainsKey(trans))
            {
                return this.map[trans];
            }
            Debug.LogError($"error,Transition:{trans.ToString()} not exist in state:" + GetType().ToString());
            return StateType.None;
        }


        public virtual void DoBeforeEntering(params object[] paras)
        {

        }
        public virtual void DoAfterEntering(params object[] paras)
        {

        }

        public virtual void DoBeforeLeaving()
        {
            this.RemoveEventListeners();
        }

        public virtual void AddEventListeners()
        {

        }

        public virtual void RemoveEventListeners()
        {

        }

        public virtual void Reason(float dt, params object[] paras)
        {

        }

        public virtual void Act(float dt, params object[] paras)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}