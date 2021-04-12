Shader "zx/wall_dside" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	}

	SubShader {
		LOD 200		
		Tags { "RenderType"="Opaque" "Queue" = "Background" }
		Pass {
			Material {
                Diffuse (1,1,1,1)
                Ambient (1,1,1,1)
            }
			Lighting On
            SetTexture [_MainTex] {
                combine previous * texture
            }
		}
		
		Pass {
			Cull front
			Color (0.1,0.1,0.1,1)
			SetTexture [_MainTex] {
                combine previous * texture
            }
		}
	}
} 