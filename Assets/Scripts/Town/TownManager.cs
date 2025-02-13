using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI.Table;

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

    // �׽�Ʈ �뵵�� ����
    [SerializeField] GameObject errorText;

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
        /*
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
    public void Register(string email, string nickname, string password)
    {
        var enterPacket = new C_RegisterRequest
        {
            //���⿡ �̸��� ���� 
            Email = email,
            Nickname = nickname,//GameManager.Instance.UserName,
            //���⿡ ��й�ȣ ����
            Password = password
        };
        GameManager.Network.Send(enterPacket);
    }
    public void Login(string email, string password)
    {
        var enterPacket = new C_LoginRequest
        {
            //���⿡ �̸��� ���� 
            Email = email,
            //���⿡ ��й�ȣ ����
            Password = password
        };

        GameManager.Network.Send(enterPacket);
    }
    // ��Ŷ ���������� �г���, Ŭ�����ε� �����ڸ� ���� Ŭ�����ۿ� ����
    public void SelectCharacterRequest(/*string nickname*/ int jobIndex)
    {
        var selectCharacterPacket = new C_SelectCharacterRequest
        {
           // Nickname = nickname,
            Class = jobIndex
        };
        uiStart.chuseObject.SetActive(false);
        GameManager.Network.Send(selectCharacterPacket);
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
    public void Animation(int animCode)
    {
        var animationPacket = new C_Animation
        {
            AnimCode = animCode
        };

        GameManager.Network.Send(animationPacket);
    }
    public void Chat(int playerId, int type, string senderName, string chatMsg)
    {
        var chatPacket = new C_Chat
        {
            PlayerId = playerId,
            Type = type,
            SenderName = senderName,
            ChatMsg = chatMsg
        };

        GameManager.Network.Send(chatPacket);
    }
    public void BuyItemRequest(string itemName, int price)
    {
        var buyItemPacket = new C_BuyItemRequest
        {
            Itemname = itemName,
            Price = price
        };

        GameManager.Network.Send(buyItemPacket);
    }
    public void EquipItemRequest(int itemId)
    {
        var equipItemPacket = new C_EquipItemRequest
        {
            ItemId = itemId
        };

        GameManager.Network.Send(equipItemPacket);
    }
    public void DisrobeItemRequest(int itemId)
    {
        var disrobeItemPacket = new C_DisrobeItemRequest
        {
            ItemId = itemId
        };

        GameManager.Network.Send(disrobeItemPacket);
    }
    public void ActiveItemRequest(int itemId)
    {
        var activeItemPacket = new C_ActiveItemRequest
        {
            ItemId = itemId
        };

        GameManager.Network.Send(activeItemPacket);
    }
    public void PartyRequest(int userId, string partyName)
    {
        var partyPacket = new C_PartyRequest
        {
            UserId = userId,
            PartyName = partyName,
        };

        GameManager.Network.Send(partyPacket);
    }
    public void EnterDungeon(int duneonCode, PlayerInfo player)
    {
        var enterDungeonPacket = new C_EnterDungeon
        {
            DungeonCode = duneonCode,
            //Players = new PlayerInfo
            //{
            //    PlayerId = player.PlayerId,
            //    Nickname = player.Nickname,
            //    Class = player.Class,
            //    Transform = player.Transform,
            //    StatInfo = player.StatInfo
            //}
        };

        GameManager.Network.Send(enterDungeonPacket);
    }
    /* ������� */

    /* �ӽ÷� ���� �޴� �޼��� �� */
    // �ڵ鷯�� ������ ���� �ʿ��� ��� ���� 

    // ȸ������ Ȯ�� �޼��� �������.
    IEnumerator erroText()
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(1f);
        errorText.SetActive(false);
    }
    // �׽�Ʈ �ڵ� 
    public void RegisterResponse(S_RegisterResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // �α��� Ȯ���� ���� ĳ���� ����â���� �̵� ����
    public void LoginResponse(S_LoginResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        if (data.Success)
        {
            uiStart.loginObject.SetActive(false);
            uiStart.chuseObject.SetActive(true);
        }
    }
    // �ٸ� �÷��̾�� ������ �������ֱ� // �Ʒ� spanwn �Լ� ����ϸ� �Ƹ� ����
    public void Enter(S_Enter data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Player.ToString());
        Spawn(data.Player);
    }

    //private Dictionary<int, Player> playerList = new();
    // ���� ������ �����ϸ� for���̵� �ݺ����̵� �����鼭 �������ֱ�.
    // ���� ��Ŷ�� �ڱ� �ڽ��� ������� �߰��������� ���ڽ��ϴ�.
    public void AllSpawn(S_Spawn data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Players.ToString());
        Debug.Log(data);
        Debug.Log("�÷��̾� �� : " + data.Players.Count);
        foreach (PlayerInfo player in data.Players)
        {
            Debug.Log("����ġ ����");
            if (player.PlayerId == data.UserId)
            {
                Spawn(player,true);
            }
            else
            {
                Spawn(player);
            }
        }
    }
    // ������ �������ֱ� 
    public void Despawn(S_Despawn data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.PlayerIds.ToString()) ;
    }
    public void AllMove(S_Move data)
    {
        // 받은 배열 만큼 반복문을 돌려야함
        // data.transformInfos는 TransformInfo 배열이므로, 이를 반복문으로 처리
        foreach (var syncTransformInfo  in data.transformInfos) {
            // 플레이어 ID
            int playerId = syncTransformInfo.PlayerId;
            
            // 트랜스폼 정보 (위치 회전)
            TransformInfo transformInfo = syncTransformInfo.transform;
            Vector3 targetPos = new Vector3(transformInfo.PosX, transformInfo.PosY, transformInfo.PosZ);
            Quaternion targetRot = Quaternion.Euler(0, transformInfo.Rot, 0);
            
            // 예상 도착시갖
            long estimatedArrivalTime = syncTransformInfo.estimatedArrivalTime;
            
            // 레이턴시
            int latency = syncTransformInfo.latency;

            // 플레이어가 존재하는지 검증.
            Player player = GetPlayerAvatarById(playerId);
            if(player == null) {
                continue;
            }

            // 플레이어가 본인인지 검증.
            if(MyPlayer.PlayerId == playerId) {
                continue;
            }

            // 플레이어에게 이동 정보를 넘긴다.
            player.Move(targetPos, targetRot, estimatedArrivalTime);
        }

        // StartCoroutine("erroText");
        // errorText.GetComponent<TextMeshProUGUI>().SetText(data.PlayerId.ToString());

        // Player player = GetPlayerAvatarById(data.PlayerId);
        // if (player == null)
        // {
        //     Debug.LogWarning("Player with ID " + data.PlayerId + " not found.");
        //     return;
        // }

        // if(MyPlayer.PlayerId != data.PlayerId)

        // // TransformInfo�� �̿��� ���ο� ��ġ�� ȸ������ ����մϴ�.
        // Vector3 targetPos = new Vector3(data.Transform.PosX, data.Transform.PosY, data.Transform.PosZ);
        // // ���⼭�� y�� ȸ���� �����Ѵٰ� ���� (�ʿ�� �ٸ� �൵ ����)
        // Quaternion targetRot = Quaternion.Euler(0, data.Transform.Rot, 0);

        // // �÷��̾��� Move() �޼��带 ȣ���Ͽ� �ε巯�� �̵� �� ȸ�� ó���� �����մϴ�.
        // player.Move(targetPos, targetRot);
    }
    //�Ƹ� ���̵� ������ �ش� id player �ִϸ��̼� 
    public void AllAnimation(S_Animation data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.ToString());

        // playerList ��ųʸ��� GetPlayerAvatarById�� �̿��� �ش� �÷��̾ ã���ϴ�.
        Player player = GetPlayerAvatarById(data.PlayerId);
        if (player == null)
        {
            Debug.LogWarning("Player with ID " + data.PlayerId + " not found for animation.");
            return;
        }

        // �÷��̾��� �ִϸ��̼� ��� �޼��� ȣ��
        player.PlayAnimation(data.AnimCode);
    }
    // ä�� �޾ƿ���
    public void ChatResponse(S_Chat data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.ChatMsg);
    }
    //  �ָ� ��ǥ �Դϴٶ���

    // ������ ��°� ���� ó��
    public void BuyItemResponse(S_BuyItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // ������ ���� ���� ó��
    public void EquipItemResponse(S_EquipItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // ������ Ż�� ���� ó��
    public void DisrobeItemResponse(S_DisrobeItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // �Һ� ���� ���� ó��
    public void ActiveItemeResponse(S_ActiveItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // ��Ƽ ���� ó��
    public void PartyResponse(S_PartyResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"��Ƽ ���� ���� ������ : {data}");
        if(data.Success)
        {
            // ��Ƽ ����
        } 
        else
        {
            // ��Ƽ ���� ����
        }
    }
    // ��� ��Ƽ ��ȸ
    public void PartyListResponse(S_PartySearchResponse data)
    {
        //StartCoroutine("errorText");
        //errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"��Ƽ ��ġ ���� ������ : {data}");
    }
    // �Ѱ� ��Ƽ ��ȸ
    public void PartySearchResponse(S_PartySearchResponse data)
    {
        //StartCoroutine("errorText");
        //errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"��Ƽ ��ġ ���� ������ : {data}");
    }
    // ���� �� ���� �߰� ����
    /* ������� */

    // �ڱ� �ڽ� �����뵵 
    public void Spawn(PlayerInfo playerInfo , bool isPlayer = false)
    {
        if (isPlayer)
        {
            Debug.Log("�÷��̾� �Դϴ�.");
            //Vector3 spawnPos = CalculateSpawnPosition(playerInfo.Transform);
            MyPlayer = CreatePlayer(playerInfo, new Vector3(playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ));//CreatePlayer(playerInfo, spawnPos);
            MyPlayer.SetIsMine(true);

            ActivateGameUI();
            return;
        }
        //CreatePlayer(playerInfo, new Vector3 (playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ + 136.5156f));
        Player player = CreatePlayer(playerInfo, new Vector3(playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ));
        player.SetIsMine(false);
    }

    //private Vector3 CalculateSpawnPosition(TransformInfo transformInfo)
    //{
    //    Vector3 spawnPos = spawnArea.position;
    //    spawnPos.x += transformInfo.PosX;
    //    spawnPos.z += transformInfo.PosZ;
    //    return spawnPos;
    //}

    public Player CreatePlayer(PlayerInfo playerInfo, Vector3 spawnPos)
    {
        Debug.Log(playerInfo.Class);
        string playerResPath = playerDb.GetValueOrDefault(playerInfo.Class, ("Player/Player"+ playerInfo.Class));
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