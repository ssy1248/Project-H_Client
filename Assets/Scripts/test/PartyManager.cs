using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PartyManager : MonoBehaviour
{
    [Header("파티 관련")]
    [SerializeField] private GameObject PartyListPrefab;
    [SerializeField] private GameObject PartyLeaderCharPrefab;
    [SerializeField] private GameObject PartyMemberCharPrefab;
    [SerializeField] private TMP_InputField PartyNameText;

    // 던전에 들어온 파티 정보를 전달할 객체
    public PartyInfo InDungeonPartyInfo;

    private void Awake()
    {
        // 마을에서 부터 던전으로 이동시킬 오브젝트
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
