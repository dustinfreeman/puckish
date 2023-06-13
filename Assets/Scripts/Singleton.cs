using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour
    where T : Singleton<T> {

    public static T Instance { get; private set; }

    void MakeSingleton() {
        if (Instance != null && Instance != this) {
            Debug.LogWarning("Already have a Singleton instance of class " + this.GetType() +
                ". Overwriting :(. Previous singleton game object: " + Instance.gameObject + ". This game object: " + gameObject);
        }
        Instance = (T)this;
    }

    protected virtual void Awake() {
        //Debug.Log("Singleton Awake");
        MakeSingleton();
    }

    void OnEnable() {
        //somehow Instance will become null, even if it wasn't post-Awake
        MakeSingleton();
    }

    protected static void CreateSingleton() {
        GameObject gameObject = new GameObject(typeof(T).Name);
        Instance = gameObject.AddComponent<T>();
    }
}
