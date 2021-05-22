using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


public class Player_v2 : MonoBehaviour {
  public float speed = 15;
  public float jumpHeight = 3f;
  public float gravity = -9.8f * 3;
  public CharacterController controller;

  public Transform groundCheck;
  public float groundDistance = 0.25f;
  public LayerMask groundMask;

  public float turnSmoothTime = 0.1f;
  private float turnSmoothVelocity;

  private Vector3 velocity;
  private bool isGrounded;

  // Update is called once per frame
  void Update() {
    Vector3 forward = flattenDirection(Camera.main.transform.forward);
    Vector3 right = flattenDirection(Camera.main.transform.right);
    
    float inputHorizontal = Input.GetAxis("Horizontal");
    float inputVertical = Input.GetAxis("Vertical");
    
    transform.rotation = rotatePlayer(forward);
    Vector3 move = right * inputHorizontal + forward * inputVertical;

    controller.Move(move * speed * Time.deltaTime);

    controller.Move(getYVelocity());
  }

  private Quaternion rotatePlayer(Vector3 cameraDirection) {
    float targetAngle = Mathf.Atan2(cameraDirection.x, cameraDirection.z) * Mathf.Rad2Deg;
    float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
    return Quaternion.Euler(0f, smoothedAngle, 0f);
  }

  private Vector3 flattenDirection(Vector3 v1) {
    var v = v1;
    v.y = 0f;
    v.Normalize();

    return v;
  }

  private Vector3 getYVelocity() {
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    if (isGrounded && Input.GetButtonDown("Jump")) {
      velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    } else if (isGrounded && velocity.y < 0) {
      // rest velocity is strong at -20f so the character sticks to terrain while
      // walking around.
      velocity.y = -20f;
    } else {
      velocity.y += gravity * Time.deltaTime;
    }

    return velocity * Time.deltaTime;
  }
}
