Shader "Custom/ProjectorShader"
{
    Properties
    {
        _ShadowTex ("Cookie", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            ZWrite Off
            ColorMask RGB
            Blend DstColor Zero

            SetTexture [_ShadowTex]
            {
                combine texture * primary
            }
        }
    }
}
