#version 430 core

in vec3 fragColor;

out vec4 Frag_Color;

void main() {
	Frag_Color = vec4(fragColor, 1);
}