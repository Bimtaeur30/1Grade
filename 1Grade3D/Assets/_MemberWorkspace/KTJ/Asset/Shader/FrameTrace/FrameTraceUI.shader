Shader "KTJ/UI/Frame Trace"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Line Color", Color) = (0.2, 1, 0.45, 1)
        _Progress ("Progress", Range(0, 1)) = 0
        _Thickness ("Thickness (Pixels)", Range(1, 100)) = 12
        _EdgeSoftness ("Edge Softness (Pixels)", Range(0.1, 10)) = 1
        _HeadSoftness ("Head Softness", Range(0.0001, 0.05)) = 0.004

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
            Name "FrameTrace"

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
            float _Thickness;
            float _EdgeSoftness;
            float _HeadSoftness;
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
                float width = max(_ScreenParams.x, 1.0);
                float height = max(_ScreenParams.y, 1.0);

                float distanceLeft = input.uv.x * width;
                float distanceRight = (1.0 - input.uv.x) * width;
                float distanceBottom = input.uv.y * height;
                float distanceTop = (1.0 - input.uv.y) * height;

                float edgeDistance = min(
                    min(distanceLeft, distanceRight),
                    min(distanceBottom, distanceTop));

                float perimeter = 2.0 * (width + height);
                float pathDistance;

                if (distanceTop <= distanceRight &&
                    distanceTop <= distanceBottom &&
                    distanceTop <= distanceLeft)
                {
                    // Top-left to top-right.
                    pathDistance = input.uv.x * width;
                }
                else if (distanceRight <= distanceBottom &&
                         distanceRight <= distanceLeft)
                {
                    // Top-right to bottom-right.
                    pathDistance = width + (1.0 - input.uv.y) * height;
                }
                else if (distanceBottom <= distanceLeft)
                {
                    // Bottom-right to bottom-left.
                    pathDistance = width + height +
                        (1.0 - input.uv.x) * width;
                }
                else
                {
                    // Bottom-left to top-left.
                    pathDistance = width * 2.0 + height +
                        input.uv.y * height;
                }

                float pathProgress = pathDistance / perimeter;
                float borderMask = 1.0 - smoothstep(
                    _Thickness - _EdgeSoftness,
                    _Thickness + _EdgeSoftness,
                    edgeDistance);
                float progressMask = smoothstep(
                    pathProgress - _HeadSoftness,
                    pathProgress,
                    saturate(_Progress));

                fixed4 color = input.color;
                color.a *= borderMask * progressMask;

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
