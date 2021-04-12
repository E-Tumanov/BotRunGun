Shader "Custom/WallSection"
{
   Properties
   {
       _Color("Color", Color) = (1,1,1,1)
       _MainTex("Albedo (RGB)", 2D) = "white" {}
       //_Glossiness("Smoothness", Range(0,1)) = 0.5
       //_Metallic("Metallic", Range(0,1)) = 0.0

       _AO_Detail("AO(mul)", 2D) = "white" {}
       //_LengthScale("_LengthScale", float) = 1
       //_HeightAmp("_HeightAmp", float) = 0
   }
      SubShader
   {
       Tags { "RenderType" = "Opaque" }
       LOD 200

       CGPROGRAM
      // Physically based Standard lighting model, and enable shadows on all light types
      #pragma surface surf Lambert //fullforwardshadows 
      //vertex:vert


   // Use shader model 3.0 target, to get nicer looking lighting
   #pragma target 3.0

   sampler2D _MainTex;
   sampler2D _AO_Detail;
   struct Input
   {
       float2 uv_MainTex;
       float2 uv_AO_Detail;
   };
   
   //half _LengthScale;
   //half _HeightAmp;

   //half _Glossiness;
   //half _Metallic;

   fixed4 _Color;

   void vert(inout appdata_full v, out Input o)
   {
      UNITY_INITIALIZE_OUTPUT(Input, o);
      /*
      float4 pos = mul(unity_WorldToObject, float4(0,0,0,1));
      float4 tex = tex2Dlod(_MainTex, float4(_LengthScale * (pos.z + v.vertex.z)/512.0, 0, 0, 0));
      */
      /*
      float4 pos = mul(unity_WorldToObject, v.vertex);
      float4 tex = tex2Dlod(_RoadTex, 1/_LengthScale * float4( (pos.z) / 512.0, 0, 0, 0));
      */
      //v.vertex.y += _HeightAmp * (1 - 2 * tex.r);
   }



   // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
   // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
   // #pragma instancing_options assumeuniformscaling
   UNITY_INSTANCING_BUFFER_START(Props)
      // put more per-instance properties here
  UNITY_INSTANCING_BUFFER_END(Props)

  //void surf(Input IN, inout SurfaceOutputStandard o)
    void surf (Input IN, inout SurfaceOutput o)
  {
      // Albedo comes from a texture tinted by color
      fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * tex2D(_AO_Detail, IN.uv_AO_Detail);
      o.Albedo = c.rgb;
      // Metallic and smoothness come from slider variables
      //o.Metallic = _Metallic;
      //o.Smoothness = _Glossiness;
      o.Alpha = 1;// c.a;
  }
  ENDCG
   }
      FallBack "Diffuse"
}
