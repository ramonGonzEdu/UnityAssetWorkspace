Shader "DaMastaCoda/GridShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Scale ("Scale", Range(0,100)) = 10
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert fullforwardshadows
        
        #pragma target 3.0
        

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;    
            float3 worldPos;
        };

        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.localPos = v.vertex.xyz;
        }
        // Use shader model 3.0 target, to get nicer looking lighting


        half _Scale;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color IN.uv_MainTex
            fixed4 c = _Color;
            o.Albedo = c.rgb;
            o.Albedo += sin(IN.worldPos.x*_Scale)>0.97 || sin(IN.worldPos.y*_Scale)>0.97 || sin(IN.worldPos.z*_Scale)>0.97;
            // o.Albedo = IN.worldPos;
            // o.Albedo.b = IN.uv_MainTex.x ;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
