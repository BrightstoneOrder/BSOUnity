// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Shaders/Terrain/TerrainShader"
{
    Properties
    {
        // TODO: Remove for multiple textures with blending!
        mTexture("Texture", 2D) = "white" {}
        mTintColor("Tint Color", Color) = (0.0, 0.0, 0.0, 1.0)
        
        // Debugging
        mWireColor("Wire Color", Color) = (0.0, 1.0, 0.0, 1.0)
        mWireThickness("Wire thickness", Range(0,1.0)) = 0.05

    }
    SubShader
    {
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" }
        ZWrite true

        Pass
        {
            CGPROGRAM
            // Shader Data
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #include "UnityCG.cginc" // TRANSFORM_TEX

            struct IN_Vertex
            {
                float4 pos : POSITION;      // position
                float2 uvQuad : TEXCOORD0;  // uvs for individual quads
                float2 uvHeight : TEXCOORD1;// uvs for heightmap
                float4 meta : TEXCOORD2;    // (xyz) wireframe, vertex id(w)
            };

            struct IN_Pixel
            {
                float4 pos : SV_POSITION;
                float2 uvQuad : TEXCOORD0;
                float2 uvHeight : TEXCOORD1;
                float3 uvWire : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };

            struct QuadCollisionData
            {
                float height;
            };

            // Common Function

            // -- Component Wise less than. 
            float3 LessThan(float3 a, float3 b)
            {
                return float3(
                    a.x < b.x ? 1.0 : 0.0,
                    a.y < b.y ? 1.0 : 0.0,
                    a.z < b.z ? 1.0 : 0.0);
            }

            // Public Properties
            sampler2D mTexture;
            float4 mTexture_ST;
            float4 mTintColor;
            float4 mWireColor;
            float mWireThickness;

            // Private Properties
            StructuredBuffer<QuadCollisionData> mTerrainData;
            float mTerrainMaxHeight;
            float4 mBrushPosition;
            float mBrushAlpha;



            // Vertex
            IN_Pixel vert(IN_Vertex IN)
            {
                IN_Pixel OUT;

                // Get Vertex Id:
                int id = floor(IN.meta.w);

                // Marshal UV's
                OUT.uvQuad = TRANSFORM_TEX(IN.uvQuad, mTexture); //IN.uv1;
                OUT.uvHeight = IN.uvHeight;
                OUT.uvWire = IN.meta.xyz;

                // Heightmap:
                IN.pos.y = lerp(0.0, mTerrainMaxHeight, mTerrainData[id].height);
                OUT.pos = mul(UNITY_MATRIX_MVP, IN.pos);
                OUT.worldPos = mul(unity_ObjectToWorld, IN.pos);

                // If debugging affected vertices..
                //float highlight = mHeightMapBuffer[id];
                //OUT.color = lerp(float3(1.0, 1.0, 1.0), _SelectColor.xyz, highlight);
            
                return OUT;
            }


            // Pixel
            fixed4 CalcWireColor(IN_Pixel IN)
            {
                if (any(LessThan(IN.uvWire, float3(mWireThickness, mWireThickness, mWireThickness))))
                {
                    return mWireThickness;
                }
                else
                {
                    return float4(mTintColor) * tex2D(mTexture, IN.uvQuad);
                }
            }

            fixed4 frag(IN_Pixel IN) : SV_Target
            {
                return CalcWireColor(IN);
            }
            ENDCG
        }
    }
}
