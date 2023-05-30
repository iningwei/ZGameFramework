/*
 * 功能：选择地区：省市县
 */
using LitJson;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 原始的地区数据
/// </summary>
[System.Serializable]
public class Old_Region
{
    public static string old_province;
    public static string old_city;
    public static string old_area;
}
/// <summary>
/// 选择的地区
/// </summary>
[System.Serializable]
public class SelectRegion
{
    public static RegionType type; //选择的阶段：省 市 区
    public static string province;
    public static string city;
    public static string area;
}

public class Region_Control :  MonoBehaviour
{
    //输入框
    public TextMeshProUGUI house_region;//地区
    public GameObject btn_parent;//按钮存放位置

    public Button Btn_Province;//省
    public Button Btn_City;//市
    public Button Btn_Area;//区

    public Button Btn_OK;//确定
    public Button Btn_Cancle;//取消
    public TextMeshProUGUI Text1;
    public TextMeshProUGUI Text2;

    /// <summary>
    /// 选项等级，是选择到市区为止，还是选择到区县为止
    /// </summary>
    public RegionType SelectLevel = RegionType.City_Type;
    public event Action OnCitySelectedEvent;
    #region 单例模式
    public static Region_Control Instance;
    void Awake()
    {
        Instance = this;
        // 防止载入新场景时被销毁
        DontDestroyOnLoad(Instance.gameObject);
    }
   
    #endregion

    void Start()
    {
        //监听
        Btn_Province.onClick.AddListener(OnClickProvince);
        Btn_City.onClick.AddListener(OnClickCity);
        Btn_Area.onClick.AddListener(OnClickArea);

        Btn_OK.onClick.AddListener(OnClickOK);
        Btn_Cancle.onClick.AddListener(OnClickCancle);

    }

    private void Update()
    {
        //输入框  实时显示选择
        if (isSave == true)
        {
            house_region.text = SelectRegion.province + SelectRegion.city + SelectRegion.area;
            Text1.text = SelectRegion.province;
            Text2.text = CacuShowString(SelectRegion.city);
        }
    }
    public string CacuShowString(string text) 
    {
        if (text.Length > 3)
        {
            StringBuilder sb = new StringBuilder(text);
            sb = sb.Replace("蒙古自治州", "");
            sb = sb.Replace("自治州", "");
            sb = sb.Replace("自治区", "");
            sb = sb.Replace("地区", "");
            sb = sb.Replace("盟", "");
            text = sb.ToString();
            if (text.Contains("族"))
            {
                text = text.Substring(0, 2);
            }
            if (text.Length > 4)
            {
                text = text.Substring(0, 2);
                text += "..";
            }
        }
        return text;
    }


    private Region read_region;//存放 解析类
    #region 读取本地数据
    /// <summary>
    /// 读取本地数据
    /// </summary>
    /// <param name="filename">Resources文件路径</param>
    public void ReadLocalDate(string filename)
    {
        read_region = new Region();
        TextAsset region_data = Resources.Load(filename) as TextAsset;
        if (!region_data)
        {
            Debug.Log("找不到本地数据文件！");
        }
        else
        {
            string text_data = region_data.text;
            read_region = JsonMapper.ToObject<Region>(text_data);
            Debug.Log("解析的本地数据：" + text_data);
        }
    }

    #endregion

    public GameObject ItemTemp;
    private List<GameObject> btn_list;//按钮存放列表
    #region  创建地区按钮
    /// <summary>
    /// 创建地区按钮
    /// </summary>
    /// <param name="count"></param>
    public void CeratBtnArea(int count)
    {
        ItemTemp.SetActive(false);
        btn_list = new List<GameObject>();
        // 清空子物体
        ClearBtnList(btn_parent.transform);
        GameObject go;
        for (int i = 0; i < count; i++)
        {
            //GameObject Prefabs = Resources.Load<GameObject>("Item");
            //// 资源在加载后要被实例化才能看到（等于克隆一个出来）
            //GameObject go= Instantiate(Prefabs);
            go = Instantiate(ItemTemp);
            go.transform.SetParent(btn_parent.transform);
            go.transform.localScale = Vector3.one;
            go.transform.rotation = Quaternion.identity;
            btn_list.Add(go);

            go.SetActive(true);
        }
        Debug.Log("生成地区按钮数量:" + count);
    }

    /// <summary>
    /// 清空子物体
    /// </summary>
    /// <param name="parent"></param>
    public void ClearBtnList(Transform parent)
    {
        List<Transform> lst = new List<Transform>();
        foreach (Transform child in parent)
        {
            lst.Add(child);
        }
        for (int i = 0; i < lst.Count; i++)
        {
            Destroy(lst[i].gameObject);
        }
        Debug.Log("清空子物体数量:" + lst.Count);
    }
    #endregion


