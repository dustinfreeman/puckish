using UnityEngine;
using UnityEngine.ProBuilder;
using static TiltFive.Input;

public class T5ForPuckish : MonoBehaviour {
  [SerializeField]
  protected GameManager Manager;
  [SerializeField]
  protected TiltFive.GameBoard T5GameBoard;

  [SerializeField]
  protected VirtualCueTip VirtualCueTip;

  [SerializeField]
  protected GameObject DebugWandIndicator;

  protected void Start() {
    Puck.Instance.ViewpointSet += ViewpointSet;
  }

  private void ViewpointSet(Transform transform) {
    Vector3 position = transform.position;
    if (Manager.CurrentHole()) {
      int meanCount = 1;
      foreach (var success in Manager.CurrentHole().SuccessDefns) {
        position += Manager.GetTarget(success.Target).transform.position;
        meanCount++;
      }
      position.Scale(Vector3.one * 1.0f / meanCount);
    }

    T5GameBoard.transform.position = new Vector3(
      position.x,
      //don't raise/lower game board
      T5GameBoard.transform.position.y,
      position.z);

  }

  float _prevTriggerDisplacement = 0f;
  float TRIGGER_MIN_HYSTERESIS = 0.2f;
  float TRIGGER_MAX_HYSTERESIS = 0.8f;

  Vector2 _prevStickTilt = new Vector2(-10, -10);

  private void CheckWandInput() {
    //Vector2 stickTilt;
    //TiltFive.Input.TryGetStickTilt(out stickTilt);
    //if (!_prevStickTilt.Equals(stickTilt)) {
    //  _prevStickTilt = stickTilt;
    //  Puck.Instance.DoYaw(stickTilt.x);
    //}

    //Wand Buttons
    if (TiltFive.Input.GetButtonDown(TiltFive.Input.WandButton.Two)) {
      Puck.Instance.Acknowledge();
      return;
    }

    float triggerDisplacement = -1;
    TiltFive.Input.TryGetTrigger(out triggerDisplacement);
    if (triggerDisplacement > _prevTriggerDisplacement && triggerDisplacement > TRIGGER_MAX_HYSTERESIS) {
      Puck.Instance.OnCueSqueezed(true);
    }
    if (triggerDisplacement < _prevTriggerDisplacement && triggerDisplacement < TRIGGER_MIN_HYSTERESIS) {
      Puck.Instance.OnCueSqueezed(false);
    }
    _prevTriggerDisplacement = triggerDisplacement;

    if (TiltFive.Input.GetButtonDown(TiltFive.Input.WandButton.X)) {
      Manager.SkipToNext();
    }
  }

  private void Update() {
    CheckWandInput();

    if (TiltFive.Input.GetWandAvailability()) {
      //Debug.Log(TiltFive.Wand.GetPosition() + " : " + TiltFive.Wand.GetRotation());

      var wandPos = TiltFive.Wand.GetPosition();
      var wandRot = TiltFive.Wand.GetRotation();
      DebugWandIndicator.transform.position = wandPos;
      DebugWandIndicator.transform.rotation = wandRot;

      Puck.Instance.transform.eulerAngles = new Vector3(0, DebugWandIndicator.transform.eulerAngles.y, 0);
      VirtualCueTip.SetWandTipPos(wandPos + wandRot * Vector3.forward * 5.5f);
    }
  }
}
