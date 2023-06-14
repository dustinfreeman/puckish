using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Puck : Singleton<Puck> {

  [Header("Feelings")]
  [SerializeField]
  float yawRate = 100;
  [SerializeField]
  float preciseYawRate = 20;
  [SerializeField]
  float shotForceMax = 4000;
  [SerializeField]
  float shotForceMin = 100;
  [SerializeField]
  float shotForceChargeTime = 5.0f;
  [SerializeField]
  float cueZTouching = -2.5f;
  [SerializeField]
  float cueZFarthestBack = -3.6f;


  [Header("Scene Objects")]
  [SerializeField]
  GameObject BallParent;

  [Header("Internal Objects")]
  [SerializeField]
  GameObject CueStick;

  [SerializeField]
  AudioSource CueHitSFX;

  private Ball _currentBall;
  public Ball CurrentBall {
    get { return _currentBall; }
    set {
      _currentBall = value;

      transform.position = _currentBall.transform.position +
        //want to be "on ground"
        -(Vector3.up * _currentBall.transform.localScale.y * 0.5f);

      transform.eulerAngles = new Vector3(0, _currentBall.transform.eulerAngles.y, 0);
    }
  }

  protected float yawing = 0;
  protected bool preciseYaw = false;

  private bool _preparingCueShot = false;
  protected bool PreparingCueShot {
    get { return _preparingCueShot; }
    set {
      _preparingCueShot = value;
      //CueStick.transform.localPosition = new Vector3(0, 0.5f, _preparingCueShot ? cueZFarthestBack : cueZTouching);
      if (!_preparingCueShot) {
        ShotForceCharged = shotForceMin;
      }
    }
  }
  private float _shotForceCharged = 0;
  protected float ShotForceCharged {
    get { return _shotForceCharged; }
    set {
      _shotForceCharged = value;
      //TODO: display non-linearly for dramatic reasons
      CueStick.transform.localPosition = new Vector3(0, 0.5f, cueZTouching +
        ((cueZFarthestBack - cueZTouching) * (ShotForceCharged - shotForceMin) / (shotForceMax - shotForceMin)));
    }
  }

  public event Action<Ball> TakeShot;
  public event Action AllBallsStopped;
  public event Action AnyAction;
  public event Action PuckAcknowledges;
  public event Action<int> Next;

  private void Start() {
    CurrentBall = BallParent.GetComponentsInChildren<Ball>().First();
    Debug.Log("Current Ball By Name: " + CurrentBall + transform.eulerAngles.ToString());
  }

  void ChooseNextBall(int dirn) {
    if (dirn == 0) {
      return;
    }
    if (!CurrentBall) {
      CurrentBall = BallParent.GetComponentsInChildren<Ball>().First();
      return;
    }

    var ballArray = BallParent.GetComponentsInChildren<Ball>();
    int currentIndex = System.Array.IndexOf(ballArray, CurrentBall);

    int nextIndex = Utils.WrapClamp(currentIndex, dirn, ballArray.Length);
    CurrentBall = ballArray[nextIndex];
  }

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
  public void OnMove(InputValue value) {
    PreparingCueShot = false;

    var v = value.Get<Vector2>();
    ChooseNextBall((int)v.y);

    yawing = v.x;
    AnyAction();
  }

  public void OnNext(InputValue value) {
    Next((int)value.Get<float>());
  }

  public void OnJump(InputValue value) {
    AnyAction();

    if (yawing != 0) {
      PreparingCueShot = false;
      return;
    }

    if (PreparingCueShot && !value.isPressed) {
      //Hit the ball!
      var rb = CurrentBall.GetComponent<Rigidbody>();
      rb.AddForce(transform.forward * ShotForceCharged);
      CueHitSFX.PlayOneShot(CueHitSFX.clip);
      this.TakeShot(CurrentBall);
      StartCoroutine(WaitAllBallsStoppedMoving());
    }
    if (!PreparingCueShot && value.isPressed) {
      //start pulling back
      ShotForceCharged = shotForceMin;
    }
    PreparingCueShot = value.isPressed;
  }

  public void OnSprint(InputValue value) {
    preciseYaw = value.isPressed;
  }

  public void OnAcknowledge(InputValue value) {
    AnyAction();
    if (value.isPressed) {
      PuckAcknowledges();
    }
  }
#endif

  protected void Update() {
    PrepareCueShot();
    Yaw();
  }

  void PrepareCueShot() {
    if (PreparingCueShot) {
      ShotForceCharged += Time.deltaTime * (shotForceMax - shotForceMin) / shotForceChargeTime;
    }
  }

  void Yaw() {
    var currentYawRate = preciseYaw ? preciseYawRate : yawRate;
    transform.eulerAngles -= Vector3.up * yawing * currentYawRate * Time.deltaTime;
  }

  IEnumerator WaitAllBallsStoppedMoving() {
    yield return new WaitForSeconds(0.5f);

    Func<bool> allBallsStoppedMoving = () =>
    {
      var rbs = BallParent.GetComponentsInChildren<Ball>().Select(ball => ball.GetComponent<Rigidbody>());
      var awakeRBs =
        //https://docs.unity3d.com/Manual/RigidbodiesOverview.html
        rbs.Where(rb => !rb.IsSleeping());

      return awakeRBs.Count() == 0;
    };

    while (true) {
      yield return null;
      if (allBallsStoppedMoving()) {
        break;
      }
    }

    Debug.Log("All Balls Stopped Moving");
    AllBallsStopped();
  }

}
