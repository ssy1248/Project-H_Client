using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartyManager : MonoBehaviour
{
    [Header("��Ƽ ����")]
    [SerializeField] private GameObject PartyListPrefab;
    [SerializeField] private GameObject PartyLeaderCharPrefab;
    [SerializeField] private GameObject PartyMemberCharPrefab;
    [SerializeField] private TMP_InputField PartyNameText;

    // ������ ���� ��Ƽ ������ ������ ��ü
    public PartyInfo InDungeonPartyInfo;

    private void Awake()
    {
        // �������� ���� �������� �̵���ų ������Ʈ
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name.StartsWith("Dungeon"))
        {
            PartyNameText =  GameObject.Find("PartyStatus").GetComponentInChildren<TMP_InputField>();
        }
    }

    void Update()
    {
        
    }
}
