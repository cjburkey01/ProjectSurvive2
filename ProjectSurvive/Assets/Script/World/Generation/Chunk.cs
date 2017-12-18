using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

	public static readonly int SIZE = 16;

	private Pos pos;
	private VoxelInstance[,,] voxels;
	private MeshFilter filter;
	private World world;
	private bool empty = true;

	public void Create(Pos pos, World world) {
		this.pos = pos;
		this.world = world;
		transform.position = GetWorldPos(pos, world);
		voxels = new VoxelInstance[SIZE, SIZE, SIZE];
	}

	public Pos GetChunkPos() {
		return pos;
	}

	public void SetVoxel(Pos inPos, IVoxel voxel) {
		if (!InChunk(inPos)) {
			return;
		}
		if (empty && voxel != null) {
			empty = false;
		}
		voxels[inPos.x, inPos.y, inPos.z] = new VoxelInstance(voxel, this, inPos, new VoxelData());
	}

	public VoxelInstance GetVoxel(Pos pos) {
		if (!InChunk(pos)) {
			return null;
		}
		return voxels[pos.x, pos.y, pos.z];
	}

	public bool ShouldRender(Pos pos, Facing direction) {
		Pos lookAt = pos.Offset(direction);
		if (InChunk(lookAt)) {
			return GetVoxel(lookAt) == null;
		}
		Pair<Pos, Pos> at = GetPosInChunk(GetChunkInWorld(GetChunkPos()).Add(lookAt));
		if (world.GetChunk(at.val1) == null) {
			return false;
		}
		return world.GetChunk(at.val1).GetVoxel(at.val2) == null;
	}

	public bool InChunk(Pos pos) {
		return pos.x >= 0 && pos.y >= 0 && pos.z >= 0 && pos.x < SIZE && pos.y < SIZE && pos.z < SIZE;
	}

	public void Render() {
		if (IsEmptyRender()) {
			return;
		}

		if (filter == null) {
			filter = GetComponent<MeshFilter>();
			if (filter == null) {
				Debug.LogError("MeshFilter not found on chunk.");
				return;
			}
		}

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		for (int x = 0; x < SIZE; x++) {
			for (int y = 0; y < SIZE; y++) {
				for (int z = 0; z < SIZE; z++) {
					VoxelInstance at = GetVoxel(new Pos(x, y, z));
					if (at != null) {
						List<Vector3> tmpVerts = new List<Vector3>();
						List<int> tmpTris = new List<int>();
						List<Vector2> tmpUvs = new List<Vector2>();
						at.GetVoxel().OnRender(tmpVerts, tmpTris, tmpUvs, verts.Count, this, at);
						verts.AddRange(tmpVerts);
						tris.AddRange(tmpTris);
						uvs.AddRange(tmpUvs);
					}
				}
			}
		}

		Mesh mesh = new Mesh {
			name = "RenderedChunk " + pos,
			vertices = verts.ToArray(),
			triangles = tris.ToArray(),
			uv = uvs.ToArray()
		};
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		filter.sharedMesh = null;
		filter.mesh = mesh;
	}

	public bool IsFilled() {
		for (int x = 0; x < SIZE; x++) {
			for (int y = 0; y < SIZE; y++) {
				for (int z = 0; z < SIZE; z++) {
					if (GetVoxel(new Pos(x, y, z)) == null) {
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool IsEmptyRender() {
		if (empty) {
			return true;
		}
		if (IsFilled()) {
			Chunk north = world.GetChunk(pos.Offset(Facing.NORTH));
			Chunk south = world.GetChunk(pos.Offset(Facing.SOUTH));
			Chunk east = world.GetChunk(pos.Offset(Facing.EAST));
			Chunk west = world.GetChunk(pos.Offset(Facing.WEST));
			Chunk up = world.GetChunk(pos.Offset(Facing.UP));
			Chunk down = world.GetChunk(pos.Offset(Facing.DOWN));
			if (north != null && !north.IsFilled()) {
				return false;
			}
			if (south != null && !south.IsFilled()) {
				return false;
			}
			if (east != null && !east.IsFilled()) {
				return false;
			}
			if (west != null && !west.IsFilled()) {
				return false;
			}
			if (up != null && !up.IsFilled()) {
				return false;
			}
			if (down != null && !down.IsFilled()) {
				return false;
			}
			return true;
		}

		for (int x = 0; x < SIZE; x++) {
			for (int y = 0; y < SIZE; y++) {
				for (int z = 0; z < SIZE; z++) {
					if (GetVoxel(new Pos(x, y, z)) != null) {
						return false;
					}
				}
			}
		}
		return true;
	}

	// Returns Pair<ChunkPosition, PositionInChunk>
	public static Pair<Pos, Pos> GetPosInChunk(Pos worldPos) {
		Pos chunkPos = new Pos(Mathf.FloorToInt(worldPos.x / (float) SIZE), Mathf.FloorToInt(worldPos.y / (float) SIZE), Mathf.FloorToInt(worldPos.z / (float) SIZE));
		Pos inPos = new Pos(worldPos.x % SIZE, worldPos.y % SIZE, worldPos.z % SIZE);
		return new Pair<Pos, Pos>(chunkPos, inPos);
	}

	// Returns the position of the world point relative to the chunk position (can be negative).
	public static Pos GetRelativePos(Pos chunk, Pos world) {
		Pos chunkWorld = GetChunkInWorld(chunk);
		return world.Add(chunkWorld.Negate());
	}

	public static Pos GetChunkInWorld(Pos chunk) {
		return chunk.Scale(SIZE);
	}

	public static Vector3 GetWorldPos(Pos chunk, World world) {
		Pos inWorld = GetChunkInWorld(chunk);
		Vector3 outV = inWorld.GetVector();
		float w = (world.width * SIZE) / 2;
		float h = (world.height * SIZE) / 2;
		outV -= new Vector3(w, h, w);
		return outV;
	}

}