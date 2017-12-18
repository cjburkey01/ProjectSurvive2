using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {

	public int width = 4;
	public int height = 16;
	public GameObject chunkPrefab;

	public long seed = long.MaxValue;
	public float scale = 25.0f;
	public float amplitude = 5.0f;
	public float cutoff = 0.35f;

	public GameObject loadingScreen;
	public Text loadingProgressText;

	private Chunk[,,] world;
	private readonly Stopwatch stopwatch = new Stopwatch();


	void Start() {
		world = new Chunk[width, height, width];
		StartCoroutine(GenerateWorld());
	}

	public IEnumerator GenerateWorld() {
		loadingScreen.SetActive(true);
		stopwatch.Start();
		float max = width * width * height;
		int i = 0;
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				for (int z = 0; z < width; z++) {
					i++;
					GenerateChunk(new Pos(x, y, z));
					if (stopwatch.ElapsedMilliseconds >= 500) {
						stopwatch.Reset();
						stopwatch.Start();
						loadingProgressText.text = "Generating: " + (i / max * 100.0f).ToString("00.00") + "%";
						yield return null;
					}
				}
			}
		}
		loadingProgressText.text = "Generated terrain.";
		yield return null;
		stopwatch.Reset();
		stopwatch.Start();
		i = 0;
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				for (int z = 0; z < width; z++) {
					i++;
					GetChunk(new Pos(x, y, z)).Render();
					if (stopwatch.ElapsedMilliseconds >= 500) {
						stopwatch.Reset();
						stopwatch.Start();
						loadingProgressText.text = "Rendering: " + (i / max * 100.0f).ToString("00.00") + "%";
						yield return null;
					}
				}
			}
		}
		loadingScreen.SetActive(false);
	}

	private void GenerateChunk(Pos pos) {
		GameObject inst = Instantiate(chunkPrefab, Vector3.zero, Quaternion.identity);
		inst.name = "Chunk " + pos;
		inst.transform.parent = transform;
		Chunk chunk = inst.GetComponent<Chunk>();
		chunk.Create(pos, this);
		world[pos.x, pos.y, pos.z] = chunk;
	}

	public Chunk GetChunk(Pos pos) {
		if (!InWorld(pos)) {
			return null;
		}
		return world[pos.x, pos.y, pos.z];
	}

	public bool InWorld(Pos pos) {
		return pos.x >= 0 && pos.y >= 0 && pos.z >= 0 && pos.x < width && pos.y < height && pos.z < width;
	}

}