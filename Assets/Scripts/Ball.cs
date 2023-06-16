using System.Collections;
using UnityEngine;



public class Ball : MonoBehaviour {
  AudioSource barks;
  float[] oofs = { 115.9f, 117.7f, 120.0f, 122.7f };

  protected void Awake() {
    ObjectRegistry.RegisterBall(this);
    barks = GetComponent<AudioSource>();
  }

  public void StopMotion() {
    var rb = GetComponent<Rigidbody>();
    rb.velocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
  }

  void InteractedWith(Collider other) {
    //Debug.LogFormat("InteractedWith! Time: {0}\tBall:{1}\tOther:{2}", Time.time, gameObject, other);
    InteractionType interactionType;
    if (other.GetComponent<Ball>()) {
      interactionType = InteractionType.BallBall;
    } else if (other.GetComponent<TargetCollider>()) {
      interactionType = InteractionType.BallTarget;
    } else {
      interactionType = InteractionType.BallWall;
    }
    InteractionRegistry.Interactions.Add(new Interaction { ball = this, time = Time.time, other = other.name, type = interactionType });
  }

  private void OnCollisionEnter(Collision collision) {
    InteractedWith(collision.collider);

    var otherBall = collision.collider.GetComponent<Ball>();
    if (!otherBall) return;
    PlayBark(Utils.ChooseRandom(oofs));
  }

  private void OnTriggerEnter(Collider other) {
    InteractedWith(other);
  }

  void PlayBark(float startTime) {
    StartCoroutine(PlayingBark(startTime));
  }

  IEnumerator PlayingBark(float startTime) {
    barks.time = startTime;
    barks.Play();
    yield return new WaitForSeconds(1.5f);
    barks.Pause();
  }
}

