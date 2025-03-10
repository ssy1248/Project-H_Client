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

    // 던전 나가기용 코루틴
    // 여기에 넣은 이유 던전 매니저를 굳이 DontDestroy할 이유가 없어서
    public void DungeonExit()
    {
        StartCoroutine(LoadTownAndExit());
    }
    private IEnumerator LoadTownAndExit()
    {
        SceneManager.LoadScene("Town");

        // 씬이 로드될 때까지 대기
        yield return new WaitUntil(() => TownManager.Instance != null);

        // TownManager가 생성된 후 DungeonExit 호출
        TownManager.Instance.DungeonExit();
    }

}
