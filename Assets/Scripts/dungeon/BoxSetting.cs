using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSetting : MonoBehaviour
{
    public int rarity = 0;

    [SerializeField] List<Material> materialList = new List<Material>();
    [SerializeField] List<GameObject> effectList = new List<GameObject>();
    [SerializeField] GameObject boxObject;

    public void InitBox(int data)
    {
        rarity = data;
        boxObject.GetComponent<Renderer>().material = materialList[rarity];
        Instantiate(effectList[rarity], boxObject.transform);
    }
    public void endBox()
    {
        Destroy(gameObject);
    }
}
