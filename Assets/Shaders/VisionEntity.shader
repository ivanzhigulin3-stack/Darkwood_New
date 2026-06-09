Shader "Test/VisionEntity"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Name "Sprite Unlit"
            Tags { "LightMode" = "Universal2D" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            
            // Параметры от источников света
            float3 _PlayerPosition;
            
            // Ближний свет (круг)
            float3 _CloseLightPosition;
            float _CloseLightRadius;
            float _CloseLightIntensity;
            
            // Конус света
            float3 _ConeLightPosition;
            float3 _ConeLightDirection;
            float _ConeLightLength;
            float _ConeLightAngle;
            float _ConeLightIntensity;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color * _Color;
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                return OUT;
            }

            // Проверка попадания в круг света
            bool IsInCircle(float3 worldPoint, float3 lightPos, float radius)
            {
                float dist = distance(worldPoint, lightPos);
                return dist <= radius;
            }
            
            // Проверка попадания в конус света
            bool IsInCone(float3 worldPoint, float3 coneOrigin, float3 coneDirection, float coneLength, float coneAngle)
            {
                float3 toPoint = worldPoint - coneOrigin;
                float distance = length(toPoint);
                
                if (distance > coneLength) return false;
                
                float3 directionToPoint = normalize(toPoint);
                float dotProduct = dot(coneDirection, directionToPoint);
                float angleRad = acos(dotProduct);
                float angleDeg = degrees(angleRad);
                float halfConeAngle = coneAngle * 0.5;
    
                return angleDeg <= halfConeAngle;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 1. Получаем базовый цвет спрайта
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;
                
                bool isVisible = false;
                if (_CloseLightIntensity > 0 && IsInCircle(IN.worldPos, _CloseLightPosition, _CloseLightRadius))
                {
                    isVisible = true;
                }
                if (_ConeLightIntensity > 0 && IsInCone(IN.worldPos, _ConeLightPosition, _ConeLightDirection, _ConeLightLength, _ConeLightAngle))
                {
                    isVisible = true;
                }
                if (!isVisible)
                {
                    discard; // Быстрое отсечение пикселя, объект исчезает
                }
               
                return color;
            }
            ENDHLSL
        }
    }
}