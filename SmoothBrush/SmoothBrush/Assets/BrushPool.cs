using UnityEngine;
using System.Collections.Generic;

public class BrushPool
{
    private struct BrushInstance
    {
        public Transform transform;
        public GameObject gameObject;
    }

    private GameObject brushPrefab;

    private Queue<BrushInstance> freeInstances;
    private List<BrushInstance> allInstances;

    public BrushPool(GameObject brushPrefab, int poolSize)
    {
        this.brushPrefab = brushPrefab;

        allInstances = new List<BrushInstance>();
        freeInstances = new Queue<BrushInstance>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateInstance();
        }
    }

    public void SetBrushInstanceAtPosition(Vector2 position)
    {
        if (freeInstances.Count == 0)
        {
            CreateInstance();
        }

        BrushInstance instance = freeInstances.Dequeue();
        instance.gameObject.SetActive(true);
        instance.transform.position = position;
    }

    public void ReturnAllInstancesToPool()
    {
        foreach (BrushInstance instance in allInstances)
        {
            instance.gameObject.SetActive(false);
            freeInstances.Enqueue(instance);
        }
    }

    private void ReturnInstanceToPool(BrushInstance instance)
    {
        instance.gameObject.SetActive(false);
        if (!freeInstances.Contains(instance))
        {
            freeInstances.Enqueue(instance);
        }
    }

    private void CreateInstance()
    {
        BrushInstance brushInstance = new BrushInstance();
        brushInstance.gameObject = GameObject.Instantiate(brushPrefab);
        brushInstance.transform = brushInstance.gameObject.transform;
        brushInstance.gameObject.SetActive(false);
        allInstances.Add(brushInstance);
        freeInstances.Enqueue(brushInstance);
    }


}
