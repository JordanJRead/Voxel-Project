#version 430 core

#define PI 3.1415926535897932384626433832795

#extension GL_ARB_bindless_texture : require

in vec3 cubeMapCoord;
in flat int instanceID;
out vec4 FragColor;
in vec3 fragNormal;

uniform float dayProgress;
uniform bool isCursor;
uniform bool isCloud;

layout(std430, binding = 2) readonly buffer textureSSBO {
	samplerCube cubeMaps[];
};

void main() {
	if (isCloud) {
        float dayStrength = sin(dayProgress * 2 * PI) / 2.0 + 0.5;
		vec4 brightColor = vec4(0.8, 0.8, 0.8, 1);
		vec4 darkColor = vec4(brightColor.xyz * 0.3, 1);
		FragColor = darkColor + dayStrength * (brightColor - darkColor);
	}
	else {
		vec3 sunPosition = vec3(cos(2 * PI * dayProgress), sin(2 * PI * dayProgress), 0);
		sunPosition *= 10;

		sunPosition = normalize(sunPosition);
		vec3 moonPosition = -sunPosition;

		vec3 objectColor = texture(cubeMaps[instanceID], cubeMapCoord).xyz;

		float sunDiffuseFactor = (dot(sunPosition, fragNormal));
		float moonDiffuseFactor = (dot(moonPosition, fragNormal));

		sunDiffuseFactor = clamp(sunDiffuseFactor, 0.3, 1.0);
		moonDiffuseFactor = clamp(moonDiffuseFactor, 0.3, 1.0);

		vec3 colorFromSun = objectColor * sunDiffuseFactor;
		vec3 colorFromMoon = objectColor * moonDiffuseFactor * vec3(0.1, 0.1, 0.4);

		float alpha = isCursor ? 0.5f : 1;
		vec3 emission = isCursor ? vec3(0.2) : vec3(0);
		FragColor = vec4(colorFromSun + colorFromMoon + emission, alpha);
	}
}