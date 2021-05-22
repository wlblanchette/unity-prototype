using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


public class Player : MonoBehaviour {
  [SerializeField] private float speed = 15;
  // TODO: GroundingDistance factors in player objects's height. This probably isn't ideal, not sure.
  public float GroundingDistance = 2.5f;

  private Rigidbody body;
  private float inputHorizontal;
  private float inputVertical;

  public float turnSmoothTime = 0.1f;
  private float turnSmoothVelocity;

  // Start is called before the first frame update
  void Start() {
    body = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update() {
    inputHorizontal = Input.GetAxis("Horizontal");
    inputVertical = Input.GetAxis("Vertical");
    
    transform.rotation = rotatePlayer(Camera.main.transform.forward.normalized);
  }

  // This function is called every fixed framerate frame, if the MonoBehaviour is enabled
  private void FixedUpdate() {
    var camera = Camera.main;

    var forward = camera.transform.forward.normalized;
    var right = camera.transform.right.normalized;

    forward.y = 0f;
    right.y = 0f;

    body.velocity = forward * inputVertical * speed + 
                    right * inputHorizontal * speed + 
                    new Vector3(0, body.velocity.y, 0);

    inputHorizontal = 0;
    inputVertical = 0;
  }

  private Quaternion rotatePlayer(Vector3 cameraDirection) {
    var forward = cameraDirection.normalized;
    forward.y = 0f;
    float targetAngle = Mathf.Atan2(cameraDirection.x, cameraDirection.z) * Mathf.Rad2Deg;
    float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
    return Quaternion.Euler(0f, smoothedAngle, 0f);
  }
}
