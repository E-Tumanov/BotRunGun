Shader "zx/floor" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	}

	SubShader {
		LOD 200		
		Tags { "RenderType"="Opaque" "Queue" = "Background" }
		Pass {
			 ColorMask 0
			//Color [_Color]
		}
	}
} 