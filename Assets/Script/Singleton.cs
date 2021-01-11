using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    #region Singleton
    protected static T instance;
    public static T Instance
    {
        get 
        {
            if(instance == null)
            {
                instance = FindObjectOfType<T>();
                if(FindObjectOfType<T>() == null)
                {
                    GameObject gameObject = new GameObject();
                    var singletonComponent = gameObject.AddComponent<T>();
                    gameObject.name = typeof(T).ToString();
                    instance = singletonComponent;
                }
            }
            return instance;
        }
    }
    protected virtual void Awake()
    {
        instance = gameObject.GetComponent<T>();
        OnAwake();
    }
    #endregion
    protected virtual void OnAwake()
    {

    }
}
