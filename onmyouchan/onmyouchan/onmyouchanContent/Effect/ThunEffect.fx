float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: ここでエフェクトのパラメーターを追加します。
texture MyTexture;
texture thunTexture;
sampler mySampler=sampler_state{
	Texture=<MyTexture>;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;

    // TODO: ここにテクスチャー座標および頂点カラーなどの
    // 入力チャンネルを追加します。

	float4 TextureCoordinate:TEXCOORD;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;

    // TODO: ここにカラーおよびテクスチャー座標などの頂点シェーダーの
    // 出力要素を追加します。これらの値は該当する三角形上で自動的に補間されて、
    // ピクセル シェーダーへの入力として提供されます。

	float4 TextureCoordinate:TEXCOORD;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    // TODO: ここで頂点シェーダー コードを追加します。
	output.TextureCoordinate=input.TextureCoordinate;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: ここでピクセル シェーダー コードを追加します。

    return float4(1, 0, 0, 1);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: ここでレンダーステートを設定します。

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
