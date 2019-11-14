
Shader "Unlit/Charge Shader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" { }
		fillColor("Fill Color", Color) = (0, 1, 0, 1)
		charge("Charge", Float) = 0
	}
	SubShader
	{ 
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Transparent+1"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 fillColor;
			float charge;
			int active;

			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			float4 _MainTex_ST;

			v2f vert(appdata_base v)
			{
				v2f output;
				output.position = UnityObjectToClipPos(v.vertex);
				output.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

				return output;
			}

			half4 frag(v2f input) : COLOR
			{
				half4 color = tex2D(_MainTex, input.uv);

				if (charge == 0)
				{
					color *= 0.6f;
				}

				if (input.uv.x < charge)
				{
					color *= fillColor;
				}

				return color;
			}

			ENDCG
		}
	}
}
