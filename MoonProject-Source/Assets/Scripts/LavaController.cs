using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : HazardController
{
	private Texture2D texture;
	private Mesh mesh;

	private void Awake()
	{
		texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		texture.SetPixel(0, 0, Color.red);

		Vector2 halfSize = GetComponent<BoxCollider2D>().size / 2;
		Vector3[] vertices =
		{
			new Vector3(-halfSize.x, -halfSize.y, 1),
			new Vector3(halfSize.x, -halfSize.y, 1), 
			new Vector3(-halfSize.x, halfSize.y, 1), 
			new Vector3(halfSize.x, halfSize.y, 1) 
		};

		Color[] colors = new Color[4];
		Vector2[] texCoords = new Vector2[4];
		Vector3[] normals = new Vector3[4];

		for (int i = 0; i < 4; i++)
		{
			colors[i] = Color.red;
			texCoords[i] = Vector2.zero;
			normals[i] = Vector3.back;
		}

		mesh = GetComponent<MeshFilter>().mesh;
		mesh.vertices = vertices;
		mesh.triangles = new [] { 0, 2, 1, 2, 3, 1 };
		mesh.colors = colors;
		mesh.uv = texCoords;

		GetComponent<MeshRenderer>().material.mainTexture = texture;
	}
	
	private void Update()
	{
	}
}
