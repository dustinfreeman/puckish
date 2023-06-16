using UnityEngine;

public class GaffeAlert : MonoBehaviour {
  [SerializeField]
  public TMPro.TextMeshPro SubText;

  private void Start() {
    gameObject.SetActive(false);
  }
}
