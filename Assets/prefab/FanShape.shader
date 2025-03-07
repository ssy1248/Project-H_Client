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

            // 중심각 60도 부채꼴을 그리기 위한 계산
            const float FAN_ANGLE = 60.0;  // 60도 부채꼴

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
                // 현재 프래그먼트가 부채꼴 범위 내에 있는지 확인
                // 부채꼴 중심을 0도에서 60도 사이로 설정

                float angle = atan2(i.normal.x, i.normal.z) * 180.0 / 3.14159;  // 수평 각도 계산

                // 부채꼴 범위: -30도 ~ 30도
                if (angle < -30.0 || angle > 30.0) {
                    discard;  // 부채꼴 범위 외의 부분은 버리기
                }

                // 부채꼴 안에 있는 경우 색상 반환
                return _DecalColor;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
