using System;
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
  public GameObject BallParent;

  [Header("Internal Objects")]
  [SerializeField]
  GameObject CueStick;
  [SerializeField]
  GameObject CurrentBallIndicator;
  [SerializeField]
  TMPro.TextMeshProUGUI CurrentBallHUD;
  [SerializeField]
  AudioSource CueHitSFX;

  public event Action<Transform> ViewpointSet;
  public void SetViewpoint(Transform viewpointTransform) {
    //Called to set where the 2D game's cue should be centered around.

    transform.position = viewpointTransform.position +
      //want to be "on ground"
      -(0.5f * viewpointTransform.localScale.y * Vector3.up);
    transform.eulerAngles = new Vector3(0, viewpointTransform.eulerAngles.y, 0);

    ViewpointSet?.Invoke(viewpointTransform);
  }

  private Ball _currentBall = null;
  public Ball CurrentBall {
    get { return _currentBall; }
    set {
      _currentBall = value;
      CanTakeShot = (bool)_currentBall;

      CurrentBallHUD.text = _currentBall ? CurrentBall.name : "";

      if (!_currentBall) { return; }
      Debug.Log("Current Ball By Name: " + CurrentBall + transform.eulerAngles.ToString());
      CurrentBall.PlayBark(BarkType.turn_start);

      SetViewpoint(_currentBall.transform);
    }
  }

  private bool _canTakeShot = true;
  public bool CanTakeShot {
    get { return _canTakeShot; }
    set {
      _canTakeShot = value;
      CueStick.SetActive(_canTakeShot);
      CurrentBallIndicator.SetActive(_canTakeShot);
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

  public event Action<Ball> TookShot;
  public event Action AnyAction;
  public event Action PuckAcknowledges;
  public event Action<int> Next;

  protected override void Awake() {
    base.Awake();
    CurrentBall = null;
  }

  void ChooseNextBall(int dirn) {
    if (dirn == 0) {
      return;
    }
    PreparingCueShot = false;
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
  public void OnExit() {
    Application.Quit();
  }

  public void OnMove(InputValue value) {
    var v = value.Get<Vector2>();
    ChooseNextBall((int)v.y);
    DoYaw(v.x);
  }

  public void DoYaw(float yaw) {
    Debug.Log("DoYaw " + yaw);
    PreparingCueShot = false;
    yawing = yaw;
    AnyAction();
  }

  public void OnNext(InputValue value) {
    Next((int)value.Get<float>());
  }

  public void OnJump(InputValue value) {
    OnCueSqueezed(value.isPressed);
  }

  public void TakeTheShot(Vector3 force) {
    if (!CurrentBall) {
      Debug.LogWarning("Attempt to take shot, but there is no CurrentBall");
      return;
    }

    var rb = CurrentBall.GetComponent<Rigidbody>();
    rb.AddForce(force);
    CueHitSFX.PlayOneShot(CueHitSFX.clip);
    this.TookShot(CurrentBall);
    CurrentBall = null;
  }

  public void OnCueSqueezed(bool isSqueezed) {
    AnyAction();

    if (yawing != 0) {
      PreparingCueShot = false;
      return;
    }
    if (!CanTakeShot) { return; }

    if (PreparingCueShot && !isSqueezed) {
      TakeTheShot(transform.forward * ShotForceCharged);
    }
    if (!PreparingCueShot && isSqueezed) {
      //start pulling back
      ShotForceCharged = shotForceMin;
    }
    PreparingCueShot = isSqueezed;
  }

  public void OnSprint(InputValue value) {
    SetPreciseYaw(value.isPressed);
  }

  public void SetPreciseYaw(bool _preciseYaw) {
    preciseYaw = _preciseYaw;
  }

  public void OnAcknowledge(InputValue value) {
    AnyAction();
    if (value.isPressed) {
      Acknowledge();
    }
  }

  public void Acknowledge() {
    PuckAcknowledges();
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
}
