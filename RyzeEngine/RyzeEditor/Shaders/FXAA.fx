struct VS_IN
{
	float4 pos : POSITION;
	float2 tex : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float2 tex : TEXCOORD0;
	float2 screenSize : TEXCOORD1;
};

float2 screenSize;

Texture2D screenTexture;
SamplerState screenSampler;

float FxaaLuma(float4 rgba)
{
	rgba.w = dot(rgba.rgb, float3(0.299, 0.587, 0.114));
	return rgba.w;
}

PS_IN VS(VS_IN input)
{
	PS_IN output;

	output.pos = input.pos;
	output.tex = input.tex;
	output.screenSize = screenSize;

	return output;
}

float4 FxaaPixelShader(
	// Use noperspective interpolation here (turn off perspective interpolation).
	// {xy} = center of pixel
	float2 pos,
	//
	// Used only for FXAA Console, and not used on the 360 version.
	// Use noperspective interpolation here (turn off perspective interpolation).
	// {xy__} = upper left of pixel
	// {__zw} = lower right of pixel
	float4 fxaaConsolePosPos,
	//
	// Input color texture.
	// {rgb_} = color in linear or perceptual color space
	// if (FXAA_GREEN_AS_LUMA == 0)
	//     {___a} = luma in perceptual color space (not linear)
	sampler2D tex,
	//
	// Only used on FXAA Console.
	// This must be from a constant/uniform.
	// This effects sub-pixel AA quality and inversely sharpness.
	//   Where N ranges between,
	//     N = 0.50 (default)
	//     N = 0.33 (sharper)
	// {x___} = -N/screenWidthInPixels  
	// {_y__} = -N/screenHeightInPixels
	// {__z_} =  N/screenWidthInPixels  
	// {___w} =  N/screenHeightInPixels 
	float4 fxaaConsoleRcpFrameOpt,
	//
	// Only used on FXAA Console.
	// Not used on 360, but used on PS3 and PC.
	// This must be from a constant/uniform.
	// {x___} = -2.0/screenWidthInPixels  
	// {_y__} = -2.0/screenHeightInPixels
	// {__z_} =  2.0/screenWidthInPixels  
	// {___w} =  2.0/screenHeightInPixels 
	float4 fxaaConsoleRcpFrameOpt2,
	// 
	// Only used on FXAA Console.
	// This used to be the FXAA_CONSOLE__EDGE_SHARPNESS define.
	// It is here now to allow easier tuning.
	// This does not effect PS3, as this needs to be compiled in.
	//   Use FXAA_CONSOLE__PS3_EDGE_SHARPNESS for PS3.
	//   Due to the PS3 being ALU bound,
	//   there are only three safe values here: 2 and 4 and 8.
	//   These options use the shaders ability to a free *|/ by 2|4|8.
	// For all other platforms can be a non-power of two.
	//   8.0 is sharper (default!!!)
	//   4.0 is softer
	//   2.0 is really soft (good only for vector graphics inputs)
	float fxaaConsoleEdgeSharpness,
	//
	// Only used on FXAA Console.
	// This used to be the FXAA_CONSOLE__EDGE_THRESHOLD define.
	// It is here now to allow easier tuning.
	// This does not effect PS3, as this needs to be compiled in.
	//   Use FXAA_CONSOLE__PS3_EDGE_THRESHOLD for PS3.
	//   Due to the PS3 being ALU bound,
	//   there are only two safe values here: 1/4 and 1/8.
	//   These options use the shaders ability to a free *|/ by 2|4|8.
	// The console setting has a different mapping than the quality setting.
	// Other platforms can use other values.
	//   0.125 leaves less aliasing, but is softer (default!!!)
	//   0.25 leaves more aliasing, and is sharper
	float fxaaConsoleEdgeThreshold,
	//
	// Only used on FXAA Console.
	// This used to be the FXAA_CONSOLE__EDGE_THRESHOLD_MIN define.
	// It is here now to allow easier tuning.
	// Trims the algorithm from processing darks.
	// The console setting has a different mapping than the quality setting.
	// This only applies when FXAA_EARLY_EXIT is 1.
	// This does not apply to PS3, 
	// PS3 was simplified to avoid more shader instructions.
	//   0.06 - faster but more aliasing in darks
	//   0.05 - default
	//   0.04 - slower and less aliasing in darks
	// Special notes when using FXAA_GREEN_AS_LUMA,
	//   Likely want to set this to zero.
	//   As colors that are mostly not-green
	//   will appear very dark in the green channel!
	//   Tune by looking at mostly non-green content,
	//   then start at zero and increase until aliasing is a problem.
	float fxaaConsoleEdgeThresholdMin)
{
	float lumaNw = FxaaLuma(screenTexture.Sample(screenSampler, fxaaConsolePosPos.xy));
	float lumaSw = FxaaLuma(screenTexture.Sample(screenSampler, fxaaConsolePosPos.xw));
	float lumaNe = FxaaLuma(screenTexture.Sample(screenSampler, fxaaConsolePosPos.zy));
	float lumaSe = FxaaLuma(screenTexture.Sample(screenSampler, fxaaConsolePosPos.zw));
	float4 rgbyM = screenTexture.Sample(screenSampler, pos.xy);

	float lumaM = rgbyM.y;

	float lumaMaxNwSw = max(lumaNw, lumaSw);
	lumaNe += 1.0 / 384.0;
	float lumaMinNwSw = min(lumaNw, lumaSw);
	float lumaMaxNeSe = max(lumaNe, lumaSe);
	float lumaMinNeSe = min(lumaNe, lumaSe);
	float lumaMax = max(lumaMaxNeSe, lumaMaxNwSw);
	float lumaMin = min(lumaMinNeSe, lumaMinNwSw);
	float lumaMaxScaled = lumaMax * fxaaConsoleEdgeThreshold;
	float lumaMinM = min(lumaMin, lumaM);

	float lumaMaxScaledClamped = max(fxaaConsoleEdgeThresholdMin, lumaMaxScaled);
	float lumaMaxM = max(lumaMax, lumaM);

	float dirSwMinusNe = lumaSw - lumaNe;
	float lumaMaxSubMinM = lumaMaxM - lumaMinM;
	float dirSeMinusNw = lumaSe - lumaNw;

	if (lumaMaxSubMinM < lumaMaxScaledClamped) return rgbyM;

	float2 dir;
	dir.x = dirSwMinusNe + dirSeMinusNw;
	dir.y = dirSwMinusNe - dirSeMinusNw;
	float2 dir1 = normalize(dir.xy);

	float4 rgbyN1 = screenTexture.Sample(screenSampler, pos.xy - dir1 * fxaaConsoleRcpFrameOpt.zw);
	float4 rgbyP1 = screenTexture.Sample(screenSampler, pos.xy + dir1 * fxaaConsoleRcpFrameOpt.zw);

	float dirAbsMinTimesC = min(abs(dir1.x), abs(dir1.y)) * fxaaConsoleEdgeSharpness;

	float2 dir2 = clamp(dir1.xy / dirAbsMinTimesC, -2.0, 2.0);
	float4 rgbyN2 = screenTexture.Sample(screenSampler, pos.xy - dir2 * fxaaConsoleRcpFrameOpt2.zw);
	float4 rgbyP2 = screenTexture.Sample(screenSampler, pos.xy + dir2 * fxaaConsoleRcpFrameOpt2.zw);
	float4 rgbyA = rgbyN1 + rgbyP1;
	float4 rgbyB = ((rgbyN2 + rgbyP2) * 0.25) + (rgbyA * 0.25);

	bool twoTap = (rgbyB.y < lumaMin) || (rgbyB.y > lumaMax);

	if (twoTap) rgbyB.xyz = rgbyA.xyz * 0.5;

	return rgbyB;
}

float4 PS(PS_IN input) : SV_Target
{
	float SCREEN_WIDTH = input.screenSize.x;
	float SCREEN_HEIGHT = input.screenSize.y;
	float pixelWidth = (1 / SCREEN_WIDTH);
	float pixelHeight = (1 / SCREEN_HEIGHT);
	float2 tc = input.tex;

	float2 pixelCenter = float2(tc.x, tc.y);
	float4 fxaaConsolePosPos = float4(tc.x - pixelWidth, tc.y + pixelHeight, tc.x + pixelWidth, tc.y - pixelHeight);

	return FxaaPixelShader(
		pixelCenter,
		fxaaConsolePosPos,
		screenSampler,
		float4(-0.50 / SCREEN_WIDTH, -0.50 / SCREEN_HEIGHT, 0.50 / SCREEN_WIDTH, 0.50 / SCREEN_HEIGHT),
		float4(-2.0 / SCREEN_WIDTH, -2.0 / SCREEN_HEIGHT, 2.0 / SCREEN_WIDTH, 2.0 / SCREEN_HEIGHT),
		8.0,
		0.125,
		0.05);
}
