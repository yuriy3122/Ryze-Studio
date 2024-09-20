struct VS_IN
{
	float3 pos                    : POSITION;
	float3 norm                   : NORMAL;
	float3 tangent                : TANGENT0;
	float3 bitangent              : TANGENT1;
	float2 tex                    : TEXCOORD;
	row_major float4x4 mTransform : INSTANCE;
};

struct PS_IN
{
	float4 pos        : SV_POSITION;
    float3 norm       : NORMAL;
    float2 tex        : TEXTURE0;
    float4 shadowPosN : TEXCOORD1;
    float4 shadowPosF : TEXCOORD2;
    float3 viewPos    : TEXCOORD3;
    float4 color      : COLOR0;
	float4 light      : COLOR1;
};

float4x4 posProj;
float4x4 normProj;
float4x4 viewProj;
float4x4 orthoViewProjNear;
float4x4 orthoViewProjFar;
float4 diffuse;
float4 light;

Texture2D diffuseTexture    : register( t0 );
Texture2D shadowMapNear     : register( t1 );
Texture2D shadowMapFar      : register( t2 );
SamplerState textureSampler : register( s0 );

SamplerState depthSampler
{
    Filter = MIN_MAG_MIP_POINT;
};

PS_IN VS(VS_IN input)
{
	PS_IN output;

	float4 position = mul(float4(input.pos, 1.0f), posProj);
    float4 instancePosition = mul(position, input.mTransform);
	float3 norm = mul(input.norm, (float3x3)normProj);

	output.pos = mul(instancePosition, viewProj);
    output.viewPos = mul(instancePosition, viewProj).xyz;
    output.shadowPosN = mul(instancePosition, orthoViewProjNear);
    output.shadowPosF = mul(instancePosition, orthoViewProjFar);
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

	float3 color = saturate(2.0 * (0.6 * ambientColor + 0.4f * lightColor * Kd * Kd));
    
    if (input.light.a > 0.9f)
    {
        const float step = 50.0f;
        const float dx = 1.0f / 2048;
        const float bias = 0.0004f;
        const float2 offsets[9] =
        {
            float2(-dx, -dx), float2(0.0f, -dx), float2(dx, -dx),
            float2(-dx, 0.0f), float2(0.0f, 0.0f), float2(dx, 0.0f),
            float2(-dx, +dx), float2(0.0f, +dx), float2(dx, +dx)
        };
    
        float dist = length(input.viewPos);
        float4 shadowPos = (dist < step) ? input.shadowPosN : input.shadowPosF;
    
        float2 tc;
        tc.x = shadowPos.x * 0.5f + 0.5f;
        tc.y = shadowPos.y * -0.5f + 0.5f;
        float percentLit = 9.0f;
    
        [unroll]
        for (int i = 0; i < 9; ++i)
        {
            float2 tx = tc + offsets[i];
            float depth = (dist < step) ? shadowMapNear.Sample(depthSampler, tx).r : shadowMapFar.Sample(depthSampler, tx).r;
       
            if ((depth + bias) < shadowPos.z)
            {
                percentLit -= 0.3f;
            }
        }
    
        percentLit /= 9.0f;
    
        color *= percentLit;
    }

    return float4(color, 0.0);
}