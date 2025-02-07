using System.Collections.Generic;
using Cinemachine;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TownManager : MonoBehaviour
{
    private static TownManager _instance;
    public static TownManager Instance => _instance;

    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private EventSystem eSystem;
    [SerializeField] private UIStart uiStart;
    [SerializeField] private UIAnimation uiAnimation;
    [SerializeField] private UIChat uiChat;
    [SerializeField] private TMP_Text txtServer;

    private const string DefaultPlayerPath = "Player/Player1";

    public CinemachineFreeLook FreeLook => freeLook;
    public EventSystem E_System => eSystem;
    public UIChat UiChat => uiChat;

    private Dictionary<int, Player> playerList = new();
    private Dictionary<int, string> playerDb = new();

    public Player MyPlayer { get; private set; }

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

    private void Start()
    {
        if (!GameManager.Network.IsConnected)
        {
            uiStart.gameObject.SetActive(true);
        }
        else
        {
            Connected();
        }
    }

    private void InitializePlayerDatabase()
    {
        playerDb[1001] = "Player/Player1";
        playerDb[1002] = "Player/Player2";
        playerDb[1003] = "Player/Player3";
        playerDb[1004] = "Player/Player4";
        playerDb[1005] = "Player/Player5";
    }

    public void GameStart(string gameServer, string port, string userName, int classIdx)
    {
        GameManager.Network.Init(gameServer, port);
        GameManager.Instance.UserName = userName;
        GameManager.Instance.ClassIdx = classIdx + 1001;
        txtServer.text = gameServer;
    }

    public void Connected()
    {
        var enterPacket = new C_RegisterRequest
        {
            //���⿡ �̸��� ���� 
            Email = "aaaaaaa",
            Nickname = GameManager.Instance.UserName,
            //���⿡ ��й�ȣ ����
            Password = "aaaaaaaa"
        };

        GameManager.Network.Send(enterPacket);
        /*var enterPacket = new C_Enter
        {
            Nickname = GameManager.Instance.UserName,
            Class = GameManager.Instance.ClassIdx
        };

        GameManager.Network.Send(enterPacket);*/
    }

    /* �ӽ÷� ���� ������ �޼��� �� */
    // �ؾ��� �� ��Ŷ�� �� �Ű������� �ްų� �ٸ������� �޾ƿ��� �������� ���� ��Ŷ ¥�� 
    public void Register()
    {
        var enterPacket = new C_RegisterRequest
        {
            //���⿡ �̸��� ���� 
            Email = "",
            Nickname = GameManager.Instance.UserName,
            //���⿡ ��й�ȣ ����
            Password = ""
        };

        GameManager.Network.Send(enterPacket);
    }
    public void Login()
    {
        var enterPacket = new C_LoginRequest
        {
            //���⿡ �̸��� ���� 
            Email = "",
            //���⿡ ��й�ȣ ����
            Password = ""
        };

        GameManager.Network.Send(enterPacket);
    }
    public void SelectCharacterRequest()
    {

    }
    public void Move()
    {

    }
    public void Animation()
    {

    }
    public void Chat()
    {

    }
    public void BuyItemRequest()
    {

    }
    public void EquipItemRequest()
    {

    }
    public void DisrobeItemRequest()
    {

    }
    public void ActiveItemRequest()
    {

    }
    public void PartyRequest()
    {

    }
    public void EnterDungeon()
    {

    }
    /* ������� */

    /* �ӽ÷� ���� �޴� �޼��� �� */
    // �ڵ鷯�� ������ ���� �ʿ��� ��� ���� 
    
    // ȸ������ Ȯ�� �޼��� �������.
    public void RegisterResponse()
    {

    }
    // �α��� Ȯ���� ���� ĳ���� ����â���� �̵� ����
    public void LoginResponse()
    {

    }
    // �ٸ� �÷��̾�� ������ �������ֱ� // �Ʒ� spanwn �Լ� ����ϸ� �Ƹ� ����
    public void Enter(PlayerInfo playerData)
    {
        Spawn(playerData);
    }
    // ���� ������ �����ϸ� for���̵� �ݺ����̵� �����鼭 �������ֱ�.
    public void AllSpawn(List<PlayerInfo> playerDatas)
    {
        foreach (PlayerInfo player in playerDatas)
        {
            Spawn(player);
        }
        
    }
    // ������ �������ֱ� 
    public void Despawn()
    {

    }
    //�Ƹ� ���̵� ������ �ش� player ������ �ִ°ɷ� �дϴ�.
    public void AllMove()
    {

    }
    //�Ƹ� ���̵� ������ �ش� id player �ִϸ��̼� 
    public void AllAnimation()
    {

    }
    // ä�� �޾ƿ���
    public void ChatResponse()
    {

    }
    //  �ָ� ��ǥ �Դϴٶ���

    // ������ ��°� ���� ó��
    public void BuyItemResponse()
    {

    }
    // ������ ���� ���� ó��
    public void EquipItemResponse()
    {

    }
    // ������ Ż�� ���� ó��
    public void DisrobeItemResponse()
    {

    }
    // �Һ� ���� ���� ó��
    public void ActiveItemeResponse()
    {

    }
    // ��Ƽ ���� ó��
    public void PartyResponse()
    {

    }
    // ���� �� ���� �߰� ����
    /* ������� */
    public void Spawn(PlayerInfo playerInfo)
    {
        Vector3 spawnPos = CalculateSpawnPosition(playerInfo.Transform);

        MyPlayer = CreatePlayer(playerInfo, spawnPos);
        MyPlayer.SetIsMine(true);

        ActivateGameUI();
    }

    private Vector3 CalculateSpawnPosition(TransformInfo transformInfo)
    {
        Vector3 spawnPos = spawnArea.position;
        spawnPos.x += transformInfo.PosX;
        spawnPos.z += transformInfo.PosZ;
        return spawnPos;
    }

    public Player CreatePlayer(PlayerInfo playerInfo, Vector3 spawnPos)
    {
        string playerResPath = playerDb.GetValueOrDefault(playerInfo.Class, DefaultPlayerPath);
        Player playerPrefab = Resources.Load<Player>(playerResPath);

        var player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.Move(spawnPos, Quaternion.identity);
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

    public void ReleasePlayer(int playerId)
    {
        if (!playerList.TryGetValue(playerId, out var player)) return;

        playerList.Remove(playerId);
        Destroy(player.gameObject);
    }

    private void ActivateGameUI()
    {
        uiStart.gameObject.SetActive(false);
        uiChat.gameObject.SetActive(true);
        uiAnimation.gameObject.SetActive(true);
    }

    public Player GetPlayerAvatarById(int playerId)
    {
        return playerList.TryGetValue(playerId, out var player) ? player : null;
    }
}