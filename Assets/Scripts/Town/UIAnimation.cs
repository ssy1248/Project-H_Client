using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Button btnBattle;
    [SerializeField] private Button[] btnList;

    [SerializeField] private MyPlayer mPlayer;
    [SerializeField] private GameObject inventory;

    [SerializeField] int inventoryPage = 1;
    [SerializeField] int slotInPage = 10;
    [SerializeField] int slotDistance = 100;
    [SerializeField] GameObject slotObject;
    [SerializeField] Dictionary<int, GameObject> slots = new Dictionary<int, GameObject>();
    void Start()
    {
        mPlayer = TownManager.Instance.MyPlayer?.MPlayer;

        if (mPlayer == null)
        {
            Debug.LogError("MyPlayer instance is missing or not initialized.");
            return;
        }

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < btnList.Length; i++)
        {
            int idx = i;
            btnList[i].onClick.AddListener(() => PlayAnimation(idx));
        }
    }

    private void PlayAnimation(int idx)
    {
        if (mPlayer == null)
        {
            Debug.LogWarning("Cannot play animation. MyPlayer instance is null.");
            return;
        }

        mPlayer.ExecuteAnimation(idx);
    }

    void InitiallzeSlots()
    {
        for (int i = 0; i < slotInPage; i++)
        {
            GameObject slotTemp = Instantiate(slotObject, inventory.transform);
            slots.Add(i, slotTemp);
        }
        
    }
    private void Update()
    {
        if (mPlayer == null)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory.activeSelf)
            {
                inventory.SetActive(false);
            }
            else
            {
                inventory.SetActive(true);
            }
        }
    }
}