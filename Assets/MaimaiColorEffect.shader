Shader "Unlit/MaimaiColorEffect"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Saturation("Saturation", Range(0,1)) = 1
		_Brightness("Brightness", Range(0,1)) = 1
		_Alpha("Alpha", Range(0,1)) = 0.5
		_Speed("Speed", Range(0.1, 10)) = 1
		_InnerLB("Inner Lower Bound", Range(0, 1)) = 0
		_InnerUB("Inner Upper Bound", Range(0.0001, 1)) = 0
		_OuterLB("Outer Lower Bound", Range(0, 0.9999)) = 0.9999
		_OuterUB("Outer Upper Bound", Range(0, 1)) = 1
	}
		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}
			LOD 100

			Pass
			{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD2;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Alpha;
			fixed _Speed;
			float _Saturation;
			float _Brightness;
			float _InnerLB;
			float _InnerUB;
			float _OuterLB;
			float _OuterUB;

			float3 hsb2rgb(float3 c) {
				float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6) - 3.0) - 1.0, 0, 1);
				rgb = rgb * rgb * (3.0 - 2.0 * rgb);
				return c.z * lerp(float3(1, 1, 1), rgb, c.y);
			}

			float clamp01(float x, float lb, float ub) {
				return (clamp(x, lb, ub) - lb) / (ub - lb);
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float x = i.uv.x - 0.5;
				float y = i.uv.y - 0.5;

				float radius = clamp((y * y + x * x) * 4, 0, 1) * _Alpha;
				float alpha = 1.0 * _Alpha * clamp01(radius, _InnerLB, _InnerUB) * clamp01(1 - radius, 1 - _OuterUB, 1 - _OuterLB);

				float angle = atan2(y, x);
				angle = angle + step(0, -angle) * 3.14159 * 2;
				angle = angle / 6.28;

				float hsb_h = ceil(fmod(angle * 3 + ceil(_Time.y * _Speed * 5) * 0.2, 1) * 5) / 5;

				float3 color = hsb2rgb(float3(hsb_h, _Saturation, _Brightness));

				fixed3 albedo = col.rgb;
				fixed3 diffues = color * albedo;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return fixed4(diffues, col.a * alpha);
			}
			ENDCG
		}
		}
}
