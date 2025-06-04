#version 430 core

layout(location = 0) in vec3 vPos;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec2 vTexCoord;

out vec2 fragTexCoord;
out flat int instanceID;
out vec3 fragNormal;
out vec3 plantNormal;
out vec3 sunNDCCoord;
out vec3 moonNDCCoord;

layout(std430, binding = 3) readonly buffer positionSSBO {
	float positions[];
};

layout(std430, binding = 4) readonly buffer growthSSBO {
	float growths[];
};

uniform mat4 view;
uniform mat4 projection;

uniform mat4 sunView;
uniform mat4 sunProjection;

uniform mat4 moonView;
uniform mat4 moonProjection;

uniform float time;

void main() {
	fragTexCoord = vTexCoord;
	instanceID = gl_InstanceID;

	vec3 scale = vec3(1, growths[gl_InstanceID], 1);

	vec3 position = vec3(positions[gl_InstanceID * 3 + 0], positions[gl_InstanceID * 3 + 1] - 0.5f * (1 - growths[gl_InstanceID]), positions[gl_InstanceID * 3 + 2]);
	vec4 worldPos = vec4(vPos * scale + position, 1);
	
	if (vPos.y > 0.25) {
		worldPos.x += 0.1 * sin(0.5 * time) * growths[gl_InstanceID];
	}

	gl_Position = projection * view * worldPos;

	vec4 sunTempPosition = sunProjection * sunView * worldPos;
	sunNDCCoord = sunTempPosition.xyz / sunTempPosition.w;

	vec4 moonTempPosition = moonProjection * moonView * worldPos;
	moonNDCCoord = moonTempPosition.xyz / moonTempPosition.w;

	fragNormal = normalize(vNormal);
	plantNormal = normalize(worldPos.xyz - (position - vec3(0, 1, 0)));
}
