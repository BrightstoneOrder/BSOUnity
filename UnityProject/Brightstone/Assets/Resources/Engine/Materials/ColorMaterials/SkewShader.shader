Shader "Custom/SkewShader" {
	Properties {
		mColor ("Color", Color) = (1,1,1,1)
		mMainTex ("Albedo (RGB)", 2D) = "white" {}
		mGlossiness ("Smoothness", Range(0,1)) = 0.5
		mMetallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows // vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D mMainTex;

		struct Input 
        {
			float2 uv_MainTex;
		};

		half mGlossiness;
		half mMetallic;
		fixed4 mColor;

        // void vert ( inout appdata_full v)
        // {
        // 
        // }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (mMainTex, IN.uv_MainTex) * mColor;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = mMetallic;
			o.Smoothness = mGlossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
