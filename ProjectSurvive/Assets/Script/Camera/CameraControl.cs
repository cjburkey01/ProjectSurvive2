using UnityEngine;

public class CameraControl : MonoBehaviour {

	public float rotationSpeed = 100.0f;

	public float movementSpeed = 35.0f;
	public float speedHeightRatio = 0.5f;
	public float scrollSpeed = 200.0f;
	public float movementSmoothing = 0.1f;
	public float minY = Chunk.SIZE;
	public float maxY = Chunk.SIZE * 10;
	public float yMinAngle = 30.0f;
	public float yMaxAngle = 60.0f;

	private Vector3 goalPos;
	private Vector3 refGoal;
	private readonly Plane zeroPlane = new Plane(Vector3.up, Vector3.zero);

	void Update() {
		HandleRotation();
		HandleMovement();
		HandleZoom();
		Move();
	}

	private void HandleRotation() {
		float rot = Input.GetAxisRaw("Mouse X");
		if (Input.GetMouseButton(1)) {
			transform.position = goalPos;
			transform.RotateAround(GetLookingAt(), Vector3.up, rot * rotationSpeed * Time.deltaTime);
			goalPos = transform.position;
		}
	}

	private void HandleMovement() {
		float hor = Input.GetAxisRaw("Horizontal");
		float ver = Input.GetAxisRaw("Vertical");
		if (hor > 0 || hor < 0 || ver > 0 || ver < 0) {
			Vector3 rotBefore = transform.rotation.eulerAngles;
			transform.rotation = Quaternion.Euler(new Vector3(0.0f, rotBefore.y, rotBefore.z));
			Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
			input = transform.TransformDirection(input.normalized);
			transform.rotation = Quaternion.Euler(rotBefore);
			input *= TransformSpeed(movementSpeed) * Time.deltaTime;
			goalPos += input;
		}
	}

	private void HandleZoom() {
		float move = Input.GetAxisRaw("Mouse ScrollWheel");
		if (move > 0 || move < 0) {
			if (move > 0 && goalPos.y <= minY) {
				return;
			}
			if (move < 0 && goalPos.y >= maxY) {
				return;
			}
			Vector3 input = new Vector3(0.0f, 0.0f, move);
			input = transform.TransformDirection(input.normalized);
			input *= TransformSpeed(scrollSpeed) * Time.deltaTime;
			goalPos += input;
		}
	}

	private void Move() {
		World world = FindObjectOfType<World>();
		if (world != null) {
			float w = (world.width * Chunk.SIZE) / 2.0f;
			goalPos.x = Mathf.Clamp(goalPos.x, -w, w);
			goalPos.z = Mathf.Clamp(goalPos.z, -w, w);
		}
		Vector3 outPos;
		goalPos.y = Mathf.Clamp(goalPos.y, minY, maxY);
		outPos = Vector3.SmoothDamp(transform.position, goalPos, ref refGoal, movementSmoothing, 1000.0f);
		transform.position = outPos;
		AngleUp();
	}

	private void AngleUp() {
		Vector3 rot = transform.rotation.eulerAngles;
		rot.x = Mathf.Lerp(yMinAngle, yMaxAngle, (transform.position.y - minY) / (maxY - minY));
		transform.rotation = Quaternion.Euler(rot);
	}

	private float TransformSpeed(float speed) {
		return speed + ((goalPos.y - minY) * speedHeightRatio);
	}

	private Vector3 GetLookingAt() {
		Ray ray = new Ray(goalPos, transform.forward);
		float dist;
		if (zeroPlane.Raycast(ray, out dist)) {
			return ray.GetPoint(dist);
		}
		return Vector3.zero;
	}

}