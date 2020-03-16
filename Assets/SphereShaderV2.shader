Shader "Custom/isoshader3"
{
	Properties
	{
		_MainTex("Ground", 2D) = "brown" {}
		_SNOW_TEX("Snow", 2D) = "white" {}
		_WATER_TEX("Water", 2D) = "blue" {}
		_RADIUS("Radius", Float) = 0
		_SNOW_ABOVE("Snow Above", Float) = 11
		_SNOW_MAX("Snow Max", Float) = 13
		_WATER_BELOW("Water Below", Float) = 10
		_WATER_MIN("Water Min", Float) = 8
	}
		SubShader
		{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			LOD 200

		
		CGPROGRAM
		#pragma surface surf Lambert
		#define PI 3.141592653589793238462643383279
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _SNOW_TEX;
		sampler2D _WATER_TEX;
		float _Blend1;
		float _RADIUS;
		float _SNOW_ABOVE;
		float _SNOW_MAX;
		float _WATER_BELOW;
		float _WATER_MIN;

		struct Input
		{
			float3 worldPos;
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			// Correct crappy seam
			float2 tc = IN.uv_MainTex;
            tc.x = (PI + atan2(IN.worldPos.x, IN.worldPos.z)) / (2 * PI);
		
			fixed4 groundCol = tex2D(_MainTex, tc);
			fixed4 snowCol = tex2D(_SNOW_TEX, tc);
			fixed4 waterCol = tex2D(_WATER_TEX, tc);

			// Calculate distance to center
			float distance = sqrt(IN.worldPos.x*IN.worldPos.x + IN.worldPos.y*IN.worldPos.y + IN.worldPos.z*IN.worldPos.z);

			fixed4 mainOutput = groundCol.rgba;
			fixed4 blendOutput;

			// Distance to radius for snow
			if (distance > _SNOW_ABOVE) {
			
				float snowPercent = 0.0;
				snowPercent = (_SNOW_MAX - distance) / (_SNOW_MAX - _SNOW_ABOVE);
				if (snowPercent < 0) {
					snowPercent = 0;
				}
				mainOutput = groundCol.rgba * snowCol.a * snowPercent;
				blendOutput = snowCol.rgba * (1.0 - (snowCol.a * snowPercent));
				
			} 
			// Distance to radius for water
			else if (distance < _WATER_BELOW) {
			
				float  waterPercent = 0.0;
				waterPercent = 1 - (distance - _WATER_MIN) / (_WATER_BELOW - _WATER_MIN);
				if (waterPercent < 0) {
					waterPercent = 0;
				}
				mainOutput = groundCol.rgba * (1.0 - (waterCol.a * waterPercent));
				blendOutput = waterCol.rgba * waterCol.a * waterPercent;
			}
			
			// Set the color values for the pixel
			o.Albedo = mainOutput.rgb + blendOutput.rgb;
			o.Alpha = mainOutput.a + blendOutput.a;
		}
		ENDCG
		}
		FallBack "Diffuse"
}