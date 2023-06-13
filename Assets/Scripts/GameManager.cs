using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
  [SerializeField]
  TMPro.TextMeshPro OverlayText;
  [SerializeField]
  AudioSource HoleStartSFX;

  readonly Dictionary<string, TargetCollider> targets = new Dictionary<string, TargetCollider>();
  public void RegisterTarget(TargetCollider target) {
    if (targets.ContainsKey(target.name)) {
      Debug.LogErrorFormat("Found Duplicate Target Name: {0}", target.name);
      return;
    }
    targets.Add(target.name, target);
  }

  void Start() {
    OverlayText.text = @"THE GARDEN:
A place of education

";

    Puck.Instance.TakeShot += Puck_TakeShot;
    Puck.Instance.AllBallsStopped += Puck_AllBallsStopped;
    Puck.Instance.PuckAcknowledges += Puck_PuckAcknowledges;
  }

  protected System.Action OnAcknowledge;
  private void Puck_PuckAcknowledges() {
    OnAcknowledge?.Invoke();
  }

  //Hole: Name, Success Requirements (Name: Function Test()), Ball, Setup (List of Balls, Positions)

  private void Puck_AllBallsStopped() {
    Debug.Log("Game Manager: Stop Stop Stop");

    bool successHole1 = TargetCollider.DoCollidersOverlap(targets["The Grove"].gameObject, Puck.Instance.CurrentBall.gameObject);
    Debug.LogFormat("Success for Hole 1? {0}", successHole1);

    bool successHole2 = TargetCollider.DoCollidersOverlap(targets["Back Lobby"].gameObject, Puck.Instance.CurrentBall.gameObject);
    Debug.LogFormat("Success for Hole 2? {0}", successHole2);

    //No Success Yet:
    OverlayText.text = @"Take Next Shot?
Press Enter";
    OnAcknowledge = () =>
    {
      Puck.Instance.CurrentBall = Puck.Instance.CurrentBall;
      //TODO: set orientation looking from ball to target;
      OnAcknowledge = null;

      //TODO: Only on success
      HoleStartSFX.PlayOneShot(HoleStartSFX.clip);
    };

    //TODO: Success
    //Give Report Card
    //Next Hole (Tap Enter)
  }

  private void Puck_TakeShot(Ball obj) {
    Debug.Log("Shots shots shots shots");
  }

  //Press A or W to Rotate your Aim
  //Press Spacebar for a light tap. Hold longer to hit more strongly
  //Get me, the Head Butler, into the Grove
}