    bool isShow;//选项是否 打开
    bool isSave;//是否 存入静态
    #region 打开地区选择

    //private Action<bool, string> selectAction;
    private event Action<bool, object> selectEvent = null;
    /// <summary>
    /// 打开地区选择
    /// </summary>
    public void OpenSelectRegion(Action<bool, object> onSelect)
    {        
        if (isShow == false)
        {
            //selectAction = onSelect;
            if (onSelect != null)
            {
                selectEvent += onSelect;
            }

            Debug.Log("打开地区选择！");
            // 读取本地数据
            ReadLocalDate("address_china");

            if (SelectRegion.province != null)
            {
                Debug.Log("存入旧的地区数据！");
                //原有数据  存入静态
                SelectRegion.province = Old_Region.old_province;
                SelectRegion.city = Old_Region.old_city;
                SelectRegion.area = Old_Region.old_area;
             
            }
            else
            {
                Debug.Log("没有旧的地区数据！");
                //数据  清空
                SelectRegion.province = "";
                SelectRegion.city = "";
                SelectRegion.area = "";
            }
            //判断开始显示的阶段：省 市 区
            StartOpenState();

            isSave = true;

            isShow = true;     
        }
        else
        {
            //关闭选项
            gameObject.SetActive(false);
            isShow = false;
            if (onSelect != null)
            {
                selectEvent -= onSelect;
            }
            Debug.Log("关闭地区选择！");
        }       
    }

    /// <summary>
    /// 点击省
    /// </summary>
    public void OnClickProvince()
    {
        Debug.Log("选择省！");
        SelectRegion.type = RegionType.Province_Type;
        SelectRegion.city = "";
        SelectRegion.area = "";

        Btn_Province.gameObject.SetActive(true);
        Btn_City.gameObject.SetActive(false);
        Btn_Area.gameObject.SetActive(false);

        //选择  省
        ShowProvince(read_region);
    
    }

    /// <summary>
    /// 点击市
    /// </summary>
    public void OnClickCity()
    {
        if (SelectRegion.province == "")
        {
            //点击省
            OnClickProvince();
        }
        else
        {
            SelectRegion.type = RegionType.City_Type;
            SelectRegion.area = "";

            Btn_Area.gameObject.SetActive(false);

            for (int i = 0; i < read_region.address.Count; i++)
            {
                if (read_region.address[i].name == SelectRegion.province)
                {
                    ShowCity(read_region.address[i]);
                    Debug.Log("选择的市属于省：" + read_region.address[i].name);
                }
            }
        }
    }

    /// <summary>
    /// 点击区
    /// </summary>
    public void OnClickArea()
    {
        if (SelectRegion.city == "")
        {
            //点击 市
            OnClickCity();
        }
        else
        {
            SelectRegion.type = RegionType.Area_Type;

            for (int i = 0; i < read_region.address.Count; i++)
            {
                if (read_region.address[i].name == SelectRegion.province)
                {
                    for (int j = 0; j < read_region.address[i].city.Count; j++)
                    {
                        if (read_region.address[i].city[j].name == SelectRegion.city)
                        {
                            ShowArea(read_region.address[i].city[j]);
                            Debug.Log("选择的区属于市：" + read_region.address[i].city[j].name);
                        }
                    }
                }
            }

        }
    }

    public void OnSelectedCity() 
    {
        this.OnCitySelectedEvent?.Invoke();
    }

    /// <summary>
    /// 点击确定
    /// </summary>
    private void OnClickOK()
    {
        gameObject.SetActive(false);
        isShow = false;
        CallEvent(true, new string[3] { SelectRegion.province, SelectRegion.city, SelectRegion.area });
    }
    /// <summary>
    /// 点击取消
    /// </summary>
    private void OnClickCancle()
    {
        gameObject.SetActive(false);
        isShow = false;
        CallEvent(false, null);
    }
    private void CallEvent(bool isOk, object data) {
        if (selectEvent!=null)
        {
            selectEvent(isOk,data);
            selectEvent = null;
        }
    }


    #endregion



    #region 判断开始显示的阶段：省 市 区

