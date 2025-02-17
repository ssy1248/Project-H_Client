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
    [SerializeField] private UIChat uiChat;
    [SerializeField] private TMP_Text txtServer;

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
    private Dictionary<int, PartyInfo> partyInfoDict = new Dictionary<int, PartyInfo>();

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
        Debug.Log(data);
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
    }
    // 나가면 삭제해주기 
    public void Despawn(S_Despawn data)
    {
        StartCoroutine("erroText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.PlayerIds.ToString());
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
        // 가입
        else if (data.Success || data.Case == 3)
        {

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

        int count = data.Info.Count;
        for(int i = 0; i < count; i++)
        {
            // 기존에 생성된 파티원 UI가 있다면 먼저 제거
            foreach (Transform child in PartyListSpawnPoint.transform)
            {
                Destroy(child.gameObject);
            }

            GameObject partyListObj = Instantiate(PartyListPrefab, PartyListSpawnPoint.transform);
            partyListObj.GetComponent<PartyListItem>().partyData = data.Info[i];
            TextMeshProUGUI[] texts = partyListObj.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (texts.Length >= 4)
            {
                // 첫 번째 텍스트: DungeonName -> 나중에 추가할 던전 선택 생기면 그것으로 대체
                texts[0].text = data.Info[i].PartyId.ToString();

                // 두 번째 텍스트: 파티 이름
                texts[1].text = data.Info[i].PartyName;

                // 세 번째 텍스트: 현재 멤버 수 / 최대 멤버 수
                texts[2].text = $"{data.Info[i].Players.Count} / {data.Info[i].Maximum}";

                texts[3].text = data.Info[i].PartyId.ToString();
            }
            else
            {
                Debug.LogWarning("PartyListPrefab 내에 TextMeshProUGUI 컴포넌트가 3개 이상 존재하지 않습니다.");
            }
        }
    }
    // 파티 검색 결과
    public void PartySearchResponse(S_PartySearchResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 서치 받은 데이터 : {data}");
    }

    // 파티 추방
    public void PartyKickResponse(S_PartyResultResponse data)
    {
        StartCoroutine("errorText");
        errorText.GetComponent<TextMeshProUGUI>().SetText(data.Message);
        Debug.Log($"파티 추방 받은 데이터 : {data}");

        if(data.Success)
        {
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

        PartyUI.SetActive(false);
        foreach (Transform child in PartyListSpawnPoint.transform)
        {
            Destroy(child.gameObject);
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
        UiRegister.gameObject.SetActive(false);
        uiChat.gameObject.SetActive(true);
        uiAnimation.gameObject.SetActive(true);
    }

    public Player GetPlayerAvatarById(int playerId)
    {
        return playerList.TryGetValue(playerId, out var player) ? player : null;
    }
}