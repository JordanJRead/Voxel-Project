#version 430 core

layout(location = 0) in vec3 vPos;

uniform mat4 view;
uniform mat4 projection;

layout(std430, binding = 0) readonly buffer matrixSSBO {
	mat4 models[];
};

void main() {
	gl_Position = projection * view * models[gl_InstanceID] * vec4(vPos, 1);
}