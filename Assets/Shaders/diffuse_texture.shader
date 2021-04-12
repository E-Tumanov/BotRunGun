Shader "zx/diffuse_tex_only"
{
	Properties
	{

		_Color("Main Color", Color) = (1,1,1,1)
		_MulA("Alpha MUL", float) = 1
		_MainTex("Base (RGB) RefStrength (A)", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		//Cull Off
		CGPROGRAM

		#pragma surface surf Lambert noforwardadd noambient

		struct Input
		{
			  float4 color : COLOR;
			  float2 uv_MainTex;
		};

		  sampler2D _MainTex;
		  float4 _Color;
		  float _MulA;

		  void surf(Input IN, inout SurfaceOutput o) {
			  //o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			  float4 _c = tex2D(_MainTex, IN.uv_MainTex);
			  o.Emission = _Color * (_c.a * _MulA + _c.rgb);
		  }
		  ENDCG
	}


	//Fallback "Diffuse"
}