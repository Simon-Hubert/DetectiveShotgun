Shader "CustomRenderTexture/CRTEffect"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0, 1)) = 0.06
        _Zoom ("Zoom", Range(1, 1.5)) = 1.04
        _Vignette ("Vignette", Range(0, 1)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _Distortion;
                float _Zoom;
                float _Vignette;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 p = (input.uv - 0.5) * 2.0;

                // Distortion
                float r2 = dot(p, p);
                float2 uv = 0.5 + 0.5 * (p * (1.0 + _Distortion * r2) / _Zoom);

                // Black outside the valid UV region
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                    return half4(0, 0, 0, 1);

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // Vignette - edge darkening
                float vignette = 1.0 - _Vignette * smoothstep(0.25, 1.0, r2);

                col.rgb *= vignette;
                col *= input.color;

                return col;
            }
            ENDHLSL
        }
    }
}
