using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;

class PacketHandler
{
    #region Town

    public static void S_RegisterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_RegisterResponse enterPacket) return;
        TownManager.Instance.RegisterResponse(enterPacket);
    }
    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_LoginResponse enterPacket) return;
        TownManager.Instance.LoginResponse(enterPacket);
    }
    public static void S_EnterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Enter enterPacket) return;
        TownManager.Instance.Enter(enterPacket);
    }
    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Spawn enterPacket) return;
        TownManager.Instance.AllSpawn(enterPacket);
    }
    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Despawn enterPacket) return;
        TownManager.Instance.Despawn(enterPacket);
    }
    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Move enterPacket) return;
        TownManager.Instance.AllMove(enterPacket);
    }
    public static void S_AnimationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Animation enterPacket) return;
        TownManager.Instance.AllAnimation(enterPacket);
    }
    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Chat enterPacket) return;
    }
    public static void S_BuyItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BuyItemResponse enterPacket) return;
    }
    public static void S_EquipItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_EquipItemResponse enterPacket) return;
    }
    public static void S_DisrobeItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_DisrobeItemResponse enterPacket) return;
    }
    public static void S_ActiveItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ActiveItemResponse enterPacket) return;
    }
    public static void S_PartyHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_PartyResponse enterPacket) return;
        if (enterPacket.Case == 1)
        {
            // 파티 생성 핸들러
            TownManager.Instance.PartyResponse(enterPacket);
        }
        else if (enterPacket.Case == 2)
        {
            // 파티 초대 핸들러
            TownManager.Instance.PartyInviteResponse(enterPacket);
        }
        else if (enterPacket.Case == 3)
        {
            // 파티 가입 핸들러
            TownManager.Instance.PartyJoinHandler(enterPacket);
        }
        else if (enterPacket.Case == 4)
        {
            // 파티 업데이트 
            TownManager.Instance.PartyUpdateResponse(enterPacket);
        }
    }
    public static void S_EnterDungeonHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_EnterDungeon enterPacket) return;
    }
    public static void S_LeaveDungeonHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_LeaveDungeon enterPacket) return;
    }
    public static void S_ScreenTextHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ScreenText enterPacket) return;
    }
    public static void S_ScreenDoneHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ScreenDone enterPacket) return;
    }
    // 조회 관련 
    public static void S_PartySearchHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_PartySearchResponse enterPacket) return;
        if (enterPacket.Case == 1)
        {
            // 모든 파티 조회
            TownManager.Instance.PartyListResponse(enterPacket);
        }
        else if (enterPacket.Case == 2)
        {
            // 파티 검색 핸들러
            TownManager.Instance.PartySearchResponse(enterPacket);
        }
    }
    // 해체 관련 패킷
    public static void S_PartyResultHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_PartyResultResponse enterPacket) return;
        if (enterPacket.Case == 1)
        {
            TownManager.Instance.PartyKickResponse(enterPacket);
        }
        else if (enterPacket.Case == 2)
        {
            TownManager.Instance.PartyExitResponse(enterPacket);
        }
    }
    public static void S_marketMyListHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_marketMyList enterPacket) return;
    }
    public static void S_marketListHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_marketList enterPacket) return;
    }
    public static void S_SellInMarketHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SellInMarket enterPacket) return;
    }
    public static void S_BuyInMarketHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BuyInMarket enterPacket) return;
    }
    public static void S_InventoryHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_InventoryResponse enterPacket) return;
    }
    /*
    public static void S_EnterHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Enter enterPacket) return;
        TownManager.Instance.Spawn(enterPacket.Player);
    }

    public static void S_LeaveHandler(PacketSession session, IMessage packet) { }

    public static void S_SpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Spawn spawnPacket) return;

        foreach (var playerInfo in spawnPacket.Players)
        {
            Vector3 spawnPosition = new Vector3(playerInfo.Transform.PosX, playerInfo.Transform.PosY, playerInfo.Transform.PosZ);
            var player = TownManager.Instance.CreatePlayer(playerInfo, spawnPosition);
            player.SetIsMine(false);
        }
    }

    public static void S_DespawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Despawn despawnPacket) return;

        foreach (int playerId in despawnPacket.PlayerIds)
        {
            TownManager.Instance.ReleasePlayer(playerId);
        }
    }

    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Move movePacket) return;

        TransformInfo transform = movePacket.Transform;
        Vector3 position = new Vector3(transform.PosX, transform.PosY, transform.PosZ);
        Quaternion rotation = Quaternion.Euler(0, transform.Rot, 0);

        var player = TownManager.Instance.GetPlayerAvatarById(movePacket.PlayerId);
        player?.Move(position, rotation);
    }

    public static void S_AnimationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Animation animationPacket) return;

        var player = TownManager.Instance.GetPlayerAvatarById(animationPacket.PlayerId);
        player?.PlayAnimation(animationPacket.AnimCode);
    }

    public static void S_ChangeCostumeHandler(PacketSession session, IMessage packet) { }

    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Chat chatPacket) return;

        var player = TownManager.Instance.GetPlayerAvatarById(chatPacket.PlayerId);
        player?.RecvMessage(chatPacket.ChatMsg);
    }

    #endregion

    #region Battle

    public static void S_EnterDungeonHandler(PacketSession session, IMessage packet)
    {

        if (packet is not S_EnterDungeon pkt) return;
 
        Scene currentScene = SceneManager.GetActiveScene();


        if (currentScene.name == GameManager.BattleScene)
        {
            BattleManager.Instance.ConfigureGame(pkt);
        }
        else
        {
            GameManager.Instance.Pkt = pkt;
            SceneManager.LoadScene(GameManager.BattleScene);

        }
    }

    public static void S_LeaveDungeonHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_LeaveDungeon pkt) return;

        SceneManager.LoadScene(GameManager.TownScene);
    }

    public static void S_ScreenTextHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ScreenText pkt) return;

        BattleManager.Instance.UiScreen?.Display(pkt.ScreenText);
    }

    public static void S_ScreenDoneHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ScreenDone pkt) return;

        var uiScreen = BattleManager.Instance.UiScreen;
        if (uiScreen != null)
        {
            uiScreen.gameObject.SetActive(false);
        }
    }

    public static void S_BattleLogHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BattleLog pkt) return;

        BattleManager.Instance.UiBattleLog?.Initialize(pkt.BattleLog);
    }

    public static void S_SetPlayerHpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SetPlayerHp pkt) return;

        BattleManager.Instance.UiPlayerInformation?.UpdateHP(pkt.Hp);
    }

    public static void S_SetPlayerMpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SetPlayerMp pkt) return;

        BattleManager.Instance.UiPlayerInformation?.UpdateMP(pkt.Mp);
    }

    public static void S_SetMonsterHpHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SetMonsterHp pkt) return;

        BattleManager.Instance.UpdateMonsterHp(pkt.MonsterIdx, pkt.Hp);
    }

    public static void S_PlayerActionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_PlayerAction pkt) return;

        Monster monster = BattleManager.Instance.GetMonster(pkt.TargetMonsterIdx);
        monster?.Hit();

        BattleManager.Instance.TriggerPlayerAnimation(pkt.ActionSet.AnimCode);
        EffectManager.Instance.SetEffectToMonster(pkt.TargetMonsterIdx, pkt.ActionSet.EffectCode);
    }

    public static void S_MonsterActionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MonsterAction pkt) return;

        Monster monster = BattleManager.Instance.GetMonster(pkt.ActionMonsterIdx);
        monster?.SetAnim(pkt.ActionSet.AnimCode);

        BattleManager.Instance.TriggerPlayerHitAnimation();
        EffectManager.Instance.SetEffectToPlayer(pkt.ActionSet.EffectCode);
    }
    */
    #endregion
}