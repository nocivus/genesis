using UnityEngine;
using System.Collections;

public class FlyCamera : MonoBehaviour {

  Vector2 rot;

  void Update() {

    rot = new Vector2(rot.x + Input.GetAxis("Mouse X") * 3, rot.y + Input.GetAxis("Mouse Y") * 3);

    transform.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up);
    transform.localRotation *= Quaternion.AngleAxis(rot.y, Vector3.left);

    transform.position += transform.forward * 10 * Input.GetAxis("Vertical");
    transform.position += transform.right * 10 * Input.GetAxis("Horizontal");
    if (Input.GetKey(KeyCode.LeftShift)) {
      transform.position += transform.up * 10;
    }
    if (Input.GetKey(KeyCode.LeftControl)) {
      transform.position -= transform.up * 10;
    }
  }
}