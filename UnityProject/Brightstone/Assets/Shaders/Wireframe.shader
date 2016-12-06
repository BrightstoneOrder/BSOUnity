// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Wireframe"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _WireColor("Wire Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _WireThickness("Wire thickness", Range(0,800)) = 100
    }
        SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma target 5.0
            #pragma only_renderers d3d11

            #include "UnityCG.cginc"

            float4 _BaseColor;
            float4 _WireColor;
            float _WireThickness;

            struct IN_Vertex 
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            struct IN_Pixel
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };


			IN_Pixel vert (IN_Vertex IN)
			{
                IN_Pixel OUT;
                OUT.pos = mul(UNITY_MATRIX_MVP, IN.pos);
                OUT.color = IN.color;
				return OUT;
			}

            

			
			fixed4 frag (IN_Pixel IN) : SV_Target
			{
                float2 uv = IN.color.xy;
                //float uv2 = IN.color.zw;
                float d = uv.x - uv.y;
                if (uv.x < _WireThickness)
                {
                    return _WireColor;
                }
                else if (uv.x > 1 - _WireThickness)
                {
                    return _WireColor;
                }
                else if (uv.y < _WireThickness)
                {
                    return _WireColor;
                }
                else if (uv.y > 1 - _WireThickness)
                {
                    return _WireColor;
                }
                else if (uv.y < _WireThickness)
                {
                    return _WireColor;
                }
                else
                {
                    return _BaseColor;
                }
			}
			ENDCG
		}
	}
}
