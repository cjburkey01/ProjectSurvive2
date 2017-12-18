using System.Collections.Generic;

public class VoxelInstance {

	private readonly IVoxel voxel;
	private readonly VoxelData data;
	private readonly Pair<Pos, Pos> position;

	public VoxelInstance(IVoxel voxel, Chunk chunk, Pos inChunk, VoxelData data) {
		this.voxel = voxel;
		this.data = data;
		position = new Pair<Pos, Pos>(chunk.GetChunkPos(), inChunk);
	}

	public IVoxel GetVoxel() {
		return voxel;
	}

	public VoxelData GetData() {
		return data;
	}

	public Pos GetChunk() {
		return position.val1;
	}

	public Pos GetPosInChunk() {
		return position.val2;
	}

	public Pos GetWorldPos() {
		return Chunk.GetChunkInWorld(GetChunk()).Add(GetPosInChunk());
	}

}

public class VoxelData {

	private readonly Dictionary<string, object> data = new Dictionary<string, object>();

	public void Set(string key, object value) {
		if (!data.ContainsKey(key)) {
			data.Add(key, value);
			return;
		}
		data[key] = value;
	}

	public T Get<T>(string key) {
		object val = Get(key);
		if (val == null || !(val is T)) {
			return default(T);
		}
		return (T) val;
	}

	public object Get(string key) {
		object val;
		if (data.TryGetValue(key, out val)) {
			return val;
		}
		return null;
	}

}