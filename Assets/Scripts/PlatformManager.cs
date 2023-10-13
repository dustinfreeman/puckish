using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlatformManager : MonoBehaviour {
  [SerializeField]
  GameObject T5Subsystem;
  [SerializeField]
  GameObject TwoDCamera;

  public void ToT5() {
    T5Subsystem.SetActive(true);
    TwoDCamera.SetActive(false);
  }

  public void To2D() {
    T5Subsystem.SetActive(false);
    TwoDCamera.SetActive(true);
  }

  public static PlatformManager GetInstance() {
    return GameObject.FindObjectOfType<PlatformManager>();
  }
}
