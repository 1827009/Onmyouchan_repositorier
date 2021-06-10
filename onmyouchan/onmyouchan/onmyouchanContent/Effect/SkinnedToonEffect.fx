struct CustomVSOutput
{
    float4 Diffuse    : COLOR0;
    float4 Specular   : COLOR1;
    float2 TexCoord   : TEXCOORD0;
    float4 PositionPS : SV_Position;

	float3 Normal:TEXCOORD1;
};

struct CustomVSOutputPixelLighting
{
    float2 TexCoord   : TEXCOORD0;
    float4 PositionWS : TEXCOORD1;
    float3 NormalWS   : TEXCOORD2;
    float4 Diffuse    : COLOR0;
    float4 PositionPS : SV_Position;
};

#include "SkinnedEffect.fxh"
#include "CustomVS.fxh"
#include "CustomPS.fxh"

// Vertex shader: vertex lighting, one bone.
CustomVSOutput CustomVL1(VSInputNmTxWeights vin)
{
    Skin(vin, 1);
    return CustomVS(vin);
}


// Vertex shader: vertex lighting, two bones.
CustomVSOutput CustomVL2(VSInputNmTxWeights vin)
{
    Skin(vin, 2);
    return CustomVS(vin);
}


// Vertex shader: vertex lighting, four bones.
CustomVSOutput CustomVL4(VSInputNmTxWeights vin)
{
    Skin(vin, 4);
    return CustomVS(vin);
}


// Vertex shader: one light, one bone.
CustomVSOutput CustomLight1(VSInputNmTxWeights vin)
{
    Skin(vin, 1);
    return CustomVSLight(vin);
}


// Vertex shader: one light, two bones.
CustomVSOutput CustomLight2(VSInputNmTxWeights vin)
{
    Skin(vin, 2);
    return CustomVSLight(vin);
}


// Vertex shader: one light, four bones.
CustomVSOutput CustomLight4(VSInputNmTxWeights vin)
{
    Skin(vin, 4);
    return CustomVSLight(vin);
}


// Vertex shader: pixel lighting, one bone.
CustomVSOutputPixelLighting CustomPixelLighting1(VSInputNmTxWeights vin)
{
    Skin(vin, 1);
    return CustomVSPixelLighting(vin);
}


// Vertex shader: pixel lighting, two bones.
CustomVSOutputPixelLighting CustomPixelLighting2(VSInputNmTxWeights vin)
{
    Skin(vin, 2);
    return CustomVSPixelLighting(vin);
}


// Vertex shader: pixel lighting, four bones.
CustomVSOutputPixelLighting CustomPixelLighting4(VSInputNmTxWeights vin)
{
    Skin(vin, 4);
    return CustomVSPixelLighting(vin);
}

VertexShader VSArray[9] =
{
    compile vs_2_0 CustomVL1(), //VSSkinnedVertexLightingOneBone(),
    compile vs_2_0 CustomVL2(),
    compile vs_2_0 CustomVL4(),

    compile vs_2_0 CustomLight1(),
    compile vs_2_0 CustomLight2(),
    compile vs_2_0 CustomLight4(),

    compile vs_2_0 CustomPixelLighting1(),
    compile vs_2_0 CustomPixelLighting2(),
    compile vs_2_0 CustomPixelLighting4(),
};


int VSIndices[18] =
{
    0,      // vertex lighting, one bone
    0,      // vertex lighting, one bone, no fog
    1,      // vertex lighting, two bones
    1,      // vertex lighting, two bones, no fog
    2,      // vertex lighting, four bones
    2,      // vertex lighting, four bones, no fog
    
    3,      // one light, one bone
    3,      // one light, one bone, no fog
    4,      // one light, two bones
    4,      // one light, two bones, no fog
    5,      // one light, four bones
    5,      // one light, four bones, no fog
    
    6,      // pixel lighting, one bone
    6,      // pixel lighting, one bone, no fog
    7,      // pixel lighting, two bones
    7,      // pixel lighting, two bones, no fog
    8,      // pixel lighting, four bones
    8,      // pixel lighting, four bones, no fog
};


PixelShader PSArray[3] =
{
    compile ps_2_0 CustomPS(),
    compile ps_2_0 CustomPSNoFog(),
    compile ps_2_0 CustomPSPixelLighting(),
};


