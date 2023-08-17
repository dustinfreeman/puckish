using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCueTip : MonoBehaviour
{
  [SerializeField]
  GameObject VirtualCueBallCollider;

  Vector3 trackedVelocity = Vector3.zero;
  Vector3 lastPosition = Vector3.zero;
  private void FixedUpdate() {
    if (lastPosition != Vector3.zero) {
      var delta = transform.position - lastPosition;
      trackedVelocity = delta / Time.deltaTime;
    }
    lastPosition = transform.position;
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject == VirtualCueBallCollider) {
      var shotForce = trackedVelocity * 50;
      Debug.LogFormat("VirtualCueTip Hit: {0} at {1} with force {2}", other.name, trackedVelocity, shotForce);
      Puck.Instance.TakeTheShot(shotForce);
    }
  }
}
