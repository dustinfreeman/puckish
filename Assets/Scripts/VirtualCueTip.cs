using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCueTip : MonoBehaviour
{
  [SerializeField]
  GameObject VirtualCueBallCollider;

  Vector3 trackedVelocity = Vector3.zero;
  Vector3 lastPosition = Vector3.zero;

  public void SetWandTipPos(Vector3 pos) {
    //visual position will be snapped to horizontal plane at centre of balls
    transform.position = new Vector3(pos.x, 0.5f, pos.z);

    //we will track the vertical position as well for trick shot reasons
    if (lastPosition != Vector3.zero) {
      var delta = pos - lastPosition;
      trackedVelocity = delta / Time.deltaTime;
    }
    lastPosition = pos;
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject == VirtualCueBallCollider) {
      Vector3 horizontalVelocity = new Vector3(trackedVelocity.x, 0, trackedVelocity.z);
      var shotForce = horizontalVelocity.normalized * 50 * trackedVelocity.magnitude;
      var verticalForce = 0.0f;
      if (trackedVelocity.y > -5.0f) {
        verticalForce = trackedVelocity.y * 200.0f;
      }
      Debug.LogFormat("VirtualCueTip Hit: {0} at {1} with force {2} vertical {3}", other.name, trackedVelocity, shotForce, verticalForce);
      Puck.Instance.TakeTheShot(shotForce, verticalForce);
    }
  }
}
