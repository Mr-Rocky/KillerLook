Shader "Dissolve/Dissolve Origin Point Pano360"
{
	Properties 
	{
		_MainTex ("Diffuse (RGBA)", 2D) = "white"{}
		_BurnGradient("Burn Gradient (RGB)", 2D) = "white"{}
		_NoiseTex ("Burn Map (RGB)", 2D) = "black"{}
		_DissolveValue ("Value", Range(0,1)) = 1.0
		_HitPos("Position", Vector) = (0.0,0.0,0.0,0.0)
		_GradientAdjust ("Gradient", Range(0.1,10.0)) = 10.0
		_LargestVal ("Largest Value", float) = 1.0
		_Color ("Main Color", Color) = (1,1,1,0.5)
	}
	SubShader 
	{
		Tags {"Queue" = "Transparent"}
		Tags { "RenderType" = "Opaque" }

        //This is used to print the texture inside of the sphere
        Cull Front
        CGPROGRAM
        #pragma surface surf SimpleLambert
        half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten)
        {
           half4 c;
           c.rgb = s.Albedo;
           return c;
        }
      
        sampler2D _MainTex;
        struct Input
        {
           float2 uv_MainTex;
           float4 myColor : COLOR;
        };
 
        fixed3 _Color;
        void surf (Input IN, inout SurfaceOutput o)
        {
           //This is used to mirror the image correctly when printing it inside of the sphere
		   IN.uv_MainTex.x = 1 - IN.uv_MainTex.x;
           fixed3 result = tex2D(_MainTex, IN.uv_MainTex)*_Color;
           o.Albedo = result.rgb;
           o.Alpha = 1;
        }
		ENDCG
		
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull back
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			sampler2D _BurnGradient;
			sampler2D _NoiseTex;
			float _DissolveValue;
			float4 _HitPos;
			float _LargestVal;
			float _GradientAdjust;
			struct vIN
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct vOUT
			{
				float4 pos : SV_POSITION;
				float3 oPos : TEXCOORD2;
				float3 hitPos : TEXCOORD1;
				float2 uv : TEXCOORD0;
			};
			
			vOUT vert(vIN v)
			{
				vOUT o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				o.oPos = v.vertex;
				o.hitPos = mul(unity_WorldToObject, _HitPos).xyz;
				return o;
			}
			
			float sqrMagnitude(float3 v)
			{
				return (v.x*v.x + v.y*v.y + v.z*v.z);
			}
			
			fixed4 frag(vOUT i) : COLOR
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv);
				fixed noiseVal = tex2D(_NoiseTex, i.uv).r;
				
				fixed toPoint =  (length(i.oPos.xyz - i.hitPos.xyz) / ((1.0001 - _DissolveValue) * _LargestVal));
				fixed d = ( (2.0 * _DissolveValue + noiseVal) * toPoint * noiseVal ) - 1.0;

				fixed overOne = saturate(d * _GradientAdjust);

				fixed4 burn = tex2D(_BurnGradient, float2(overOne, 0.5));
				return mainTex * burn;
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}
