using UnityEngine;

public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instanse { get; private set; }
    protected virtual void Awake() => Instanse = this as T;

    private void OnApplicationQuit()
    {
        Instanse = null;
        Destroy(gameObject);
    }
}

public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instanse != null) Destroy(gameObject);
        base.Awake();
    }
}


public abstract class SingletonPersistent<T> : StaticInstance<T> where T : MonoBehaviour
{
    [Header("Singleton settings")]
    public bool IsRootObject = true;
    protected override void Awake()
    {
        if (Instanse == null)
        {
            base.Awake();
            if (IsRootObject)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject.transform.parent.gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}