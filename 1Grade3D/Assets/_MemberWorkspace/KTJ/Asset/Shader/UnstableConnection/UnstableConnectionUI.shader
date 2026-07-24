Shader "KTJ/UI/Unstable Connection"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _GlitchStrength ("Glitch Strength", Range(0, 1)) = 0.45
        _Speed ("Speed", Range(0.1, 10)) = 2.5
        _DisplacementPixels ("Horizontal Tear (Pixels)", Range(0, 30)) = 8
        _RGBSplitPixels ("RGB Split (Pixels)", Range(0, 10)) = 1.5
        _NoiseAmount ("Noise Amount", Range(0, 0.5)) = 0.12
        _ScanlineAmount ("Scanline Amount", Range(0, 0.5)) = 0.1

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
            Name "UnstableConnection"

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
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float _GlitchStrength;
            float _Speed;
            float _DisplacementPixels;
            float _RGBSplitPixels;
            float _NoiseAmount;
            float _ScanlineAmount;

            float Hash(float value)
            {
                return frac(sin(value * 127.1) * 43758.5453);
            }

            float Hash2(float2 value)
            {
                return frac(
                    sin(dot(value, float2(12.9898, 78.233))) *
                    43758.5453);
            }

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
                float time = _Time.y * _Speed;
                float timeStep = floor(time * 14.0);
                float band = floor(input.uv.y * 80.0);
                float bandNoise = Hash(band + timeStep * 17.0);
                float burstNoise = Hash(floor(time * 3.5));

                float burst = smoothstep(
                    0.72 - _GlitchStrength * 0.35,
                    0.98,
                    burstNoise);
                float activeBand = step(
                    0.78 - _GlitchStrength * 0.25,
                    bandNoise);

                float direction = Hash(band * 3.7 + timeStep) * 2.0 - 1.0;
                float tearPixels =
                    direction * _DisplacementPixels * activeBand * burst;
                float2 tornUV = input.uv;
                tornUV.x += tearPixels * _MainTex_TexelSize.x;

                float splitPixels =
                    _RGBSplitPixels * _GlitchStrength * (0.3 + burst);
                float splitUV = splitPixels * _MainTex_TexelSize.x;

                fixed4 centerSample =
                    tex2D(_MainTex, tornUV) + _TextureSampleAdd;
                fixed4 leftSample =
                    tex2D(_MainTex, tornUV - float2(splitUV, 0.0)) +
                    _TextureSampleAdd;
                fixed4 rightSample =
                    tex2D(_MainTex, tornUV + float2(splitUV, 0.0)) +
                    _TextureSampleAdd;

                fixed4 color;
                color.r = rightSample.r;
                color.g = centerSample.g;
                color.b = leftSample.b;
                color.a = centerSample.a;
                color *= input.color;

                float grain = Hash2(
                    floor(input.uv * _ScreenParams.xy) +
                    float2(timeStep, timeStep * 0.37));
                float noise = (grain * 2.0 - 1.0) *
                    _NoiseAmount * _GlitchStrength;

                float scanline =
                    sin(input.uv.y * _ScreenParams.y * 1.35 + time * 8.0);
                scanline = scanline * 0.5 + 0.5;
                float scanlineDarkening =
                    1.0 - scanline * _ScanlineAmount * _GlitchStrength;

                float dropout = 1.0 -
                    activeBand * burst * step(0.88, bandNoise) *
                    (0.15 + 0.45 * Hash(timeStep + band));

                color.rgb =
                    saturate((color.rgb + noise) * scanlineDarkening * dropout);

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
