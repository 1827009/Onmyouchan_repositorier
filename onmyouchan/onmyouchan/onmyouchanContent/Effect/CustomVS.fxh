CustomVSOutput CustomVS(VSInputNmTxWeights vin)
{
    CustomVSOutput output;
    
    float4 pos_ws = mul(vin.Position, World);
    float3 eyeVector = normalize(EyePosition - pos_ws.xyz);
    float3 worldNormal = normalize(mul(vin.Normal, WorldInverseTranspose));

    ColorPair lightResult = ComputeLights(eyeVector, worldNormal, 3);
    
    output.PositionPS = mul(vin.Position, WorldViewProj);
    output.Diffuse = float4(lightResult.Diffuse, DiffuseColor.a);
    output.Specular = float4(lightResult.Specular, ComputeFogFactor(vin.Position));
    
    output.TexCoord = vin.TexCoord;

	output.Normal=worldNormal;
    
    return output;
}

CustomVSOutput CustomVSLight(VSInputNmTxWeights vin)
{
    CustomVSOutput output;
    
    float4 pos_ws = mul(vin.Position, World);
    float3 eyeVector = normalize(EyePosition - pos_ws.xyz);
    float3 worldNormal = normalize(mul(vin.Normal, WorldInverseTranspose));

    ColorPair lightResult = ComputeLights(eyeVector, worldNormal, 1);
    
    output.PositionPS = mul(vin.Position, WorldViewProj);
    output.Diffuse = float4(lightResult.Diffuse, DiffuseColor.a);
    output.Specular = float4(lightResult.Specular, ComputeFogFactor(vin.Position));
    
    output.TexCoord = vin.TexCoord;
    
	output.Normal=worldNormal;

    return output;
}

CustomVSOutputPixelLighting CustomVSPixelLighting(VSInputNmTxWeights vin)
{
    CustomVSOutputPixelLighting output;
    
    output.PositionPS = mul(vin.Position, WorldViewProj);
    output.PositionWS = float4(mul(vin.Position, World).xyz, ComputeFogFactor(vin.Position));
    output.NormalWS = normalize(mul(vin.Normal, WorldInverseTranspose));
    
    output.Diffuse = float4(1, 1, 1, DiffuseColor.a);
    output.TexCoord = vin.TexCoord;

    return output;
}

//*************************************************************************************************************
// アウトライン
//*************************************************************************************************************
float EdgeWidth=3;

CustomVSOutput OutLineCustomVS(VSInputNmTxWeights vin)
{
	float widthX=vin.Normal.x*EdgeWidth;
	float widthY=vin.Normal.y*EdgeWidth;
	float widthZ=vin.Normal.z*EdgeWidth;
	float4 width4=float4(widthX,widthY,widthZ,0);

    CustomVSOutput output;
	output.PositionPS = mul(vin.Position+width4, WorldViewProj);
	
    float4 pos_ws = mul(vin.Position, World);
    float3 eyeVector = normalize(EyePosition - pos_ws.xyz);
    float3 worldNormal = normalize(mul(vin.Normal, WorldInverseTranspose));
    ColorPair lightResult = ComputeLights(eyeVector, worldNormal, 3);    
    output.Diffuse = float4(lightResult.Diffuse, DiffuseColor.a);
    output.Specular = float4(lightResult.Specular, ComputeFogFactor(vin.Position));    
    output.TexCoord = vin.TexCoord;
	output.Normal=worldNormal;
    
    return output;
}

CustomVSOutput OutLineCustomVSLight(VSInputNmTxWeights vin)
{
	float widthX=vin.Normal.x*EdgeWidth;
	float widthY=vin.Normal.y*EdgeWidth;
	float widthZ=vin.Normal.z*EdgeWidth;
	float4 width4=float4(widthX,widthY,widthZ,0);

    CustomVSOutput output;
	output.PositionPS = mul(vin.Position+width4, WorldViewProj);
	
    float4 pos_ws = mul(vin.Position, World);
    float3 eyeVector = normalize(EyePosition - pos_ws.xyz);
    float3 worldNormal = normalize(mul(vin.Normal, WorldInverseTranspose));
    ColorPair lightResult = ComputeLights(eyeVector, worldNormal, 3);    
    output.Diffuse = float4(lightResult.Diffuse, DiffuseColor.a);
    output.Specular = float4(lightResult.Specular, ComputeFogFactor(vin.Position));    
    output.TexCoord = vin.TexCoord;
	output.Normal=worldNormal;
    
    return output;
}

CustomVSOutputPixelLighting OutLineCustomVSPixelLighting(VSInputNmTxWeights vin)
{
    CustomVSOutputPixelLighting output;
    
    output.PositionPS = mul(vin.Position, WorldViewProj);
    output.PositionWS = float4(mul(vin.Position, World).xyz, ComputeFogFactor(vin.Position));
    output.NormalWS = normalize(mul(vin.Normal, WorldInverseTranspose));
    
    output.Diffuse = float4(1, 1, 1, DiffuseColor.a);
    output.TexCoord = vin.TexCoord;

    return output;
}
