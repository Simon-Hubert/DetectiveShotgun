Shader "Hidden/FullScreenBlur"
{
    SubShader
    {
        Tags
        { 
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "Downsample"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float _Intensity;
            Texture2D _OriginalBlit;
            
            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

                float2 highResTexelSize = _BlitTexture_TexelSize.xy;
                float2 offset = highResTexelSize * _Intensity;

                half4 center = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv) * 4.0h;

                half4 topRight = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(offset.x, offset.y));
                half4 bottomRight = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(offset.x, -offset.y));
                half4 bottomLeft = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-offset.x, -offset.y));
                half4 topLeft = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-offset.x, offset.y));
                
                return (center + topRight + bottomRight + bottomLeft + topLeft) * 0.125h;
            }

            ENDHLSL
        }

        Pass
        {
            Name "Upsample"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float _Intensity;
            Texture2D _OriginalBlit;

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

                float2 lowResTexelSize = _BlitTexture_TexelSize.xy;
                float2 offset = lowResTexelSize * _Intensity;
                float2 twiceOffset = offset * 2.0;

                half4 up = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0.0, twiceOffset.y));
                half4 right = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(twiceOffset.x, 0.0));
                half4 bottom = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(0.0, -twiceOffset.y));
                half4 left = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-twiceOffset.x, 0.0));

                half4 topRight = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(offset.x, offset.y));
                half4 bottomRight = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(offset.x, -offset.y));
                half4 bottomLeft = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-offset.x, -offset.y));
                half4 topLeft = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + float2(-offset.x, offset.y));

                half4 cross = up + right + bottom + left;
                half4 diagonal = topRight + bottomRight + bottomLeft + topLeft;

                return (cross + (diagonal * 2.0h)) * 0.083333h;

            }

            ENDHLSL
        }

        Pass
        {
            Name "FinalBlit"

            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            float _Intensity;
            Texture2D _OriginalBlit;
            float4 _MousePosition;

            #pragma vertex Vert
            #pragma fragment Frag
            
            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.texcoord;                
                
                half4 blurCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);
                half4 col = SAMPLE_TEXTURE2D_X(_OriginalBlit, sampler_LinearClamp, uv);
                
                half2 mouse = _MousePosition.xy / _ScreenParams.xy;
                mouse = mouse * 2 - 1;
                //mouse = half2(mouse.x, -mouse.y);
                
                uv = uv * 2 - 1;
                uv.y *= _ScreenParams.y/_ScreenParams.x;
                
                half2 center = uv;
                center -= mouse/1.45f;
                
                float mask = length(center);
                mask = smoothstep(0.40,0.60,mask);

                return lerp(col, blurCol, mask);

            }
            
            ENDHLSL
        }
    }

    Fallback Off
}