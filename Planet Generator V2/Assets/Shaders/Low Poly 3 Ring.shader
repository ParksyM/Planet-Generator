﻿// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

///
/// Shader taken from use MakakWasTaken and adapted using changes by use Bunny83
/// Available from: http://answers.unity3d.com/comments/1214445/view.html
///

Shader "Custom/Low Poly 3 Ring"
{
	Properties
	{
		_Color("Color", Color) = (1,0,0,1)
		_MainTex("Main Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		lighting on
		cull off
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdbase

#include "UnityCG.cginc"
#include "AutoLight.cginc"

		uniform float4 _LightColor0;

	uniform sampler2D _MainTex;
	uniform float4 _Color;

	struct v2f
	{
		float4  pos : SV_POSITION;
		float3	norm : NORMAL;
		float2  uv : TEXCOORD0;
		float3  wPos : POSITION1;
		LIGHTING_COORDS(1,2)
	};

	float4 _MainTex_ST;

	v2f vert(appdata_full v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.norm = v.normal;
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.wPos = mul(unity_ObjectToWorld, v.vertex);
		TRANSFER_VERTEX_TO_FRAGMENT(o);
		return o;
	}

	fixed4 frag(v2f IN) : COLOR
	{
		float3 x = ddx(IN.wPos);
		float3 y = ddy(IN.wPos);
		float3 vn = normalize(cross(x, y));

		float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
		float attenuation = LIGHT_ATTENUATION(IN);

		float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;

		float3 diffuseReflection =
			attenuation * _LightColor0.rgb * _Color.rgb
			* max(0.0, dot(-vn, lightDirection)); //For some reason this line will always be 0 thus making diffuseReflection 0

		fixed4 texcol = tex2D(_MainTex, IN.uv);
		return fixed4(texcol * (ambientLighting + diffuseReflection), 1.0);
	}

		ENDCG
	}
	}
		FallBack "Diffuse"
}