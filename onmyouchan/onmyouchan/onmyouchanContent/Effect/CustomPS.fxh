// Pixel shader: vertex lighting.

texture MyTexture;
sampler mySampler=sampler_state{
	Texture=<MyTexture>;
	MagFilter=Linear;
	MinFilter=Linear;
};

texture ToonTexture;
sampler toonSampler=sampler_state{
	Texture=<ToonTexture>;
	MagFilter=Linear;
	MinFilter=Linear;	
};

float4 CustomPS(CustomVSOutput pin) : SV_Target0
{
    float4 color = SAMPLE_TEXTURE(Texture, pin.TexCoord) * pin.Diffuse;
    
    AddSpecular(color, pin.Specular.rgb);
    ApplyFog(color, pin.Specular.w);
    
    // Custom: Apply basic toon shading
    color = round(color * 5) / 5;
    
    return color;
}


// Pixel shader: vertex lighting, no fog.
float4 CustomPSNoFog(CustomVSOutput pin) : SV_Target0
{
    //float4 color = SAMPLE_TEXTURE(Texture, pin.TexCoord) * pin.Diffuse;
    
    //AddSpecular(color, pin.Specular.rgb);
    
    // Custom: Apply basic toon shading
	float p=dot(pin.Normal,DirLight0Direction)*0.5f+0.5f;
	p=p*p;

	float4 col=tex2D(toonSampler,float2(0,p));
	
	float4 color=col*tex2D(mySampler,pin.TexCoord);
   
    return color;
}


// Pixel shader: pixel lighting.
float4 CustomPSPixelLighting(CustomVSOutputPixelLighting pin) : SV_Target0
{
    float4 color = SAMPLE_TEXTURE(Texture, pin.TexCoord) * pin.Diffuse;
    
    float3 eyeVector = normalize(EyePosition - pin.PositionWS.xyz);
    float3 worldNormal = normalize(pin.NormalWS);
    
    ColorPair lightResult = ComputeLights(eyeVector, worldNormal, 3);
    
    color.rgb *= lightResult.Diffuse;

    AddSpecular(color, lightResult.Specular);
    ApplyFog(color, pin.PositionWS.w);
    
    // Custom: Apply basic toon shading
    color = round(color * 5) / 5;
    
    return color;
}

//*************************************************************************************************************
// アウトライン
//*************************************************************************************************************
// Pixel shader: vertex lighting.
float4 OutLineCustomPS(CustomVSOutput pin) : SV_Target0
{    
    return float4(0,0,0,0);
}


// Pixel shader: vertex lighting, no fog.
float4 OutLineCustomPSNoFog(CustomVSOutput pin) : SV_Target0
{
    return float4(0,0,0,0);
}


// Pixel shader: pixel lighting.
float4 OutLineCustomPSPixelLighting(CustomVSOutputPixelLighting pin) : SV_Target0
{
    return float4(0,0,0,0);
}