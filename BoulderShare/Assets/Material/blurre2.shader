Shader "Custom/Frost"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)

        _MainTex("Diffuse", 2D) = "white" {}
        _Noise("Noise", 2D) = "black" {}

        _Range("Range", Float) = 0.025
        // _Blur("Blur", Float) = 0.005 // Blurは廃止、サンプリング位置ずらしはテクセルサイズに基づいた形に変更
        _Sigma("Sigma", Range(0.01, 8.0)) = 1.0 // σを追加
    }

    SubShader
    {
        // スカイボックスも含めてグラブするため、キューをTransparentに変更
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

        Cull Off

        GrabPass{ "_Frost" }

        CGINCLUDE
        #include "UnityCG.cginc"

        half4 _Color;

        sampler2D _MainTex;
        float4 _MainTex_ST;

        sampler2D _Frost;

        // グラブテクスチャのテクセルサイズを追加
        float4 _Frost_TexelSize;

        sampler2D _Noise;
        float4 _Noise_ST;

        half _Range;
        // half _Blur; // _Blurは廃止
        float _Sigma; // _Sigmaを追加

        // 重み計算用関数
        // ご質問者さんの重み関数 1/(2*π*σ)*exp(-(x^2+y^2)/(2*σ^2)) と同様ですが、係数は1に変更しました
        // 係数を付けて正規化した重みで有限の範囲をたたみ込むと、重みの総和が1より小さくなってしまうため
        // できあがったぼかし画像は、もとの画像より暗くなってしまうと思われます
        // そこで重み関数には係数を付けず、たたみ込みの際に重みの総和を求めて、最後にたたみ込み結果を
        // 重みの総和で割ることで明るさが維持されるようにしました
        inline float getWeight(float2 xy)
        {
            return exp(-dot(xy, xy) / (2.0 * _Sigma * _Sigma));
        }

        // カーネルサイズ計算用関数
        // たたみ込み範囲の片側幅...カーネルの一辺の長さ2*n+1のnをいくつにするかですが、さしあたり
        // 中心から最も遠い点(カーネルの角)における重みが十分小さくなる(0.0001を切る)大きさにしました
        // σ=1でn=4となり、サンプリング回数は(2*4+1)^2=81回になります(多分...)
        // σ=2で225回、σ=4で729回、σ=8で2601回...といった具合に、2乗のオーダーでサンプリング回数が
        // 増大しますので、あんまりσを大きくしすぎるのは控えた方がいいでしょう
        // ぼかしを縦方向と横方向の二段階に分けることで、サンプリング回数の増大を1乗のオーダーに
        // 抑えるテクニックもありますので、負荷を軽減したい場合は採用を検討してみてもいいでしょう
        inline int getKernelN()
        {
            return (int)ceil(_Sigma * sqrt(-log(0.0001)));
        }

        ENDCG

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            struct v2f
            {
                float4 pos : SV_POSITION;

                float3 uv : TEXCOORD;
                float4 screenPos : TEXCOORD1;
                float3 ray : TEXCOORD2;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.screenPos = ComputeGrabScreenPos(o.pos);
                o.ray = UnityObjectToViewPos(v.vertex).xyz * float3(-1, -1, 1);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // rayは使用していないようですが、一応残しておきました
                i.ray = i.ray * (_ProjectionParams.z / i.ray.z);

                float2 uv = i.screenPos.xy / i.screenPos.w;

                float2 frostUV = tex2D(_Noise, i.uv * _Noise_ST.xy + _Noise_ST.zw).xy;

                frostUV -= 0.5;
                frostUV *= _Range;
                frostUV += uv;

                // 霜のついたガラスを表現するためサンプリング位置にノイズを加えているようですが、
                // もし純粋にぼかし効果だけをかけたい場合は、uvをそのまま使うといいでしょう
                // frostUV = uv;

                int kernelN = getKernelN();
                float weightSum = 0.0;
                float4 frost = 0.0;

                // 注目しているピクセルを中心に、-kernelN ～ +kernelNの範囲をたたみ込む
                for (int m = -kernelN; m <= kernelN; m++)
                {
                    for (int n = -kernelN; n <= kernelN; n++)
                    {
                        float2 texelOffset = float2(n, m);
                        float weight = getWeight(texelOffset);
                        weightSum += weight;
                        frost += weight * tex2D(_Frost, frostUV + texelOffset * _Frost_TexelSize.xy);
                    }
                }

                // 最後に、重みの総和で割る
                frost /= weightSum;

                half4 diffuse = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);

                half alpha = _Color.a * diffuse.a;

                return half4(frost.xyz + (diffuse.rgb * _Color.rgb * alpha), 1);
            }

            ENDCG
        }
    }

    Fallback Off
}