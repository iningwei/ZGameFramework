using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_Select : MonoBehaviour {

    public Button btn_Select;
    public GameObject Select_Plane;

    void Start () {

        Select_Plane.SetActive(false);
        btn_Select.onClick.AddListener(OnClick);
    }
		
	void OnClick()
    {
        Select_Plane.SetActive(true);
        Region_Control.Instance.OpenSelectRegion(OnSelectEnd);
    }
    void OnSelectEnd(bool isOk, object data) {
        string[] datas = (string[])data;
        if (isOk)
        {
            Debug.Log($"确认回调！省：{datas[0]}     市：{datas[1]}     区：{datas[2]}");
        }
        else {
            Debug.Log("取消操作！");
        }
    }
}
