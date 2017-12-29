Shader "Custom/ShaderTextureSplice"
{
	Properties
	{
		[HideInInspector]
		_MainTex("Main Texture", 2D) = "white" {}
		_Tex1("Render Texture A", 2D) = "white" {}
		_Tex2("Render Texture B", 2D) = "white" {}
		_PosA("Position A", Vector) = (0, 0, 0, 0)
		_PosB("Position B", Vector) = (0, 0, 0, 0)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct FRAG_IN
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_TexelSize;

			FRAG_IN vert(appdata v)
			{
				FRAG_IN o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv;

		#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - o.uv1.y;
		#endif

				return o;
			}

			sampler2D _Tex1;
			sampler2D _Tex2;
			vector _PosA;
			vector _PosB;

			fixed4 frag(FRAG_IN i) : SV_Target
			{
				vector diff = _PosA - _PosB;
				vector normal = normalize(diff);
				
				float multiple = diff.y > 0 ? 1 : 0;

				// normal.x < 0.5 = p1 left of p2 on x axis
				// normal.y < 0.5 = p1 top of p2 on y axis

				// counterclockwise perpendicular
				vector perpendicular = vector(-normal.y, normal.x, 0, 0);
				// the line
				float m = perpendicular.y / perpendicular.x;
				float test = 0.5 + (m)*(i.uv.x - 0.5);
				
				/*
				d^2 = (Am+Bn+C)^2 / (A^2+B^2)
				Ax+By+C = 0 is the line and (m,n) is the point
				so for point (i.uv.x, i.uv.y) and line y=(-m)x => A=1/-m, B=1, C=0
				d^2 = ((-1/m)i.uv.x + i.uv.y)^2 / (-1/m)^2
				*/

				float mRecipNeg = -1 / m;
				float e1 = m * i.uv.x - i.uv.y + (0.5 - 0.5 * m);
				float e2 = m * m + 1;
				float d2 = (e1 * e1) / e2;

				if (0.4 < d2 && d2 < 0.6)
				{
					return vector(0, 0, 0, 0);
				}

				if (i.uv1.y < test)
					return diff.y < 0 ? tex2D(_Tex1, i.uv1) : tex2D(_Tex2, i.uv1);
				else
					return diff.y < 0 ? tex2D(_Tex2, i.uv1) : tex2D(_Tex1, i.uv1);

			}

			ENDCG
		}
	}
}
