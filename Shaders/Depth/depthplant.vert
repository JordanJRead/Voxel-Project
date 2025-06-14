#version 430 core

layout(location = 0) in vec3 vPos;
layout(location = 2) in vec2 vTexCoord;

out vec2 fragTexCoord;
out flat int instanceID;

layout(std430, binding = 3) readonly buffer positionSSBO {
	float positions[];
};

layout(std430, binding = 4) readonly buffer growthSSBO {
	float growths[];
};

uniform mat4 view;
uniform mat4 projection;
uniform float time;

void main() {
	vec3 scale = vec3(1, growths[gl_InstanceID], 1);

	vec3 position = vec3(positions[gl_InstanceID * 3 + 0], positions[gl_InstanceID * 3 + 1] - 0.5f * (1 - growths[gl_InstanceID]), positions[gl_InstanceID * 3 + 2]);
	vec4 worldPos = vec4(vPos * scale + position, 1);
	
	if (vPos.y > 0.25) {
		worldPos.z += 0.1 * sin(0.5 * time) * growths[gl_InstanceID];
	}

	gl_Position = projection * view * worldPos;

	fragTexCoord = vTexCoord;
	instanceID = gl_InstanceID;
}
