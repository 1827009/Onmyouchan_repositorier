float4x4 View;
float4x4 Projection;

// TODO: ここでエフェクトのパラメーターを追加します。
texture MyTexture;
sampler mySampler=sampler_state{
	Texture=<MyTexture>;
};

struct VertexPositionTexture
{
    float4 Position : POSITION;

    // TODO: ここにテクスチャー座標および頂点カラーなどの
    // 入力チャンネルを追加します。

	float4 TextureCoordinate:TEXCOORD;
};

VertexPositionTexture MyVertexShader(VertexPositionTexture input)
{
	VertexPositionTexture output;
	output.Position=mul(input.Position,mul(View,Projection));
	output.TextureCoordinate=input.TextureCoordinate;
	return output;
}

float4 MyPixelShader(float2 textureCoordinate : TEXCOORD) : COLOR
{
	return tex2D(mySampler,textureCoordinate);
}

technique MyTechnique
{
    pass MyPass
    {
        // TODO: ここでレンダーステートを設定します。

        VertexShader = compile vs_2_0 MyVertexShader();
        PixelShader = compile ps_2_0 MyPixelShader();
    }
}
