using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour
{
	public int gridSize = 20;
	public float buildingSpacing = 4f;
	public int roadWidth = 1;
	public float roadHeight = 0.05f;
	public List<GameObject> buildingPrefabs;
	public GameObject roadPrefab;

	public float generationTime = 0.01f;

	public float waveSpeed = 5f;
	public float waveHeight = 5f;
	public float waveDuration = 0.5f;

	private HashSet<int> roadXPositions;
	private HashSet<int> roadZPositions;
	public int minRoads = 2;
	public int maxRoads = 4;

	private bool isGenerating = false;

	void Start()
	{
		GenerateCity();
	}

	public void GenerateCity()
	{
		if (isGenerating)
		{
			StopAllCoroutines();
			isGenerating = false;
		}

		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}

		GenerateRoads();
		StartCoroutine(GenerateOverTime());
	}

	void GenerateRoads()
	{
		roadXPositions = new HashSet<int>();
		roadZPositions = new HashSet<int>();

		int numberOfXRoads = Random.Range(minRoads, maxRoads + 1);
		int numberOfZRoads = Random.Range(minRoads, maxRoads + 1);

		while (roadXPositions.Count < numberOfXRoads)
		{
			int xPos = Random.Range(0, gridSize);
			roadXPositions.Add(xPos);
		}

		while (roadZPositions.Count < numberOfZRoads)
		{
			int zPos = Random.Range(0, gridSize);
			roadZPositions.Add(zPos);
		}
	}

	IEnumerator GenerateOverTime()
	{
		isGenerating = true;
		Vector2 gridCenter = new Vector2(gridSize / 2f, gridSize / 2f);

		for (int x = 0; x < gridSize; x++)
		{
			for (int z = 0; z < gridSize; z++)
			{
				Vector3 position = new Vector3(x * buildingSpacing, 0, z * buildingSpacing);

				if (IsRoad(x, z))
				{
					if (roadPrefab != null)
					{
						GameObject road = Instantiate(roadPrefab, position, Quaternion.identity, this.transform);
						road.transform.localScale = new Vector3(buildingSpacing, roadHeight, buildingSpacing);
					}
					yield return new WaitForSeconds(generationTime);
					continue;
				}

				if (buildingPrefabs != null && buildingPrefabs.Count > 0)
				{
					GameObject buildingPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Count)];
					GameObject building = Instantiate(buildingPrefab, position, Quaternion.identity, this.transform);

					float randomHeight = Random.Range(1f, 3f);
					building.transform.localScale = new Vector3(
						building.transform.localScale.x,
						building.transform.localScale.y * randomHeight,
						building.transform.localScale.z
					);

					float distanceFromCenter = Vector2.Distance(
						new Vector2(x, z),
						gridCenter
					);

					StartCoroutine(Wave(building, position, distanceFromCenter));

					float yRotation = Random.Range(0, 4) * 90f;
					building.transform.Rotate(0, yRotation, 0);
				}
				else
				{
					Debug.LogWarning("no building dude");
				}

				yield return new WaitForSeconds(generationTime);
			}
		}

		isGenerating = false;
	}

	IEnumerator Wave(GameObject building, Vector3 finalPosition, float distanceFromCenter)
	{
		Renderer renderer = building.GetComponentInChildren<Renderer>();
		if (renderer == null) yield break;

		Vector3 buildingSize = renderer.bounds.size;
		float finalY = buildingSize.y / 2;

		building.transform.position = new Vector3(finalPosition.x, -buildingSize.y, finalPosition.z);

		float delay = distanceFromCenter / waveSpeed;
		yield return new WaitForSeconds(delay);

		float elapsedTime = 0f;
		Vector3 startPos = building.transform.position;
		Vector3 endPos = new Vector3(finalPosition.x, finalY, finalPosition.z);

		while (elapsedTime < waveDuration)
		{
			elapsedTime += Time.deltaTime;
			float progress = elapsedTime / waveDuration;

			float waveOffset = Mathf.Sin(progress * Mathf.PI) * waveHeight;
			float currentY = Mathf.Lerp(startPos.y, endPos.y, progress) + waveOffset;

			building.transform.position = new Vector3(
				finalPosition.x,
				currentY,
				finalPosition.z
			);

			yield return null;
		}

		building.transform.position = endPos;
	}

	bool IsRoad(int x, int z)
	{
		for (int i = 0; i < roadWidth; i++)
		{
			if (roadXPositions.Contains(x - i))
			{
				return true;
			}
		}

		for (int i = 0; i < roadWidth; i++)
		{
			if (roadZPositions.Contains(z - i))
			{
				return true;
			}
		}

		return false;
	}
}
