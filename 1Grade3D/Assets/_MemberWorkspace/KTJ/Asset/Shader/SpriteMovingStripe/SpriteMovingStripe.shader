Shader "KTJ/Sprite Moving Stripe"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _StripeColor ("Stripe Color", Color) = (1, 0.75, 0.1, 0.65)
        _StripeDensity ("Stripe Density", Range(1, 30)) = 9
        _StripeWidth ("Stripe Width", Range(0.02, 0.95)) = 0.35
        _StripeSoftness ("Stripe Softness", Range(0.001, 0.25)) = 0.06
        _StripeSpeed ("Stripe Speed", Range(-5, 5)) = 1
        _StripeIntensity ("Stripe Intensity", Range(0, 2)) = 1
        _OutlineColor ("Outline Color", Color) = (1, 0.61, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(0, 10)) = 0
        _OutlineMultiplierX ("Outline Multiplier X", Float) = 0.002
        _OutlineMultiplierY ("Outline Multiplier Y", Float) = 0.002
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
                float4 _StripeColor;
                float _StripeDensity;
                float _StripeWidth;
                float _StripeSoftness;
                float _StripeSpeed;
                float _StripeIntensity;
                float4 _OutlineColor;
                float _OutlineThickness;
                float _OutlineMultiplierX;
                float _OutlineMultiplierY;
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
                half4 spriteColor =
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 tintedSprite = spriteColor * input.color;

                float stripePhase = frac(
                    (input.uv.x + input.uv.y) * _StripeDensity -
                    _Time.y * _StripeSpeed);
                float halfWidth = _StripeWidth * 0.5;
                float stripeDistance = abs(stripePhase - 0.5);
                float stripeMask = 1.0 - smoothstep(
                    halfWidth,
                    halfWidth + _StripeSoftness,
                    stripeDistance);
                float stripeBlend = saturate(
                    stripeMask * _StripeColor.a * _StripeIntensity);

                tintedSprite.rgb = lerp(
                    tintedSprite.rgb,
                    _StripeColor.rgb,
                    stripeBlend * spriteColor.a);

                float2 outlineOffset = float2(
                    _OutlineMultiplierX,
                    _OutlineMultiplierY) * _OutlineThickness;
                half surroundingAlpha = 0.0h;
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        input.uv + float2(outlineOffset.x, 0.0)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        input.uv - float2(outlineOffset.x, 0.0)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        input.uv + float2(0.0, outlineOffset.y)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        input.uv - float2(0.0, outlineOffset.y)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        input.uv + outlineOffset).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        input.uv - outlineOffset).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        input.uv + float2(outlineOffset.x, -outlineOffset.y)).a);
                surroundingAlpha = max(surroundingAlpha,
                    SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,
                        input.uv + float2(-outlineOffset.x, outlineOffset.y)).a);

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
