Shader "Custom/StencilMask"
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
                Comp Always    
                Pass Replace   
            }
            ColorMask 0        
        }
    }
}
