using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    private Dictionary<GameObject, Queue<GameObject>> pool = new();

    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);

    }

    public GameObject GetObject(GameObject prefab)
    {
        if (!pool.TryGetValue(prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pool[prefab] = queue;
        }

        GameObject obj;

        if (queue.Count > 0)
        {
            obj = queue.Dequeue();
        }
        else
        {
            obj = Instantiate(prefab);
            obj.AddComponent<Poolable>();
        }

        obj.GetComponent<Poolable>().prefab = prefab;
        obj.SetActive(true);

        ParticleSystem[] systems = obj.GetComponentsInChildren<ParticleSystem>();
        if (systems != null)
        {
            foreach (ParticleSystem ps in systems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Clear();
                ps.Play();
            }

        }
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        var poolable = obj.GetComponent<Poolable>();
        if (poolable == null || poolable.prefab == null)
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);

        if (!pool.TryGetValue(poolable.prefab, out var queue))
        {
            queue = new Queue<GameObject>();
            pool[poolable.prefab] = queue;
        }

        queue.Enqueue(obj);
    }
}