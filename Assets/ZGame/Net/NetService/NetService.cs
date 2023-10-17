using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetService
{

    public NetService()
    {
        this.RegisterRcvMsg();
    }
    
    public virtual void RegisterRcvMsg()
    {

    } 

    public virtual void UnregisterRcvMsg()
    {
         
    }
    public void Dispose()
    {
        this.UnregisterRcvMsg();
    }
}
