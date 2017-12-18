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
	protected readonly ResourceLocation texture;
	protected readonly float miningDifficulty;

	public BasicVoxel(ResourceLocation loc, ResourceLocation texture) {
		this.loc = loc;
		this.texture = texture;
		miningDifficulty = 0.0f;
	}

	public ResourceLocation GetResourceLocation() {
		return loc;
	}

	public ResourceLocation GetTextureLocation() {
		return texture;
	}

	public float GetMiningDifficulty(VoxelInstance voxel) {
		return miningDifficulty;
	}

	public void OnRender(List<Vector3> verts, List<int> tris, List<Vector2> uvs, int i, Chunk chunk, VoxelInstance voxel) {
		Vector3 corner = voxel.GetPosInChunk().GetVector();

		Rect textPos = TextureHandler.Instance.GetTexture(texture);
		Vector2 padd = new Vector2(textPos.width / 100000.0f, textPos.height / 100000.0f);
		Vector2 uvMin = textPos.min + padd;
		Vector2 uvMax = textPos.max - padd;

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