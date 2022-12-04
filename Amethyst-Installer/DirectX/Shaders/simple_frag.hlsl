struct PixelShaderInput
{
    float3 color : COLOR0;
};

float4 main(PixelShaderInput input) : SV_TARGET
{
    return float4(input.color,1.0f);
}