using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragListerner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{
    public Action onDragBegin;
    public Action<Vector2> onDrag;
    public Action onDragEnd;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onDragBegin?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDragEnd?.Invoke();
    }

 
}
