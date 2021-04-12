Shader "zx/toon"
{

    Properties
    {
       _MainTex("Albedo (RGB)", 2D) = "white" {}

       //_ColorB("Tint BLACK", Color) = (0,0,0,1)
       //_CB_POS("BLACK pos", Range(0.0, 1.0)) = 0

       _ColorW("Tint", Color) = (1,1,1,1)
       //_CW_POS("WHITE pos", Range(0.0, 1.0)) = 1

          _BCC("BackColor", Color) = (0,0,0,0)
         // _BCL("Back light", Range(0,1)) = 0.0

          _LMUL("Self Illum", Range(0,1)) = 0.0
          

          _Glance("Glance", Range(0,1)) = 0.0

          //_Distortion2 ("_Distortion", Float) = 0
          //_Power2("_Power", Float) = 1
          //_Scale2("_Scale", Float) = 0.1
          
          [Toggle(USE_FRESNEL)] _UseFresnel("Use Fresnel", Float) = 0
          [Toggle(USE_PXR_LAMBERT)] _UsePxrLambert("Use PxrLambert", Float) = 1
          //[Toggle(USE_SSS)] _UseSSS("Use SSS", Float) = 0
          
    }

       

    SubShader
    {
   
      /*

         Константы UNITY
         
         https://docs.unity3d.com/2019.3/Documentation/Manual/SL-UnityShaderVariables.html

      */


        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
       // fullforwardshadows - для точек и спотов
       // addshadow - только для солнца
       // noambient
        #pragma surface surf XToon vertex:myvert addshadow noambient //nofog

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
      
         #pragma shader_feature USE_FRESNEL
          #pragma shader_feature USE_PXR_LAMBERT

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            float3 reflectDir;
        };
        
        void myvert(inout appdata_full v, out Input data)
        {
           UNITY_INITIALIZE_OUTPUT(Input, data);

           data.worldNormal = mul((float3x3)UNITY_MATRIX_M, v.normal);
           half3 wpos = mul(UNITY_MATRIX_M, float4(v.vertex.xyz, 1));
           data.viewDir = normalize(wpos - _WorldSpaceCameraPos);// mul((float3x3)UNITY_MATRIX_V, float3(0, 0, 1));
           data.reflectDir = reflect(data.viewDir, data.worldNormal);
        }


        float _Distortion2;
        float _Power2;
        float _Scale2;
        float _Glance;

        fixed4 _Color;

        half4 _ColorB;
        half4 _ColorW;
        half _CB_POS;
        half _CW_POS;
        half _LMUL;

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
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = (1 + _LMUL) * c.rgb;
            o.Alpha = dot(IN.viewDir, IN.worldNormal);

            half fresnel = 0;
#ifdef USE_FRESNEL            
            fresnel = 0.05 + 0.95 * pow(1 - o.Alpha, 4); // френель
#endif
            o.Emission = (0.5 + 0.5 * o.Albedo) * fresnel * _BCC;

            /*
            float3 val = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, IN.reflectDir);
            o.Emission += fresnel * val;
            */
        }


        //float3 reflectedDir =
         //reflect(input.viewDir, normalize(input.normalDir));

        half4 LightingXToon(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
        {

#define AMBIENT_LIGHT_POWER 0.5
//float AMBIENT_LIGHT_POWER = unity_AmbientSky;

           
           float3 L = lightDir;
           float3 V = viewDir;
           float3 N = s.Normal;
           
           /*
           float3 H = normalize(L + V);
           disney  pixar
           https://static1.squarespace.com/static/58586fa5ebbd1a60e7d76d3e/t/593a3afa46c3c4a376d779f6/1496988449807/s2012_pbs_disney_brdf_notes_v2.pdf
           float HnL = dot(H, L) * _Glance;
           float D90 = AMBIENT_LIGHT_POWER + 2 * HnL * HnL - 1;
           float NdotL = 1 + D90 * pow(1 - max(0,dot(N, L)) * atten, 4);
           NdotL *= 1 + D90 * pow(1 - dot(N, V), 4);
           */

#if USE_PXR_LAMBERT
           float D90 = AMBIENT_LIGHT_POWER - 1;
           float NdotL = 1 + D90 * pow((1 - max(0, dot(N, L)) * atten), 3);// 10 * _Glance * _Glance);
           
           NdotL *= 1 + D90 * pow(1 - s.Alpha, 5);// s.alpha = dot(N, V);
           NdotL *= NdotL;

           float4 c = 1;
           c.rgb = NdotL * _ColorW * s.Albedo * _LightColor0;
           
           return c;
#else
           float4 c = 1;
           c.rgb = max(0, dot(N, L)) * atten +  0.5 * AMBIENT_LIGHT_POWER;
           
           c.rgb *=  _ColorW * s.Albedo * _LightColor0;

           //c.rgb = unity_AmbientSky;
           return c;
#endif
           /*
           float4 c;
           
           float3 _LT = s.Albedo.r * (AMBIENT_LIGHT_POWER + NdotL);// *atten;
           _CB_POS *= _CB_POS;
           _CW_POS *= _CW_POS;
           float mag = abs(_CB_POS - _CW_POS);
           float _LP = _LT.r;
           _LP = max(_CB_POS, _LP);
           _LP = min(_CW_POS, _LP);

           
           float3 color = lerp(_ColorB, _ColorW, (_LP - _CB_POS) / mag);
           c.rgb = color * (1 + _LMUL * 2) * _LightColor0 * (1 - AMBIENT_LIGHT_POWER);
           c.a = 1;

#ifdef SKIP_USE_SSS
           float3 H = normalize(L + N * _Distortion2);
           float VnH = dot(V, -H);
           float I = pow(min(1, max(0, VnH)), _Power2) * _Scale2;
           c.rgb += I * color;
#endif
           return c;
           */
        }

        ENDCG
    }

    FallBack "Diffuse"
}

/*
float CharlieD(float roughness, float ndoth)
{
   //https://knarkowicz.wordpress.com/2018/01/04/cloth-shading/
   float invR = 1. / roughness;
   float cos2h = ndoth * ndoth;
   float sin2h = 1. - cos2h;
   return (2. + invR) * pow(sin2h, invR * .5) / (2. * 3.1415);
}

float AshikhminV(float ndotv, float ndotl)
{
   return 1. / (4. * (ndotl + ndotv - ndotl * ndotv));
}
*/