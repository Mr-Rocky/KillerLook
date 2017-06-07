Shader "Unlit/Pano360Shader"
{
   Properties
   {
       //_MainTex ("Base (RGB)", 2D) = "white" {}
	   _MainTex("Base (RGB)", 2D) = "white" {}
	   //_MainTex("Color (RGB) Alpha (A)", 2D) = "white" {}
       _Color ("Main Color", Color) = (1,1,1,0.5)
   }
   SubShader 
   {
      Tags { "RenderType" = "Transparent" }
	  Tags{ "Queue" = "Transparent" }
      //This is used to print the texture inside of the sphere
      Cull Front
      CGPROGRAM
      //#pragma surface surf SimpleLambert
	  #pragma surface surf Lambert alpha
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
		 o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
         //o.Alpha = 1;
      }
      ENDCG
   }
   Fallback "Diffuse"
}