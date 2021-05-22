using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The magic here is that there's an empty that the camera is parented to which serves
// as a pivot point. The pivot takes the position of the Player and the Camera
// offset is simply local coordinates from the empty.
//
// The gimbal lock problem is solved by not rotating relative to world axis. Rotating is
// achieved by transforming the x, y vector of mouse delta coordinates to a Euler delta
// and applying that rotation to the pivot point.
//

public class PlayerFollow_v2 : MonoBehaviour {
  // public
  [Range(0f, 1f)] 
  public float BUMP_UPWARDS_AMOUNT = 1.0f;
  public Transform PlayerTransform;  
  public float CameraDistance = 30f;
  public float MouseSensativity = 5f;
  public float OrbitDampening = 20f;
  public LayerMask groundRaycastTargets;

  // private
  private Transform _Transform_Camera;
  private Transform _Transform_Pivot;
  private Vector3 _LocalRotation;
  private bool shouldOrbit = false;

  private void Start() {
    _Transform_Camera = transform;
    _Transform_Pivot = transform.parent;
    _Transform_Camera.localPosition = new Vector3(0, 0, -1f * CameraDistance);
  }

  // Update is called every frame, if the MonoBehaviour is enabled
  private void Update() {
    var holdingRightMouseButton = Input.GetMouseButton(1);
    shouldOrbit = holdingRightMouseButton;

    // lock cursor while orbiting
    cursorLock(shouldOrbit);
  }

  private void LateUpdate() {
    if (shouldOrbit) {
      // Rotation of the Camera is based on the x/y coordinates of mouse
      if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
        _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensativity;
        _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensativity;

        // Clamp rotation to the horizon in either direction and not flipping over the top
        _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, 0f, 89f);
      }
    }

    // Actual Camera Rig Transformations
    Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
    _Transform_Pivot.rotation = Quaternion.Lerp(_Transform_Pivot.rotation, QT, Time.deltaTime * OrbitDampening);
    _Transform_Pivot.position = PlayerTransform.position;

    // Adjust position for collisions with terrain
    _Transform_Pivot.position = AdjustForTerrain(_Transform_Camera.position, _Transform_Pivot.position, PlayerTransform.position);

  }


  #region private methods
  //------------------------

  // cast a ray from player to camera, if the ray collides with something, adjust camera position.
  private Vector3 AdjustForTerrain(Vector3 cameraPosition, Vector3 parentPosition, Vector3 playerPosition) {
    var adjustedCollisionPoint = cameraPosition - Vector3.up * BUMP_UPWARDS_AMOUNT;
    RaycastHit terrainHit = new RaycastHit();
    Debug.DrawLine(parentPosition, adjustedCollisionPoint, Color.cyan);
    if (Physics.Linecast(parentPosition, adjustedCollisionPoint, out terrainHit, groundRaycastTargets)) {
      Debug.DrawRay(terrainHit.point, Vector3.up, Color.red);
      // shift the parent position by how much the camera needs to move.
      return (terrainHit.point + Vector3.up * BUMP_UPWARDS_AMOUNT) - cameraPosition + parentPosition;
    } else {
      return playerPosition;
    }
  }

  private void cursorLock(bool shouldLock) {
    Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
  }

  //------------------------
  #endregion
}
