Shader "ThreeGlasses/DepthComposite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			sampler2D _CameraDepthTexture;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;

#if defined (UNITY_UV_STARTS_AT_TOP) // fix DX uv
				o.uv = float2(o.uv.x, 1 - o.uv.y);
#endif
				return o;
			}
			
			float getDepth(float2 uv)
			{
				return tex2D(_CameraDepthTexture, uv).x;
			}

			float4 frag (v2f i) : COLOR
			{
				float depth = 1.0 - getDepth(i.uv);
				float4 col = float4(tex2D(_MainTex, i.uv.xy).rgb, depth);
				return col;
			}
			ENDCG
		}
	}
}
