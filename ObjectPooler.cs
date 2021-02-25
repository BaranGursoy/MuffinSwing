using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    private void Awake()
    {
        SharedInstance = this; // Set SharedInstance to "this"
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();

        for(int i=0; i<amountToPool; i++) // Instantiate objects for desired amount, set them inactive and add them to the list
        {
            GameObject obj = (GameObject)Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject() // A function to use in other codes while getting the objects from pool
    {
        for(int i=0; i<pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy) // If the object is inactive get that object and do something with them in the other codes
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

}
