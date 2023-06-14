using System;
using UnityEngine;

[Serializable]
public struct BallStartTransform {
  public string BallName;
  public Transform StartTransform;
}

[Serializable]
public struct SuccessDefn {
  public string BallName;
  public string Target;
}

public class HoleDefn : Act {
  public string Ball; //Player-Controlled
  public BallStartTransform[] BallStartTransforms;
  public SuccessDefn[] SuccessDefns;
  public int Par = 0;
}
