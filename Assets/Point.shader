Shader "Unlit/Point"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                // 중심으로부터 일정한 거리 radius까지는 1의 밝기로, 그 이후부터는 점차 어두어지는 식으로 디자인

                float2 centerOffset = (i.uv.xy - 0.5) * 2;
                float sqrDst = dot(centerOffset, centerOffset);
                float dst = sqrt(sqrDst);

                float alpha = 1 - smoothstep(1-0.01, 1.01, dst);
                
                return float4(_Color.rgb, alpha * _Color.a);
            }
            ENDCG
        }
    }
}
