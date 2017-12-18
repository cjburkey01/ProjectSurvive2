using System.Collections.Generic;
using UnityEngine;

public class RenderHelper {

	public static void AddQuad(List<Vector3> verts, List<int> tris, List<Vector2> uvs, Vector3 corner, Vector3 up, Vector3 right, Vector2 minUv, Vector2 maxUv, int i) {
		verts.AddRange(new Vector3[] { corner, corner + up, corner + up + right, corner + right });
		tris.AddRange(new int[] { i, i + 1, i + 2, i + 2, i + 3, i });
		uvs.AddRange(new Vector2[] { minUv, new Vector2(minUv.x, maxUv.y), new Vector2(maxUv.x, maxUv.y), new Vector2(maxUv.x, minUv.y) });
	}

}