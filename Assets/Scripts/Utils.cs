static class Utils {
  public static int WrapClamp(int current, int delta, int length) {
    int next = current + delta;
    if (next < 0) {
      next = length - 1;
    }
    if (next == length) {
      next = 0;
    }
    return next;
  }

  public static float ChooseRandom(float[] arr) {
    return arr[UnityEngine.Random.Range(0, arr.Length)];
  }
}
