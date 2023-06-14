public class Course : Singleton<Course> {
  public HoleDefn[] GetHoles() {
    return GetComponentsInChildren<HoleDefn>();
  }
}
