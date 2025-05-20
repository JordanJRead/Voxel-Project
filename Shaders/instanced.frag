#version 430 core

#extension GL_ARB_bindless_texture : require

in vec3 cubeMapCoord;
in flat int instanceID;
out vec4 FragColor;
in vec3 fragNormal;

uniform bool isCursor;

layout(std430, binding = 2) readonly buffer textureSSBO {
	samplerCube cubeMaps[];
};

void main() {
	vec3 lightDir = normalize(vec3(0.4, -1, 0.4)); // Angled down
	vec3 objectColor = texture(cubeMaps[instanceID], cubeMapCoord).xyz;
	float diffuseFactor = (dot(-lightDir, fragNormal) + 1) * 0.5;

	float alpha = isCursor ? 0.5f : 1;
	vec3 emission = isCursor ? vec3(0.2) : vec3(0);
	FragColor = vec4(objectColor * diffuseFactor + emission, alpha);
}