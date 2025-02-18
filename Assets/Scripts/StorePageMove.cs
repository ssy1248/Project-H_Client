using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class PageManager : MonoBehaviour
{
    // 각 페이지를 리스트로 받을 수 있도록 설정
    public List<GameObject> consumablePages = new List<GameObject>();
    public List<GameObject> equipmentPages = new List<GameObject>();
    public List<GameObject> sellPages = new List<GameObject>();

    [SerializeField] public GameObject consembleObject;
    [SerializeField] public GameObject equipmentObject;
    [SerializeField] public GameObject sellObject;

    // 버튼
    public Button consumablePageButton;
    public Button equipmentPageButton;
    public Button sellPageButton;

    public Button previousPageButton;
    public Button nextPageButton;

    // 텍스트 (페이지 번호 등)
    public TextMeshProUGUI pageNumberText;

    private int currentConsumablePageIndex = 0;
    private int currentEquipmentPageIndex = 0;
    private int currentSellPageIndex = 0;

    private void Start()
    {
        consembleObject.SetActive(true);
        equipmentObject.SetActive(false);
        sellObject.SetActive(false);

        consumablePageButton.onClick.AddListener(showConsemblePage);
        equipmentPageButton.onClick.AddListener(showeEquipmentPage);
        sellPageButton.onClick.AddListener(showSellPage);
    }

    private void showConsemblePage()
    {
        consembleObject.SetActive(true);
        equipmentObject.SetActive(false);
        sellObject.SetActive(false);

    }

    private void showeEquipmentPage()
    {
        consembleObject.SetActive(false);
        equipmentObject.SetActive(true);
        sellObject.SetActive(false);

    }
    private void showSellPage()
    {
        consembleObject.SetActive(false);
        equipmentObject.SetActive(false);
        sellObject.SetActive(true);
    }





}
