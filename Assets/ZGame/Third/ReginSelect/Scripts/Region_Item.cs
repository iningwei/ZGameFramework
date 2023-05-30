/*
 * 功能：地区按钮模板  显示地区信息
 */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Region_Item : MonoBehaviour {
    /// <summary>
    /// //按钮文字信息 用于展示
    /// </summary>
    public TextMeshProUGUI text_Info;
    /// <summary>
    /// //按钮文字信息 用于缓存完整数据
    /// </summary>
    [HideInInspector]
    public string content_info;

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(OnClickBtn);
    }

    
    public void SetTextInfo(string text)
    {
        content_info = text;
        text_Info.text = Region_Control.Instance.CacuShowString(text);
        
    }
    /// <summary>
    /// 点击按钮
    /// </summary>
    public void OnClickBtn()
    {
        switch (SelectRegion.type)
        {
            case RegionType.Province_Type:
                SelectRegion.province = content_info;
                Debug.Log("选择的省：" + content_info);

                //选择省后，打开市
                if (Region_Control.Instance.SelectLevel > RegionType.Province_Type)
                {
                    Region_Control.Instance.Btn_City.gameObject.SetActive(true);
                    Region_Control.Instance.OnClickCity();
                }
                break;
            case RegionType.City_Type:
                SelectRegion.city = content_info;
                Debug.Log("选择的市：" + content_info);
                Region_Control.Instance.OnSelectedCity();
                //选择市后，打开区
                if (Region_Control.Instance.SelectLevel > RegionType.City_Type)
                {
                    Region_Control.Instance.Btn_Area.gameObject.SetActive(true);
                    Region_Control.Instance.OnClickArea();
                }
                break;
            case RegionType.Area_Type:
                SelectRegion.area = content_info;

                Debug.Log("选择的区：" + content_info);
                break;
        }
        //改变按钮颜色
        Region_Control.Instance.ChangeBtnColor(SelectRegion.type);
    }

}