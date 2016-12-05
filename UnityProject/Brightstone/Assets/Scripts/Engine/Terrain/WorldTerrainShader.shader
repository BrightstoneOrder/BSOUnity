Shader "Custom/WorldTerrainShader" 
{
	Properties 
    {
        mTexture("Texture", 2D) = "white" {}
	}
	SubShader 
    {
		Tags { "RenderType"="Opaque" }
		
        Pass
        {
            CGPROGRAM
            #pragma vertex main_vertex 
            #pragma fragment main_pixel
            #pragma multi_compile VERTEX_COLOR_OFF VERTEX_COLOR_ON

            #include "UnityCG.cginc" // TRANSFORM_TEX

            struct IN_VertexData
            {
                float4 position : POSITION;
#if VERTEX_COLOR_ON 
                float4 color : COLOR0;
#else
                float2 uv0 : TEXCOORD0;
#endif
            };

            struct IN_PixelData
            {
                float4 position : SV_POSITION;
#if VERTEX_COLOR_ON
                float4 color : COLOR0;
#else
                float2 uv0 : TEXCOORD0;
#endif
            };

            sampler2D mTexture;
            float4 mTexture_ST;

            IN_PixelData main_vertex(IN_VertexData IN)
            {
                IN_PixelData OUT;
                OUT.position = mul(UNITY_MATRIX_MVP, IN.position);
#if VERTEX_COLOR_ON
                OUT.color = IN.color;
#else
                OUT.uv0 = TRANSFORM_TEX(IN.uv0, mTexture);
#endif
                return OUT;
            }

            fixed4 main_pixel(IN_PixelData IN) : SV_Target
            {
#if VERTEX_COLOR_ON
                return fixed4(IN.color);
#else
                return fixed4(tex2D(mTexture, IN.uv0));
#endif
            }
            ENDCG
        }
	}
	FallBack "Diffuse"
    CustomEditor "WorldTerrainMaterialEditor"
}
