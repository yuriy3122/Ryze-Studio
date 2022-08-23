struct VS_IN
{
	float3 pos : POSITION;
	float3 norm : NORMAL;
	float3 tangent: TANGENT0;
	float3 bitangent: TANGENT1;
	float2 tex : TEXCOORD;
	row_major float4x4 mTransform : INSTANCE;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
    float3 norm : NORMAL;
    float2 tex : TEXTURE0;
    float4 color : COLOR0;
	float4 light : COLOR1;
};

float4x4 posProj;
float4x4 normProj;
float4x4 viewProj;
float4 diffuse;
float4 light;

Texture2D diffuseTexture;
SamplerState textureSampler;

PS_IN VS(VS_IN input)
{
	PS_IN output;

	float4 position = mul(float4(input.pos, 1.0f), posProj);
    float4 instancePosition = mul(position, input.mTransform);
	float3 norm = mul(input.norm, (float3x3)normProj);

	output.pos = mul(instancePosition, viewProj);
    output.norm = mul(norm, (float3x3)input.mTransform);
    output.tex = input.tex;
	output.color = diffuse;
	output.light = light;
	
	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	float len = length(input.color.xyz);
	float3 ambientColor = diffuseTexture.Sample(textureSampler, input.tex).xyz;

	if (len > 0)
	{
		ambientColor = input.color.xyz;
	}

    float3 lightColor = 1 - ambientColor;

    float3 N = normalize(input.norm.xyz);
	float3 L = normalize(input.light.xyz);
    float Kd = dot(N, L);

	float3 color = (0.6 * ambientColor + 0.4f * lightColor * Kd * Kd);

    return float4(color, 0.0);
}