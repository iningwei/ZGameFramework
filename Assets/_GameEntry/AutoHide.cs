using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
    float delay;
    private void OnEnable()
    {
        this.delay = 3f;
    }
    void Update()
    {
        delay -= Time.deltaTime;
        if (delay < 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
