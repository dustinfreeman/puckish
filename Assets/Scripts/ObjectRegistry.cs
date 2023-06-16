using System.Collections.Generic;
using UnityEngine;

public class ObjectRegistry : MonoBehaviour {

  protected readonly static Dictionary<string, TargetCollider> targets = new();
  public static void RegisterTarget(TargetCollider target) {
    if (targets.ContainsKey(target.name)) {
      Debug.LogErrorFormat("Found Duplicate Target Name: {0}", target.name);
      return;
    }
    targets.Add(target.name, target);
  }

  protected readonly static Dictionary<string, Ball> balls = new();
  public static void RegisterBall(Ball ball) {
    if (balls.ContainsKey(ball.name)) {
      Debug.LogErrorFormat("Found Duplicate Ball Name: {0}", ball.name);
      return;
    }
    balls.Add(ball.name, ball);
  }
  protected void SanityCheckCourse(Act[] course) {
    foreach (var act in course) {
      if (!(act is HoleDefn)) {
        continue;
      }
      var hole = (HoleDefn)act;
      if (!balls.ContainsKey(hole.Ball)) {
        Debug.LogErrorFormat("{1}: Missing Ball with name {0}", hole.Ball, hole.name);
      }
      foreach (var ballStart in hole.BallStartTransforms) {
        if (!balls.ContainsKey(ballStart.BallName)) {
          Debug.LogErrorFormat("{1}: Missing Ball with name {0}", ballStart.BallName, hole.name);
        }
      }
      foreach (var successDefn in hole.SuccessDefns) {
        if (!balls.ContainsKey(successDefn.BallName)) {
          Debug.LogErrorFormat("{1}: Missing Ball with name {0}", successDefn.BallName, hole.name);
        }
        if (!targets.ContainsKey(successDefn.Target)) {
          Debug.LogErrorFormat("{1}: Missing Target with name {0}", successDefn.Target, hole.name);
        }
      }
    }
  }

}
