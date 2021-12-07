using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���°汾��Unity���Ѿ���ʾSystemInfo.supportsImageEffects��SystemInfo.supportsRenderTextures ��always return true��
//��˲���Ҫ���ж�ƽ̨֧�����

namespace ZGame.PostProcess.CameraPostProcess
{

    [RequireComponent(typeof(Camera))]
    public class PostEffectBase : MonoBehaviour
    {
        public Camera attachedCam;
        public string shaderName;

        private Shader _shader;
        private Material _mat;

        public Shader shader
        {
            get
            {
                if (_shader == null)
                {
                    _shader = Shader.Find(shaderName);
                }
                return _shader;
            }
        }
        public Material mat
        {
            get
            {
                _mat = CheckShaderAndCreateMaterial(shader, _mat);
                return _mat;
            }
        }

        protected Material CheckShaderAndCreateMaterial(Shader s, Material m)
        {
            if (s == null || !s.isSupported)
            {
                return null;
            }

            if (s.isSupported && m && m.shader == s)
            {
                return m;
            }
            m = new Material(s);
            m.hideFlags = HideFlags.DontSave;
            return m ? m : null;
        }


        public virtual void Start()
        {
            attachedCam = this.GetComponent<Camera>();
            if (attachedCam == null)
            {
                Debug.LogError("error,no camera attached to:" + this.gameObject.GetHierarchy());
            }
        }


        public virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
        {

        }

        public virtual void OnDestroy()
        {
            Debug.Log("PostEffectBase OnDestroy");
            if (_mat != null)
            {
                GameObject.DestroyImmediate(_mat);
            }
        }
    }
}