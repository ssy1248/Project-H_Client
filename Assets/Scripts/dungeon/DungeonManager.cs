using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private static DungeonManager _instance;
    public static DungeonManager Instance => _instance;


    // 파티 데이터 
    private Dictionary<string, PartyInfo> partyInfoDict = new Dictionary<string, PartyInfo>();
    // 자기 자신 
    public Player MyPlayer { get; private set; }
    public RewardAuction rewardAuction;

    // 플레이어들 몬스터들 데이터 
    private Dictionary<int , Player> players = new Dictionary<int, Player>();
    private Dictionary<int, Monster> monsters = new Dictionary<int, Monster>();
    private Dictionary<int, Player> playerList = new();
    private Dictionary<int, string> playerDb = new();
    private Dictionary<int, string> monsterDb = new();
    private void InitializePlayerDatabase()
    {
        playerDb[1001] = "Player/Player1";
        playerDb[1002] = "Player/Player2";
        playerDb[1003] = "Player/Player3";
        playerDb[1004] = "Player/Player4";
        playerDb[1005] = "Player/Player5";
        monsterDb[1] = "Monster/Monster1";
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializePlayerDatabase();
    }
    // 클라이언트 에서 보내기

    public void EnterAuctionBid(int gold ,string id)
    {
        var Packet = new C_EnterAuctionBid
        {
            Gold = gold,
            Id = id,
        };
        GameManager.Network.Send(Packet);
    }

    public void DungeonEnter()
    {
        var Packet = new C_DungeonEnter
        {

        };
        GameManager.Network.Send(Packet);
    }
    public void DungeonExit()
    {
        var Packet = new C_DungeonExit
        {

        };
        GameManager.Network.Send(Packet);
    }

    // 서버에서 받기 
    public void FinalizeBuyAuctionResponse(S_FinalizeBuyAuction data)
    {
        rewardAuction.GetReward(data.Name,data.ItemId,true);
    }
    public void FinalizeAllAuctionResponse(S_FinalizeAllAuction data)
    {
        rewardAuction.GetReward(data.Name, data.Gold, false);
    }
    public void EnterAuctionBidResponse(S_EnterAuctionBid data)
    {
        rewardAuction.ChangeGold(data);
    }
    public void EndAuctionResponse(S_EndAuction data)
    {
        rewardAuction.EndAuction();
    }
    public void SetAuctionDataResponse(S_SetAuctionData data)
    {
        rewardAuction.StartAuction(data);
    }
    public void WaitAuctionResponse(S_WaitAuction data)
    {
        rewardAuction.WaitAuction(data);
    }
    public void DungeonSpawn(S_DungeonSpawn data)
    {
        Debug.Log(data);
        for (int i =0; i < data.DungeonInfo.PartyInfo.Players.Count; i++)
        {
            var player = data.DungeonInfo.PartyInfo.Players[i];
            if (player.PlayerId == data.UserId)
            {
                Spawn(player, data.PlayerTransforms[i],true);
                continue;
            }
            Spawn(player, data.PlayerTransforms[i]);
        }
        
    }
    public void DungeonDeSpawn(S_DungeonDeSpawn data)
    {

    }
    // 스폰용도 
    public void Spawn(PlayerStatus playerData, TransformInfo playerTransform , bool isPlayer = false)
    {
        if (isPlayer)
        {
            Debug.Log("플레이어 입니다.");
            //Vector3 spawnPos = CalculateSpawnPosition(playerInfo.Transform);
            MyPlayer = CreatePlayer(playerData, new Vector3(playerTransform.PosX, playerTransform.PosY, playerTransform.PosZ));//CreatePlayer(playerInfo, spawnPos);
            MyPlayer.SetIsMine(true);

            return;
        }
        //CreatePlayer(playerInfo, new Vector3 (playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ + 136.5156f));
        Player player = CreatePlayer(playerData, new Vector3(playerTransform.PosX, playerTransform.PosY, playerTransform.PosZ));
        player.SetIsMine(false);

        // 플레이어를 리스트에 추가
        players.Add(playerData.PlayerId, player);
    }
    public Player CreatePlayer(PlayerStatus playerData, Vector3 spawnPos)
    {
        Debug.Log(playerData.PlayerClass);
        string playerResPath = playerDb.GetValueOrDefault(playerData.PlayerClass, ("Player/Player" + playerData.PlayerClass));
        Player playerPrefab = Resources.Load<Player>(playerResPath);

        var player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.Move(spawnPos, Quaternion.identity, 0);
        player.SetPlayerId(playerData.PlayerId);
        player.SetNickname(playerData.PlayerName);

        if (playerList.TryGetValue(playerData.PlayerId, out var existingPlayer))
        {
            playerList[playerData.PlayerId] = player;
            Destroy(existingPlayer.gameObject);
        }
        else
        {
            playerList.Add(playerData.PlayerId, player);
        }

        return player;
    }
    public Monster CreateMonster(Vector3 spawnPos)// 아직 몬스터 인포가 없음 
    {
        //var monster = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        return new Monster();
    }
}
