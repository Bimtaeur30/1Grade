Shader "KTJ/UI/Spiral Transition"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Transition Color", Color) = (0, 0, 0, 1)
        _Progress ("Progress", Range(0, 1)) = 0
        _BladeCount ("Blade Count", Range(2, 20)) = 8
        _Twist ("Twist", Range(-5, 5)) = 1.5
        _Rotation ("Rotation", Range(-3, 3)) = 0.5
        _Softness ("Edge Softness", Range(0.001, 0.1)) = 0.01
        _RadialSoftness ("Radial Edge Softness", Range(0.001, 0.2)) = 0.04

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "SpiralTransition"

            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 2.0
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct Attributes
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            float _Progress;
            float _BladeCount;
            float _Twist;
            float _Rotation;
            float _Softness;
            float _RadialSoftness;
            float4 _ClipRect;

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.worldPosition = input.vertex;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = input.uv;
                output.color = input.color * _Color;
                return output;
            }

            fixed4 Fragment(Varyings input) : SV_Target
            {
                const float TWO_PI = 6.28318530718;
                float progress = saturate(_Progress);

                float2 centeredUV = input.uv - 0.5;
                float aspect = _ScreenParams.x / max(_ScreenParams.y, 1.0);
                centeredUV.x *= aspect;

                float maxRadius = 0.5 * sqrt(aspect * aspect + 1.0);
                float radius = length(centeredUV) / max(maxRadius, 0.0001);
                float angle = atan2(centeredUV.y, centeredUV.x);
                float angle01 = angle / TWO_PI + 0.5;

                float phase = frac(
                    angle01 * max(_BladeCount, 1.0) +
                    radius * _Twist +
                    progress * _Rotation);

                float mask = 1.0 - smoothstep(
                    progress - _Softness,
                    progress + _Softness,
                    phase);

                float radialFront = lerp(1.1, -0.1, progress);
                float radialMask = smoothstep(
                    radialFront - _RadialSoftness,
                    radialFront + _RadialSoftness,
                    radius);
                mask *= radialMask;

                mask = progress <= 0.0001 ? 0.0 : mask;
                mask = progress >= 0.9999 ? 1.0 : mask;

                fixed4 color = input.color;
                color.a *= mask;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(
                    input.worldPosition.xy,
                    _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
            ENDCG
        }
    }
}
