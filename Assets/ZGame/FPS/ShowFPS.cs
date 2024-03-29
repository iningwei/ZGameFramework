using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    public TextMeshProUGUI tmpText;               // 文本组件
    public Text text;
    public float sampleTime = 0.5f; // 采样时间
    private int frame;              // 经过帧数
    private float time = 0;         // 运行时间

    private void Update()
    {
        frame += 1;
        time += Time.deltaTime;

        // 刷新帧率
        if (time >= sampleTime)
        {
            float fps = frame / time;
            if (tmpText != null)
            {
                tmpText.text = fps.ToString("F2");
            }
            else
            {
                text.text = fps.ToString("F2");
            }
            frame = 0;
            time = 0;
        }
    }
}