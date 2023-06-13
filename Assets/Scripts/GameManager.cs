using UnityEngine;

public class GameManager : MonoBehaviour {
  [SerializeField]
  TMPro.TextMeshPro OverlayText;

  void Start() {
    OverlayText.text = @"THE GARDEN:
A place of education

";
  }
}
