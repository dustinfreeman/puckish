using UnityEngine;

public class TargetCollider : MonoBehaviour {
  public static bool DoCollidersOverlap(GameObject a, GameObject b) {
    Vector3 direction; float distance;
    bool collides = Physics.ComputePenetration(a.GetComponent<Collider>(), a.transform.position, a.transform.rotation,
      b.GetComponent<Collider>(), b.transform.position, b.transform.rotation, out direction, out distance);
    return collides;
  }

  protected void Awake() {
    ObjectRegistry.RegisterTarget(this);

    if (!GetComponent<Collider>().isTrigger) {
      Debug.LogErrorFormat("My Collider should be isTrigger, but it is not: {0}", gameObject.name);
    }
  }

  private void Update() {
    //Debug.Log(DoCollidersOverlap(Puck.Instance.CurrentBall.gameObject, gameObject));
  }

}
