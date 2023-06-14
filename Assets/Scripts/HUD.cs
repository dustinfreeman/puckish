using UnityEngine;

public class HUD : MonoBehaviour {
  [SerializeField]
  GameObject HudPlane;

  void FixedUpdate() {
    RaycastHit[] hits = Physics.SphereCastAll(
      transform.position, 0.4f, transform.forward,
      Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Collide);
    string hudString = "";
    Vector3 closestHit = transform.position + (transform.forward * 10000);
    foreach (var _hit in hits) {
      if (_hit.collider.GetComponent<TargetCollider>() ||
        _hit.collider.GetComponent<Ball>()) {
        if ((transform.position - closestHit).magnitude >
            (transform.position - _hit.point).magnitude
          ) {
          closestHit = _hit.point;
          hudString = _hit.collider.name;
        }
      }
    }
    HudPlane.SetActive(hudString.Length > 0);
    if (hudString.Length > 0) {
      //Debug.LogFormat("closestHit {0}", hudString);
      HudPlane.transform.position = closestHit;
      HudPlane.transform.rotation = transform.rotation;
      HudPlane.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = hudString;
    }
  }
}
