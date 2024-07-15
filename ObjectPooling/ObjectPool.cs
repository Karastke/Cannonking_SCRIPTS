using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string layerName;
        public GameObject prefab;
        public int size;
    }

    #region Singleton
    public static ObjectPool Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
{
    poolDictionary = new Dictionary<string, Queue<GameObject>>();

    foreach (Pool pool in pools)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();
        GameObject poolContainer = new GameObject(pool.layerName + " Pool");

        for (int i = 0; i < pool.size; i++)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.AddComponent<PlayerBallDebug>();
            obj.SetActive(false);
            obj.layer = LayerMask.NameToLayer(pool.layerName);
            obj.transform.parent = poolContainer.transform;  // 부모 오브젝트 설정
            objectPool.Enqueue(obj);
        }

        poolDictionary.Add(pool.layerName, objectPool);
    }
}

    public GameObject GetFromPool(string layerName, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(layerName))
        {
            Debug.LogWarning("Pool with layer " + layerName + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[layerName].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[layerName].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        // 여기서 추가적인 리셋 로직을 구현할 수 있습니다.
        // 예: obj.transform.position = Vector3.zero;
    }
}