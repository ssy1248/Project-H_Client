using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marketplace : MonoBehaviour
{
    [SerializeField] List<Button> buttons = new List<Button>();
    [SerializeField] int page = 1;
    [SerializeField] Dictionary<int, GameObject> slots = new Dictionary<int, GameObject>();
    
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }
    public void GetMarketData()
    {

    }
    
    public void SetSellData()
    {

    }
    public void SetBuyData()
    {

    }
}
