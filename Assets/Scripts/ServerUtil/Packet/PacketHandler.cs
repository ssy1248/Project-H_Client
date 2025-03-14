﻿using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Diagnostics;
using UnityEngine.Events;
using UnityEngine;

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
        if (TownManager.Instance != null)
        {
            TownManager.Instance.Despawn(enterPacket);
            return;
        }
        if (DungeonManager.Instance != null)
        {
            DungeonManager.Instance.Despawn(enterPacket);
            return;
        }

    }
    public static void S_MoveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Move enterPacket) return;
        if (TownManager.Instance != null)
        {
            TownManager.Instance.AllMove(enterPacket);
            //return;
        }
        if (DungeonManager.Instance != null)
        {
            DungeonManager.Instance.AllMove(enterPacket);
            return;
        }

    }
    public static void S_AnimationHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Animation enterPacket) return;
        TownManager.Instance.AllAnimation(enterPacket);
    }
    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_Chat enterPacket) return;
        TownManager.Instance.ChatResponse(enterPacket);
    }
    public static void S_BuyItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BuyItemResponse enterPacket) return;
    }

    public static void S_EquipItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_EquipItemResponse enterPacket) return;
        TownManager.Instance.S_EquipItemEvent.Invoke(enterPacket);
    }

    public static void S_DisrobeItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_DisrobeItemResponse enterPacket) return;
        TownManager.Instance.S_DisrobeItemEvent?.Invoke(enterPacket);
    }

    public static void S_MoveItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MoveItemResponse enterPacket) return;
        TownManager.Instance.S_MoveItemEvent?.Invoke(enterPacket);
    }

    public static void S_ActiveItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ActiveItemResponse enterPacket) return;
        TownManager.Instance.S_ActiveItemEvent?.Invoke(enterPacket);
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
        var dm = DungeonManager.Instance;
        if(!dm) return;
        dm.DungeonFailureHandler();
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
    public static void S_MarketMyListHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MarketMyList enterPacket) return;
        TownManager.Instance.MarketMyListResponse(enterPacket);
    }
    public static void S_MarketListHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MarketList enterPacket) return;
        TownManager.Instance.MarketListResponse(enterPacket);
    }
    public static void S_SellInMarketHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SellInMarket enterPacket) return;
        TownManager.Instance.SellInMarketResponse(enterPacket);
    }
    public static void S_BuyInMarketHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BuyInMarket enterPacket) return;
        TownManager.Instance.BuyInMarketResponse(enterPacket);
    }
   
    public static void S_InventoryHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_InventoryResponse enterPacket) return;
        TownManager.Instance.S_InventoryEvent?.Invoke(enterPacket);
    }
    public static void S_MatchResponse(PacketSession session, IMessage packet)
    {
        if (packet is not S_MatchResponse enterPacket) return;
        TownManager.Instance.MatchResponse(enterPacket);
    }
    public static void S_MatchStopResponse(PacketSession session, IMessage packet)
    {
        if (packet is not S_MatchStopResponse enterPacket) return;
        TownManager.Instance.MatchStop(enterPacket);
    }
    public static void S_MatchingNotification(PacketSession session, IMessage packet)
    {
        if (packet is not S_MatchingNotification enterPacket) return;
        TownManager.Instance.MatchingNotification(enterPacket);
    }
    public static void S_MarketSelectBuyNameHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MarketSelectBuyName enterPacket) return;
        TownManager.Instance.MarketSelectBuyName(enterPacket);
    }
    public static void S_SellItemHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SellItemResponse enterPacket) return;
        TownManager.Instance.SellItemResponse(enterPacket);
    }
    public static void S_ShopInventoryListHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ShopInventoryList enterPacket) return;
        TownManager.Instance.ShopInventoryList(enterPacket);
    }
    public static void S_EndAuctionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_EndAuction enterPacket) return;
        DungeonManager.Instance.EndAuctionResponse(enterPacket);
    }
    public static void S_EnterAuctionBidHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_EnterAuctionBid enterPacket) return;
        DungeonManager.Instance.EnterAuctionBidResponse(enterPacket);
    }
    public static void S_FinalizeAllAuctionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_FinalizeAllAuction enterPacket) return;
        DungeonManager.Instance.FinalizeAllAuctionResponse(enterPacket);
    }
    public static void S_FinalizeBuyAuctionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_FinalizeBuyAuction enterPacket) return;
        DungeonManager.Instance.FinalizeBuyAuctionResponse(enterPacket);
    }
    public static void S_SetAuctionDataHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SetAuctionData enterPacket) return;
        DungeonManager.Instance.SetAuctionDataResponse(enterPacket);
    }
    public static void S_WaitAuctionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_WaitAuction enterPacket) return;
        DungeonManager.Instance.WaitAuctionResponse(enterPacket);
    }
    public static void S_DungeonSpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_DungeonSpawn enterPacket) return;
        DungeonManager.Instance.DungeonSpawn(enterPacket);
    }
    public static void S_DungeonDeSpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_DungeonDeSpawn enterPacket) return;
        DungeonManager.Instance.DungeonDeSpawn(enterPacket);
    }

    public static void S_PlayerActionHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_PlayerAction enterPacket) return;
        PlayerActionManager.Instance.PlayerActionHandler(enterPacket);
    }

    // 몬스터 관련.
    public static void S_MonsterSpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MonsterSpawn spawnPacket) return;

        // 몬스터 스폰 관련 함수.
        MonsterManager.Instance.CreateMonsters(spawnPacket);
    }

    public static void S_MonsterMoveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MonsterMove monsterMovePacket) return;

        // 몬스터 이동 관련 함수.
        MonsterManager.Instance.UpdateMonsters(monsterMovePacket);
    }

