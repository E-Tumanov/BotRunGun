Shader "UI/zx_bw_gradient_tint"
{


   Properties
   {
       [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
       _ColorB("Tint BLACK", Color) = (0,0,0,1)
       _CB_POS("BLACK pos", Range(0.0, 1.0)) = 0
       _ColorW("Tint WHITE", Color) = (1,1,1,1)
       _CW_POS("WHITE pos", Range(0.0, 1.0)) = 1

       _StencilComp("Stencil Comparison", Float) = 8
       _Stencil("Stencil ID", Float) = 0
       _StencilOp("Stencil Operation", Float) = 0
       _StencilWriteMask("Stencil Write Mask", Float) = 255
       _StencilReadMask("Stencil Read Mask", Float) = 255

       _ColorMask("Color Mask", Float) = 15

       [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
   }

      SubShader
       {
           Tags
           {
               "Queue" = "Transparent"
               "IgnoreProjector" = "True"
               "RenderType" = "Transparent"
               "PreviewType" = "Plane"
               "CanUseSpriteAtlas" = "True"
           }

           Stencil
           {
               Ref[_Stencil]
               Comp[_StencilComp]
               Pass[_StencilOp]
               ReadMask[_StencilReadMask]
               WriteMask[_StencilWriteMask]
           }

           Cull Off
           Lighting Off
           ZWrite Off
           ZTest[unity_GUIZTestMode]
           Blend One OneMinusSrcAlpha
           ColorMask[_ColorMask]

           Pass
           {
               Name "Default"
           CGPROGRAM
               #pragma vertex vert
               #pragma fragment frag
               #pragma target 2.0

               #include "UnityCG.cginc"
               #include "UnityUI.cginc"
          
               #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
               #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
          
               struct appdata_t
               {
                   float4 vertex   : POSITION;
                   float4 color    : COLOR;
                   float2 texcoord : TEXCOORD0;
                   UNITY_VERTEX_INPUT_INSTANCE_ID
               };

               struct v2f
               {
                   float4 vertex   : SV_POSITION;
                   fixed4 color : COLOR;
                   float2 texcoord  : TEXCOORD0;
                   float4 worldPosition : TEXCOORD1;
                   half4  mask : TEXCOORD2;
                   UNITY_VERTEX_OUTPUT_STEREO
               };

               sampler2D _MainTex;
               fixed4 _ColorB;
               fixed4 _ColorW;
               fixed4 _TextureSampleAdd;
               float4 _ClipRect;
               float4 _MainTex_ST;
               float _MaskSoftnessX;
               float _MaskSoftnessY;
               float _CB_POS;
               float _CW_POS;

               v2f vert(appdata_t v)
               {
                   v2f OUT;
                   UNITY_SETUP_INSTANCE_ID(v);
                   UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                   float4 vPosition = UnityObjectToClipPos(v.vertex);
                   OUT.worldPosition = v.vertex;
                   OUT.vertex = vPosition;

                   float2 pixelSize = vPosition.w;
                   pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                   float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                   float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                   OUT.texcoord = float4(v.texcoord.x, v.texcoord.y, maskUV.x, maskUV.y);
                   OUT.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + abs(pixelSize.xy)));

                   OUT.color = v.color;// *_ColorA;
                   return OUT;
               }

               fixed4 frag(v2f IN) : SV_Target
               {
                  //half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                  half4 _LT = tex2D(_MainTex, IN.texcoord);

                  float mag = abs(_CB_POS - _CW_POS);
                  //float _LP = LinearRgbToLuminance(_LT.rgb) * IN.color.r;
                  float _LP = dot(_LT.rgb, 0.5) * IN.color.r;
                  _LP = max(_CB_POS, _LP);
                  _LP = min(_CW_POS, _LP);
                  

                  half4 color = lerp(_ColorB, _ColorW, (_LP - _CB_POS) / mag);// _LT.r* IN.color.r);
                  color.a = color.a * _LT.a * IN.color.a;
                  //half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                  
                  #ifdef UNITY_UI_CLIP_RECT
                   half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(IN.mask.xy)) * IN.mask.zw);
                   color.a *= m.x * m.y;
                  #endif
                   
                  #ifdef UNITY_UI_ALPHACLIP
                   clip(color.a - 0.001);
                  #endif
                   
                   color.rgb *= color.a;
                  
                   return color;
               }
           ENDCG
           }
       }
}
