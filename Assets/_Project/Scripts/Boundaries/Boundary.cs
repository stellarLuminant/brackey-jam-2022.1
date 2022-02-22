using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class EnemySpawn
{
	public GameObject prefab;
	public Vector3 spawnPosition;
	public GameObject spawnedPrefab;
}

public class Boundary : MonoBehaviour
{
	public static int HighCamPriority = 20;
	public static int LowCamPriority = 10;

	public CinemachineVirtualCamera VirtualCamera;
	public Collider2D Collider { get; private set; }

	[Header("Fade Ins and Outs")]
	public Tilemap boundaryTilemap;
	public float FadeTime = 0.65f;
	public Color fadeInColor = Color.white;
	Color fadeOutColor = Color.clear;
	bool firstTick = false;

	[Header("Enemies")]
	public EnemySpawn[] enemySpawns;

	Coroutine fadeCoroutine;

	// Start is called before the first frame update
	void Start()
	{
		Collider = GetComponent<Collider2D>();

		// Sets it to whatever it can find, if not set in the inspector
		if (!VirtualCamera)
		{
			VirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
			var confiner = VirtualCamera.GetComponent<CinemachineConfiner>();
			confiner.m_BoundingShape2D = Collider;
		}

		CheckEnemySpawns();

		if (boundaryTilemap)
		{
			boundaryTilemap.gameObject.SetActive(true);
			//fadeInColor = boundaryTilemap.color;
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void CheckEnemySpawns()
	{
		foreach (var item in enemySpawns)
		{
			// If it's been destroyed, create a new one at the same position
			if (!item.spawnedPrefab)
			{
				item.spawnedPrefab = Instantiate(item.prefab, item.spawnPosition, item.prefab.transform.rotation);
			}
		}
	}

	/// <summary>
	/// Updates every frame for being in this boundary.
	/// </summary>
	public void UpdateShow()
	{
		// Set that to the highest priority
		if (VirtualCamera.Priority != HighCamPriority ||
			// Intented to trigger on first "tick"/update
			// (if needed) on the camera, so it properly hides/shows
			(!firstTick && VirtualCamera.Priority == HighCamPriority))
		{
			firstTick = true;

			// Cinemachine Transistions
			VirtualCamera.Priority = HighCamPriority;

			// Fade logic
			if (fadeCoroutine != null) StopCoroutine(fadeCoroutine); // else Debug.Log("routine is null");
			fadeCoroutine = StartCoroutine(FadeTilemapColor(fadeInColor));
		}
	}

	/// <summary>
	/// Updates every frame for not being in this boundary.
	/// </summary>
	public void UpdateHide()
	{
		// Set that to the lower priority
		if (VirtualCamera.Priority != LowCamPriority ||
			// Intented to trigger on first "tick"/update
			// (if needed) on the camera, so it properly hides/shows
			(!firstTick && VirtualCamera.Priority == LowCamPriority))
		{
			firstTick = true;

			CheckEnemySpawns();

			// Cinemachine Transistions
			VirtualCamera.Priority = LowCamPriority;

			// Fade logic
			if (fadeCoroutine != null) StopCoroutine(fadeCoroutine); // else Debug.Log("routine is null");
			fadeCoroutine = StartCoroutine(FadeTilemapColor(fadeOutColor));
		}
	}

	IEnumerator FadeTilemapColor(Color newColor)
	{
		if (boundaryTilemap)
		{
			Color currentColor = boundaryTilemap.color;

			float counter = 0;
			while (counter < FadeTime)
			{
				counter += Time.deltaTime;
				boundaryTilemap.color = Color.Lerp(currentColor, newColor, counter / FadeTime);
				yield return null;
			}
		}
	}
}
