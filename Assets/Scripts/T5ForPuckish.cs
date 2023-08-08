using UnityEngine;
using static TiltFive.Input;

public class T5ForPuckish : MonoBehaviour {

  Vector2 _prevStickTilt = new Vector2(-10, -10);

  private void CheckWandInput() {
    float triggerDisplacement = -1;
    TiltFive.Input.TryGetTrigger(out triggerDisplacement);
    //Debug.Log("Tilt Five trigger: " + triggerDisplacement);
    const float TRIGGER_THRESHOLD = 0.9f;
    //if (triggerDisplacement == TRIGGER_THRESHOLD ||
    //    triggerDisplacement == 0) {
    //  //Puck.Instance.OnCueSqueezed(triggerDisplacement == TRIGGER_THRESHOLD);
    //  return;
    //}

    const WandButton CueButton = TiltFive.Input.WandButton.One;
    if (TiltFive.Input.GetButtonDown(CueButton)) {
      Puck.Instance.OnCueSqueezed(true);
    }
    if (TiltFive.Input.GetButtonUp(CueButton)) {
      Puck.Instance.OnCueSqueezed(false);
    }
    if (TiltFive.Input.GetButton(CueButton)) {
      return; //do not proceed further
    }

    Vector2 stickTilt;
    TiltFive.Input.TryGetStickTilt(out stickTilt);
    if (!_prevStickTilt.Equals(stickTilt)) {
      _prevStickTilt = stickTilt;
      Puck.Instance.DoYaw(stickTilt.x);
    }
  }

  private void Update() {
    CheckWandInput();

    //if (TiltFive.Input.GetButtonUp(TiltFive.Input.WandButton.Three)) {
    //  Debug.Log("Tilt Five Down: Three");
    //}
  }
}
