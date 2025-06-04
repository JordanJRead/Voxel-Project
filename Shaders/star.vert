#version 430 core

#define PI 3.1415926535897932384626433832795

layout(location = 0) in vec3 vPos;

uniform mat4 view;
uniform mat4 projection;
uniform float dayProgress;

layout(std430, binding = 0) readonly buffer matrixSSBO {
	mat4 models[];
};

vec4 rotateZAxis(vec4 input, float theta) {
	mat4 rotationMatrix;
	rotationMatrix[0] = vec4(cos(theta), sin(theta), 0, 0);
	rotationMatrix[1] = vec4(-sin(theta), cos(theta), 0, 0);
	rotationMatrix[2] = vec4(0, 0, 1, 0);
	rotationMatrix[3] = vec4(0, 0, 0, 1);

	return rotationMatrix * input;
}

void main() {
	vec4 worldPos = models[gl_InstanceID] * vec4(vPos, 1);
	gl_Position = projection * view * rotateZAxis(worldPos, dayProgress * 2 * PI);
}