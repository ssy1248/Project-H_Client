using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance => _instance;
 
    
    private NetworkManager network;
    public static NetworkManager Network => _instance.network;


    public const string BattleScene = "Battle";
    public const string TownScene = "Town";

    public S_EnterDungeon Pkt;
    
    public string UserName;
    public int ClassIdx;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        
            network = new NetworkManager();
        
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Update()
    {
        if(network != null)
            network.Update();
    }

    // ���� ������� �ڷ�ƾ
    // ���⿡ ���� ���� ���� �Ŵ����� ���� DontDestroy�� ������ ���
    public void DungeonExit()
    {
        StartCoroutine(LoadTownAndExit());
    }
    private IEnumerator LoadTownAndExit()
    {
        SceneManager.LoadScene("Town");

        // ���� �ε�� ������ ���
        yield return new WaitUntil(() => TownManager.Instance != null);

        // TownManager�� ������ �� DungeonExit ȣ��
        TownManager.Instance.DungeonExit();
    }

}
