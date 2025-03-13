using Cinemachine;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    private static DungeonManager _instance;
    public static DungeonManager Instance => _instance;
    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private EventSystem eSystem;
    public EventSystem E_System => eSystem;
    public CinemachineFreeLook FreeLook => freeLook;

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
        DungeonEnter();
        if (_instance == null)
        {
            _instance = this;
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
        GameManager.Instance.DungeonExit();
    }
    public void Move(Vector3 pos, float rotation)
    {
        var movePacket = new C_Move
        {
            Transform = new TransformInfo
            {
                PosX = pos.x,
                PosY = pos.y,
                PosZ = pos.z,
                Rot = rotation
            }
        };

        GameManager.Network.Send(movePacket);
    }
    // 서버에서 받기 
    public void Despawn(S_Despawn data)
    {
        Player playerToRemove = players[data.PlayerId];

        if (playerToRemove != null)
        {
            players.Remove(data.PlayerId);
            playerList.Remove(data.PlayerId);
            playerToRemove.DespawnEffect();
        }
    }
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
    public void AllMove(S_Move data)
    {
        //Debug.Log(data);
        // 받은 배열 만큼 반복문을 돌려야함
        // data.transformInfos는 TransformInfo 배열이므로, 이를 반복문으로 처리
        foreach (var syncTransformInfo in data.TransformInfos)
        {
            // 플레이어 ID
            int playerId = syncTransformInfo.PlayerId;

            // 트랜스폼 정보 (위치 회전)
            TransformInfo transformInfo = syncTransformInfo.Transform;
            Vector3 targetPos = new Vector3(transformInfo.PosX, transformInfo.PosY, transformInfo.PosZ);
            Quaternion targetRot = Quaternion.Euler(0, transformInfo.Rot, 0);

            // 스피드
            float speed = syncTransformInfo.Speed;


            // 플레이어가 존재하는지 검증.
            Player player = GetPlayerAvatarById(playerId);
            if (player == null)
            {
                continue;
            }

            // 플레이어가 본인인지 검증.
            if (MyPlayer.PlayerId == playerId)
            {
                MyPlayer.MPlayer.UpdateUserPosition(targetPos, targetRot, speed);
                continue;
            }


            // 플레이어에게 이동 정보를 넘긴다.
            player.Move(targetPos, targetRot, speed);
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
            FreeLook.LookAt = MyPlayer.gameObject.transform;
            FreeLook.Follow = MyPlayer.gameObject.transform;
            FreeLook.gameObject.SetActive(true);
            return;
        }
        //CreatePlayer(playerInfo, new Vector3 (playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ + 136.5156f));
        Player player = CreatePlayer(playerData, new Vector3(playerTransform.PosX, playerTransform.PosY, playerTransform.PosZ));
        player.SetIsMine(false);
        Destroy(player.gameObject.GetComponent<RogueController>());
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

    public Player GetPlayerAvatarById(int playerId)
    {
        return playerList.TryGetValue(playerId, out var player) ? player : null;
    }
}
