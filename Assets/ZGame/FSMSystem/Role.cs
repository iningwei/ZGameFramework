using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;
using ZGame.FSM;

public class Role
{
    public GameObject obj = null;
    public FSMSystem fsm = null;

    public Role(GameObject obj)
    {
        this.obj = obj;
        this.fsm = new FSMSystem();
        this.InitFSMSystem();
    }

    public virtual void InitFSMSystem()
    {

    }

    public virtual void Update(float dt)
    {
        if (fsm != null)
        {
            this.fsm.Update(dt);
        }
    }
}
