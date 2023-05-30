using UnityEngine;
using UnityEngine.UI;


public class GRender : MonoBehaviour
{
    [SerializeField]
    private Camera blackCam;
    [SerializeField]
    private Camera whiteCam;
    //////[SerializeField]
    //////private float m_OrthographicSize = 3.2f;
    [SerializeField]
    private Vector2 rtSize;
    [SerializeField]
    private RenderTexture blackRT;
    [SerializeField]
    private RenderTexture whiteRT;
    [SerializeField]
    private Material rawImgMat;
    [SerializeField]
    private RawImage rawImg;

    public Transform refCamTran;




    public void StartRender(Vector2 rtSize, RawImage rawImg, Transform refCamTran)
    {
        this.rtSize = rtSize;
        this.rawImg = rawImg;
        if (refCamTran.gameObject.activeInHierarchy)
        {
            refCamTran.gameObject.SetActive(false);
        }
        this.refCamTran = refCamTran;


        if (!blackCam)
            blackCam = CreateCamera("BlackCameraForRT", Color.black);
        if (!whiteCam)
            whiteCam = CreateCamera("WhiteCameraForRT", Color.white);
        if (!blackRT)
            blackRT = CreateTexture();
        if (!whiteRT)
            whiteRT = CreateTexture();

        blackCam.targetTexture = blackRT;
        whiteCam.targetTexture = whiteRT;



        AddImage(rawImg);
    }

    public RenderTexture StartRenderRT(Vector2 rtSize, Transform refCamTran)
    {
        this.rtSize = rtSize;
        if (refCamTran.gameObject.activeInHierarchy)
        {
            refCamTran.gameObject.SetActive(false);
        }
        this.refCamTran = refCamTran;

        if (!blackCam)
            blackCam = CreateCamera("BlackCameraForRT", Color.black);
        if (!whiteCam)
            whiteCam = CreateCamera("WhiteCameraForRT", Color.white);
        if (!blackRT)
            blackRT = CreateTexture();
        if (!whiteRT)
            whiteRT = CreateTexture();

        blackCam.targetTexture = blackRT;
        whiteCam.targetTexture = whiteRT;
        return whiteRT;
    }

    public void StartRenderRawImg(RawImage rawImg)
    {
        if (whiteRT != null)
        {
            this.rawImg = rawImg;
            this.AddImage(rawImg);
        }

    }


    protected void OnDestroy()
    {
        if (rawImgMat)
            Destroy(rawImgMat);
        if (blackRT)
        {
            Destroy(blackRT);
            blackRT = null;
        }
        if (whiteRT)
        {
            Destroy(whiteRT);
            whiteRT = null;
        }

        if (blackCam)
            Destroy(blackCam.gameObject);
        if (whiteCam)
            Destroy(whiteCam.gameObject);


    }


    ///// <summary>正交相机大小</summary>
    //public float orthographicSize
    //{
    //    get { return m_OrthographicSize; }
    //    set { ChangeCameraSize(value); }
    //}

    /// <summary>是否灰度化</summary>
    public bool grey
    {
        set
        {
            if (rawImgMat)
                rawImgMat.SetFloat("_Grey", value ? 1 : 0);
        }
    }

    /// <summary>当前位置</summary>
    public Vector3 Center
    {
        get { return transform.position; }
    }

    /// <summary>添加渲染目标</summary>
    public void AddImage(RawImage img)
    {
        if (!img)
            return;
        if (whiteRT == null)
        {
            Debug.LogError("whiteRT is null");
        }
        img.texture = whiteRT;

        if (rawImgMat != null)
        {
            GameObject.Destroy(rawImgMat);
        }

        rawImgMat = CreateMaterial();
        img.material = rawImgMat;

    }

    private Camera CreateCamera(string name, Color bgColor)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = refCamTran.localPosition;
        obj.transform.localScale = refCamTran.localScale;
        obj.transform.rotation = refCamTran.localRotation;


        //相机的参数调整成和refCam一致
        var refCam = refCamTran.GetComponent<Camera>();
        var camera = obj.AddComponent<Camera>();
        camera.backgroundColor = bgColor;

        camera.allowMSAA = false;
        camera.allowHDR = false;
        camera.fieldOfView = refCam.fieldOfView;
        camera.clearFlags = CameraClearFlags.Color;
        camera.farClipPlane = refCam.farClipPlane;
        camera.nearClipPlane = refCam.nearClipPlane;
        camera.depth = refCam.depth;
        camera.cullingMask = refCam.cullingMask;


        return camera;
    }

    private RenderTexture CreateTexture()
    {
        //24bit with stencil
        //otherwise these may be some render order error  with model
        var texture = new RenderTexture((int)rtSize.x, (int)rtSize.y, 24, RenderTextureFormat.ARGB32)
        {
            antiAliasing = 1,
            filterMode = FilterMode.Bilinear,
            anisoLevel = 0,
            useMipMap = false,
            autoGenerateMips = false,
        };

        texture.name = transform.name + "_RT";
        return texture;
    }

    private Material CreateMaterial()
    {
        Material mat = new Material(Shader.Find("UI/GRenderShader"));
        mat.SetTexture("_MainTex", whiteRT);
        mat.SetTexture("_BlackTex", blackRT);
        return mat;
    }


}
