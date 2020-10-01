using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable] public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    //untuk memuat prefab pool
    public List<Pool> pools;
    //untuk mengambil objek dalam pool yang sesuai dengan tagnya
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    //singleton untuk object pooler
    public static ObjectPooler Instance;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        AddIntoPool();
    }

    //menambahkan prefab object ke dalam pool
    void AddIntoPool()
    {
        pools.ForEach((pool) =>
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            int i = 0;
            while(i < pool.size)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                i++;
            }
            poolDictionary.Add(pool.tag, objectPool);
        });
    }

    //mengambil object dari pool
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        //cek jika dalam poolDictionary terdapat objek yang di request
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Object with tag " + tag + " doesnt exist");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }
}
