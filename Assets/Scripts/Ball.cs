using UnityEngine;

public class Ball : MonoBehaviour {
  protected void Awake() {
    GameManager.RegisterBall(this);
  }

  private void OnCollisionEnter(Collision collision) {
    var otherBall = collision.collider.GetComponent<Ball>();
    if (!otherBall) return;

    Debug.LogFormat("I, {0}, collided with {1}!", this, otherBall);
  }
}

