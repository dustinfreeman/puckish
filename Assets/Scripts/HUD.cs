using UnityEngine;

public class HUD : MonoBehaviour {
  [SerializeField]
  GameObject DistantTargetHUD;

  void FixedUpdate() {
    string hudString = "";
    Vector3 closestHit = transform.position + (transform.forward * 10000);
    RaycastHit[] hits = Physics.SphereCastAll(
      transform.position,
      0.4f, transform.forward,
      Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Collide);
    foreach (var _hit in hits) {
      if (_hit.collider.GetComponent<TargetCollider>() ||
        _hit.collider.GetComponent<Ball>()) {

        if (Puck.Instance.CurrentBall == _hit.collider.GetComponent<Ball>()) {
          //exclude the current ball
          continue;
        }

        if ((transform.position - closestHit).magnitude >
            (transform.position - _hit.point).magnitude
          ) {
          closestHit = _hit.point;
          hudString = _hit.collider.name;
        }
      }
    }

    //RaycastHit hit;
    //Physics.SphereCast(transform.position, 0.4f, transform.forward,
    //  out hit,
    //  Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Collide);
    //if (hit.collider.GetComponent<Ball>() || hit.collider.GetComponent<TargetCollider>()) {
    //  hudString = hit.collider.name;
    //  closestHit = hit.point;
    //}

    DistantTargetHUD.SetActive(hudString.Length > 0);
    if (hudString.Length > 0) {
      DistantTargetHUD.transform.SetPositionAndRotation(closestHit, transform.rotation);
      var text = DistantTargetHUD.GetComponentInChildren<TMPro.TextMeshProUGUI>();
      text.text = hudString;
      float textScale = Mathf.Sqrt((transform.position - closestHit).magnitude);
      text.transform.localScale = Vector3.one * textScale;
    }
  }
}
