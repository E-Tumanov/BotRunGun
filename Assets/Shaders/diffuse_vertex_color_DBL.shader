Shader "zx/diffuse_vert_color_DBL" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base", 2D) = "white" {}
		_LMAP ("LMAP", 2D) = "white" {} 
	}
	
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      
      #pragma surface surf Lambert noforwardadd noambient
      struct Input {
          float4 color : COLOR;
          float2 uv_MainTex;
          float2 uv2_LMAP;
      };
      
      sampler2D _MainTex;
      sampler2D _LMAP;
      
      void surf (Input IN, inout SurfaceOutput o) {
          //o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          //o.Emission = 1 * IN.color * tex2D (_MainTex, IN.uv_MainTex).rgb;
		  float3 lm = tex2D (_LMAP, IN.uv2_LMAP).rgb * 1.0	 + 0.25;
		  //lm *= lm;
		  o.Emission = tex2D (_MainTex, IN.uv_MainTex).rgb * lm;
					   
		 //o.Emission *= 0.5;
      }
      ENDCG
      
      //
      //
      //
      
      Cull front
	  CGPROGRAM
      
      #pragma surface surf Lambert noforwardadd noambient
      struct Input {
          float2 uv_MainTex;
      };
      
      sampler2D _MainTex;
      
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = 0;
          o.Emission = tex2D (_MainTex, IN.uv_MainTex).rgb * 0.3;
      }
      ENDCG
    }
    Fallback "Diffuse"
}
