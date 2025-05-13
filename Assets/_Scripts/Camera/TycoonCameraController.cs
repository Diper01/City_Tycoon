using UnityEngine;

public class TycoonCameraController : MonoBehaviour {
  [Header("Pan Settings")]
  public float panSpeed = 20f;
  public float dragSpeed = 0.1f;

  [Header("Zoom Settings")]
  public float scrollSpeed = 200f;
  public float minY = 10f;
  public float maxY = 100f;

  [Header("Rotation Settings")]
  public float rotateSpeed = 100f;

  private Vector3 dragOrigin;

  void Update() {
    HandleKeyboardPan();
    HandleMouseDrag();
    HandleScrollZoom();
    HandleRotation();
  }

  void HandleKeyboardPan() {
    float x = Input.GetAxis("Horizontal");
    float z = Input.GetAxis("Vertical");
    Vector3 move = new Vector3(x, 0, z) * (panSpeed * Time.deltaTime);
    transform.Translate(move, Space.Self);
  }

  void HandleMouseDrag() {
    if (Input.GetMouseButtonDown(2)) {
      dragOrigin = Input.mousePosition;
      return;
    }

    if (Input.GetMouseButton(2)) {
      Vector3 delta = Input.mousePosition - dragOrigin;
      Vector3 move = new Vector3(-delta.x, 0, -delta.y) * dragSpeed;
      transform.Translate(move * Time.deltaTime, Space.Self);
      dragOrigin = Input.mousePosition;
    }
  }

  void HandleScrollZoom() {
    float scroll = Input.GetAxis("Mouse ScrollWheel");

    if (Mathf.Abs(scroll) > 0.01f) {
      Vector3 pos = transform.position;
      pos.y -= scroll * scrollSpeed * Time.deltaTime;
      pos.y = Mathf.Clamp(pos.y, minY, maxY);
      transform.position = pos;
    }
  }

  void HandleRotation() {
    float h = 0f;

    if (Input.GetKey(KeyCode.Q))
      h = -1f;

    if (Input.GetKey(KeyCode.E))
      h = 1f;

    if (Input.GetMouseButton(1))
      h = Input.GetAxis("Mouse X");

    if (Mathf.Abs(h) > 0.01f) {
      transform.Rotate(Vector3.up, h * rotateSpeed * Time.deltaTime, Space.World);
    }
  }
}