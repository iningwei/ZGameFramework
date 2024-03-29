Shader "My/PostProcess/Gray"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_SceneTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
         Tags { "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent" }
         
		 ZTest Always
		 Cull Off
		 ZWrite Off
		 Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
			
			
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			
			sampler2D _SceneTex;
            float4 _SceneTex_ST;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			
			fixed4 sceneCol=tex2D(_SceneTex,i.uv);
			
                
                fixed4 col = tex2D(_MainTex, i.uv);
				
				if(col.a==0){
					return sceneCol;
				}
				
                col.rgb=dot(col.rgb,fixed3(0.299, 0.587, 0.114));
				 
                return col;
            }
            ENDCG
        }
    }
}
