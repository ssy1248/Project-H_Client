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
    float centerWidth = 0; // ������ �����Ǵ� ���� ũ�� (0.0 ~ 0.5 ���� ��)
    float gradientRange = 1.5; // �׶��̼� ���� (���� Ŭ���� õõ�� �����)

    float distanceFromCenter = abs(i.uv.x - 0.5);

    // ��� ���� ������ ���� ������ ����
    float alpha = (distanceFromCenter < centerWidth) ? 1.0 : (1.0 - (distanceFromCenter - centerWidth) * gradientRange);
    
    alpha = saturate(alpha); // 0~1 ���� ������ ����
    return fixed4(_Color.rgb, alpha);
}
            ENDCG
        }
    }
}
