Shader "Unlit/StencilLight"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Stencil
            {
                Ref 1              
                Comp NotEqual      
                Pass Keep          
            }

            ColorMask RGB
            ZWrite On
            ZTest LEqual
            Lighting On

            Material
            {
                Diffuse [_Color]
            }
        }
    }
}