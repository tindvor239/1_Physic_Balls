using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    [SerializeField]
    private string name;
    [SerializeField]
    private List<GameObject> objectsPool;
    [SerializeField]
    private GameObject[] objectsToPool;
    #region Properties
    public List<GameObject> ObjectsPool { get => objectsPool; }
    public string Name { get => name; }
    public GameObject[] ObjectsToPool { get => objectsToPool; }
    public bool CanExtend { get => IsExtend(); }
    #endregion
    private void SetPooledObject(GameObject gameObject, bool isActive)
    {
        gameObject.SetActive(isActive);
        if(gameObject.GetComponent<Collider2D>())
            gameObject.GetComponent<Collider2D>().enabled = isActive;

    }
    private bool IsExtend()
    {
        if(ObjectsPool.Count > 0)
        {
            foreach(GameObject gameObj in objectsPool)
            {
                if (gameObj.activeInHierarchy == false)
                    return false;
            }
        }
        return true;
    }
    public void GetBackToPool(GameObject obj, Vector2 position)
    {
        SetPooledObject(obj, false);
        obj.transform.position = position;
    }

    public GameObject GetOutOfPool(Vector2 position)
    {
        foreach(GameObject obj in objectsPool)
        {
            if(obj.activeInHierarchy == false)
            {
                SetPooledObject(obj, true);
                obj.transform.position = position;
                return obj;
            }
        }
        return null;
    }
}
