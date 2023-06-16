using System.Collections.Generic;

public enum InteractionType {
  BallBall,
  BallTarget,
  BallWall,
}

public struct Interaction {
  public float time;
  public InteractionType type;
  public Ball ball;
  public string other; //name of ball, target or wall
}

public static class InteractionRegistry {
  public static List<Interaction> Interactions = new();
}
