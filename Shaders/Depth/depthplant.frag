#version 430 core

#extension GL_ARB_bindless_texture : require

layout(std430, binding = 5) readonly buffer textureSSBO {
	sampler2D textures[];
};

in vec2 fragTexCoord;
in flat int instanceID;

void main() {
	if (texture(textures[instanceID], fragTexCoord).w < 0.5) {
		discard;
	}
}