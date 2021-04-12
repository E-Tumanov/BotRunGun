Shader "zx/diffuse_vert_color_tex" {
   Properties{
      _Color("Light color", Color) = (1,1,1,1)
      _MulLight("Light power", float) = 2
		_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	}

    SubShader {
      Tags { "RenderType" = "Opaque" }
	  //Cull Off
      CGPROGRAM
      
      #pragma surface surf Lambert noforwardadd noambient
      struct Input {
          float4 color : COLOR;
          float2 uv_MainTex;
      };
      
      sampler2D _MainTex;
      float4    _Color;
      float     _MulLight;

      void surf (Input IN, inout SurfaceOutput o) 
      {
          float _c = tex2D (_MainTex, IN.uv_MainTex).r;
          o.Emission = _MulLight * _Color * IN.color * _c;
      }
      ENDCG
    }
	Fallback "Diffuse"
} 