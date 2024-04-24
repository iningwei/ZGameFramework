using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.TimerTween;

public class DragAutoRotateLogic : MonoBehaviour
{
    AutoRotate autoRotate;
    DragListerner dragListerner;

    bool flag = false;
    public void Init(AutoRotate autoRotate, DragListerner dragListerner)
    {
        this.autoRotate = autoRotate;
        this.dragListerner = dragListerner;

        this.dragListerner.onDragBegin += this.onDragBegin;
        this.dragListerner.onDrag += this.onDrag;
        this.dragListerner.onDragEnd += this.onDragEnd;
    }

    private void onDragEnd()
    {
        this.triggerRotate();
        flag = false;
    }

    Timer triggerRotateTimer;
    long triggerRotateId;
    private void triggerRotate()
    {
        TimerTween.Cancel(this.triggerRotateTimer, triggerRotateId);

        this.triggerRotateTimer = TimerTween.Delay(3, () =>
        {
            autoRotate.StartRotate();
        });
        this.triggerRotateTimer.Start(out triggerRotateId);
    }

    private void onDrag(Vector2 delta)
    {
        if (flag)
        {
            this.autoRotate.transform.RotateAround(this.autoRotate.transform.position, Vector3.up, -delta.x * Time.deltaTime * 20);
        }
    }

    private void onDragBegin()
    {
        this.autoRotate.StopRotate();
        TimerTween.Cancel(this.triggerRotateTimer, triggerRotateId);
        flag = true;
    }

    private void OnDestroy()
    {
        TimerTween.Cancel(this.triggerRotateTimer, triggerRotateId);
        this.dragListerner.onDragBegin = null;
        this.dragListerner.onDrag = null;
        this.dragListerner.onDragEnd = null;
    }
}
