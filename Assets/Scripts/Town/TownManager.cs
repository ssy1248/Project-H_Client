using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
// using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.Rendering.DebugUI.Table;

public class TownManager : MonoBehaviour
{
    private static TownManager _instance;
    public static TownManager Instance => _instance;

    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private EventSystem eSystem;
    [SerializeField] private UIRegister UiRegister;
    [SerializeField] private UIAnimation uiAnimation;
    [SerializeField] Marketplace market;
    [SerializeField] private UIChat uiChat;
    [SerializeField] private TMP_Text txtServer;
    [SerializeField] ShopUI shopUi;

    #region 파티 UI
    [Header("파티 UI 모음")]
    [SerializeField] private GameObject PartyUI;
    [SerializeField] private TMP_InputField PartyNameInputField;
    [SerializeField] private GameObject PartyStatusSpawnPoint;
    [SerializeField] private GameObject LeaderStatusPrefab;
    [SerializeField] private GameObject MemberStatusPrefab;
    [SerializeField] private GameObject PartyListSpawnPoint;
    [SerializeField] private GameObject PartyListPrefab;
    [SerializeField] private GameObject PartyMemberSpawnPoint;
    [SerializeField] private GameObject LeaderMemberPrefab;
    [SerializeField] private GameObject NormalMemberPrefab;
    [SerializeField] private GameObject SearchResultUI;
    #endregion

    #region 매칭 UI
    [Header("매칭 UI ")]
    [SerializeField] private GameObject MatchingWindow;
    #endregion

    [Header("테스트")]
    // 테스트 용도로 생성
    [SerializeField] GameObject errorText;

    private const string DefaultPlayerPath = "Player/Player1";

    public CinemachineFreeLook FreeLook => freeLook;
    public EventSystem E_System => eSystem;
    public UIChat UiChat => uiChat;

    private Dictionary<int, Player> playerList = new();
    private Dictionary<int, string> playerDb = new();

    // 파티 인포를 저장할 딕셔너리
    private Dictionary<string, PartyInfo> partyInfoDict = new Dictionary<string, PartyInfo>();

    public Player MyPlayer { get; private set; }

    // 유저들 동기화에 사용할 딕셔너리
    private List<Player> players = new List<Player>();

    // partyInfoDict -> 파티 아이디로 파티 인포를 저장

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
            UiRegister.gameObject.SetActive(true);
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

