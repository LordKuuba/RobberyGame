Shader "Unlit/UnlitTransparent"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Transparency ("Transparency", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100

        // Disable lighting and shadows
        Lighting Off
        ZWrite Off
        Cull Off

        Pass
        {
            // Blend SrcAlpha and OneMinusSrcAlpha to support transparency
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };
            
            float4 _Color;
            float _Transparency;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Apply transparency
                fixed4 color = i.color;
                color.a *= _Transparency;
                return color;
            }
            ENDCG
        }
    }
    FallBack Off
}