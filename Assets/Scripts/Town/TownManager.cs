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

    // 테스트 용도로 생성
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
            //여기에 이메일 연결 
            Email = "aaaaaaa",
            Nickname = GameManager.Instance.UserName,
            //여기에 비밀번호 연결
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

    /* 임시로 만든 보내는 메서드 들 */
    // 해야할 일 패킷에 들어갈 매개변수로 받거나 다른곳에서 받아오는 형식으로 보낼 패킷 짜기 
    public void Register(string email, string nickname, string password)
    {
        var enterPacket = new C_RegisterRequest
        {
            //여기에 이메일 연결 
            Email = email,
            Nickname = nickname,//GameManager.Instance.UserName,
            //여기에 비밀번호 연결
            Password = password
        };
        GameManager.Network.Send(enterPacket);
    }
    public void Login(string email, string password)
    {
        var enterPacket = new C_LoginRequest
        {
            //여기에 이메일 연결 
            Email = email,
            //여기에 비밀번호 연결
            Password = password
        };

        GameManager.Network.Send(enterPacket);
    }
    // 패킷 명세에서는 닉네임, 클래스인데 생성자를 보니 클래스밖에 없음
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
    public void PartyRequest(int userId)
    {
        var partyPacket = new C_PartyRequest
        {
            UserId = userId
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
    /* 여기까지 */

    /* 임시로 만든 받는 메서드 들 */
    // 핸들러와 연결후 각각 필요한 기능 구현 

    // 회원가입 확인 메세지 출력정도.
    IEnumerator erroText()
    {
        errorText.SetActive(true);
        yield return new WaitForSeconds(1f);
        errorText.SetActive(false);
    }
    // 테스트 코드 
    public void RegisterResponse(S_RegisterResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // 로그인 확인후 다음 캐릭터 선택창으로 이동 구현
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
    // 다른 플레이어들 들어오면 생성해주기 // 아래 spanwn 함수 사용하면 아마 구현
    public void Enter(S_Enter data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Player.ToString());
        Spawn(data.Player);
    }

    //private Dictionary<int, Player> playerList = new();
    // 내가 마을에 참가하면 for문이든 반복문이든 돌리면서 생성해주기.
    // 여기 패킷에 자기 자신이 몇번인지 추가해줬으면 좋겠습니다.
    public void AllSpawn(S_Spawn data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Players.ToString());
        foreach (PlayerInfo player in data.Players)
        {
            if (player)
            {
                Spawn(player,true);
            }
            Spawn(player);
        }
    }
    // 나가면 삭제해주기 
    public void Despawn(S_Despawn data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.PlayerIds.ToString()) ;
    }
    public void AllMove(S_Move data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.PlayerId.ToString());

        Player player = GetPlayerAvatarById(data.PlayerId);
        if (player == null)
        {
            Debug.LogWarning("Player with ID " + data.PlayerId + " not found.");
            return;
        }

        // TransformInfo를 이용해 새로운 위치와 회전값을 계산합니다.
        Vector3 targetPos = new Vector3(data.Transform.PosX, data.Transform.PosY, data.Transform.PosZ);
        // 여기서는 y축 회전만 적용한다고 가정 (필요시 다른 축도 적용)
        Quaternion targetRot = Quaternion.Euler(0, data.Transform.Rot, 0);

        // 플레이어의 Move() 메서드를 호출하여 부드러운 이동 및 회전 처리를 위임합니다.
        player.Move(targetPos, targetRot);
    }
    //아마 아이디 받은뒤 해당 id player 애니메이션 
    public void AllAnimation(S_Animation data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.PlayerId.ToString());

        // playerList 딕셔너리나 GetPlayerAvatarById를 이용해 해당 플레이어를 찾습니다.
        Player player = GetPlayerAvatarById(data.PlayerId);
        if (player == null)
        {
            Debug.LogWarning("Player with ID " + data.PlayerId + " not found for animation.");
            return;
        }

        // 플레이어의 애니메이션 재생 메서드 호출
        player.PlayAnimation(data.AnimCode);
    }
    // 채팅 받아오기
    public void ChatResponse(S_Chat data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.ChatMsg);
    }
    //  주말 목표 입니다람쥐

    // 아이템 사는거 응답 처리
    public void BuyItemResponse(S_BuyItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // 아이템 장착 응답 처리
    public void EquipItemResponse(S_EquipItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // 아이템 탈착 응답 처리
    public void DisrobeItemResponse(S_DisrobeItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // 소비 장착 응답 처리
    public void ActiveItemeResponse(S_ActiveItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // 파티 응답 처리
    public void PartyResponse(S_PartyResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    // 던전 쪽 추후 추가 예정
    /* 여기까지 */

    // 자기 자신 스폰용도 
    public void Spawn(PlayerInfo playerInfo , bool isPlayer = false)
    {
        if (isPlayer)
        {
            Vector3 spawnPos = CalculateSpawnPosition(playerInfo.Transform);
            MyPlayer = CreatePlayer(playerInfo, spawnPos);
            MyPlayer.SetIsMine(true);

            ActivateGameUI();
        }
        CreatePlayer(playerInfo, new Vector3 (playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ));
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