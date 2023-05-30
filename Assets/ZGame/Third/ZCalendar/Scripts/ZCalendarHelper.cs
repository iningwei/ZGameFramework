using System;
using Unity.VisualScripting;
using UnityEngine;
using ZGame;
using ZTools;
/// <summary>
/// ������չ
/// </summary>
public class ZCalendarHelper : MonoBehaviour
{
    [Header("Debugģʽ���Զ���ʼ��,���༭������Ч��")]
    public bool DebugMode = false;
    public ZCalendar zCalendar;
    private string selectedTimeStr;
    void Start()
    {
#if UNITY_EDITOR
        if (DebugMode)
        {
            zCalendar.Init();
        }
#endif
        zCalendar.UpdateDateEvent += ZCalendar_UpdateDateEvent;
        zCalendar.ChoiceDayEvent += ZCalendar_ChoiceDayEvent;
        zCalendar.RangeTimeEvent += ZCalendar_RangeTimeEvent;
        zCalendar.CompleteEvent += ZCalendar_CompleteEvent;
        //zCalendar.Init();
        //zCalendar.Init(System.DateTime.Now);
        //zCalendar.Init("2022-02-02");
        //zCalendar.Show();
        //zCalendar.Hide();
    }
    /// <summary>
    /// ���ؽ���
    /// </summary>
    private void ZCalendar_CompleteEvent()
    {
        Debug.Log("ZCalendar���ؽ���");
        if (null != zCalendar.CrtTime)
        {
            Debug.Log($"��ǰʱ��{zCalendar.CrtTime.Day}");
        }
    }

    /// <summary>
    /// ����ʱ��
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void ZCalendar_RangeTimeEvent(ZCalendarDayItem arg1, ZCalendarDayItem arg2)
    {
        Debug.Log($"ѡ���ʱ�����䣺{arg1.Day}��{arg2.Day}");
    }

    /// <summary>
    /// ��ȡѡ�������
    /// </summary>
    /// <param name="obj"></param>
    private void ZCalendar_ChoiceDayEvent(ZCalendarDayItem obj)
    {
        Debug.Log($"ѡ������ڣ�{obj.Year},{obj.Month},{obj.Day}");
        selectedTimeStr = $"{obj.Year}-{obj.Month}-{obj.Day}";
        _onSelected?.Invoke(selectedTimeStr);
    }

    /// <summary>
    /// �л��·�ʱ�����õ�ÿһ���item����
    /// </summary>
    /// <param name="obj"></param>
    private void ZCalendar_UpdateDateEvent(ZCalendarDayItem obj)
    {
        //Debug.Log($"�������ڣ�{obj.Day}");
    }
    private void OnDestroy()
    {
        zCalendar.UpdateDateEvent -= ZCalendar_UpdateDateEvent;
        zCalendar.ChoiceDayEvent -= ZCalendar_ChoiceDayEvent;
        zCalendar.RangeTimeEvent -= ZCalendar_RangeTimeEvent;
        zCalendar.CompleteEvent -= ZCalendar_CompleteEvent;
    }

    /// <summary>
    /// ��ȡѡ�����ڵ�ʱ���
    /// </summary>
    /// <returns></returns>
    public string GetSelectedTimeStr()
    {
        return selectedTimeStr;
    }
    private event Action<string> _onSelected;
    /// <summary>
    /// ����ָ��ʱ���ʼ��
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="day"></param>
    public void InitDateByTime(string timeStr, Action<string> onSelected)
    {
        _onSelected += onSelected;
        selectedTimeStr = timeStr;
        zCalendar.Init(timeStr);
        //����༭��ģʽ�����debug����ɶ��γ�ʼ��������
        DebugMode = false;
    }
}
