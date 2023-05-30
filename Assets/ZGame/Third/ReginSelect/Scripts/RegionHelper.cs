using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionHelper : MonoBehaviour
{
    /// <summary>
    /// ����debug������༭���ﵥ�����Ը����
    /// </summary>
    public bool DebugMode = false;
    private GameObject regionPlane;
    public void Awake()
    {
        GameObject debugBtn = gameObject.FindChild("unsafeBg/SelectRegion/DebugBtn");
        if (debugBtn!=null)
        {
            if (DebugMode)
            {
                debugBtn.GetComponent<Button>().onClick.AddListener(OnDebugBtnClick);
                debugBtn.SetActive(true);
                regionPlane = gameObject.FindChild("unsafeBg/SelectRegion/Plane_SelectRegion");
                regionPlane.SetActive(false);
            }
            else
            {
                debugBtn.SetActive(false);
            }
        }
        
    }
    public void Start()
    {
        Region_Control.Instance.OnCitySelectedEvent += onCitySelected;
    }
    private void OnDebugBtnClick() {
        regionPlane.SetActive(true);
        /*        ShowSelectRegion((bool isOk, object data) => {
                    Debug.Log(isOk);
                    if (isOk)
                    {
                        string[] datas = data as string[];
                        Debug.Log($"ʡ��{datas[0]}�У�{datas[1]}");
                    }
                });*/
        ShowSelectRegionForTarget("�Ĵ�-�ɶ�");
    }

    public void ShowSelectRegion(Action<bool, object> onSelectEnd) {
        Region_Control.Instance.OpenSelectRegion(onSelectEnd);
    }

    [Obsolete("�÷�����ʱ���ã�����ע�������رհ�ť�����¼�")]
    public void ShowSelectRegionForTarget(string targetRegion, Action<bool, object> onSelectEnd)
    {
        string[] data = targetRegion.Split("-");
        if (data.Length >= 2)
        {
            Old_Region.old_province = data[0];
            Old_Region.old_city = data[1];
            //Old_Region.old_area = data[2];
        }
        Region_Control.Instance.OpenSelectRegion(onSelectEnd);
    }

    public void ShowSelectRegionForTarget(string targetRegion)
    {
        string[] data = targetRegion.Split("-");
        if (data.Length>=2)
        {
            Old_Region.old_province = data[0];
            Old_Region.old_city = data[1];
            //Old_Region.old_area = data[2];
            SelectRegion.province = Old_Region.old_province;
        }
        Region_Control.Instance.OpenSelectRegion(null);
    }

    private event Action<string> _onCitySelected;
    public void ShowSelectRegionForTarget(string targetRegion, Action<string> onCitySelected)
    {
        _onCitySelected += onCitySelected;
        ShowSelectRegionForTarget(targetRegion);
    }

    public string GetSelectedData() {
        string province = string.IsNullOrEmpty(SelectRegion.city) ? Old_Region.old_province:SelectRegion.province;
        string city = string.IsNullOrEmpty(SelectRegion.city) ? Old_Region.old_city : SelectRegion.city;

        return $"{province}-{city}";
    }

    private void onCitySelected()
    {
        string dataStr = GetSelectedData();
        _onCitySelected?.Invoke(dataStr);
    }
}
