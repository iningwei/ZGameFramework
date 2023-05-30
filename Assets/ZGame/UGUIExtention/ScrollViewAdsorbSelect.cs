using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZGame.UGUIExtention
{ 
    /// <summary>
    /// Îü¸½Ñ¡ÖÐ
    /// </summary>
    public class ScrollViewAdsorbSelect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public class ScrollViewAdsorbSelectEvent : UnityEvent<int, bool, Transform> { }


        public Color SelectedColor;
        public Color DefaultColor = Color.white;

        ZGame.UGUIExtention.ScrollView scrollView;

        float checkHight = 0;

        public ScrollViewAdsorbSelectEvent onCenterItemRender = new ScrollViewAdsorbSelectEvent();
        public ScrollView.ScrollRenderEvent onSelectEnd = new ScrollView.ScrollRenderEvent();

        private void Awake()
        {
            scrollView = GetComponent<ZGame.UGUIExtention.ScrollView>();
            scrollView.onItemRender.AddListener(onItemRender);
            checkHight = scrollView.prefab.rect.height + scrollView.space.y;
            //prefabSizeRatio = (scrollView.prefab.rect.height + scrollView.space.y) / scrollView.viewport.rect.height;
            onCenterItemRender.AddListener(onCenterItemRenderCallBack);
            onSelectEnd.AddListener(onSelectEndCallBack);
        }
        // Start is called before the first frame update
        void Start()
        {
            //testBtn.onClick.AddListener(() => { scrollToIndex(18); });
            Invoke("delayTriggerAdsorb", 0.2f);
        }

        public Button testBtn;

        void onSelectEndCallBack(int index, Transform tran) 
        {
            //Debug.Log("selected:" + index + "   name:" + tran.gameObject.name);
        }
        void onCenterItemRenderCallBack(int index, bool show, Transform tran)
        {
            //Debug.Log(index + "   " + show);
            if (show)
            {
                tran.Find("Text").GetComponent<TMPro.TMP_Text>().color = SelectedColor;
            }
            else
            {
                tran.Find("Text").GetComponent<TMPro.TMP_Text>().color = DefaultColor;
            }
        
        }

        float y = 0;
        public float Duration = 0.5f;
        float lerpTime = 0;
        //float prefabSizeRatio = 0;

        float[] yArray;

        float checkTime = 0;
        Transform curSelectedTran;
        Transform lastSelectedTran;
        // Update is called once per frame
        void Update()
        {
            checkTime -= Time.deltaTime;
            if (checkTime < 0)
            {
                checkTime = 0.1f;
                for (int i = 0; i < scrollView.content.childCount; i++)
                {
                    var center_y = scrollView.viewport.InverseTransformPoint(scrollView.content.GetChild(i).position).y + scrollView.viewport.rect.height / 2;

                    if (Mathf.Abs(center_y) < scrollView.prefab.rect.height / 2)
                    {
                        if (curSelectedTran == null)
                        {
                            onSelectEnd.Invoke(int.Parse(scrollView.content.GetChild(i).name), scrollView.content.GetChild(i));
                        }
                        curSelectedTran = scrollView.content.GetChild(i);
                        if (lastSelectedTran != null)
                        {
                            if (curSelectedTran.gameObject.name != lastSelectedTran.gameObject.name)
                            {
                                onCenterItemRender.Invoke(int.Parse(curSelectedTran.gameObject.name), true, curSelectedTran);
                                onCenterItemRender.Invoke(int.Parse(lastSelectedTran.gameObject.name), false, lastSelectedTran);
                            }
                        }
                        else
                        {
                            onCenterItemRender.Invoke(int.Parse(curSelectedTran.gameObject.name), true, curSelectedTran);
                        }
                        lastSelectedTran = curSelectedTran;
                    }
                    else
                    {
                        if (lastSelectedTran != null && lastSelectedTran.name == scrollView.content.GetChild(i).gameObject.name)
                        {
                            onCenterItemRender.Invoke(int.Parse(lastSelectedTran.gameObject.name), false, lastSelectedTran);
                            lastSelectedTran = null;
                        }
                    
                    }
                }
            
            }
            if (m_Dragging == false && m_startUpdate == true) 
            {
                float dist = Mathf.Abs(scrollView.content.localPosition.y - y);
                //Debug.Log(dist / Time.deltaTime / duration + "  " + checkHight / duration);
                if (dist / Time.deltaTime < checkHight)
                {
                    if (yArray == null || yArray.Length != scrollView.content.childCount)
                    {
                        yArray = new float[scrollView.content.childCount];
                    }

                    for (int i = 0; i < scrollView.content.childCount; i++)
                    {
                        var local_y = scrollView.viewport.InverseTransformPoint(scrollView.content.GetChild(i).position).y + scrollView.viewport.rect.height / 2;
                        yArray[i] = local_y;
                    }
                    for (int i = 0; i < yArray.Length; i++)
                    {
                        for (int j = i; j < yArray.Length; j++)
                        {
                            if (Mathf.Abs(yArray[j]) < Mathf.Abs(yArray[i]))
                            {
                                var t = yArray[i];
                                yArray[i] = yArray[j];
                                yArray[j] = t;
                            }
                        }
                    }
                    if(yArray.Length == 0)
                    {
                        return;
                    }
                    lerpTime -= Time.deltaTime;
                    var y = scrollView.content.localPosition.y - Mathf.Lerp(0, yArray[0], Duration - lerpTime);
                    scrollView.content.localPosition = new Vector3(scrollView.content.localPosition.x, y, scrollView.content.localPosition.z);
                    if (lerpTime <= 0 )
                    {
                    
                        for (int i = 0; i < scrollView.content.childCount; i++)
                        {
                            /*Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(scrollView.viewport, scrollView.content.GetChild(i));
                            if (Mathf.Abs(Mathf.Abs(bounds.center.y / scrollView.viewport.rect.height) - 0.5f) < prefabSizeRatio / 2)
                            {
                                m_startUpdate = false;
                                Debug.Log("selected:" + scrollView.content.GetChild(i).gameObject.name);
                            }*/
                            var center_y = scrollView.viewport.InverseTransformPoint(scrollView.content.GetChild(i).position).y + scrollView.viewport.rect.height / 2;
                            if (Mathf.Abs(center_y) < 10)
                            {
                                onSelectEnd.Invoke(int.Parse(scrollView.content.GetChild(i).name), scrollView.content.GetChild(i));
                                m_startUpdate = false;
                            
                            }
                        }
                    }
                }
                else
                {
                    lerpTime = Duration;
                }
            }
            else
            {
                lerpTime = Duration;
            }
            y = scrollView.content.localPosition.y;
        }


        bool m_Dragging = false;
        bool m_startUpdate = false;
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            m_Dragging = true;
            m_startUpdate = true;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            m_Dragging = false;
        }

        void onItemRender(int index, Transform tran)
        {
            tran.GetComponentInChildren<TMPro.TMP_Text>().text = index.ToString();
        }

        void scrollToIndex(int index)
        {
            scrollView.ScrollTo(index - 1, Duration);
            Invoke("delayTriggerAdsorb", Duration);
        }

        void delayTriggerAdsorb()
        {
            if (gameObject.IsDestroyed())
                return;
            m_startUpdate = true;
        }    
    }
}