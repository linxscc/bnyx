Shader "Tang/NoLight" {
	Properties {
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _Ambient("Ambient", color)=(0.3, 0.3, 0.3, 0.3)
        _Specular("Specular", color)=(1, 1, 1, 1)
        _Shininess("Shininess", range(0, 8)) = 4
        _Emission("Emission", color)=(1, 1, 1, 1)
    }
    SubShader {				
        // Color[_Color]        
        Pass {
            Material
            {
                diffuse[_Color]
                // ambient[_Ambient]
                // specular[_Specular]
                // shininess[_Shininess]
                // emission[_Emission]
            }
            Lighting on
            SeparateSpecular on

            SetTexture [_MainTex] {
				// Combine texture * primary double
                Combine texture
			}
        }
    } 
    FallBack "Transparent/Cutout/VertexLit"
}
