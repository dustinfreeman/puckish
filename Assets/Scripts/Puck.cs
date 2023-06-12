using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Puck : MonoBehaviour {

  [Header("Feelings")]
  [SerializeField]
  float yawRate = 100;
  [SerializeField]
  float preciseYawRate = 20;
  [SerializeField]
  float shotForce = 600;

  [Header("Scene Objects")]
  [SerializeField]
  GameObject BallParent;

  private Ball _currentBall;
  protected Ball currentBall {
    get { return _currentBall; }
    set {
      _currentBall = value;

      transform.position = _currentBall.transform.position +
        //want to be "on ground"
        -(Vector3.up * _currentBall.transform.localScale.y * 0.5f);

      //TODO: reset rotation?
      //transform.eulerAngles = Vector3.zero;
    }
  }

  protected float yawing = 0;
  protected bool preciseYaw = false;

  private void Start() {
    currentBall = BallParent.GetComponentsInChildren<Ball>().First();
    Debug.Log("Current Ball By Name: " + currentBall + transform.eulerAngles.ToString());
  }

  void ChooseNextBall(int dirn) {
    if (dirn == 0) {
      return;
    }
    if (!currentBall) {
      currentBall = BallParent.GetComponentsInChildren<Ball>().First();
      return;
    }

    var ballArray = BallParent.GetComponentsInChildren<Ball>();
    int currentIndex = System.Array.IndexOf(ballArray, currentBall);

    //TODO: surely I can do this index rotation more tersely?
    int nextIndex = (currentIndex + dirn) % ballArray.Length;
    if (nextIndex < 0) {
      nextIndex = ballArray.Length - 1;
    }

    currentBall = ballArray[nextIndex];
  }

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
  public void OnMove(InputValue value) {
    var v = value.Get<Vector2>();
    ChooseNextBall((int)v.y);

    yawing = v.x;
  }

  public void OnJump(InputValue value) {
    //Hit the ball!
    if (value.isPressed) {
      var rb = currentBall.GetComponent<Rigidbody>();
      rb.AddForce(transform.forward * shotForce);
      StartCoroutine(WaitAllBallsStoppedMoving());
    }
  }

  public void OnSprint(InputValue value) {
    preciseYaw = value.isPressed;
  }
#endif

  protected void Update() {
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
  }

}
