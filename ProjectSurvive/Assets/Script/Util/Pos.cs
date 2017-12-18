using UnityEngine;

public struct Pos {

	public static readonly Pos ZERO = new Pos(0, 0, 0);
	public static readonly Pos ERROR = new Pos(int.MinValue, int.MinValue, int.MinValue);

	public readonly int x;
	public readonly int y;
	public readonly int z;

	public Pos(float x, float y, float z) : this(Mathf.FloorToInt(x), Mathf.FloorToInt(y), Mathf.FloorToInt(z)) {
	}

	public Pos(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Pos Offset(Facing dir) {
		return Offset(this, dir);
	}

	public Pos Offset(Facing dir, int scale) {
		return Offset(this, dir, scale);
	}

	public Pos Scale(float scalar) {
		return Scale(this, scalar);
	}

	public Pos Add(Pos other) {
		return Add(this, other);
	}

	public Pos Negate() {
		return Negate(this);
	}

	public Vector3 GetVector() {
		return new Vector3(x, y, z);
	}

	public override string ToString() {
		return "(" + x + ", " + y + ", " + z + ")";
	}

	public static Pos Offset(Pos pos, Facing dir) {
		return Offset(pos, dir, 1);
	}

	public static Pos Offset(Pos pos, Facing dir, int scale) {
		switch (dir) {
		case Facing.NORTH:
			return new Pos(pos.x, pos.y, pos.z + scale);
		case Facing.SOUTH:
			return new Pos(pos.x, pos.y, pos.z - scale);
		case Facing.EAST:
			return new Pos(pos.x + scale, pos.y, pos.z);
		case Facing.WEST:
			return new Pos(pos.x - scale, pos.y, pos.z);
		case Facing.UP:
			return new Pos(pos.x, pos.y + scale, pos.z);
		case Facing.DOWN:
			return new Pos(pos.x, pos.y - scale, pos.z);
		}
		return ERROR;
	}

	// Floors the values
	public static Pos Scale(Pos pos, float scalar) {
		return new Pos(pos.x * scalar, pos.y * scalar, pos.z * scalar);
	}

	public static Pos Add(Pos pos, Pos other) {
		return new Pos(pos.x + other.x, pos.y + other.y, pos.z + other.z);
	}

	public static Pos Negate(Pos pos) {
		return new Pos(-pos.x, -pos.y, -pos.z);
	}

}

public enum Facing {

	NORTH,
	SOUTH,
	EAST,
	WEST,
	UP,
	DOWN

}