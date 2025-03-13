using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSetting : MonoBehaviour
{
    public int rarity = 0;

    [SerializeField] List<Material> materialList = new List<Material>();
    [SerializeField] List<GameObject> effectList = new List<GameObject>();
    [SerializeField] GameObject boxObject;
    [SerializeField] Light lightTemp;

    public void InitBox(int data)
    {
        rarity = data;
        boxObject.GetComponent<Renderer>().material = materialList[rarity];
        Instantiate(effectList[rarity], boxObject.transform);
        switch (rarity)
        {
            case 0:
                lightTemp.color = Color.white;
                break;
            case 1:
                lightTemp.color = new Color(0.502f, 0f, 0.502f);
                break;
            case 2:
                lightTemp.color = new Color(1f, 1f, 0f);
                break;
        }
    }
    public void EndBox()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            DungeonManager.Instance.LoootingBox();
        }
    }
}
