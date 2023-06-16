using System.Collections;
using UnityEngine;


public class Ball : MonoBehaviour {
  [SerializeField]
  string VoiceArchetype;
  AudioSource barksSource;
  BarksDefn barks;

  protected void Awake() {
    ObjectRegistry.RegisterBall(this);
  }

  private void Start() {
    barksSource = GetComponent<AudioSource>();
    barks = Voices.Instance.GetBarks(VoiceArchetype);
    barksSource.clip = barks.voice.clip;
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

    if (GetComponent<Rigidbody>().velocity.magnitude >
      otherBall.GetComponent<Rigidbody>().velocity.magnitude) {
      //I have greater velocity, thereform I am doing the hitting
      PlayBark(BarkType.hits);
    } else {
      PlayBark(BarkType.hit_by);
    }
  }

  private void OnTriggerEnter(Collider other) {
    InteractedWith(other);
  }

  public void PlayBark(BarkType type) {
    float[] barkOptions = barks.barkOptionTimes[type];
    if (barkOptions.Length == 0) {
      Debug.LogErrorFormat("{0} did not have a bark option for {1}", name, type);
    }

    float duration;
    switch (type) {
      case BarkType.hits:
      case BarkType.hit_by:
        duration = 1.5f;
        break;
      default:
        duration = 3.0f;
        break;
    }

    PlaySpecificBark(Utils.ChooseRandom(barkOptions), duration);
  }

  void PlaySpecificBark(float startTime, float duration = 2.0f) {
    StartCoroutine(PlayingBark(startTime, duration));
  }

  IEnumerator PlayingBark(float startTime, float duration = 2.0f) {
    barksSource.time = startTime;
    barksSource.Play();
    yield return new WaitForSeconds(duration);
    barksSource.Pause();
  }
}