    // 닉네임으로 플레이어 조회 함수
    public Player GetPlayerByNickname(string nickname)
    {
        foreach (var player in playerList.Values)
        {
            if (player.nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase))
                return player;
        }
        return null;
    }

    public PartyInfo GetPartyInfoByPlayerId(int playerId)
    {
        // partyInfoDict:  Key = partyId,  Value = PartyInfo
        foreach (PartyInfo partyInfo in partyInfoDict.Values)
        {
            // PartyInfo.Players는 PlayerStatus 리스트(닉네임, 레벨, HP 등)
            // 각 PlayerStatus와 실제 TownManager의 Player 객체를 대조해봐야 할 수도 있음
            foreach (var pStatus in partyInfo.Players)
            {
                // TownManager에는 GetPlayerByNickname()이 존재하므로 닉네임으로 Player 객체를 찾는다
                Player player = GetPlayerByNickname(pStatus.PlayerName);
                if (player == null)
                    continue;

                // 찾은 Player의 PlayerId가 우리가 찾고자 하는 playerId와 같다면,
                // 이 PartyInfo가 해당 플레이어가 속한 파티!
                if (player.PlayerId == playerId)
                {
                    return partyInfo;
                }
            }
        }

        // 못 찾으면 null 반환
        return null;
    }

    public void UpdatePartyMembersUI(PartyInfo partyData)
    {
        // PartyMemberSpawnPoint 아래의 기존 UI 제거
        foreach (Transform child in PartyMemberSpawnPoint.transform)
        {
            Destroy(child.gameObject);
        }

        // partyData.Players를 순회하면서, 파티 리더이면 LeaderMemberPrefab, 그렇지 않으면 NormalMemberPrefab 생성
        foreach (var playerStatus in partyData.Players)
        {
            int playerId = 0;
            Player player = GetPlayerByNickname(playerStatus.PlayerName);
            if (player != null)
            {
                playerId = player.PlayerId;
            }

            GameObject prefabToInstantiate = (playerId == partyData.PartyLeaderId)
                ? LeaderMemberPrefab
                : NormalMemberPrefab;

            GameObject memberObj = Instantiate(prefabToInstantiate, PartyMemberSpawnPoint.transform);

            // memberObj의 자식에 TextMeshProUGUI 컴포넌트가 있다고 가정하고, 해당 텍스트를 플레이어 닉네임으로 설정
            TextMeshProUGUI memberText = memberObj.GetComponentInChildren<TextMeshProUGUI>();
            if (memberText != null)
            {
                memberText.text = playerStatus.PlayerName;
            }
            else
            {
                Debug.LogWarning("Member prefab에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
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
        UiRegister.chuseObject.SetActive(false);
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
    public void SellItemRequest(int inventoryId, int price)
    {
        var sellPacket = new C_SellItemRequest
        {
            InventoryId = inventoryId,
            Price = price,
        };
        GameManager.Network.Send(sellPacket);
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
    public void EnterDungeon(int duneonCode, PlayerInfo player)
    {
        var enterDungeonPacket = new C_EnterDungeon
        {
            //DungeonCode = duneonCode,
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

    public void MarketListRequest(int page, int count)
    {
        var marketListPacket = new C_MarketList
        {
            Page = page,
            Count = count,
        };
        GameManager.Network.Send(marketListPacket);
    }
    public void SellInMarketRequest(int inventoryId, int itemId)
    {
        var SellInMarketPacket = new C_SellInMarket
        {
            InventoryId = inventoryId,
            ItemId = itemId,
        };
        GameManager.Network.Send(SellInMarketPacket);
    }
    public void BuyInMarketRequest(int marketId)
    {
        var BuyInMarketPacket = new C_BuyInMarket
        {
            MarketId = marketId,
        };
        GameManager.Network.Send(BuyInMarketPacket);
    }
    public void MarketMyListRequest(int page, int count)
    {
        var marketListPacket = new C_MarketMyList
        {
            Page = page,
            Count = count,
        };
        GameManager.Network.Send(marketListPacket);
    }
    public void MarketSelectBuyNameRequest(int page, int count ,string name)
    {
        var marketListPacket = new C_MarketSelectBuyName
        {
            Page = page,
            Count = count,
            Name = name,
        };
        GameManager.Network.Send(marketListPacket);
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
            UiRegister.loginObject.SetActive(false);
            UiRegister.chuseObject.SetActive(true);
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
        Debug.Log(data.StoreList);
        Debug.Log("플레이어 수 : " + data.Players.Count);
        foreach (PlayerInfo player in data.Players)
        {
            Debug.Log("포이치 들어옴");
            if (player.PlayerId == data.UserId)
            {
                Spawn(player, true);
            }
            else
            {
                Spawn(player);
            }
        }
        shopUi.GetBuyData(data.StoreList.ToList());
    }
    // 나가면 삭제해주기 
    public void Despawn(S_Despawn data)
    {
        //StartCoroutine("erroText");
        //errorText.GetComponent<TextMeshProUGUI>().SetText(data.PlayerIds.ToString()) ;

        // 나중에 주석풀자.


        Player playerToRemove = players.FirstOrDefault(p => p.PlayerId == data.PlayerId);

        if (playerToRemove != null)
        {
            players.Remove(playerToRemove);
            playerList.Remove(data.PlayerId);
            playerToRemove.Despawn();

        }
    }
    public void AllMove(S_Move data)
    {
        // 받은 배열 만큼 반복문을 돌려야함
        // data.transformInfos는 TransformInfo 배열이므로, 이를 반복문으로 처리
        foreach (var syncTransformInfo  in data.TransformInfos) {
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
            if(player == null) {
                continue;
            }

            // 플레이어가 본인인지 검증.
            if(MyPlayer.PlayerId == playerId) {
                continue;
            }

            
            // 플레이어에게 이동 정보를 넘긴다.
            player.Move(targetPos, targetRot, speed);
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
        Debug.Log(data);
        uiChat.PushMessage(data.ChatMsg, data.PlayerId == MyPlayer.PlayerId, UIChat.ChatType.Global);
    }
    //  주말 목표 입니다람쥐

    // 아이템 사는거 응답 처리
    public void BuyItemResponse(S_BuyItemResponse data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    public void SellItemResponse(S_SellItemResponse data)
    {

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
        Debug.Log($"파티 생성 받은 데이터 : {data.Party}");

        // 기존에 생성된 파티원 UI가 있다면 먼저 제거
        foreach (Transform child in PartyStatusSpawnPoint.transform)
        {
            Destroy(child.gameObject);
        }

        // 파티 생성
        if (data.Success || data.Case == 1)
        {
            PartyUI.SetActive(true);
            PartyNameInputField.text = data.Party.PartyName;

            // 파티 정보 딕셔너리에 저장 (키: partyId, 값: PartyInfo)
            if (partyInfoDict.ContainsKey(data.Party.PartyId))
            {
                partyInfoDict[data.Party.PartyId] = data.Party;
            }
            else
            {
                partyInfoDict.Add(data.Party.PartyId, data.Party);
            }

            if (MyPlayer != null && MyPlayer.PlayerId == data.Party.PartyLeaderId)
            {
                // PartyStatusSpawnPoint의 자식으로 LeaderStatusPrefab 인스턴스 생성
                GameObject leaderStatusObj = Instantiate(LeaderStatusPrefab, PartyStatusSpawnPoint.transform);

                // 인스턴스된 오브젝트의 자식에서 TextMeshProUGUI 컴포넌트 찾기
                TextMeshProUGUI leaderText = leaderStatusObj.GetComponentInChildren<TextMeshProUGUI>();
                if (leaderText != null)
                {
                    leaderText.text = MyPlayer.nickname;
                }
                else
                {
                    Debug.LogWarning("LeaderStatusPrefab에 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
                }
            }
        }
    }
    public void PartyInviteResponse(S_PartyResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 초대 후 받은 데이터 : {data}");

        // 파티 UI 활성화
        PartyUI.SetActive(true);
        // 파티원 추방 버튼은 비활성화 해야할듯? 리더만 활성화해서 
        PartyNameInputField.text = data.Party.PartyName;

        // 응답받은 PartyInfo를 딕셔너리에 업데이트
        if (partyInfoDict.ContainsKey(data.Party.PartyId))
        {
            partyInfoDict[data.Party.PartyId] = data.Party;
        }
        else
        {
            partyInfoDict.Add(data.Party.PartyId, data.Party);
        }


        // 기존에 생성된 파티원 UI가 있다면 먼저 제거
        foreach (Transform child in PartyStatusSpawnPoint.transform)
        {
            Destroy(child.gameObject);
        }

        // PartyInfo의 Players 리스트 순회
        foreach (var playerStatus in data.Party.Players)
        {
            GameObject prefabToInstantiate;

            // PlayerStatus에 PlayerId가 0(또는 기본값)인 경우, 닉네임을 통해 플레이어를 찾아 보완합니다.
            int playerId = 0;
            Player player = GetPlayerByNickname(playerStatus.PlayerName);
            if (player != null)
            {
                playerId = player.PlayerId;
            }

            // 파티 리더이면 LeaderStatusPrefab, 아니면 MemberStatusPrefab 사용
            if (playerId == data.Party.PartyLeaderId)
            {
                prefabToInstantiate = LeaderStatusPrefab;
            }
            else
            {
                prefabToInstantiate = MemberStatusPrefab;
            }

            // 프리팹 인스턴스 생성 및 PartyStatusSpawnPoint의 자식으로 추가
            GameObject statusObj = Instantiate(prefabToInstantiate, PartyStatusSpawnPoint.transform);

            // 인스턴스의 자식에서 TextMeshProUGUI 컴포넌트 찾기 및 닉네임 설정
            TextMeshProUGUI statusText = statusObj.GetComponentInChildren<TextMeshProUGUI>();
            if (statusText != null)
            {
                statusText.text = playerStatus.PlayerName;
            }
            else
            {
                Debug.LogWarning("프리팹에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }
    //파티 가입
    public void PartyJoinHandler(S_PartyResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 가입 받은 데이터 : {data}");

        // 파티 UI 활성화 및 파티 이름 업데이트
        PartyUI.SetActive(true);
        PartyNameInputField.text = data.Party.PartyName;

        // 기존에 생성된 파티원 UI 제거 (중복 갱신 방지)
        foreach (Transform child in PartyStatusSpawnPoint.transform)
        {
            Destroy(child.gameObject);
        }

        if (partyInfoDict.ContainsKey(data.Party.PartyId))
        {
            partyInfoDict[data.Party.PartyId] = data.Party;
        }
        else
        {
            partyInfoDict.Add(data.Party.PartyId, data.Party);
        }

        // 새롭게 업데이트된 PartyInfo의 Players 리스트를 순회하여 UI 생성
        foreach (var playerStatus in data.Party.Players)
        {
            GameObject prefabToInstantiate;

            // 만약 PlayerStatus에 PlayerId 값이 0이라면, 닉네임으로 플레이어를 찾아 보완
            int playerId = 0;
            Player player = GetPlayerByNickname(playerStatus.PlayerName);
            if (player != null)
            {
                playerId = player.PlayerId;
            }

            // 파티 리더이면 LeaderStatusPrefab, 그렇지 않으면 MemberStatusPrefab 사용
            if (playerId == data.Party.PartyLeaderId)
            {
                prefabToInstantiate = LeaderStatusPrefab;
            }
            else
            {
                prefabToInstantiate = MemberStatusPrefab;
            }

            // 프리팹 인스턴스 생성 후 PartyStatusSpawnPoint의 자식으로 추가
            GameObject statusObj = Instantiate(prefabToInstantiate, PartyStatusSpawnPoint.transform);

            // 인스턴스된 오브젝트의 자식에서 TextMeshProUGUI 컴포넌트를 찾아 파티원 닉네임 설정
            TextMeshProUGUI statusText = statusObj.GetComponentInChildren<TextMeshProUGUI>();
            if (statusText != null)
            {
                statusText.text = playerStatus.PlayerName;
            }
            else
            {
                Debug.LogWarning("프리팹에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }
    // 모든 파티 조회
    public void PartyListResponse(S_PartySearchResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 리스트 조회 받은 데이터 : {data}");

        // 기존에 생성된 파티원 UI가 있다면 먼저 제거
        foreach (Transform child in PartyListSpawnPoint.transform)
        {
            Destroy(child.gameObject);
        }

        int count = data.Info.Count;
        Debug.Log($"파티 인포 : {count}");
        for(int i = 0; i < count; i++)
        {
            GameObject partyListObj = Instantiate(PartyListPrefab, PartyListSpawnPoint.transform);
            partyListObj.GetComponent<PartyListItem>().partyData = data.Info[i];
            TextMeshProUGUI[] texts = partyListObj.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (texts.Length >= 4)
            {
                // 첫 번째 텍스트: DungeonName -> 나중에 추가할 던전 선택 생기면 그것으로 대체
                texts[0].text = "Dungeon " + data.Info[i].DungeonIndex.ToString();

                // 두 번째 텍스트: 파티 이름
                texts[1].text = data.Info[i].PartyName;

                // 세 번째 텍스트: 현재 멤버 수 / 최대 멤버 수
                texts[2].text = $"{data.Info[i].Players.Count} / {data.Info[i].Maximum}";

                texts[3].text = data.Info[i].PartyId.ToString();
            }
            else
            {
                Debug.LogWarning("PartyListPrefab 내에 TextMeshProUGUI 컴포넌트가 4개 이상 존재하지 않습니다.");
            }
        }
    }
    // 파티 검색 결과 // 코드 구현
    public void PartySearchResponse(S_PartySearchResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 서치 받은 데이터 : {data}");

        // 검색 성공
        if (data.Success)
        {
            UIPartyPopUp input = FindAnyObjectByType<UIPartyPopUp>();
            string searchTerm = input.partySearchInputField.text;
            Debug.Log($"클라 값 : {searchTerm}");

            // PartyListSpawnPoint 아래에 있는 기존 파티 리스트 UI 제거
            foreach (Transform child in PartyListSpawnPoint.transform)
            {
                Destroy(child.gameObject);
            }

            // data.Info에서 검색어에 맞는 파티들을 LINQ로 찾기 (대소문자 무시)
            var matchingParties = data.Info
                .Where(p => p.PartyName.ToLower().Contains(searchTerm.ToLower()))
                .ToList();

            if (matchingParties.Count > 0)
            {
                foreach (var partyInfo in matchingParties)
                {
                    GameObject partyListObj = Instantiate(PartyListPrefab, PartyListSpawnPoint.transform);
                    partyListObj.GetComponent<PartyListItem>().partyData = partyInfo;
                    TextMeshProUGUI[] texts = partyListObj.GetComponentsInChildren<TextMeshProUGUI>(true);
                    if (texts.Length >= 4)
                    {
                        texts[0].text = partyInfo.PartyId.ToString();
                        texts[1].text = partyInfo.PartyName;
                        texts[2].text = $"{partyInfo.Players.Count} / {partyInfo.Maximum}";
                        texts[3].text = partyInfo.PartyId.ToString();
                    }
                    else
                    {
                        Debug.LogWarning("PartyListPrefab 내에 예상된 TextMeshProUGUI 컴포넌트가 부족합니다.");
                    }
                }
            }
            else
            {
                Debug.Log("검색 결과가 없습니다.");
                SearchResultUI.SetActive(true);
            }
        }
    }

    // 파티 추방
    public void PartyKickResponse(S_PartyResultResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 추방 받은 데이터 : {data}");

        if(data.Success)
        {
            // [1] 추방된 유저가 누구인지 찾는다 (닉네임 등)
            Player kickedPlayer = GetPlayerAvatarById(data.UserId);
            if (kickedPlayer == null)
            {
                Debug.LogWarning("추방된 유저를 로컬에서 찾을 수 없습니다.");
            }

            // [2] 딕셔너리에서 해당 유저가 속해있는 PartyInfo를 찾아서 수정/삭제
            // ---- 기존: List<int> removePartyList = new List<int>();
            List<string> removePartyList = new List<string>();

            foreach (var kvp in partyInfoDict)
            {
                // kvp.Key는 string 타입
                var partyId = kvp.Key;
                var pInfo = kvp.Value;

                int removedCount = 0;
                for (int i = pInfo.Players.Count - 1; i >= 0; i--)
                {
                    if (kickedPlayer != null && pInfo.Players[i].PlayerName == kickedPlayer.nickname)
                    {
                        pInfo.Players.RemoveAt(i);
                        removedCount++;
                    }
                }

                if (removedCount > 0)
                {
                    if (pInfo.Players.Count == 0)
                    {
                        // ---- 기존: removePartyList.Add(partyId); 에서 partyId가 int가 아니므로 에러
                        removePartyList.Add(partyId); // partyId는 string
                    }
                    else
                    {
                        partyInfoDict[partyId] = pInfo;
                    }
                    break;
                }
            }

            // 실제 파티 딕셔너리에서 완전히 제거
            // ---- 기존: foreach (int removeId in removePartyList)
            foreach (string removeId in removePartyList)
            {
                if (partyInfoDict.ContainsKey(removeId))
                {
                    partyInfoDict.Remove(removeId);
                }
            }

            // [3] UI 갱신 처리
            PartyUI.SetActive(false);
            foreach (Transform child in PartyListSpawnPoint.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.Log("강퇴 권한 X");
        }
    }

    // 파티 탈퇴
    public void PartyExitResponse(S_PartyResultResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 탈퇴 받은 데이터 : {data}");

        if (data.Success)
        {
            Player exitPlayer = GetPlayerAvatarById(data.UserId);
            if (exitPlayer == null)
            {
                Debug.LogWarning("탈퇴한 유저를 로컬에서 찾을 수 없습니다.");
            }

            // ---- 기존: List<int> removePartyList = new List<int>();
            List<string> removePartyList = new List<string>();

            foreach (var kvp in partyInfoDict)
            {
                var partyId = kvp.Key;
                var pInfo = kvp.Value;

                int removedCount = 0;
                for (int i = pInfo.Players.Count - 1; i >= 0; i--)
                {
                    if (exitPlayer != null && pInfo.Players[i].PlayerName == exitPlayer.nickname)
                    {
                        pInfo.Players.RemoveAt(i);
                        removedCount++;
                    }
                }

                if (removedCount > 0)
                {
                    if (pInfo.Players.Count == 0)
                    {
                        removePartyList.Add(partyId); // string
                    }
                    else
                    {
                        partyInfoDict[partyId] = pInfo;
                    }
                    break;
                }
            }

            // ---- 기존: foreach (int removeId in removePartyList)
            foreach (string removeId in removePartyList)
            {
                if (partyInfoDict.ContainsKey(removeId))
                {
                    partyInfoDict.Remove(removeId);
                }
            }

            PartyUI.SetActive(false);
            foreach (Transform child in PartyListSpawnPoint.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.Log("파티 탈퇴 실패 혹은 이미 파티에 속해있지 않은 유저입니다.");
        }
    }

    public void PartyUpdateResponse(S_PartyResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 업데이트 받은 데이터 : {data}");

        if (MyPlayer == null || !data.Party.Players.Any(ps => {
            Player p = GetPlayerByNickname(ps.PlayerName);
            return p != null && p.PlayerId == MyPlayer.PlayerId;
        }))
        {
            // 내 플레이어가 해당 파티에 없다면 파티 UI를 숨기고 업데이트 중단
            PartyUI.SetActive(false);
            Debug.Log("내 플레이어가 업데이트된 파티에 포함되어 있지 않습니다. 파티 UI를 숨깁니다.");
            return;
        }

        // 파티 UI 활성화 및 파티 이름 업데이트
        PartyUI.SetActive(true);
        PartyNameInputField.text = data.Party.PartyName;

        // 기존에 생성된 파티원 UI 제거 (중복 갱신 방지)
        foreach (Transform child in PartyStatusSpawnPoint.transform)
        {
            Destroy(child.gameObject);
        }

        // 새롭게 업데이트된 PartyInfo의 Players 리스트를 순회하여 UI 생성
        foreach (var playerStatus in data.Party.Players)
        {
            GameObject prefabToInstantiate;

            // 만약 PlayerStatus에 PlayerId 값이 0이라면, 닉네임으로 플레이어를 찾아 보완
            int playerId = 0;
            Player player = GetPlayerByNickname(playerStatus.PlayerName);
            if (player != null)
            {
                playerId = player.PlayerId;
            }

            // 파티 리더이면 LeaderStatusPrefab, 그렇지 않으면 MemberStatusPrefab 사용
            if (playerId == data.Party.PartyLeaderId)
            {
                prefabToInstantiate = LeaderStatusPrefab;
            }
            else
            {
                prefabToInstantiate = MemberStatusPrefab;
            }

            // 프리팹 인스턴스 생성 후 PartyStatusSpawnPoint의 자식으로 추가
            GameObject statusObj = Instantiate(prefabToInstantiate, PartyStatusSpawnPoint.transform);

            // 인스턴스된 오브젝트의 자식에서 TextMeshProUGUI 컴포넌트를 찾아 파티원 닉네임 설정
            TextMeshProUGUI statusText = statusObj.GetComponentInChildren<TextMeshProUGUI>();
            if (statusText != null)
            {
                statusText.text = playerStatus.PlayerName;
            }
            else
            {
                Debug.LogWarning("프리팹에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }
    // 던전 쪽 추후 추가 예정
    /* 여기까지 */
    // 마켓 관련 패킷 추가 
    public void MarketListResponse(S_MarketList data)
    {
        market.SetBuyData(data);
    }
    public void SellInMarketResponse(S_SellInMarket data)
    {
        
        StartCoroutine("errorText");
        Debug.Log(data);
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
    }
    public void BuyInMarketResponse(S_BuyInMarket data)
    {
        StartCoroutine("errorText");
        Debug.Log(data);
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);

    }
    public void MarketMyListResponse(S_MarketMyList data)
    {
        market.SetSellData(data);
    }
    public void MarketSelectBuyName(S_MarketSelectBuyName data)
    {
        market.SetSelectData(data);
    }
    public void MatchingNotification(S_MatchingNotification data)
    {
        if (data.IsStart)
            MatchingWindow.SetActive(true);
        else
            MatchingWindow.SetActive(false);
    }

    // 자기 자신 스폰용도 
    public void Spawn(PlayerInfo playerInfo, bool isPlayer = false)
    {
        if (isPlayer)
        {
            Debug.Log("플레이어 입니다.");
            //Vector3 spawnPos = CalculateSpawnPosition(playerInfo.Transform);
            MyPlayer = CreatePlayer(playerInfo, new Vector3(playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ));//CreatePlayer(playerInfo, spawnPos);
            MyPlayer.SetIsMine(true);

            ActivateGameUI();
            return;
        }
        //CreatePlayer(playerInfo, new Vector3 (playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ + 136.5156f));
        Player player = CreatePlayer(playerInfo, new Vector3(playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ));
        player.SetIsMine(false);

        // 플레이어를 리스트에 추가
        players.Add(player);
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

    public void ReleasePlayer(int playerId)
    {
        if (!playerList.TryGetValue(playerId, out var player)) return;

        playerList.Remove(playerId);
        Destroy(player.gameObject);
    }

    private void ActivateGameUI()
    {
        UiRegister.gameObject.SetActive(false);
        uiChat.gameObject.SetActive(true);
        uiAnimation.Init();
        uiAnimation.Show();
    }

    public void UpdateInventory(S_InventoryResponse data){
        // 인벤토리 갱신
        uiAnimation.UpdateInventory(data);
    }

    public Player GetPlayerAvatarById(int playerId)
    {
        return playerList.TryGetValue(playerId, out var player) ? player : null;
    }
}