Shader "KTJ/Sprite Scan Pixelate"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _ScanProgress ("Scan Progress", Range(0, 1)) = 1
        _StartPixelCount ("Start Pixel Count", Float) = 5
        _OutlineColor ("Outline Color", Color) = (1, 0.61, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0, 10)) = 0
        _OutlineMultiplierX ("Outline Multiplier X", Float) = 0.002
        _OutlineMultiplierY ("Outline Multiplier Y", Float) = 0.002
        [HideInInspector] _SpriteUVRect ("Sprite UV Rect", Vector) = (0, 0, 1, 1)
        [HideInInspector] _SpritePixelSize ("Sprite Pixel Size", Vector) = (256, 256, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Unlit"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "SpriteUnlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _ScanProgress;
                float _StartPixelCount;
                float4 _OutlineColor;
                float _OutlineThickness;
                float _OutlineMultiplierX;
                float _OutlineMultiplierY;
                float4 _SpriteUVRect;
                float4 _SpritePixelSize;
            CBUFFER_END

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                float progress = saturate(_ScanProgress);
                float2 rectSize = max(_SpriteUVRect.zw, float2(0.00001, 0.00001));
                float2 localUV = (input.uv - _SpriteUVRect.xy) / rectSize;

                float2 originalPixelCount = max(_SpritePixelSize.xy, float2(1.0, 1.0));
                float aspectRatio = originalPixelCount.y / originalPixelCount.x;
                float2 startPixelCount = float2(
                    max(_StartPixelCount, 1.0),
                    max(_StartPixelCount * aspectRatio, 1.0));
                float2 currentPixelCount = lerp(
                    startPixelCount,
                    originalPixelCount,
                    progress);

                float2 pixelatedLocalUV =
                    (floor(localUV * currentPixelCount) + 0.5) / currentPixelCount;
                float2 pixelatedUV =
                    _SpriteUVRect.xy + saturate(pixelatedLocalUV) * rectSize;

                float useOriginalUV = step(0.9999, progress);
                float2 sampleUV = lerp(pixelatedUV, input.uv, useOriginalUV);
                half4 spriteColor =
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, sampleUV);

                float2 outlineOffset = float2(
                    _OutlineMultiplierX,
                    _OutlineMultiplierY) * _OutlineThickness;
                half surroundingAlpha = 0.0h;
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        sampleUV + float2(outlineOffset.x, 0.0)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        sampleUV - float2(outlineOffset.x, 0.0)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        sampleUV + float2(0.0, outlineOffset.y)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        sampleUV - float2(0.0, outlineOffset.y)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        sampleUV + outlineOffset).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        sampleUV - outlineOffset).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        sampleUV + float2(outlineOffset.x, -outlineOffset.y)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        sampleUV + float2(-outlineOffset.x, outlineOffset.y)).a);

                half4 tintedSprite = spriteColor * input.color;
                half outlineAlpha =
                    saturate(surroundingAlpha - spriteColor.a) *
                    _OutlineColor.a;
                half4 outputColor;
                outputColor.rgb = lerp(
                    _OutlineColor.rgb,
                    tintedSprite.rgb,
                    spriteColor.a);
                outputColor.a = max(tintedSprite.a, outlineAlpha);
                return outputColor;
            }
            ENDHLSL
        }
    }
}
