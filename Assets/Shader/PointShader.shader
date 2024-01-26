// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MyDefaultPoint" {
    
    Subshader {
        Tags { "Queue"="Overlay" }
        LOD 100
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            //struct Point {
            //    float3 pos;
            //    half4 col;
            //};

            //float _PointSize = 5.0f;
            float4x4 _Transform;
            // StructuredBuffer<Point> _PointBuffer;
            StructuredBuffer<float3> _Positions;
            StructuredBuffer<uint> _Colors;
            StructuredBuffer<float3> _Center;

            struct v2f {
                float4 pos : SV_POSITION;
                half4 col : COLOR;
            };

            v2f vert(uint vid : SV_VertexID) {
                
                v2f o;

                float4 pos =  UnityObjectToClipPos(_Positions[vid]);
                uint icol = _Colors[vid];
                half4 col = half4(
                    ((icol >> 16) & 0xff) / 255.0f,
                    ((icol >>  8) & 0xff) / 255.0f,
                    ((icol      ) & 0xff) / 255.0f,
                1);   
                o.pos = pos;
                o.col = col;
                    
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                return i.col;
            }

            ENDCG

        }
    }
}