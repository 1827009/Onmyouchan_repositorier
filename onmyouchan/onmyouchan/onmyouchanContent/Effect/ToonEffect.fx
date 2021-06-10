float4x4 World;

// TODO: ここでエフェクトのパラメーターを追加します。
float4x4 WorldViewProjection;
float4 LigetPosition = float4(0.0,1000.0,0.0,0.0);

// シャドウマップ
float2 ShiftUV = float2(0.0,0.0);
texture MyTexture;
texture ToonTexture;

sampler mySampler=sampler_state{
	Texture=<MyTexture>;
	MagFilter=Linear;
	MinFilter=Linear;
};
sampler toonSampler=sampler_state{
	Texture=<ToonTexture>;
	MagFilter=Linear;
	MinFilter=Linear;	
};

//***********************************************************************************************
//影部分
//***********************************************************************************************
struct VertexShaderInput
{
    float4 Position : POSITION0;

    // TODO: ここにテクスチャー座標および頂点カラーなどの
    // 入力チャンネルを追加します。

	float4 TextureCoordinate:TEXCOORD0;
	float3 Normal:NORMAL;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;

    // TODO: ここにカラーおよびテクスチャー座標などの頂点シェーダーの
    // 出力要素を追加します。これらの値は該当する三角形上で自動的に補間されて、
    // ピクセル シェーダーへの入力として提供されます。

	float2 TextureCoord:TEXCOORD0;
	float3 Normal:TEXCOORD1;
	float3 LightVector:TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	
    // TODO: ここで頂点シェーダー コードを追加します。
    output.Position = mul(input.Position, WorldViewProjection);
	output.TextureCoord=input.TextureCoordinate + ShiftUV;

	output.Normal=mul(input.Normal,World);
	
	float4 worldPos=mul(input.Position,World);
	output.LightVector=LigetPosition.xyz-worldPos.xyz;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
    // TODO: ここでピクセル シェーダー コードを追加します。
	float3 N=normalize(input.Normal);
	float3 L=normalize(input.LightVector);

	float p=dot(N,-L)*0.5f+0.5f;
	p=p*p;

	float4 Col=tex2D(toonSampler,float2(0,p));

	float4 output=Col*tex2D(mySampler,input.TextureCoord);

    return output;
}


//***********************************************************************************************
//アウトライン
//***********************************************************************************************
float EdgeWidth=0.1;

struct VertexShaderOutlineInput
{
    float4 Position : POSITION0;

    // TODO: ここにテクスチャー座標および頂点カラーなどの
    // 入力チャンネルを追加します。

	float3 Normal:NORMAL;
};
struct VertexShaderOutlineOutput
{
    float4 Position : POSITION0;

    // TODO: ここにテクスチャー座標および頂点カラーなどの
    // 入力チャンネルを追加します。
};

VertexShaderOutlineOutput VertexShaderOutlineFunction(VertexShaderOutlineInput input)
{
    VertexShaderOutlineOutput output;
	
    // TODO: ここで頂点シェーダー コードを追加します。
	float widthX=input.Normal.x*EdgeWidth;
	float widthY=input.Normal.y*EdgeWidth;
	float widthZ=input.Normal.z*EdgeWidth;
	float4 width4=float4(widthX,widthY,widthZ,0);

    output.Position = mul(input.Position+width4, WorldViewProjection);

    return output;
}

float4 PixelShaderOutlineFunction(VertexShaderOutlineOutput input) : COLOR
{
    // TODO: ここでピクセル シェーダー コードを追加します。
    return float4(0,0,0,1);
}


technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderOutlineFunction();
		PixelShader = compile ps_2_0 PixelShaderOutlineFunction();
	}

    pass Pass2
    {
        // TODO: ここでレンダーステートを設定します。

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
