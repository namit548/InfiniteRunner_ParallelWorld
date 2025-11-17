Shader "Bend Shader/CutOut" {
	Properties {
		_Color ("Main Color", Vector) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
		[HideInInspector] _V_CW_Rim_Color ("", Vector) = (1,1,1,1)
		[HideInInspector] _V_CW_Rim_Bias ("", Range(-1, 1)) = 0.2
		[HideInInspector] _V_CW_Fog_Color ("", Vector) = (1,1,1,1)
		[HideInInspector] _V_CW_Fog_Density ("", Range(0, 1)) = 1
		[HideInInspector] _V_CW_Fog_Start ("", Float) = 0
		[HideInInspector] _V_CW_Fog_End ("", Float) = 100
		[HideInInspector] _V_CW_IBL_Intensity ("", Float) = 1
		[HideInInspector] _V_CW_IBL_Contrast ("", Float) = 1
		[HideInInspector] _V_CW_IBL_Cube ("", Cube) = "" {}
		[HideInInspector] _V_CW_Emission_Color ("", Vector) = (1,1,1,1)
		[HideInInspector] _V_CW_Emission_Strength ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}