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

  protected int ShotsTakenThisHole = 0;

  private int _holeIndex = 0;
  public int HoleIndex {
    get { return _holeIndex; }
    set {
      _holeIndex = value;
      Debug.LogFormat("Hole Index {0}", HoleIndex);
      ShotsTakenThisHole = 0;

      var act = Course.Instance.GetActs()[HoleIndex];
      OverlayText.text = act.Description;
      if (act is HoleDefn) {
        var hole = (HoleDefn)act;
        foreach (var ballStart in hole.BallStartTransforms) {
          balls[ballStart.BallName].transform.position = ballStart.StartTransform.position;
          balls[ballStart.BallName].transform.rotation = ballStart.StartTransform.rotation;
        }
        Puck.Instance.CurrentBall = balls[hole.Ball];

        if (hole.Par > 0) {
          OverlayText.text += string.Format("\n\nPar: {0}", hole.Par);
        }
      } else {
        Puck.Instance.CurrentBall = null;
        Puck.Instance.SetViewpoint(act.StartView);
        OverlayText.text += "\n\n(Press Enter to Continue)";
        OnAcknowledge = () =>
        {
          HoleIndex += 1;
          OnAcknowledge = null;
        };
      }
      Puck.Instance.GetComponent<HUD>().ShowTargetHUD = false;
    }
  }
  public HoleDefn CurrentHole() {
    var act = Course.Instance.GetActs()[HoleIndex];
    if (act is HoleDefn) {
      return (HoleDefn)act;
    }
    return null;
  }

  void SanityCheckCourse() {
    foreach (var act in Course.Instance.GetActs()) {
      if (!(act is HoleDefn)) {
        continue;
      }
      var hole = (HoleDefn)act;
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

    Puck.Instance.TakeShot += Puck_TakeShot;
    Puck.Instance.AnyAction += Puck_AnyAction;
    Puck.Instance.AllBallsStopped += Puck_AllBallsStopped;
    Puck.Instance.PuckAcknowledges += Puck_PuckAcknowledges;
    Puck.Instance.Next += Puck_Next;

    HoleIndex = 0;
  }

  private void Puck_AnyAction() {
    OverlayText.text = "";
    Puck.Instance.GetComponent<HUD>().ShowTargetHUD = true;
  }

  private void Puck_Next(int nextDirn) {
    Debug.Log("Puck says next hole " + nextDirn);
    HoleIndex = Utils.WrapClamp(HoleIndex, nextDirn, Course.Instance.GetActs().Length);
  }

  protected System.Action OnAcknowledge;
  private void Puck_PuckAcknowledges() {
    OnAcknowledge?.Invoke();
  }

  private string ParDisplay() {
    if (CurrentHole().Par == 0) {
      return "";
    }
    return string.Format("Shots Taken: {0}  Par: {1}\n", ShotsTakenThisHole, CurrentHole().Par);
  }

  private void Puck_AllBallsStopped() {
    //Debug.Log("Game Manager: Stop Stop Stop");

    if (!CurrentHole()) {
      return;
    }

    bool success = true;
    foreach (var successDefn in CurrentHole().SuccessDefns) {
      if (!TargetCollider.DoCollidersOverlap(targets[successDefn.Target].gameObject, balls[successDefn.BallName].gameObject)) {
        success = false;
      }
    }

    if (!success) {
      OverlayText.text = ParDisplay() + @"Take Next Shot?
Press Enter";
      OnAcknowledge = () =>
      {
        Puck.Instance.CurrentBall = balls[CurrentHole().Ball];
        //TODO: set orientation looking from ball to target;
        OnAcknowledge = null;
      };
    } else { //completed hole
      HoleStartSFX.PlayOneShot(HoleStartSFX.clip);

      if (HoleIndex < Course.Instance.GetActs().Length - 1) {
        OverlayText.text = ParDisplay() + @"Hole Completed!
Press Enter for Next";
        OnAcknowledge = () =>
        {
          HoleIndex += 1;
          OnAcknowledge = null;
        };
      } else {
        OverlayText.text = @"You have finished
this Evening's Course

Press Enter to End";

        HoleIndex = 0; //to the main screen again
      }
    }
    Puck.Instance.GetComponent<HUD>().ShowTargetHUD = true;
  }

  private void Puck_TakeShot(Ball obj) {
    Puck.Instance.CanTakeShot = false;
    ShotsTakenThisHole++;
    Debug.Log("Shots shots shots shots");
  }

}
