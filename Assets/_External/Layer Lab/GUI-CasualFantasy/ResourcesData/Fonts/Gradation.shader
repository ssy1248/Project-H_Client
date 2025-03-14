Shader "Custom/UIGradient"
{
    Properties
    {
        _Color ("Main Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

fixed4 frag(v2f i) : SV_Target
{
    float centerWidth = 0; // 검은색 유지되는 영역 크기 (0.0 ~ 0.5 사이 값)
    float gradientRange = 1.5; // 그라데이션 범위 (값이 클수록 천천히 사라짐)

    float distanceFromCenter = abs(i.uv.x - 0.5);

    // 가운데 일정 영역은 완전 검은색 유지
    float alpha = (distanceFromCenter < centerWidth) ? 1.0 : (1.0 - (distanceFromCenter - centerWidth) * gradientRange);
    
    alpha = saturate(alpha); // 0~1 사이 값으로 제한
    return fixed4(_Color.rgb, alpha);
}
            ENDCG
        }
    }
}
