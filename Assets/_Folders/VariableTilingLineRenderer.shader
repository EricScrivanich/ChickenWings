Shader "Custom/VariableTilingLineRenderer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _BaseTiling ("Base Tiling", Float) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float tilingMultiplier : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _BaseTiling;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                // The alpha channel contains our tiling multiplier from the gradient
                o.tilingMultiplier = v.color.a;
                
                // Apply variable tiling to U coordinate
                // The UV.x from LineRenderer goes 0->1 along the line
                o.uv = float2(v.uv.x * _BaseTiling * o.tilingMultiplier, v.uv.y);
                
                // Pass through color (RGB only, we used alpha for tiling data)
                o.color = float4(v.color.rgb, 1.0);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample texture with modified UVs
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Apply color tint
                col *= _Color * i.color;
                
                return col;
            }
            ENDCG
        }
    }
}