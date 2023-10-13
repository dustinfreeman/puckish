using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuckishEditorMenu : MonoBehaviour {
  
  [MenuItem("Puckish/Platform To T5")]
  static void PlatformToT5() {
    PlatformManager.GetInstance().ToT5();
  }

  [MenuItem("Puckish/Platform To 2D")]
  static void PlatformTo2D() {
    PlatformManager.GetInstance().To2D();
  }
}
