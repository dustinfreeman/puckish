using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
  [SerializeField]
  TMPro.TextMeshProUGUI[] OverlayText;
  [SerializeField]
  GaffeAlert[] Gaffer;
  [SerializeField]
  GameObject[] EncounterTasksParent;

  public void SetOverlayText(string text) {
    foreach (var t in OverlayText) {
      t.text = text;
    }
  }

  public void ShowGaffe(string text) {
    foreach (var gaffer in Gaffer) {
      gaffer.gameObject.SetActive(text != "");
      gaffer.SubText.text = text;
    }
  }

  public void EncounterTasksHideAll() {
    foreach (var encounterParent in EncounterTasksParent) {
      foreach (var questUI in encounterParent.GetComponentsInChildren<OneQuest>()) {
        questUI.gameObject.SetActive(false);
      }
    }
  }

  public void EncounterTaskSetup(int i, string successDescription = "") {
    foreach (var encounterParent in EncounterTasksParent) {
      var questUI = encounterParent.GetComponentsInChildren<OneQuest>(true)[i];
      questUI.State = OneQuest.QuestState.None;
      questUI.gameObject.SetActive(true);
      questUI.text.text = successDescription;
    }
  }

  public void EncounterTaskUpdate(int i, OneQuest.QuestState state) {
    foreach (var encounterParent in EncounterTasksParent) {
      var questUI = encounterParent.GetComponentsInChildren<OneQuest>(true)[i];
      questUI.State = state;
    }
  }

}
