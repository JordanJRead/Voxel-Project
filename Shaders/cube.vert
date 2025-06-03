#version 430 core
layout(location = 0) in vec3 vPos;
layout(location = 1) in vec3 vNormal;

out vec3 cubeMapCoord;
out flat int instanceID;
out vec3 fragNormal;
out vec3 sunClipPosition;

layout(std430, binding = 0) readonly buffer positionSSBO {
	float positions[];
};

layout(std430, binding = 1) readonly buffer scaleSSBO {
	float scales[];
};

uniform mat4 view;
uniform mat4 projection;

uniform mat4 sunView;
uniform mat4 sunProjection;

void main() {
	fragNormal = normalize(vNormal);
	cubeMapCoord = vPos;
	instanceID = gl_InstanceID;

	vec3 position = vec3(positions[gl_InstanceID * 3 + 0], positions[gl_InstanceID * 3 + 1], positions[gl_InstanceID * 3 + 2]);
	vec3 scale = vec3(scales[gl_InstanceID * 3 + 0], scales[gl_InstanceID * 3 + 1], scales[gl_InstanceID * 3 + 2]);
	vec4 worldPos = vec4(vPos * scale + position, 1);

	gl_Position = projection * view * worldPos;
	vec4 sunTempPosition = sunProjection * sunView * worldPos;
	sunClipPosition = sunTempPosition.xyz / sunTempPosition.w;
}