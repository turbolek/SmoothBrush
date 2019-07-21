using UnityEngine;
using System.Collections.Generic;

public class BrushPool
{
    private GameObject brushPrefab;

    private Queue<GameObject> freeInstances;
    private List<GameObject> allInstances;

    public BrushPool(GameObject brushPrefab, int poolSize)
    {
        this.brushPrefab = brushPrefab;

        allInstances = new List<GameObject>();
        freeInstances = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateInstance();
        }
    }

    public GameObject GetInstance()
    {
        if (freeInstances.Count == 0)
        {
            CreateInstance();
        }

        GameObject instance = freeInstances.Dequeue();
        instance.SetActive(true);
        return instance;

    }

    public void ReturnAllInstancesToPool()
    {
        foreach (GameObject instance in allInstances)
        {
            instance.gameObject.SetActive(false);
            freeInstances.Enqueue(instance);
        }
    }

    private void ReturnInstanceToPool(GameObject instance)
    {
        instance.gameObject.SetActive(false);
        if (!freeInstances.Contains(instance))
        {
            freeInstances.Enqueue(instance);
        }
    }

    private void CreateInstance()
    {
        GameObject newInstance = GameObject.Instantiate(brushPrefab);
        newInstance.SetActive(false);
        allInstances.Add(newInstance);
        freeInstances.Enqueue(newInstance);
    }


}
