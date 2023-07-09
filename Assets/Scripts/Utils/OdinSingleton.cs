
using Sirenix.OdinInspector;

namespace Utils
{
    public abstract class OdinStaticInstance<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
    {
        public static T Instanse { get; private set; }
        protected virtual void Awake() => Instanse = this as T;

        private void OnApplicationQuit()
        {
            Instanse = null;
            Destroy(gameObject);
        }
    }

    public abstract class OdinSingleton<T> : OdinStaticInstance<T> where T : SerializedMonoBehaviour
    {
        protected override void Awake()
        {
            if (Instanse != null) Destroy(gameObject);
            base.Awake();
        }
    }
}
