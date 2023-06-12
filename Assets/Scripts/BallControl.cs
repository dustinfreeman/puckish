using UnityEngine;
using UnityEngine.InputSystem;

public class BallControl : MonoBehaviour {

  public void OnJump(InputValue value) {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    if (value.isPressed) {
      var rb = this.GetComponent<Rigidbody>();
      rb.AddForce(Vector3.left * 50);
    }

#endif
  }

}

