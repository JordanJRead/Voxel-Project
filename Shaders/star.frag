#version 430 core

out vec4 FragColor;

uniform float nightStrength;

void main() {
	FragColor = vec4(1, 1, 1, nightStrength * nightStrength * nightStrength);
}