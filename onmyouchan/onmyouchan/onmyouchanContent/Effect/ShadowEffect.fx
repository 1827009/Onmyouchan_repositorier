float4x4 ShadowMapWorld;
float4x4 ShadowMapView;
float4x4 ShadowMapProjection;

// TODO: ここでエフェクトのパラメーターを追加します。
//*******************************************************************
// シャドウマップ作製
//*******************************************************************
struct VertexShaderInput
{
    float4 Position : POSITION0;

    // TODO: ここにテクスチャー座標および頂点カラーなどの
    // 入力チャンネルを追加します。
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;

    // TODO: ここにカラーおよびテクスチャー座標などの頂点シェーダーの
    // 出力要素を追加します。これらの値は該当する三角形上で自動的に補間されて、
    // ピクセル シェーダーへの入力として提供されます。
	
	float DistanceCamera:TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, ShadowMapWorld);
    float4 viewPosition = mul(worldPosition, ShadowMapView);
    output.Position = mul(viewPosition, ShadowMapProjection);

    // TODO: ここで頂点シェーダー コードを追加します。
	output.DistanceCamera=output.Position.z;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: ここでピクセル シェーダー コードを追加します。

    return input.DistanceCamera;
}
//********************************************************************
// 影つけ
//********************************************************************
float4x4 World;
float4x4 View;
float4x4 Projection;

texture ShadowMap;
sampler ShadowMapSampler=sampler_state
{
	Texture=(ShadowMap);
};

float4x4 LightView;
float4x4 LightProjection;

struct ShadowMapInput
{
	float4 Position:POSITION0;
	float4 Color:COLOR0;
};
struct ShadowMapOutput
{
	float4 Position:POSITION0;
	float4 PositionOnShadowMap:TEXCOORD0;
	float4 Color:COLOR0;
};

ShadowMapOutput VertexShadowFunction(ShadowMapInput input)
{
    ShadowMapOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    
    output.Color = input.Color;
    
    output.PositionOnShadowMap = mul(
        worldPosition, 
        mul(LightView, LightProjection)
        );

    return output;
}

bool isLighted(float4 positionOnShadowMap)
{
    float2 texCoord;
    texCoord.x = (positionOnShadowMap.x / positionOnShadowMap.w + 1) / 2;
    texCoord.y = (-positionOnShadowMap.y / positionOnShadowMap.w + 1) / 2;
	
    //誤差があるはずなので、光が当たっているかどうかは
   //ほんの少しだけ甘く判定します。
   return positionOnShadowMap.z <= tex2D(ShadowMapSampler, texCoord).x + 0.001f;
}

float4 PixelShadowFunction(ShadowMapOutput input) : COLOR0
{
    if(isLighted(input.PositionOnShadowMap))
        return input.Color;
    else 
        return input.Color / 3;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: ここでレンダーステートを設定します。

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }

	pass Pass2
	{		
        VertexShader = compile vs_2_0 VertexShadowFunction();
        PixelShader = compile ps_2_0 PixelShadowFunction();
	}
}
