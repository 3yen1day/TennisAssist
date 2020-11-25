Shader "Custom/Transparent" {
	Properties{
		_MainColor("Color", Color) = (1,1,1,0.25)
		//_RimColor("Color", Color) = (1,1,1,1)
		_Alpha("Alpha", Range(0,1)) = 0.25
	}
		SubShader{
			Tags{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			}
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert alpha:fade
			#pragma target 3.0

			struct Input {
				float2 uv_MainTex;
				float3 worldNormal;
				float3 viewDir;
			};

			fixed4 _MainColor;
			//fixed4 _RimColor;
			fixed _Alpha;

			void surf(Input IN, inout SurfaceOutput o) {

				o.Albedo = _MainColor;
				float rim = 1 - saturate(dot(IN.viewDir, o.Normal));
				o.Alpha = _Alpha;
				o.Emission = _MainColor * pow(rim, 2.5);
			}
			ENDCG
	}
		FallBack "Diffuse"
}