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

  private void OnCollisionEnter(Collision collision) {
    var otherBall = collision.collider.GetComponent<Ball>();
    if (!otherBall) return;
    PlayBark(Utils.ChooseRandom(oofs));
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

