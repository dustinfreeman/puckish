using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
  [SerializeField]
  TMPro.TextMeshPro OverlayText;
  [SerializeField]
  GameObject QuestParent;
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

      //Hide Target Colliders; will unhide later only ones relevant to hole
      foreach (var target in targets.Values) {
        target.gameObject.SetActive(false);
      }
      //Hide Quests
      foreach (var quest in QuestParent.GetComponentsInChildren<OneQuest>()) {
        quest.gameObject.SetActive(false);
      }

      if (act is HoleDefn) {
        var hole = (HoleDefn)act;
        foreach (var success in hole.SuccessDefns) {
          targets[success.Target].gameObject.SetActive(true);
        }
        foreach (var ballStart in hole.BallStartTransforms) {
          balls[ballStart.BallName].transform.SetPositionAndRotation(ballStart.StartTransform.position, ballStart.StartTransform.rotation);
        }
        Puck.Instance.CurrentBall = balls[hole.Ball];

        if (hole.Par > 0) {
          OverlayText.text += string.Format("\n\nPar: {0}", hole.Par);
        }
      } else { //Simple Inter-Act Screen
        Puck.Instance.CurrentBall = null;
        Puck.Instance.SetViewpoint(act.StartView);
        //TODO: total par, strokes.
        OverlayText.text += string.Format("\n\n(Press Enter to {0})",
          HoleIndex < Course.Instance.GetActs().Length - 1 ? "Continue" : "Restart Game");
        OnAcknowledge = () =>
        {
          OnAcknowledge = null;
          HoleIndex = Utils.WrapClamp(HoleIndex, +1, Course.Instance.GetActs().Length);
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
    if (Puck.Instance.CurrentBall != null) {
      //ready to orient for the shot, hide overlay
      OverlayText.text = "";
    }

    Puck.Instance.GetComponent<HUD>().ShowTargetHUD = true;
  }

  private void Puck_Next(int nextDirn) {
    Debug.Log("Puck says next hole " + nextDirn);
    HoleIndex = Utils.WrapClamp(HoleIndex, nextDirn, Course.Instance.GetActs().Length);
  }

  protected System.Action OnAcknowledge;
  private void Puck_PuckAcknowledges() {
    //Debug.Log("Puck_PuckAcknowledges " + OnAcknowledge?.ToString());
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
        OnAcknowledge = null;
        Puck.Instance.CurrentBall = balls[CurrentHole().Ball];
        //TODO: set orientation looking from ball to target;
      };
    } else { //completed hole
      HoleStartSFX.PlayOneShot(HoleStartSFX.clip);

      if (HoleIndex < Course.Instance.GetActs().Length - 1) {
        OverlayText.text = ParDisplay() + @"Hole Completed!
Press Enter for Next";
        OnAcknowledge = () =>
        {
          OnAcknowledge = null;
          HoleIndex += 1;
        };
      } else {
        //HACK: only used if there isn't a final Act screen
        OverlayText.text = @"You have finished
this Evening's Course

Press Enter to End";
        HoleIndex = 0; //to the main screen again
      }
    }
    Puck.Instance.GetComponent<HUD>().ShowTargetHUD = false;
  }

  private void Puck_TakeShot(Ball obj) {
    Puck.Instance.CanTakeShot = false;
    ShotsTakenThisHole++;
  }

}
