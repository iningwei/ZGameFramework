using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public static class UGUIRenderModel
{
    public static void RenderToUGUIRawImage(this GameObject targetModelObj, Vector2 rtSize, RawImage visualRawImg, Transform refCamTran, bool updateRef = false)
    {
        var render = targetModelObj.GetOrAddComponent<GRender>();

        render.StartRender(rtSize, visualRawImg, refCamTran);
        render.SetUpdatePR2RefCam(updateRef);
    }

    public static void RenderToTexture(this GameObject targetModelObj, Vector2 rtSize, Transform refCamTran, out RenderTexture whiteRTexture)
    {
        var render = targetModelObj.GetOrAddComponent<GRender>();
        whiteRTexture = render.StartRenderRT(rtSize, refCamTran);
    }

    public static void RenderToRawImg(this GameObject targetModelObj, RawImage rawImg)
    {
        var render = targetModelObj.GetOrAddComponent<GRender>();
        render.StartRenderRawImg(rawImg);
    }
}
