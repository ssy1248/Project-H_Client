using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private static DungeonManager _instance;
    public static DungeonManager Instance => _instance;


    // ��Ƽ ������ 
    private Dictionary<string, PartyInfo> partyInfoDict = new Dictionary<string, PartyInfo>();
    // �ڱ� �ڽ� 
    public Player MyPlayer { get; private set; }

    // �÷��̾�� ���͵� ������ 
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
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializePlayerDatabase();
    }
    // �ڱ� �ڽ� �����뵵 
    public void Spawn(PlayerInfo playerInfo, bool isPlayer = false)
    {
        if (isPlayer)
        {
            Debug.Log("�÷��̾� �Դϴ�.");
            //Vector3 spawnPos = CalculateSpawnPosition(playerInfo.Transform);
            MyPlayer = CreatePlayer(playerInfo, new Vector3(playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ));//CreatePlayer(playerInfo, spawnPos);
            MyPlayer.SetIsMine(true);

            return;
        }
        //CreatePlayer(playerInfo, new Vector3 (playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ + 136.5156f));
        Player player = CreatePlayer(playerInfo, new Vector3(playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ));
        player.SetIsMine(false);

        // �÷��̾ ����Ʈ�� �߰�
        players.Add(playerInfo.PlayerId, player);
    }
    public Player CreatePlayer(PlayerInfo playerInfo, Vector3 spawnPos)
    {
        Debug.Log(playerInfo.Class);
        string playerResPath = playerDb.GetValueOrDefault(playerInfo.Class, ("Player/Player" + playerInfo.Class));
        Player playerPrefab = Resources.Load<Player>(playerResPath);

        var player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.Move(spawnPos, Quaternion.identity, 0);
        player.SetPlayerId(playerInfo.PlayerId);
        player.SetNickname(playerInfo.Nickname);

        if (playerList.TryGetValue(playerInfo.PlayerId, out var existingPlayer))
        {
            playerList[playerInfo.PlayerId] = player;
            Destroy(existingPlayer.gameObject);
        }
        else
        {
            playerList.Add(playerInfo.PlayerId, player);
        }

        return player;
    }
    public Monster CreateMonster(Vector3 spawnPos)// ���� ���� ������ ���� 
    {
        //var monster = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        return new Monster();
    }
}
