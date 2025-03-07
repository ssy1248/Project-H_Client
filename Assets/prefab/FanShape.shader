Shader "Custom/FanShape60Degree"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DecalColor ("Decal Color", Color) = (1, 1, 1, 1)
        _FanRadius ("Fan Radius", Float) = 5.0
    }

    SubShader
    {
        Tags { "RenderType"="Decal" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float3 normal : TEXCOORD0;
                float4 color : COLOR;
            };

            uniform float _FanRadius;
            uniform float4 _DecalColor;

            // �߽ɰ� 60�� ��ä���� �׸��� ���� ���
            const float FAN_ANGLE = 60.0;  // 60�� ��ä��

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = _DecalColor;
                o.normal = v.normal;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // ���� �����׸�Ʈ�� ��ä�� ���� ���� �ִ��� Ȯ��
                // ��ä�� �߽��� 0������ 60�� ���̷� ����

                float angle = atan2(i.normal.x, i.normal.z) * 180.0 / 3.14159;  // ���� ���� ���

                // ��ä�� ����: -30�� ~ 30��
                if (angle < -30.0 || angle > 30.0) {
                    discard;  // ��ä�� ���� ���� �κ��� ������
                }

                // ��ä�� �ȿ� �ִ� ��� ���� ��ȯ
                return _DecalColor;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
