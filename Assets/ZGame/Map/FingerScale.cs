using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FingerScale : MonoBehaviour,IDragHandler
{
    public RectTransform root;
    RectTransform rect_transform;
    Vector2 center = Vector2.zero;
    Vector2 screen_size = Vector2.zero;
    Vector2 drag_pos = Vector2.zero;
    Vector2 drag_offset = Vector2.zero;
    bool init_scale = false;
    bool init_drag = false;
    float init_distance = 0;
    float speed = 1;
    float scale = 1;
    float last_scale = 1;

    Action scaleMinCallBack;




    // Start is called before the first frame update
    void Start()
    {
        rect_transform = this.GetComponent<RectTransform>();
        Init();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        float offset = Input.GetAxis("Mouse ScrollWheel");
        scale -= offset * speed;
        scale = Mathf.Clamp(scale,0.8f,1.5f);
        if (last_scale != scale)
        {
            last_scale = scale;
            rect_transform.localScale = Vector3.one * scale;
            drag_offset = (screen_size - rect_transform.sizeDelta * rect_transform.localScale) * 0.5f;
            drag_pos = rect_transform.anchoredPosition;
            drag_pos.x = Mathf.Clamp(drag_pos.x,-Mathf.Abs(drag_offset.x),Mathf.Abs(drag_offset.x));
            drag_pos.y = Mathf.Clamp(drag_pos.y,-Mathf.Abs(drag_offset.y),Mathf.Abs(drag_offset.y));
            rect_transform.anchoredPosition = drag_pos;
            
            if (scale < 1)
            {
                if (null != scaleMinCallBack)
                {
                    scaleMinCallBack();
                }
            }
        }
#else
        if (Input.touchCount == 2)
        {
            if (init_scale)
            {
                var scale = Vector2.Distance(Input.GetTouch(0).position , Input.GetTouch(1).position)/init_distance;
                rect_transform.localScale = Vector3.one * scale;
                drag_offset = (screen_size - rect_transform.sizeDelta * rect_transform.localScale) * 0.5f;
            }
            else
            {
                init_distance = Vector2.Distance(Input.GetTouch(0).position , Input.GetTouch(1).position);
                init_scale = true;
            }
        }
        else
        {
            init_scale = false;
        }
#endif
    
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.dragging)
        {
            if (!init_drag)
            {
                init_drag = true;
                drag_pos = rect_transform.anchoredPosition;
            }
            else
            {
                drag_pos += eventData.delta;
                drag_pos.x = Mathf.Clamp(drag_pos.x,-Mathf.Abs(drag_offset.x),Mathf.Abs(drag_offset.x));
                drag_pos.y = Mathf.Clamp(drag_pos.y,-Mathf.Abs(drag_offset.y),Mathf.Abs(drag_offset.y));
                rect_transform.anchoredPosition = drag_pos;
            }
        }   
        else
        {
            init_drag = false;
        }
    }

    public void Init(Action callback=null)
    {
        rect_transform.localScale = Vector3.one;
        rect_transform.anchoredPosition = Vector3.zero;

        if (root)
        {
            screen_size.x = root.rect.width;
            screen_size.y = root.rect.height;
        }
        else
        {
            screen_size.x = Screen.width;
            screen_size.y = Screen.height;
        }
        rect_transform.sizeDelta = Vector2.one * screen_size.y;
        drag_offset = (screen_size - rect_transform.sizeDelta * rect_transform.localScale) * 0.5f;
        scaleMinCallBack = callback;
    }
}
