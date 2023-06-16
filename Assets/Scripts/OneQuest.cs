using UnityEngine;


public class OneQuest : MonoBehaviour {
  public enum QuestState {
    None,
    Passed,
    Failed
  }

  [SerializeField]
  public TMPro.TextMeshProUGUI text;

  [SerializeField]
  GameObject CheckPassed;
  [SerializeField]
  GameObject CheckFailed;

  public QuestState State {
    set {
      CheckFailed.SetActive(value == QuestState.Failed);
      CheckPassed.SetActive(value == QuestState.Passed);
    }
  }

  private void Awake() {
    State = QuestState.None;
    text.text = "";
  }
}
