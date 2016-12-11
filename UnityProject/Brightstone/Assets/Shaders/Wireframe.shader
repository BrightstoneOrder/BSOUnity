// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Wireframe"
{
    Properties
    {
        _Texture("Texture", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _WireColor("Wire Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _SelectColor("Select Color", Color) = (0.0, 0.0, 1.0, 1.0)
        _WireThickness("Wire thickness", Range(0,1.0)) = 0.05

        // Terrain Stuff
        _Heightmap("Heightmap", 2D) = "black" {}
    }
        SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
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
                float4 pos : POSITION;
                float2 uv1 : TEXCOORD0; // texture
                float4 uv2 : TEXCOORD1; // debug wireframe (xyz) and id(w)
                float2 uv3 : TEXCOORD2; // height
            };

            struct IN_Pixel
            {
                float4 pos : SV_POSITION;
                float2 uv1 : TEXCOORD0;
                float3 uv2 : TEXCOORD1;
                float2 uv3 : TEXCOORD2;
                float3 color : COLOR0;
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
            
            const float EPSILON = 0.000001f;

            static bool Equals(float a, float b)
            {
                return a > b ? ((a - b) < EPSILON) : ((b - a) < EPSILON);
            }

            // Shader Uniforms
            sampler2D _Texture;
            float4 _Texture_ST;
            float4 _TintColor;
            float4 _WireColor;
            float4 _SelectColor;
            float _WireThickness;
            sampler2D _Heightmap;
            float4 _Heightmap_ST;

            // Private Uniforms
            float _Selected;
            float _Height;
            


            // Vertex
			IN_Pixel vert (IN_Vertex IN)
			{
                IN_Pixel OUT;
                
                OUT.uv1 = TRANSFORM_TEX(IN.uv1, _Texture); //IN.uv1;
                OUT.uv2 = IN.uv2.xyz;
                OUT.uv3 = TRANSFORM_TEX(IN.uv3, _Texture); //IN.uv3;

                int id = floor(IN.uv2.w);
                int selected = floor(_Selected);
                if (id == selected)
                {
                    OUT.color = _SelectColor.xyz;
                }
                else
                {
                    OUT.color = float3(1.0, 1.0, 1.0);
                }

                // Apply Heightmap
                float4 heightInfo = tex2Dlod(_Heightmap, float4(OUT.uv3, 0.0, 0.0));
                IN.pos.y = lerp(0, _Height, heightInfo.r);
                OUT.pos = mul(UNITY_MATRIX_MVP, IN.pos);
				return OUT;
			}


            // Pixel
            fixed4 CalcWireColor(IN_Pixel IN)
            {
                if (any(LessThan(IN.uv2, float3(_WireThickness, _WireThickness, _WireThickness))))
                {
                    return _WireColor;
                }
                else
                {
                    return float4(IN.color, 1.0) * tex2D(_Texture, IN.uv1);
                }
            }

			fixed4 frag (IN_Pixel IN) : SV_Target
			{
                return CalcWireColor(IN);
			}
			ENDCG
		}
	}
}
