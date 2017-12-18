using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class TextureHandler : MonoBehaviour {

	public static TextureHandler Instance {
		private set; get;
	}

	private readonly Dictionary<ResourceLocation, Rect> coords = new Dictionary<ResourceLocation, Rect>();
	private readonly Dictionary<ResourceLocation, FileInfo> files = new Dictionary<ResourceLocation, FileInfo>();

	private Texture2D textureMap;
	private bool loaded;

	public void Start() {
		Instance = this;
		LoadTextures();
	}

	public void LoadTextures() {
		loaded = false;
		coords.Clear();
		files.Clear();
		foreach (DirectoryInfo inDir in new DirectoryInfo("Assets/Resources/Texture").GetDirectories()) {
			GetInDir(inDir, inDir);
		}
		Dictionary<ResourceLocation, Texture2D> textures = new Dictionary<ResourceLocation, Texture2D>();
		foreach (KeyValuePair<ResourceLocation, FileInfo> pair in files) {
			string dir = "Texture/" + pair.Key.ToString().Replace(':', '/').Substring(0, pair.Key.ToString().Length - 4);
			Texture2D texture = Resources.Load(dir, typeof(Texture2D)) as Texture2D;
			if (texture == null) {
				Debug.LogError("Unable to load texture: " + pair.Key);
				continue;
			}
			textures.Add(pair.Key, texture);
		}
		textureMap = new Texture2D(16, 16);
		List<ResourceLocation> res = new List<ResourceLocation>();
		List<Texture2D> tex = new List<Texture2D>();
		foreach (KeyValuePair<ResourceLocation, Texture2D> pair in textures) {
			res.Add(pair.Key);
			tex.Add(pair.Value);
		}
		Rect[] packed = textureMap.PackTextures(tex.ToArray(), 0);
		if (packed == null) {
			Debug.LogError("Unable to pack textures to texture atlas");
			return;
		}
		for (int i = 0; i < packed.Length; i++) {
			coords.Add(res[i], packed[i]);
		}
		textureMap.filterMode = FilterMode.Point;
		textureMap.name = "MainTextureAtlas";
		textureMap.wrapMode = TextureWrapMode.Clamp;
		Debug.Log("Packed to texture map. Size: " + textureMap.width + "x" + textureMap.height);
		loaded = true;
	}

	public bool IsLoaded() {
		return loaded;
	}

	public Texture2D GetAtlas() {
		return textureMap;
	}

	public Rect GetTexture(ResourceLocation loc) {
		if (!HasTexture(loc)) {
			return default(Rect);
		}
		Rect outPos;
		if (!coords.TryGetValue(loc, out outPos)) {
			Debug.LogError("Failed to load texture: " + loc + " had " + outPos);
			return default(Rect);
		}
		return outPos;
	}

	public bool HasTexture(ResourceLocation loc) {
		return coords.ContainsKey(loc);
	}

	private void GetInDir(DirectoryInfo domain, DirectoryInfo dir) {
		foreach (DirectoryInfo inDir in dir.GetDirectories()) {
			GetInDir(domain, inDir);
		}
		foreach (FileInfo inDir in dir.GetFiles()) {
			if ((inDir.Attributes & FileAttributes.Directory) != FileAttributes.Directory && inDir.Name.ToLower().EndsWith(".png", System.StringComparison.Ordinal)) {
				AddTexture(domain, inDir);
			}
		}
	}

	private void AddTexture(DirectoryInfo parent, FileInfo file) {
		ResourceLocation loc = new ResourceLocation(parent.Name, BuildPath(file, parent));
		files.Add(loc, file);
		Debug.Log("Added texture: " + loc);
	}

	private string BuildPath(FileInfo file, DirectoryInfo main) {
		List<string> strings = new List<string> { file.Name };
		DirectoryInfo dir = file.Directory;
		while (!dir.FullName.Equals(main.FullName)) {
			strings.Add(dir.Name);
			dir = dir.Parent;
		}
		strings.Reverse();
		string path = "";
		for (int i = 0; i < strings.Count; i++) {
			path += strings[i];
			if (i < strings.Count - 1) {
				path += '/';
			}
		}
		return path;
	}

}