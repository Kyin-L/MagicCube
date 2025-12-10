// 遮罩物体Shader
Shader "URP/StencilMask"
{
    Properties
    {
        _MaskID("Mask ID", Int) = 1
    }
    
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-100"  // 提前渲染
            "ForceNoShadowCasting" = "True"
        }
        
        Pass
        {
            ColorMask 0  // 不写入颜色
            ZWrite Off   // 不写入深度
            
            Stencil
            {
                Ref [_MaskID]
                Comp Always
                Pass Replace
            }
        }
    }
}