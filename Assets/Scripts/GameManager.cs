using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
  [SerializeField]
  TMPro.TextMeshPro OverlayText;
  [SerializeField]
  AudioSource HoleStartSFX;

  readonly static Dictionary<string, TargetCollider> targets = new Dictionary<string, TargetCollider>();
  public static void RegisterTarget(TargetCollider target) {
    if (targets.ContainsKey(target.name)) {
      Debug.LogErrorFormat("Found Duplicate Target Name: {0}", target.name);
      return;
    }
    targets.Add(target.name, target);
  }

  readonly static Dictionary<string, Ball> balls = new Dictionary<string, Ball>();
  public static void RegisterBall(Ball ball) {
    if (balls.ContainsKey(ball.name)) {
      Debug.LogErrorFormat("Found Duplicate Ball Name: {0}", ball.name);
      return;
    }
    balls.Add(ball.name, ball);
  }

  private int _holeIndex = 0;
  public int HoleIndex {
    get { return _holeIndex; }
    set {
      _holeIndex = value;

      var hole = CurrentHole();
      foreach (var ballStart in hole.BallStartTransforms) {
        balls[ballStart.BallName].transform.position = ballStart.StartTransform.position;
        balls[ballStart.BallName].transform.rotation = ballStart.StartTransform.rotation;
      }

      Puck.Instance.CurrentBall = balls[hole.Ball];
    }
  }
  public HoleDefn CurrentHole() {
    return Course.Instance.GetHoles()[HoleIndex];
  }

  void SanityCheckCourse() {
    foreach (var hole in Course.Instance.GetHoles()) {
      if (!balls.ContainsKey(hole.Ball)) {
        Debug.LogErrorFormat("{1}: Missing Ball with name {0}", hole.Ball, hole.name);
      }
      foreach (var ballStart in hole.BallStartTransforms) {
        if (!balls.ContainsKey(ballStart.BallName)) {
          Debug.LogErrorFormat("{1}: Missing Ball with name {0}", ballStart.BallName, hole.name);
        }
      }
      foreach (var successDefn in hole.SuccessDefns) {
        if (!balls.ContainsKey(successDefn.BallName)) {
          Debug.LogErrorFormat("{1}: Missing Ball with name {0}", successDefn.BallName, hole.name);
        }
        if (!targets.ContainsKey(successDefn.Target)) {
          Debug.LogErrorFormat("{1}: Missing Target with name {0}", successDefn.Target, hole.name);
        }
      }
    }
  }

  void Start() {
    SanityCheckCourse();

    var gardenOverlay = @"THE GARDEN:
A place of education

";
    var manorOverlay = @"THE MANOR:
A tall, dark strangers returns, 
disrupting a dinner party
";

    OverlayText.text = manorOverlay;



    Puck.Instance.TakeShot += Puck_TakeShot;
    Puck.Instance.AllBallsStopped += Puck_AllBallsStopped;
    Puck.Instance.PuckAcknowledges += Puck_PuckAcknowledges;
    Puck.Instance.Next += Puck_Next;
  }

  private void Puck_Next(int nextDirn) {
    Debug.Log("Puck says next hole " + nextDirn);
    HoleIndex = Utils.WrapClamp(HoleIndex, nextDirn, Course.Instance.GetHoles().Length);
  }

  protected System.Action OnAcknowledge;
  private void Puck_PuckAcknowledges() {
    OnAcknowledge?.Invoke();
  }

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