public static void S_MonsterHitHandler(PacketSession session, IMessage packet)
{
    if (packet is not S_MonsterHit monsterHitPacket) return;

        MonsterManager.Instance.MonsterHitAnimation(monsterHitPacket);

}

    public static void S_MonsterAttckHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MonsterAttck monsterAttckPacket) return;

        MonsterManager.Instance.MonsterAttckAnimation(monsterAttckPacket);
    }

    public static void S_MonsterDieHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_MonsterDie monsterDiePacket) return;

        MonsterManager.Instance.DeleteMonster(monsterDiePacket);
    }
    public static void S_SetUserHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_SetUserState setUserStatePacket) return;

        TownManager.Instance.SetUserState(setUserStatePacket);
    }

    // [보스]
    public static void S_BossSpawnHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BossSpawn bossSpawnPacket) return;
        
        BossManager.Instance.SpawnBoss(bossSpawnPacket);
        UnityEngine.Debug.Log("스폰 패킷");

    }

    public static void S_BossMoveHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BossMove bossMovePacket) return;

        BossManager.Instance.MoveBoss(bossMovePacket);
        UnityEngine.Debug.Log("무브 패킷");

    }

    public static void S_BossHitHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BossHit bossHitPacket) return;

        
    }

    public static void S_BossDieHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BossDie bossDiePacket) return;
        BossManager.Instance.OnBossDeath();



    }

    public static void S_BossSkillStartHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BossSkillStart bossSkillStartPacket) return;
        BossManager.Instance.ReceiveSkillPacket(bossSkillStartPacket);



    }

    public static void S_BossSkillEndHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_BossSkillEnd bossSkillEndPacket) return;

        
    }
    public static void S_ClearBoxHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_ClearBox boxPacket) return;
        DungeonManager.Instance.ClearBox(boxPacket);

    }
    public static void S_GetExpEndHandler(PacketSession session, IMessage packet)
    {
        if (packet is not S_GetExp expPacket) return;
        DungeonManager.Instance.GetExp(expPacket);
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