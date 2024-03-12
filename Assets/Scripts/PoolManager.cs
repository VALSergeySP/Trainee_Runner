using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    class Pool
    {
        List<GameObject> inactive = new List<GameObject>();
        GameObject prefab;

        public Pool(GameObject prefab) { this.prefab = prefab; }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            GameObject obj = null;

            if(inactive.Count == 0)
            {
                obj = Instantiate(prefab, position, rotation);
                obj.name = prefab.name;
                obj.transform.SetParent(Singleton.Instance.PoolManagerInstance.transform);
            } else
            {
                obj = inactive[^1];
                inactive.RemoveAt(inactive.Count - 1);
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true); 
            
            return obj;
        }

        public void Despawn(GameObject obj)
        {
            obj.SetActive(false);
            inactive.Add(obj);
        }
    }

    Dictionary<string, Pool> allPools = new();

    void Initialization(GameObject prefab)
    {
        if(prefab != null && !allPools.ContainsKey(prefab.name))
        {
            allPools[prefab.name] = new Pool(prefab);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Initialization(prefab);

        return allPools[prefab.name].Spawn(position, rotation);
    }

    public void Despawn(GameObject obj)
    {
        if(allPools.ContainsKey(obj.name))
        {
            allPools[obj.name].Despawn(obj);
        } else
        {
            Destroy(obj);
        }
    }
}
