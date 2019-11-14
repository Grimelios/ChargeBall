using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMetadataController : MonoBehaviour
{
	private const int LevelCount = 10;
	
	private static bool[] scenesLoaded = new bool[LevelCount];

	private Bounds colliderBounds;

	public CameraController cameraPrefab;

	public int sceneIndex;

	private void Awake()
	{
		colliderBounds = GetComponent<Collider2D>().bounds;
		scenesLoaded[sceneIndex] = true;
	}

	private void OnDestroy()
	{
		scenesLoaded[sceneIndex] = false;
	}

	private void Start()
	{
		// The camera doesn't belong in the scene metadata class, but I needed a way to easily instantiate a camera the
		// first time the player spawns into a scene. Since every scene is guaranteed to have a metadata object, it makes
		// some sense to do that here.
		if (FindObjectOfType<CameraController>() == null)
		{
			Instantiate(cameraPrefab);
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (!collider.gameObject.CompareTag("Player"))
		{
			return;
		}

		// Load the previous scene.
		if (sceneIndex > 0 && !scenesLoaded[sceneIndex - 1])
		{
			StartCoroutine(LoadScene(sceneIndex - 1));
		}

		// Load the next scene.
		if (sceneIndex < LevelCount - 1 && !scenesLoaded[sceneIndex + 1])
		{
			StartCoroutine(LoadScene(sceneIndex + 1));
		}
	}

	private void OnTriggerExit2D(Collider2D collider)
	{
		GameObject obj = collider.gameObject;

		if (!obj.CompareTag("Player"))
		{
			return;
		}

		// At first, touching a new trigger zone both loaded new scenes and unloaded old ones. During testing, however,
		// I noticed a semi-rare bug where touching a seam between triggers without fully exiting the current collider
		// could cause an adjacent scene to be unloaded, but not properly reloaded if the player immediately reversed
		// direction. As such, I changed my approach to load scenes on trigger enter, but only unload scenes on trigger
		// exit. Given the sequential side-by-side layout of the world, the appropriate stages to unload can be
		// determined by the player's position.
		bool exitRight = obj.transform.position.x > colliderBounds.center.x;

		if (exitRight)
		{
			// In practice, the player should only move sequentially forward or backwards through scene indices.
			// However, using a loop ensures that scenes will still unload properly if the player were to
			// hypothetically teleport or otherwise sequence break out-of-bounds to reach stages in an unintended
			// order.
			for (int i = 0; i < sceneIndex; i++)
			{
				if (scenesLoaded[i])
				{
					StartCoroutine(UnloadScene(i));
				}
			}

			return;
		}

		for (int i = sceneIndex + 1; i < LevelCount; i++)
		{
			if (scenesLoaded[i])
			{
				StartCoroutine(UnloadScene(i));
			}
		}
	}

	private IEnumerator LoadScene(int index)
	{
		Debug.Log("Loading scene " + index + ".");
		
		AsyncOperation async = SceneManager.LoadSceneAsync("Level" + index, LoadSceneMode.Additive);

		while (!async.isDone)
		{
			yield return null;
		}
	}

	private IEnumerator UnloadScene(int index)
	{
		Debug.Log("Unloading scene " + index + ".");

		AsyncOperation async = SceneManager.UnloadSceneAsync("Level" + index);

		while (!async.isDone)
		{
			yield return null;
		}
	}
}
