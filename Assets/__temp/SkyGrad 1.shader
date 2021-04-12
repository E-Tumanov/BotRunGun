Shader "Custom/SkyGrad"
{
   Properties
   {
       _Color("Color", Color) = (1,1,1,1)
       _Color1("Color", Color) = (1,1,1,1)
      _LMUL("light MUL", Range(0,5)) = 1.0
       _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Offset("Offset", float) = 1

   }
      SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200
           Cull Front

            CGPROGRAM
           // Physically based Standard lighting model, and enable shadows on all light types
           #pragma surface surf Ramp nofog

           // Use shader model 3.0 target, to get nicer looking lighting
           #pragma target 3.0

           sampler2D _MainTex;

           struct Input
           {
               float2 uv_MainTex;
               float4 screenPos;
               float3 worldNormal;
               float3 worldPos;
           };


           fixed4 _Color;
           fixed4 _Color1;
           float _Offset;
           half _LMUL;

           half4 LightingRamp(SurfaceOutput s, half3 lightDir, half atten) {
              /*
              half NdotL = dot(s.Normal, lightDir);
              half diff = NdotL * 0.5 + 0.5;
              half3 ramp = tex2D(_Ramp, float2(diff)).rgb;
              half4 c;
              */
              half4 c;
              c.rgb = 0;// s.Albedo* _LightColor0.rgb* ramp* atten;
              c.a = s.Alpha;
              return c;
           }

           void surf(Input IN, inout SurfaceOutput  o)
           {
              //float2 scrUV = IN.screenPos.xy / IN.screenPos.w;
              //fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * lerp(_Color1, _Color, 0.5+0.5*IN.worldNormal.y);
              
              //float v = 0.5 + 0.5 * normalize(IN.worldPos).y;
              
               //fixed4 c = lerp(_Color1, _Color, max(0,min(1, _Offset + (1 - _Offset) * v)));
               o.Emission = _Color * _LMUL;
               o.Alpha = 1;
               o.Albedo = 0;
          }
          ENDCG
        }
  // FallBack "Diffuse"
}
