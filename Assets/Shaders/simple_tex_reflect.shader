Shader "zx/simple_tex_reflect" {
	Properties {

		_Color ("Main Color", Color) = (1,1,1,1)
		_CtrColor ("Contour Color", Color) = (1,1,1,1)
		_MulA ("Ambient mul", float) = 1
		_MulR ("Reflect mul", float) = 1
			_b ("bias", float) = 1
		_Metal("Metal(S)", float) = 0
		_MainTex ("Base (RGB)", 2D) = "white" {} 
	    _AmbientTex ("Ambient Cube (RGB)", Cube) = "white" {} 
		_ReflectTex ("Reflect Cube (RGB)", Cube) = "white" {} 
	}

    SubShader {
    	
      Tags { "RenderType" = "Opaque" }
	  //Cull Off
	  //------------------------------------------------------
	  /*
	  Pass {       
		//Tags { "LightMode" = "PrepassBase" }
		CGPROGRAM
			#pragma target 3.0 
			#pragma vertex vert
        	#pragma fragment frag
        	#include "UnityCG.cginc"
        	
			sampler2D _MainTex;
			samplerCUBE _ReflectTex;
		    samplerCUBE _AmbientTex;
    
			float4 _Color;
			float _MulA;
			float _MulR;
			float _b;
			
			struct v2f {
	          	float4 pos        		: POSITION;
				float4 color 			: COLOR;
				float3 worldRefl 		: TEXCOORD0;
				float3 worldNormal 		: TEXCOORD1;
	        };
			
			v2f vert ( appdata_full v )
        	{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
				float4 w_pos = mul(_Object2World, v.vertex );
				o.worldNormal = mul(_Object2World, float4 ( v.normal, 0)).xyz;
				o.worldRefl   = reflect (normalize (w_pos.xyz - _WorldSpaceCameraPos), o.worldNormal ).xyz;
				//o.worldRefl   = normalize (o.worldNormal).xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0  {	 
				float3 cc = texCUBElod (_ReflectTex, float4(i.worldRefl, _b) ).rgb;
        		return fixed4(cc,1);
        	}
		ENDCG
	}
	}			
	*/
	//------------------------------------------
	
     CGPROGRAM
     	 #pragma target 3.0 
      #pragma surface surf Lambert noforwardadd noambient
      struct Input {
		float4 color : COLOR;
		float3 worldRefl;
		float3 worldNormal;
		float2 uv_MainTex;
		float3 viewDir;
      };

      
	sampler2D _MainTex;
	samplerCUBE _ReflectTex;
    samplerCUBE _AmbientTex;
    
      float4 _Color;
		float4 _CtrColor;
      float _MulA;
      float _MulR;
      float _b;
		float _Metal;

      void surf (Input IN, inout SurfaceOutput o) {
          //o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		  //float4 _c = tex2D (_MainTex, IN.uv_MainTex);
			
			float fresnel = pow (1 - dot(IN.viewDir, IN.worldNormal), 4);

			float3 _ambl = texCUBElod(_AmbientTex, float4(-IN.worldNormal, 6)).rgb;
			float3 _refl = _MulR * texCUBElod(_ReflectTex, float4(IN.worldRefl, _b + 2)).rgb;
			_refl *= _Metal * _Color + (1 - _Metal);
			o.Emission = _MulA * _Color * _ambl * tex2D(_MainTex, IN.uv_MainTex).rgb + _refl;
			
			o.Emission += _CtrColor.rgb * 2 * fresnel;

         // o.Emission = _MulA * _Color * texCUBE (_AmbientTex, IN.worldNormal).rgb + _MulR * texCUBElod (_ReflectTex, float4(IN.worldRefl, _b + 2) ).rgb;
          //o.Emission = _MulA * _Color + _MulR * texCUBElod (_ReflectTex, float4(IN.worldRefl, _b + 2) ).rgb;
      }
      ENDCG
    }
	Fallback "Diffuse"
} 