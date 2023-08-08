using UnityEngine;
using static TiltFive.Input;

public class T5ForPuckish : MonoBehaviour {
  [SerializeField]
  protected GameObject DebugWandIndicator;

  float _prevTriggerDisplacement = 0f;
  float TRIGGER_MIN_HYSTERESIS = 0.2f;
  float TRIGGER_MAX_HYSTERESIS = 0.8f;

  Vector2 _prevStickTilt = new Vector2(-10, -10);

  private void CheckWandInput() {
    if (TiltFive.Input.GetButtonDown(TiltFive.Input.WandButton.Two)) {
      Puck.Instance.Acknowledge();
      return;
    }

    float triggerDisplacement = -1;
    TiltFive.Input.TryGetTrigger(out triggerDisplacement);
    const WandButton CueButton = TiltFive.Input.WandButton.One;
    if (triggerDisplacement > _prevTriggerDisplacement && triggerDisplacement > TRIGGER_MAX_HYSTERESIS) {
      Puck.Instance.OnCueSqueezed(true);
    }
    if (triggerDisplacement < _prevTriggerDisplacement && triggerDisplacement < TRIGGER_MIN_HYSTERESIS) {
      Puck.Instance.OnCueSqueezed(false);
    }
    _prevTriggerDisplacement = triggerDisplacement;
    if (TiltFive.Input.GetButton(CueButton)) {
      return; //do not proceed further
    }

    //const WandButton PreciseButton = TiltFive.Input.WandButton.Two;
    //bool preciseButtonDown = false;
    //TiltFive.Input.TryGetButtonDown(PreciseButton, out preciseButtonDown);
    //Puck.Instance.SetPreciseYaw(preciseButtonDown);

    Vector2 stickTilt;
    TiltFive.Input.TryGetStickTilt(out stickTilt);
    if (!_prevStickTilt.Equals(stickTilt)) {
      _prevStickTilt = stickTilt;
      Puck.Instance.DoYaw(stickTilt.x);
    }
  }

  private void Update() {
    CheckWandInput();

    if (TiltFive.Input.GetWandAvailability()) {
      Debug.Log(TiltFive.Wand.GetPosition() + " : " + TiltFive.Wand.GetRotation());

      DebugWandIndicator.transform.position = TiltFive.Wand.GetPosition();
      DebugWandIndicator.transform.rotation = TiltFive.Wand.GetRotation();

      Puck.Instance.transform.eulerAngles = new Vector3(0, DebugWandIndicator.transform.eulerAngles.y, 0);
    }

  }
}
