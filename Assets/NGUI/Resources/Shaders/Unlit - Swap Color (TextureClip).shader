// Community contribution: http://www.tasharen.com/forum/index.php?topic=9268.0
Shader "Hidden/Unlit/Swap Color (TextureClip)"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _ClipTex;
			float4 _ClipRange0 = float4(0.0, 0.0, 1.0, 1.0);

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				half4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 clipUV : TEXCOORD1;
				half4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				o.clipUV = (v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy) * 0.5 + float2(0.5, 0.5);
				return o;
			}

			half4 frag (v2f IN) : SV_Target
			{
				half4 color = tex2D(_MainTex, IN.texcoord);
				fixed r = IN.color.r;
				if (r == 0) r = 0.01;
				fixed b = IN.color.b;
				if (b == 0) b = 0.01;
				fixed g = IN.color.g;
				if (g == 0) g = 0.01;
				color.rgb = color.r * fixed3(1, 1, 1) + color.b * fixed3(0.7, 0.7, 0.7) + color.g * fixed3(0.6 / r, 0.76 / g, 0.9 / b);
				color = color * fixed4(r, g, b, IN.color.a);

				//color.a *= tex2D(_ClipTex, IN.clipUV).a;
				return color;
			}
			ENDCG
		}
	}
	Fallback "Unlit/Transparent Colored"
}
