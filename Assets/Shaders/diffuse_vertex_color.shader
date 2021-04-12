Shader "zx/diffuse_vert_color" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
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
          float3 worldNormal;
      };
      
      sampler2D _MainTex;
      float4 _Color;
      
      void surf (Input IN, inout SurfaceOutput o) {
          //o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
          o.Emission = _Color * IN.color;//* tex2D (_MainTex, IN.uv_MainTex).rgb;
      }
      ENDCG
    }
	Fallback "Diffuse"
} 