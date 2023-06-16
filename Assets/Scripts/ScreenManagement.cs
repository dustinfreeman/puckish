using UnityEngine;

public class ScreenManagement : MonoBehaviour {
  private void Start() {
#if UNITY_STANDALONE
    //fit UI to 16:10 ratio
    //Screen.SetResolution(1280, 800, FullScreenMode.FullScreenWindow);
    //https://forum.unity.com/threads/game-build-looks-different-than-game-in-editor.1339562/
#endif
  }
}
