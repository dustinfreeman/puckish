using System.Collections;
using System.Linq;
using UnityEngine;

public class GameManager : ObjectRegistry {
  [SerializeField]
  TMPro.TextMeshPro OverlayText;
  [SerializeField]
  GameObject QuestParent;
  [SerializeField]
  GaffeAlert Gaffer;
  [SerializeField]
  AudioSource HoleStartSFX;

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
        for (int i = 0; i < hole.SuccessDefns.Length; i++) {
          var success = hole.SuccessDefns[i];
          targets[success.Target].gameObject.SetActive(true);

          var questUI = QuestParent.GetComponentsInChildren<OneQuest>(true)[i];
          questUI.State = OneQuest.QuestState.None;
          questUI.gameObject.SetActive(true);
          questUI.text.text = success.Description();
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


  void Start() {
    SanityCheckCourse(Course.Instance.GetActs());

    Puck.Instance.TakeShot += Puck_TakeShot;
    Puck.Instance.AnyAction += Puck_AnyAction;
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

  bool skipNextAllBallsStopped = false;
  private void Puck_Next(int nextDirn) {
    if (AreAnyBallsMoving()) {
      foreach (var ball in balls.Values) {
        ball.StopMotion();
      }
    } else {
      Debug.Log("Puck says next hole " + nextDirn);
      HoleIndex = Utils.WrapClamp(HoleIndex, nextDirn, Course.Instance.GetActs().Length);
      skipNextAllBallsStopped = true;
      foreach (var ball in balls.Values) {
        ball.StopMotion();
      }
    }
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

  protected void Update() {
    UpdateQuestUI();
  }

  void UpdateQuestUI() {
    var hole = CurrentHole();
    if (!hole) {
      return;
    }
    for (int i = 0; i < hole.SuccessDefns.Length; i++) {
      var successDefn = hole.SuccessDefns[i];
      var questUI = QuestParent.GetComponentsInChildren<OneQuest>(true)[i];

      questUI.State = TargetCollider.DoCollidersOverlap(targets[successDefn.Target].gameObject, balls[successDefn.BallName].gameObject) ?
        OneQuest.QuestState.Passed : OneQuest.QuestState.None;
    }
  }

  void Gaffe(string gaffe = "") {
    Gaffer.SubText.text = gaffe;
    Gaffer.gameObject.SetActive(gaffe.Length > 0);

    //TODO: player deals with consequences
  }

  private void AllBallsStopped() {
    //Debug.Log("Game Manager: Stop Stop Stop");
    if (!CurrentHole()) {
      return;
    }
    Puck.Instance.GetComponent<HUD>().ShowTargetHUD = false;

    //Check Interaction Registry for Gaffes!
    if (InteractionRegistry.Interactions.Count == 0) {
      Gaffe("You didn't do anything at all! So awkward!");

      OnAcknowledge = () =>
      {
        OnAcknowledge = null;
        Gaffe();

        //HACK: just continue along
        Puck.Instance.CurrentBall = balls[CurrentHole().Ball];
      };
      return;
    }

    bool successHole = true;
    foreach (var successDefn in CurrentHole().SuccessDefns) {
      if (!TargetCollider.DoCollidersOverlap(targets[successDefn.Target].gameObject, balls[successDefn.BallName].gameObject)) {
        successHole = false;
      }
    }

    if (!successHole) {
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
  }

  private void Puck_TakeShot(Ball obj) {
    InteractionRegistry.Interactions.Clear();
    Puck.Instance.CanTakeShot = false;
    ShotsTakenThisHole++;
    skipNextAllBallsStopped = false;
    StartCoroutine(WaitAllBallsStoppedMoving());
  }

  bool AreAnyBallsMoving() {
    var rbs = Puck.Instance.BallParent.GetComponentsInChildren<Ball>().Select(ball => ball.GetComponent<Rigidbody>());
    var awakeRBs =
      //https://docs.unity3d.com/Manual/RigidbodiesOverview.html
      rbs.Where(rb => !rb.IsSleeping());

    return awakeRBs.Count() > 0;
  }

  IEnumerator WaitAllBallsStoppedMoving() {
    yield return new WaitForSeconds(0.5f);

    while (true) {
      yield return null;
      if (!AreAnyBallsMoving()) {
        break;
      }
    }

    if (!skipNextAllBallsStopped) {
      Debug.Log("All Balls Stopped Moving");
      AllBallsStopped();
    }
    skipNextAllBallsStopped = false;
  }
}