    /// <summary>
    /// 判断开始显示的阶段：省 市 区
    /// </summary>
    public void StartOpenState()
    {
        Debug.Log("判断开始显示的阶段！");
        SelectRegion.type = RegionType.Null;

        if (SelectRegion.province == "")
        {
            OnClickProvince();
        }
        else if (SelectRegion.city == "")
        {
            OnClickCity();
        }
        else if (SelectRegion.area == "")
        {
            OnClickArea();
        }
        else
        {

            string state = "";//状态
            for (int i = 0; i < read_region.address.Count; i++)
            {
                if (read_region.address[i].name == SelectRegion.province)
                {
                    Debug.Log("显示正确：省！===" + SelectRegion.province);
                    state = "省";

                    /*for (int j = 0; j < read_region.address[i].city.Count; j++)
                    {
                        if (read_region.address[i].city[j].name == SelectRegion.city)
                        {
                            Debug.Log("显示正确：市！==="+ SelectRegion.city);
                            state = "市";

                            for (int n = 0; n < read_region.address[i].city[j].area.Length; n++)
                            {
                                if (read_region.address[i].city[j].area[n] == SelectRegion.area)
                                {
                                    Debug.Log("显示正确：区！===" + SelectRegion.area);
                                    state = "区";
                                }
                            }
                        }
                    }*/
                }
            }

            if (state == "")
            {
                Btn_Province.gameObject.SetActive(true);
                Btn_City.gameObject.SetActive(false);
                Btn_Area.gameObject.SetActive(false);
                //点击省
                OnClickProvince();
            }
            if (state == "省")
            {
                Btn_Province.gameObject.SetActive(true);
                Btn_City.gameObject.SetActive(true);
                Btn_Area.gameObject.SetActive(false);
                //点击市
                OnClickCity();
            }
            if (state == "市" || state == "区")
            {
                Btn_Province.gameObject.SetActive(true);
                Btn_City.gameObject.SetActive(true);
                Btn_Area.gameObject.SetActive(true);
                //点击区
                OnClickArea();
            }

            //改变按钮颜色
            ChangeBtnColor(SelectRegion.type);
        }
    }

    #endregion


    #region 显示  省  市  县（区）

    /// <summary>
    /// 显示  省
    /// </summary>
    public void ShowProvince(Region region)
    {
        Debug.Log("显示：省！");
        //创建按钮 省
        CeratBtnArea(region.address.Count);

        for (int i = 0; i < region.address.Count; i++)
        {
            btn_list[i].GetComponent<Region_Item>().SetTextInfo(region.address[i].name);
        }
    }

    /// <summary>
    /// 显示  市
    /// </summary>
    public void ShowCity(Province province)
    {
        Debug.Log("显示：市！");
        //创建按钮 市
        CeratBtnArea(province.city.Count);

        for (int i = 0; i < province.city.Count; i++)
        {
            btn_list[i].GetComponent<Region_Item>().SetTextInfo(province.city[i].name);
        }
    }

    /// <summary>
    /// 显示  县（区）
    /// </summary>
    public void ShowArea(City city)
    {
        Debug.Log("显示： 县（区）！");
        //创建按钮 县（区）
        CeratBtnArea(city.area.Length);

        for (int i = 0; i < city.area.Length; i++)
        {
            btn_list[i].GetComponent<Region_Item>().SetTextInfo(city.area[i]);
        }
    }

    #endregion


    #region 改变按钮颜色

    /// <summary>
    /// 改变按钮颜色
    /// </summary>
    /// <param name="type"></param>
    public void ChangeBtnColor(RegionType type)
    {
        string btn_info = "";//按钮显示文本

        switch (type)
        {
            case RegionType.Province_Type:
                btn_info = SelectRegion.province;

                break;
            case RegionType.City_Type:
                btn_info = SelectRegion.city;

                break;
            case RegionType.Area_Type:
                btn_info = SelectRegion.area;

                break;
            default:
                break;
        }

        List<Transform> btn_list = new List<Transform>();
        foreach (Transform child in btn_parent.transform)
        {
            btn_list.Add(child);
        }

        if (btn_list.Count != 0)
        {
            for (int i = 0; i < btn_list.Count; i++)
            {
                //其他色
                btn_list[i].GetComponent<Image>().color = DefaultAlpha;
                btn_list[i].transform.Find("Text").GetComponent<TextMeshProUGUI>().color = DefaultTextColor;

                if (btn_list[i].GetComponent<Region_Item>().content_info == btn_info)
                {
                    //改变颜色 
                    btn_list[i].GetComponent<Image>().color = SelectedAlpha;
                    btn_list[i].transform.Find("Text").GetComponent<TextMeshProUGUI>().color = SelectedTextColor;
                }
            }
        }        
    }
    public Color DefaultTextColor;
    public Color SelectedTextColor;
    public Color DefaultAlpha = new Color(1, 1, 1, 0);
    public Color SelectedAlpha = new Color(1, 1, 1, 1);
    #endregion


}