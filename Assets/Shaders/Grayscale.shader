Shader "Custom/Grayscale"
{
    // Properties er parametre, som kan ændres via Unitys Inspector.
    Properties
    {
        // _MainTex: Teksturen, der skal påføres materialet (standard er "white").
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        // Tags definerer renderingsegenskaber, fx "Opaque" til uigennemsigtige materialer.
        Tags { "RenderType"="Opaque" }
        Pass
        {
            // Start af GPU-programmet.
            CGPROGRAM
            #pragma vertex vert // Vertex-shader funktion.
            #pragma fragment frag // Fragment-shader funktion.
            #include "UnityCG.cginc" // Indeholder Unitys fælles funktioner og makroer.

            sampler2D _MainTex; // Tekstur, der bruges som input i fragment-shaderen.

            // Input-struktur til vertex-shaderen.
            struct appdata_t
            {
                float4 vertex : POSITION; // Vertexens position i objektets lokale koordinater.
                float2 texcoord : TEXCOORD0; // Teksturkoordinater for hver vertex.
            };

            // Output-struktur fra vertex-shaderen til fragment-shaderen.
            struct v2f
            {
                float2 texcoord : TEXCOORD0; // Sender teksturkoordinater videre.
                float4 vertex : SV_POSITION; // Skærmposition efter transformation.
            };

            // Vertex-shader: Transformerer vertex-positionen og videresender teksturkoordinater.
            v2f vert (appdata_t v)
            {
                v2f o;
                // Konverterer lokal objektposition til skærmposition (clip space).
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Overfører teksturkoordinater direkte.
                o.texcoord = v.texcoord;
                return o;
            }

            // Fragment-shader: Beregner pixelens farve baseret på teksturen.
            fixed4 frag (v2f i) : SV_Target
            {
                // Henter farven fra teksturen baseret på teksturkoordinater.
                fixed4 color = tex2D(_MainTex, i.texcoord);

                // Konverterer farven til gråtoner vha. luminansværdier for RGB.
                // (0.3, 0.59, 0.11) er vægte, der efterligner det menneskelige øjes følsomhed.
                float grayscale = dot(color.rgb, float3(0.3, 0.59, 0.11)); 

                // Returnerer en ny farve, hvor R, G og B er ens (gråtonet farve),
                // men alpha-værdien (gennemsigtighed) forbliver uændret.
                return fixed4(grayscale, grayscale, grayscale, color.a);
            }
            ENDCG // Slut på GPU-programmet.
        }
    }
}