using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCueTip : MonoBehaviour
{
  [SerializeField]
  GameObject VirtualCueBallCollider;

  Vector3 trackedVelocity = Vector3.zero;
  Vector3 lastPosition = Vector3.zero;
  Quaternion lastRot = Quaternion.identity;

  public void SetWandTip(Vector3 pos, Quaternion rot) {
    //visual position will be snapped to horizontal plane at centre of balls
    transform.position = new Vector3(pos.x, 0.5f, pos.z);

    //we will track the vertical position as well for trick shot reasons
    if (lastPosition != Vector3.zero) {
      var delta = pos - lastPosition;
      trackedVelocity = delta / Time.deltaTime;
    }
    lastPosition = pos;
    lastRot = rot;
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject == VirtualCueBallCollider) {
      Vector3 horizontalVelocity = new Vector3(trackedVelocity.x, 0, trackedVelocity.z);
      var shotForce = horizontalVelocity.normalized * 50 * trackedVelocity.magnitude;
      var verticalForce = 0.0f;

      if (Mathf.Abs(lastRot.eulerAngles.x) > 35) {
        verticalForce = trackedVelocity.y * 200.0f;
      }
      Debug.LogFormat("VirtualCueTip Hit: {0} at {1} with rot {2} with force {3} vertical {4}", 
        other.name, trackedVelocity, lastRot.eulerAngles, shotForce, verticalForce);
      Puck.Instance.TakeTheShot(shotForce, verticalForce);
    }
  }
}
