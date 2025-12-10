Shader "Unlit/Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0.01, 0.3)) = 0.05
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
            float4 _OutlineColor;
            float _OutlineWidth;
            CBUFFER_END
            
            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.position = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }
            
            half4 frag(Varyings i) : SV_Target
            {
                float edgeSize = _OutlineWidth;
                
                // 检查四个边缘
                float leftEdge = step(i.uv.x, edgeSize);
                float rightEdge = step(1.0 - edgeSize, i.uv.x);
                float bottomEdge = step(i.uv.y, edgeSize);
                float topEdge = step(1.0 - edgeSize, i.uv.y);
                
                float isOutline = max(max(leftEdge, rightEdge), max(bottomEdge, topEdge));
                
                if (isOutline > 0.5)
                {
                    // 描边：黑色
                    return _OutlineColor;
                }
                else
                {
                    // 中间：完全透明
                    return half4(0, 0, 0, 0);
                }
            }
            ENDHLSL
        }
    }
}
