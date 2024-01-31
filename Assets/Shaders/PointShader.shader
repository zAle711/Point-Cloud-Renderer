//// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Shader "Custom/MyDefaultPoint" {
    
//    Subshader {
//        Tags { "Queue"="Overlay" }
//        LOD 100
//        Pass {
//            CGPROGRAM

//            #pragma vertex vert
//            #pragma fragment frag

//            #include "UnityCG.cginc"

//            //struct Point {
//            //    float3 pos;
//            //    half4 col;
//            //};

//            //float _PointSize = 5.0f;
//            float4x4 _Transform;
//            // StructuredBuffer<Point> _PointBuffer;
//            StructuredBuffer<float3> _Positions;
//            StructuredBuffer<uint> _Colors;
//            StructuredBuffer<float3> _Center;

//            struct v2f {
//                float4 pos : SV_POSITION;
//                half4 col : COLOR;
//            };

//            v2f vert(uint vid : SV_VertexID) {
                
//                v2f o;

//                float4 pos =  UnityObjectToClipPos(_Positions[vid]);
//                uint icol = _Colors[vid];
//                half4 col = half4(
//                    ((icol >> 16) & 0xff) / 255.0f,
//                    ((icol >>  8) & 0xff) / 255.0f,
//                    ((icol      ) & 0xff) / 255.0f,
//                1);   
//                o.pos = pos;
//                o.col = col;
                    
//                return o;
//            }

//            half4 frag(v2f i) : SV_Target {
//                return i.col;
//            }

//            ENDCG

//        }
//    }
//}

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PointShader" {
    Properties
   {
       _LeftEyeColor("Left Eye Color", COLOR) = (0,1,0,1)
       _RightEyeColor("Right Eye Color", COLOR) = (1,0,0,1)
   }
    Subshader {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 100
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            float4 _LeftEyeColor;
            float4 _RightEyeColor; 

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

            struct v2f {
                float4 pos : SV_POSITION;
                half4 col : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID 
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(uint vid : SV_VertexID) {
                
                v2f o;
                //UNITY_SETUP_INSTANCE_ID(vid);
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

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

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex); //Insert

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); //Insert
    
                fixed4 col = i.col; //Inser
    
                // invert the colors
    
                //col = 1 - col;
    
                return col;
            }
            ENDCG

        }
    }
}