Shader "Custom/Skybox_HorizonDip"

{
    Properties
    {
        [NoScaleOffset] _Tex ("Cubemap", Cube) = "grey" {}
        _Tint ("Tint Color", Color) = (0.5, 0.5, 0.5, 1)
        _Exposure ("Exposure", Range(0, 8)) = 1.0
        _HorizonDip ("Horizon Dip", Range(0, 1)) = 0.0
    }
    
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURECUBE(_Tex);
            SAMPLER(sampler_Tex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Tint;
                half _Exposure;
                float _HorizonDip;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.viewDir = input.positionOS.xyz;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float3 dir = normalize(input.viewDir);

                // Manipulera horisonten: Genom att addera ett värde till Y-axeln 
                // kollar vi "högre upp" i texturen, vilket resulterar i att hela 
                // skyboxen och dess horisont trycks nedåt visuellt.
                dir.y += _HorizonDip;
                
                // Åter-normalisera så vi inte förvränger samplingen i en oval
                dir = normalize(dir); 

                // Hämta färg från Cubemap
                half4 texColor = SAMPLE_TEXTURECUBE(_Tex, sampler_Tex, dir);
                
                // Applicera färg och exponering
                half3 finalColor = texColor.rgb * _Tint.rgb * _Exposure;

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}
