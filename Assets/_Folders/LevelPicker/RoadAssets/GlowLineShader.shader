Shader "Custom/GlowLineShader"
{
    Properties
    {
        _MainTex ("Glow Texture", 2D) = "white" {}
        _Tint ("Tint Color", Color) = (1,1,1,1)

        _ClipStart ("Clip Start", Range(0,1)) = 0
        _ClipEnd ("Clip End", Range(0,1)) = 1

        _Offset ("UV Offset", Float) = 0

        _Softness ("Softness", Range(0,0.25)) = 0.05
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Tint;

            float _ClipStart;
            float _ClipEnd;
            float _Offset;
            float _Softness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;   // distance along line (0–1)
            };

            v2f vert(appdata v, uint id : SV_VertexID)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = float2(0, 0); // Filled by UV2 in mesh
                return o;
            }

            float smoothMask(float t, float start, float end, float softness)
            {
                // Soft fade on both sides
                float edge0 = start;
                float edge1 = start + softness;

                float edge2 = end - softness;
                float edge3 = end;

                float fadeIn = smoothstep(edge0, edge1, t);
                float fadeOut = 1 - smoothstep(edge2, edge3, t);

                return saturate(fadeIn * fadeOut);
            }

            float4 frag(v2f i) : SV_Target
            {
                // Compute distance along the line
                float t = i.uv2.x;

                // Soft clipping mask
                float mask = smoothMask(t, _ClipStart, _ClipEnd, _Softness);

                // If fully out of range → invisible
                if (mask <= 0.0001)
                    discard;

                // Repeating UV scrolling
                float2 uv = i.uv;
                uv.x += _Offset;

                float4 col = tex2D(_MainTex, uv) * _Tint;
                col.a *= mask;  // apply fade mask

                return col;
            }
            ENDCG
        }
    }
}