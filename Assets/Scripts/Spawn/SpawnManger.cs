using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManger : MonoBehaviour
{
    private static SpawnManger _instance;
    public static SpawnManger Instance => _instance;

    [SerializeField] GameObject spawneffectObject;
    [SerializeField] Transform parent;

    [SerializeField] List<GameObject> spawnObjects = new List<GameObject>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            //spawnObjects.Add(Instantiate(spawneffectObject, parent));
        }
    }
    public GameObject getData(Transform tr)
    {
        GameObject temp = spawnObjects[0];
        spawnObjects.RemoveAt(0);

        
        temp.transform.SetParent(tr);
        temp.transform.localPosition = new Vector3(0, 0, 0);
        temp.SetActive(true);

        return temp; 
    }
    public void setData(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(parent);
        spawnObjects.Add(obj);
    }
}
