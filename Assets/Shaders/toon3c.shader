Shader "zx/toon3C"
{
    Properties
    {
       /*
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        */
       _MainTex("Albedo (RGB)", 2D) = "white" {}

       _ColorB("Tint BLACK", Color) = (0,0,0,1)
       _ColorM("Tint MIDTONE", Color) = (0.5,0.5,0.5,1)
       _ColorW("Tint WHITE", Color) = (1,1,1,1)

          _BCC("BackColor", Color) = (0,0,0,0)
          _BCL("Back light", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
       // fullforwardshadows - для точек и спотов
       // addshadow - только для солнца
       // noambient
        #pragma surface surf XToon addshadow 
          //noambient

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
        };

        fixed4 _Color;

        half4 _ColorB;
        half4 _ColorM;
        half4 _ColorW;
        
        half _BCL;
        half4 _BCC;
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        void surf (Input IN, inout SurfaceOutput o)
        {
           fixed4 c = tex2D(_MainTex, IN.uv_MainTex);// *_Color;
            o.Albedo = c.rgb;
            o.Alpha = pow(1 - dot(IN.viewDir, IN.worldNormal), 4);
            
            float grnd = max(0, -0.5 * IN.worldNormal.y - 0.1);
            o.Emission = _BCC * grnd * grnd;
        }


        half4 LightingXToon(SurfaceOutput s, half3 lightDir, half atten)
        {
           half4 c;
           /*
           half LDOT = dot(s.Normal, lightDir);
           half NdotL = max(0, LDOT);
           half4 c;

           float bcl = 0 * max(0, -LDOT);
           //bcl *= bcl;
           float grnd = max(0, -0.5 * s.Normal.y - 0.1);
           bcl = s.Alpha * _BCL + grnd * grnd;

           half4 _LT = s.Albedo.r * NdotL * atten * 0.3 * _LightColor0.r;//step(0, NdotL)
           _CB_POS *= _CB_POS;
           _CW_POS *= _CW_POS;
           float mag = abs(_CB_POS - _CW_POS);
           float _LP = _LT.r;
           _LP = max(_CB_POS, _LP);
           _LP = min(_CW_POS, _LP);


           half4 color = lerp(_ColorB, _ColorW, (_LP - _CB_POS) / mag);
           */
           c.rgb = 0;// bcl* _BCC + color.rgb;
           c.a = 1;
           return c;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
