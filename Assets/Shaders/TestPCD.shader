Shader "Unlit/TestPCD"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            StructuredBuffer<uint> _Colors;

            float inverseLerp(float min, float max, float y) {
                return (y - min) / (max - min);
            }

            half4 lerpColor(half4 start, half4 end, float t) {
                return lerp(start, end, t);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v, uint vid : SV_VertexID)
            {
                float min = 0.0;  // Valore minimo
                float max = 1.5;  // Valore massimo


                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                float y = v.vertex.y;
                float interpolated = inverseLerp(min, max, y);

                half4 start = half4(0.0, 0.0, 1.0, 1.0);  // Colore di partenza (rosso)
                half4 end = half4(0.0, 1.0, 0.0, 1.0);    // Colore di arrivo (blu)
                half4 c = lerpColor(start, end, interpolated);
                // o.color = c;

                uint icol = _Colors[vid];
                half4 col = half4(
                    ((icol >> 16) & 0xff) / 255.0f,
                    ((icol >>  8) & 0xff) / 255.0f,
                    ((icol      ) & 0xff) / 255.0f,
                1);  

                o.color = col;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return i.color;
            }
            ENDCG
        }
    }
}
