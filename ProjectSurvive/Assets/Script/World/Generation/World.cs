using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NoiseTest;

public class World : MonoBehaviour {

	public static OpenSimplexNoise noise;

	public int width = 4;
	public int height = 16;
	public GameObject chunkPrefab;

	public long seed = long.MaxValue;
	public float scale = 25.0f;
	public float amplitude = 5.0f;

	public GameObject loadingScreen;
	public Text loadingProgressText;
	public Image IMG;
	public Material chunkMaterial;

	private Chunk[,,] world;
	private readonly Stopwatch stopwatch = new Stopwatch();

	void Start() {
		world = new Chunk[width, height, width];
		if (noise == null) {
			if (seed == long.MaxValue) {
				noise = new OpenSimplexNoise();
			} else {
				noise = new OpenSimplexNoise(seed);
			}
		}
		StartCoroutine(GenerateWorld());
	}

	public IEnumerator GenerateWorld() {
		while (TextureHandler.Instance == null || !TextureHandler.Instance.IsLoaded()) {
			print("Waiting for texture handler to load...");
			yield return null;
		}

		IMG.sprite = Sprite.Create(TextureHandler.Instance.GetAtlas(), new Rect(0, 0, TextureHandler.Instance.GetAtlas().width, TextureHandler.Instance.GetAtlas().height), Vector2.zero);

		chunkMaterial.mainTexture = TextureHandler.Instance.GetAtlas();
		print("Set chunk material texture.");

		loadingScreen.SetActive(true);
		loadingProgressText.text = "Please wait...";
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
						loadingProgressText.text = "Initializing: " + (i / max * 100.0f).ToString("00.00") + "%";
						yield return null;
					}
				}
			}
		}

		loadingProgressText.text = "Initialized terrain.";
		yield return null;
		stopwatch.Reset();
		stopwatch.Start();
		i = 0;
		max = width * width * Chunk.SIZE * Chunk.SIZE;
		for (int x = 0; x < width * Chunk.SIZE; x++) {
			for (int z = 0; z < width * Chunk.SIZE; z++) {
				i++;
				int y = Mathf.FloorToInt(GetNoise(x, z));
				SetVoxel(new Pos(x, y, z), Voxels.Grass);
				for (int j = y - 1; j >= y - 4; j--) {
					SetVoxel(new Pos(x, j, z), Voxels.Dirt);
				}
				for (int j = y - 4; j >= 0; j--) {
					SetVoxel(new Pos(x, j, z), Voxels.Stone);
				}
				if (stopwatch.ElapsedMilliseconds >= 500) {
					stopwatch.Reset();
					stopwatch.Start();
					loadingProgressText.text = "Generating: " + (i / max * 100.0f).ToString("00.00") + "%";
					yield return null;
				}
			}
		}

		loadingProgressText.text = "Generated terrain.";
		yield return null;
		stopwatch.Reset();
		stopwatch.Start();
		i = 0;
		max = width * width * height;
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

	public void SetVoxel(Pos pos, IVoxel voxel) {
		Pair<Pos, Pos> chunkPos = Chunk.GetPosInChunk(pos);
		Chunk c = GetChunk(chunkPos.val1);
		if (c == null) {
			return;
		}
		c.SetVoxel(chunkPos.val2, voxel);
	}

	private float GetNoise(int x, int z) {
		double outD = noise.Evaluate(x / scale, z / scale) * amplitude;
		outD += noise.Evaluate(x / scale / 2.0f, z / scale / 2.0f) * amplitude / 2.0f;
		outD += noise.Evaluate(x / scale * 2.0f, z / scale * 2.0f) * amplitude / 5.0f;
		outD += noise.Evaluate(x / scale / 7.0f, z / scale / 7.0f) * amplitude * 4.0f;
		return (float) outD + (height * Chunk.SIZE / 2);
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