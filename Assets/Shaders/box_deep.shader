Shader "zx/box_deep"
{

    Properties
    {
       _Color("Color", Color) = (0,0,0,1)
      _MainTex("Albedo (RGB)", 2D) = "white" {}
      _Cloud("Cloud (RGB)", 2D) = "white" {}
      
      
      _BCL("Back light", Range(0,1)) = 0.0
      _LMUL("light MUL", Range(0,1)) = 0.0

      _FogColor("FogColor", Color) = (1,1,1,1)
      _FogDeep("FogDeep", float) = 50
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

       // fullforwardshadows - для точек и спотов
       // addshadow - только для солнца
       // noambient

        #pragma surface surf XFlat vertex:myvert noambient nofog

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
         sampler2D _Cloud;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            float3 worldPos;
            float LT;
            float __fog;
        };

        fixed4 _Color;

     
        half _LMUL;
        
        half4 _FogColor;
        half _FogDeep;

        half _BCL;
        half4 _BCC;
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        //#pragma instancing_options assumeuniformscaling
        //UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        //UNITY_INSTANCING_BUFFER_END(Props)


        void myvert(inout appdata_full v, out Input data)
        {
           UNITY_INITIALIZE_OUTPUT(Input, data);

           float4 wpos = mul(unity_ObjectToWorld, float4(v.vertex.xyz,1));
           //data.__fog = 0;// 1 - max(0, min(1, wpos.y / _FogDeep - 1));
           //data.__fog = 1 - wpos.y / _FogDeep;
           data.LT = max(0, dot(normalize(v.normal), _WorldSpaceLightPos0.xyz));
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
           //fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
           //fixed cl = 0.5 + tex2D(_Cloud, IN.worldPos.xz / 256).r;
           //o.Albedo = IN.LT * _Color;// +_Color;// c.rgb;// *cl.r;
           o.Alpha =  max(0, min(1, IN.worldPos.y / _FogDeep - 1));
           o.Albedo = lerp((_BCL + IN.LT) * _Color, _FogColor.rgb, o.Alpha) ;// ;          
        }


        half4 LightingXFlat(SurfaceOutput s, half3 lightDir, half atten)
        {
           half4 c;
           c.rgb = s.Albedo;
           // s.Albedo;// (_BCL + s.Albedo);// *_LightColor0;
           //c.rgb = lerp(_FogColor.rgb, c.rgb, s.Alpha);
           //c.rgb = lerp(c.rgb, 1, s.Alpha);
           // c.rgb = s.Albedo;
           c.a = 1;
           return c;
        }

        ENDCG
    }
    //FallBack "Diffuse"
}
