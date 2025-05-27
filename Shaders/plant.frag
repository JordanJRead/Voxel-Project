#version 430 core

#extension GL_ARB_bindless_texture : require

in vec2 fragTexCoord;
in flat int instanceID;
in vec3 fragNormal;
out vec4 FragColor;

layout(std430, binding = 5) readonly buffer textureSSBO {
	sampler2D textures[];
};

void main() {
	vec3 normal = fragNormal;
	if (gl_FrontFacing) {
		normal *= -1;
	}
	vec3 lightDir = normalize(vec3(0.4, -1, 0.4)); // Angled down
	vec4 objectColor = texture(textures[instanceID], fragTexCoord);
	float diffuseFactor = (dot(-lightDir, normal) + 1) * 0.5;

	FragColor = vec4(objectColor.xyz * diffuseFactor, objectColor.w);
}