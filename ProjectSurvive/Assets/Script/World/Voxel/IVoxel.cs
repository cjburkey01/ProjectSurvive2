using System.Collections.Generic;
using UnityEngine;

// This probably shouldn't be implemented elsewhere.
public interface IVoxel {

	// Data
	ResourceLocation GetResourceLocation();
	float GetMiningDifficulty(VoxelInstance voxel);

	// Rendering
	void OnRender(List<Vector3> verts, List<int> tris, List<Vector2> uvs, int i, Chunk chunk, VoxelInstance voxel);

}

public class BasicVoxel : IVoxel {

	protected readonly ResourceLocation loc;
	protected readonly float miningDifficulty;

	public BasicVoxel(ResourceLocation loc) {
		this.loc = loc;
		miningDifficulty = 0.0f;
	}

	public ResourceLocation GetResourceLocation() {
		return loc;
	}

	public float GetMiningDifficulty(VoxelInstance voxel) {
		return miningDifficulty;
	}

	public void OnRender(List<Vector3> verts, List<int> tris, List<Vector2> uvs, int i, Chunk chunk, VoxelInstance voxel) {
		Vector3 corner = voxel.GetPosInChunk().GetVector();
		Vector2 uvMin = Vector2.zero;
		Vector2 uvMax = new Vector2(1.0f, 1.0f);

		if (chunk.ShouldRender(voxel.GetPosInChunk(), Facing.SOUTH)) {
			RenderHelper.AddQuad(verts, tris, uvs, corner, Vector3.up, Vector3.right, uvMin, uvMax, i + verts.Count);
		}
		if (chunk.ShouldRender(voxel.GetPosInChunk(), Facing.NORTH)) {
			RenderHelper.AddQuad(verts, tris, uvs, corner + Vector3.forward + Vector3.right, Vector3.up, Vector3.left, uvMin, uvMax, i + verts.Count);
		}
		if (chunk.ShouldRender(voxel.GetPosInChunk(), Facing.EAST)) {
			RenderHelper.AddQuad(verts, tris, uvs, corner + Vector3.right, Vector3.up, Vector3.forward, uvMin, uvMax, i + verts.Count);
		}
		if (chunk.ShouldRender(voxel.GetPosInChunk(), Facing.WEST)) {
			RenderHelper.AddQuad(verts, tris, uvs, corner + Vector3.forward, Vector3.up, Vector3.back, uvMin, uvMax, i + verts.Count);
		}
		if (chunk.ShouldRender(voxel.GetPosInChunk(), Facing.DOWN)) {
			RenderHelper.AddQuad(verts, tris, uvs, corner + Vector3.forward, Vector3.back, Vector3.right, uvMin, uvMax, i + verts.Count);
		}
		if (chunk.ShouldRender(voxel.GetPosInChunk(), Facing.UP)) {
			RenderHelper.AddQuad(verts, tris, uvs, corner + Vector3.up, Vector3.forward, Vector3.right, uvMin, uvMax, i + verts.Count);
		}
	}

}