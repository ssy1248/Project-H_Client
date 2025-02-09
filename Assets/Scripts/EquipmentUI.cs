using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public GameObject EquipmentPanel;
    bool activeEquipment = false;

    private void Start()
    {
        EquipmentPanel.SetActive(activeEquipment);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            activeEquipment = !activeEquipment;
            EquipmentPanel.SetActive(activeEquipment);
        }
    }
}