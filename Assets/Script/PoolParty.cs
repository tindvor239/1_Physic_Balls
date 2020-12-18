using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolParty : MonoBehaviour
{
    [SerializeField]
    private List<Pool> pools;
    #region Properties
    public List<Pool> Pools { get => pools; }
    public Pool GetPool(string name)
    {
        foreach(Pool pool in pools)
        {
            if (pool.Name == name)
                return pool;
        }
        return null;
    }
    #endregion
    public void Expand(int amount)
    {
        for(int i = 0; i < amount; i++)
        {
            Pool pool = new Pool();
            pools.Add(pool);
        }
    }
    public GameObject CreateItem(Pool pool, Vector2 position, int index, Transform parent)
    {
        GameObject newGameObject = Instantiate(pool.ObjectsToPool[index], parent);
        newGameObject.transform.position = position;
        pool.ObjectsPool.Add(newGameObject);
        return newGameObject;
    }
}
