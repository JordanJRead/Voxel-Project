#version 430 core

layout(location = 0) in vec2 vPos;
layout(location = 1) in vec2 vTexCoord;

out vec2 fragTexCoord;

uniform vec2 position;
uniform float scale;
uniform float aspectRatio;

void main() {
	fragTexCoord = vTexCoord;

	vec2 normPos = vec2(vPos.x * scale + position.x, vPos.y * scale * aspectRatio + position.y);
	gl_Position = vec4(normPos.x * 2 - 1, normPos.y * 2 - 1, 0, 1);
}