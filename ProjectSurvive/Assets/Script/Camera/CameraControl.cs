using UnityEngine;

public class CameraControl : MonoBehaviour {

	public float rotationSpeed = 100.0f;
	public float movementSpeed = 35.0f;
	public float zoomSpeed = 50.0f;
	public float speedRatio = 2.5f;
	//public float damping = 0.025f;
	public float minY = Chunk.SIZE;
	public float maxY = 50;

	private Vector3 goalPos;
	private Vector3 refVel;

	private readonly Plane zeroPlane = new Plane(Vector3.up, Vector3.zero);

	void Start() {
		goalPos = transform.position;
	}

	void Update() {
		DoRotation();
		DoMovement();
		DoZooming();
		Move();
	}

	private void DoRotation() {
		if (Input.GetMouseButton(1)) {
			float rotation = Input.GetAxisRaw("Mouse X") * rotationSpeed * Time.deltaTime;
			transform.RotateAround(GetLookingAt(), Vector3.up, rotation);
			goalPos = transform.position;
		}
	}

	private void DoMovement() {
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		if (x > 0 || y > 0 || x < 0 || y < 0) {
			Vector3 movement = new Vector3(x, 0.0f, y);
			movement = transform.TransformDirection(movement.normalized);
			movement.y = 0.0f;
			movement *= (movementSpeed + speedRatio * goalPos.y) * Time.deltaTime;
			goalPos += movement;
		}
	}

	private void DoZooming() {
		float amt = Input.GetAxisRaw("Mouse ScrollWheel");
		if (amt > 0 || amt < 0) {
			if (goalPos.y <= minY && amt > 0) {
				return;
			}
			if (goalPos.y >= maxY && amt < 0) {
				return;
			}
			Vector3 zoom = new Vector3(0.0f, 0.0f, amt);
			zoom = transform.TransformDirection(zoom.normalized);
			zoom *= (zoomSpeed + speedRatio * goalPos.y) * Time.deltaTime;
			goalPos += zoom;
		}
	}

	private void Move() {
		World world = FindObjectOfType<World>();
		goalPos.x = Mathf.Clamp(goalPos.x, -world.width * Chunk.SIZE / 2, world.width * Chunk.SIZE / 2);
		goalPos.z = Mathf.Clamp(goalPos.z, -world.width * Chunk.SIZE / 2, world.width * Chunk.SIZE / 2);
		goalPos.y = Mathf.Clamp(goalPos.y, minY, maxY);

		//transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref refVel, damping, 100.0f);
		transform.position = goalPos;
	}

	private Vector3 GetLookingAt() {
		Ray ray = new Ray(goalPos, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1000.0f)) {
			return hit.point;
		} else {
			float dist;
			if (zeroPlane.Raycast(ray, out dist)) {
				return ray.GetPoint(dist);
			}
		}
		return Vector3.zero;
	}

}