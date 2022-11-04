// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ProceduralSpace"
{
	Properties
	{
	    _Transparency("Transparency", Range( 0.0, 1.0)) = 0.25
		_Seed("Seed", Float) = 1
		_NumStars("NumStars", Range( 0 , 1)) = 0.5
		_StarColor1("StarColor1", Color) = (1,0.7294118,0.7294118,1)
		_StarColor2("StarColor2", Color) = (0.7411765,0.764706,1,1)
		_DustAmount("DustAmount", Range( 0 , 1)) = 0.5
	}
	
	SubShader
	{
		Tags { "Queue"="Transparent-1" "RenderType"="Transparent" }
		LOD 100
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		

		Pass
		{
			Name "Unlit"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityShaderVariables.cginc"
			#include "AutoLight.cginc"
			#include "UnityStandardBRDF.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			float _Transparency;
			uniform float _Seed;
			uniform float _DustAmount;
			uniform float _NumStars;
			uniform float4 _StarColor1;
			uniform float4 _StarColor2;
			float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }
			float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
			float snoise( float3 v )
			{
				const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
				float3 i = floor( v + dot( v, C.yyy ) );
				float3 x0 = v - i + dot( i, C.xxx );
				float3 g = step( x0.yzx, x0.xyz );
				float3 l = 1.0 - g;
				float3 i1 = min( g.xyz, l.zxy );
				float3 i2 = max( g.xyz, l.zxy );
				float3 x1 = x0 - i1 + C.xxx;
				float3 x2 = x0 - i2 + C.yyy;
				float3 x3 = x0 - 0.5;
				i = mod3D289( i);
				float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
				float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
				float4 x_ = floor( j / 7.0 );
				float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
				float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 h = 1.0 - abs( x ) - abs( y );
				float4 b0 = float4( x.xy, y.xy );
				float4 b1 = float4( x.zw, y.zw );
				float4 s0 = floor( b0 ) * 2.0 + 1.0;
				float4 s1 = floor( b1 ) * 2.0 + 1.0;
				float4 sh = -step( h, 0.0 );
				float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
				float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
				float3 g0 = float3( a0.xy, h.x );
				float3 g1 = float3( a0.zw, h.y );
				float3 g2 = float3( a1.xy, h.z );
				float3 g3 = float3( a1.zw, h.w );
				float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
				g0 *= norm.x;
				g1 *= norm.y;
				g2 *= norm.z;
				g3 *= norm.w;
				float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
				m = m* m;
				m = m* m;
				float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
				return 42.0 * dot( m, px);
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.ase_texcoord1.xyz = ase_worldPos;
				
				o.ase_texcoord = v.vertex;
				o.ase_texcoord2.xyz = v.ase_texcoord.xyz;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord2.w = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float3 appendResult78 = (float3(_Seed , 0.0 , _Seed));
				float3 Position79 = ( appendResult78 + i.ase_texcoord.xyz );
				float simplePerlin3D42 = snoise( ( Position79 * 1.0 ) );
				float simplePerlin3D44 = snoise( ( Position79 * 5.0 ) );
				float lerpResult29 = lerp( -0.6 , -0.9 , ( 1.0 - _NumStars ));
				float simplePerlin3D19 = snoise( ( Position79 * 80.0 ) );
				float smoothstepResult25 = smoothstep( lerpResult29 , -1.0 , simplePerlin3D19);
				float simplePerlin2D36 = snoise( ( Position79 * 2.0 ).xy );
				float4 lerpResult40 = lerp( _StarColor1 , _StarColor2 , (simplePerlin2D36*0.5 + 0.5));

				float4 color249 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			
				float4 lerpResult248 = color249;
				float4 color259 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float simplePerlin3D171 = snoise( ( ( Position79 + 100.0 ) * 0.9 ) );
				float simplePerlin3D172 = snoise( ( Position79 * 0.9 ) );
				float4 lerpResult258 =  color259 ;
				#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
				float4 ase_lightColor = 0;
				#else //aselc
				float4 ase_lightColor = _LightColor0;
				#endif //aselc
				float temp_output_340_0 = ( 1.0);
				float lerpResult315 = lerp( 0.5 , 0.99 , temp_output_340_0);
				float3 ase_worldPos = i.ase_texcoord1.xyz;
				float3 worldSpaceLightDir = Unity_SafeNormalize(UnityWorldSpaceLightDir(ase_worldPos));
				float3 uv307 = i.ase_texcoord2.xyz;
				uv307.xy = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_297_0 = ( 1.0 - length( ( worldSpaceLightDir - uv307 ) ) );
				float smoothstepResult299 = smoothstep( ( lerpResult315 - 0.01 ) , lerpResult315 , temp_output_297_0);
				float smoothstepResult330 = smoothstep( lerpResult315 , 1.0 , temp_output_297_0);
				float lerpResult331 = lerp( 5.0 , 15.0 , temp_output_340_0);
				float simplePerlin3D323 = snoise( ( Position79 * lerpResult331 ) );
				float4 lerpResult301 = lerp( ( ( (( simplePerlin3D42 + simplePerlin3D44 )*0.1 + _DustAmount) * 0.1 ) + ( smoothstepResult25 * lerpResult40 ) + lerpResult248 + lerpResult258 ) , ase_lightColor , ( smoothstepResult299 + ( smoothstepResult330 * ( simplePerlin3D323 * 0.2 ) ) ));
				float lerpResult318 = lerp( 0.2 , 1.0 , temp_output_340_0);
				float smoothstepResult309 = smoothstep( ( lerpResult318 - 0.2 ) , lerpResult318 , temp_output_297_0);
				float lerpResult320 = lerp( 0.1 , 0.5 , temp_output_340_0);
				
				finalColor = ( lerpResult301 + ( ( smoothstepResult309 * lerpResult320 ) * ( ase_lightColor / 3.0 ) ) );
				finalColor.a = _Transparency;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}

//CHKSM=1C42086296F33F7F125CDCF0E910D86E5A5089E5