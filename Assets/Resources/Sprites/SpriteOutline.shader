Shader "Custom/SpriteOutlineURP"
{
    Properties
    {
        [MainTexture] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MainColor] _Color ("Color", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineWidth ("Outline Width", Range(0,10)) = 3
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "SpriteOutline"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)

                float4 _Color;
                float4 _OutlineColor;
                float _OutlineWidth;

            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                output.positionHCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv;
                output.color = input.color * _Color;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 offset =
                    _MainTex_TexelSize.xy * _OutlineWidth;

                half4 original =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv
                    );

                float alpha = original.a;

                float surroundingAlpha = 0;

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv + float2(offset.x, 0)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv + float2(-offset.x, 0)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv + float2(0, offset.y)
                    ).a
                );

                surroundingAlpha = max(
                    surroundingAlpha,
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv + float2(0, -offset.y)
                    ).a
                );

                // Original sprite
                if (alpha > 0.01)
                {
                    return original * input.color;
                }

                // Outline
                if (surroundingAlpha > 0.01)
                    {
                        return _OutlineColor * input.color;
                    }


                return half4(0, 0, 0, 0);
            }

            ENDHLSL
        }
    }
}
