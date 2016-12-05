Shader "Custom/Unlit/TintTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
            
            // Vertex Data
			struct IN_Vertex
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

            // Pixel Data
			struct IN_Pixel
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _TintColor;
			
			IN_Pixel vert (IN_Vertex IN)
			{
                IN_Pixel OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.pos);
			    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
				UNITY_TRANSFER_FOG(OUT,OUT.vertex);
				return OUT;
			}
			
			fixed4 frag (IN_Pixel IN) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, IN.uv);
				// apply fog
				UNITY_APPLY_FOG(IN.fogCoord, col);
                // tint
				return col * _TintColor;
			}
			ENDCG
		}
	}
}
