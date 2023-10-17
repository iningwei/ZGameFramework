using UnityEngine;
using UnityEngine.SceneManagement;
using ZGame;
using ZGame.Ress.AB;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;
using ZGame.Window;
using System.Collections.Generic;

class People
{
    public string name;
    public int age;
    public People(string name, int age)
    {
        this.name = name;
        this.age = age;
    }
}


public class Launcher : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("@launcher!!!");
        DontDestroyOnLoad(gameObject);
        AppManager.Instance.Init(this.transform);
    }
}
