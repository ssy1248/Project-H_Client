using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManger : MonoBehaviour
{
    [SerializeField] GameObject spawneffectObject;
    [SerializeField] Transform parent;

    [SerializeField] List<GameObject> spawnObjects = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            spawnObjects.Add(Instantiate(spawneffectObject, parent));
        }
    }
}
