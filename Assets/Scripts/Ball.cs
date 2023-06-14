using System.Collections;
using UnityEngine;



public class Ball : MonoBehaviour {

  AudioSource barks;
  float[] oofs = { 115.9f, 117.7f, 120.0f, 122.7f };

  protected void Awake() {
    GameManager.RegisterBall(this);
    barks = GetComponent<AudioSource>();
  }

  private void OnCollisionEnter(Collision collision) {
    var otherBall = collision.collider.GetComponent<Ball>();
    if (!otherBall) return;

    Debug.LogFormat("I, {0}, collided with {1}!", this, otherBall);

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

