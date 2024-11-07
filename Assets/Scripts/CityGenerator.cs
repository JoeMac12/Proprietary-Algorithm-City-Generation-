using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
	public int gridSize = 10;
	public float buildingSpace = 2f;

	void Start()
	{
		GenerateCity();
	}

	// Clear old gen and generate a new one
	public void GenerateCity()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		GenerateGrid();
	}

	// Generate city
	void GenerateGrid()
	{
		for (int x = 0; x < gridSize; x++)
		{
			for (int z = 0; z < gridSize; z++)
			{
				Vector3 position = new Vector3(x * buildingSpace, 0, z * buildingSpace);
				GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);

				// Random height
				float height = Random.Range(1f, 5f);
				building.transform.localScale = new Vector3(1, height, 1);

				// Adjust pos
				building.transform.position = new Vector3(position.x, height / 2, position.z);

				// Random color
				Color color = new Color(Random.value, Random.value, Random.value);
				building.GetComponent<Renderer>().material.color = color;

				building.transform.parent = this.transform;
			}
		}
	}
}