int PSIndices[18] =
{
    0,      // vertex lighting, one bone
    1,      // vertex lighting, one bone, no fog
    0,      // vertex lighting, two bones
    1,      // vertex lighting, two bones, no fog
    0,      // vertex lighting, four bones
    1,      // vertex lighting, four bones, no fog
    
    0,      // one light, one bone
    1,      // one light, one bone, no fog
    0,      // one light, two bones
    1,      // one light, two bones, no fog
    0,      // one light, four bones
    1,      // one light, four bones, no fog
    
    2,      // pixel lighting, one bone
    2,      // pixel lighting, one bone, no fog
    2,      // pixel lighting, two bones
    2,      // pixel lighting, two bones, no fog
    2,      // pixel lighting, four bones
    2,      // pixel lighting, four bones, no fog
};

int ShaderIndex = 0;

//*************************************************************************************************************
// アウトライン
//*************************************************************************************************************

// Vertex shader: vertex lighting, one bone.
CustomVSOutput OutLineCustomVL1(VSInputNmTxWeights vin)
{
    Skin(vin, 1);
    return OutLineCustomVS(vin);
}


// Vertex shader: vertex lighting, two bones.
CustomVSOutput OutLineCustomVL2(VSInputNmTxWeights vin)
{
    Skin(vin, 2);
    return OutLineCustomVS(vin);
}


// Vertex shader: vertex lighting, four bones.
CustomVSOutput OutLineCustomVL4(VSInputNmTxWeights vin)
{
    Skin(vin, 4);
    return OutLineCustomVS(vin);
}


// Vertex shader: one light, one bone.
CustomVSOutput OutLineCustomLight1(VSInputNmTxWeights vin)
{
    Skin(vin, 1);
    return OutLineCustomVSLight(vin);
}


// Vertex shader: one light, two bones.
CustomVSOutput OutLineCustomLight2(VSInputNmTxWeights vin)
{
    Skin(vin, 2);
    return OutLineCustomVSLight(vin);
}


// Vertex shader: one light, four bones.
CustomVSOutput OutLineCustomLight4(VSInputNmTxWeights vin)
{
    Skin(vin, 4);
    return OutLineCustomVSLight(vin);
}


// Vertex shader: pixel lighting, one bone.
CustomVSOutputPixelLighting OutLineCustomPixelLighting1(VSInputNmTxWeights vin)
{
    Skin(vin, 1);
    return OutLineCustomVSPixelLighting(vin);
}


// Vertex shader: pixel lighting, two bones.
CustomVSOutputPixelLighting OutLineCustomPixelLighting2(VSInputNmTxWeights vin)
{
    Skin(vin, 2);
    return OutLineCustomVSPixelLighting(vin);
}


// Vertex shader: pixel lighting, four bones.
CustomVSOutputPixelLighting OutLineCustomPixelLighting4(VSInputNmTxWeights vin)
{
    Skin(vin, 4);
    return OutLineCustomVSPixelLighting(vin);
}
VertexShader OutLineVSArray[9] =
{
    compile vs_2_0 OutLineCustomVL1(), //VSSkinnedVertexLightingOneBone(),
    compile vs_2_0 OutLineCustomVL2(),
    compile vs_2_0 OutLineCustomVL4(),

    compile vs_2_0 OutLineCustomLight1(),
    compile vs_2_0 OutLineCustomLight2(),
    compile vs_2_0 OutLineCustomLight4(),

    compile vs_2_0 OutLineCustomPixelLighting1(),
    compile vs_2_0 OutLineCustomPixelLighting2(),
    compile vs_2_0 OutLineCustomPixelLighting4(),
};
PixelShader OutLinePSArray[3] =
{
    compile ps_2_0 OutLineCustomPS(),
    compile ps_2_0 OutLineCustomPSNoFog(),
    compile ps_2_0 OutLineCustomPSPixelLighting(),
};



Technique SkinnedEffect
{
    pass Pass1
    {
        VertexShader = (VSArray[VSIndices[ShaderIndex]]);
        PixelShader  = (PSArray[PSIndices[ShaderIndex]]);
    }
	pass Pass2
	{
        VertexShader = (OutLineVSArray[VSIndices[ShaderIndex]]);
        PixelShader  = (OutLinePSArray[PSIndices[ShaderIndex]]);
	}
}
