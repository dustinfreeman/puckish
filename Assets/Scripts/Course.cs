public class Course : Singleton<Course> {
  public Act[] GetActs() {
    return GetComponentsInChildren<Act>();
  }
}
