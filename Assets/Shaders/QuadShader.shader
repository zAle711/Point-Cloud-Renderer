// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/QuadShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off  
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
         
            #include "UnityCG.cginc"
 

            StructuredBuffer<float3> _Positions;
            StructuredBuffer<float3> _Normals;
            StructuredBuffer<uint> _Colors;
 
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                half4 color : COLOR;
            };
 
            v2f vert (uint vid : SV_VertexID)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(_Positions[vid]);
                uint icol = _Colors[vid];
                half4 col = half4(
                    ((icol >> 16) & 0xff) / 255.0f,
                    ((icol >>  8) & 0xff) / 255.0f,
                    ((icol      ) & 0xff) / 255.0f,
                1);  
                o.normal = _Normals[vid];
                o.color = col;
                return o;
            }
         
            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}