struct VS_IN
{
	float3 pos : POSITION;
	float3 color : COLOR;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
    float4 color : COLOR;
};

float4x4 worldViewProj;

PS_IN VS(VS_IN input)
{
	PS_IN output;
	
    float4 position = mul(float4(input.pos, 1.0f), worldViewProj);

	output.pos = position;
	output.color = float4(input.color, 0.0f);
	
	return output;
}

float4 PS(PS_IN input) : SV_Target
{
    return input.color;
}