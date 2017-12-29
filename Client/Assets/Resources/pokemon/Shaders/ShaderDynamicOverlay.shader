Shader "Custom/ShaderDynamicOverlay"
{
	Properties
	{
		[HideInInspector]
		_MainTex ("Texture", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0, 1)) = 0.0
		_Color("Screen Color", Color) = (1, 1, 1, 1)
		[MaterialToggle] _Distort("Distort", Float) = 0
		_Fade("Fade", Range(0, 1)) = 0
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

			FRAG_IN vert (appdata v)
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

			sampler2D _MainTex;
			sampler2D _TransitionTex;
			float _Cutoff;
			fixed4 _Color;
			int _Distort;
			float _Fade;

			fixed4 frag(FRAG_IN i) : SV_Target
			{
				fixed4 transition = tex2D(_TransitionTex, i.uv1);

				fixed2 direction = float2(0,0);
				if (_Distort)
					direction = normalize(float2((transition.r - 0.5) * 2, (transition.g - 0.5) * 2));

				fixed4 col = tex2D(_MainTex, i.uv + _Cutoff * direction);

				if (transition.b < _Cutoff)
					return col = lerp(col, _Color, _Fade);
				
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
