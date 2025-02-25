using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using UnityEngine;

class PacketManager
{
    #region Singleton
    static PacketManager _instance = new();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager()
    {
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

    public void Register()
    {
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_RegisterResponse>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_RegisterHandler);
        _onRecv.Add((ushort)MsgId.SLoginresponse, MakePacket<S_LoginResponse>);
        _handler.Add((ushort)MsgId.SLoginresponse, PacketHandler.S_LoginHandler);
        _onRecv.Add((ushort)MsgId.SEnter, MakePacket<S_Enter>);
        _handler.Add((ushort)MsgId.SEnter, PacketHandler.S_EnterHandler);
        _onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
        _handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);
        _onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
        _handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);
        _onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
        _handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);
        _onRecv.Add((ushort)MsgId.SAnimation, MakePacket<S_Animation>);
        _handler.Add((ushort)MsgId.SAnimation, PacketHandler.S_AnimationHandler);
        _onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
        _handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);
        _onRecv.Add((ushort)MsgId.SBuyitemresponse, MakePacket<S_BuyItemResponse>);
        _handler.Add((ushort)MsgId.SBuyitemresponse, PacketHandler.S_BuyItemHandler);
        _onRecv.Add((ushort)MsgId.SEquipitemresponse, MakePacket<S_EquipItemResponse>);
        _handler.Add((ushort)MsgId.SEquipitemresponse, PacketHandler.S_EquipItemHandler);
        _onRecv.Add((ushort)MsgId.SDisrobeitemresponse, MakePacket<S_DisrobeItemResponse>);
        _handler.Add((ushort)MsgId.SDisrobeitemresponse, PacketHandler.S_DisrobeItemHandler);
        _onRecv.Add((ushort)MsgId.SActiveitemrequest, MakePacket<S_ActiveItemResponse>);
        _handler.Add((ushort)MsgId.SActiveitemrequest, PacketHandler.S_ActiveItemHandler);
        _onRecv.Add((ushort)MsgId.SPartyresponse, MakePacket<S_PartyResponse>);
        _handler.Add((ushort)MsgId.SPartyresponse, PacketHandler.S_PartyHandler);
        _onRecv.Add((ushort)MsgId.SEnterdungeon, MakePacket<S_EnterDungeon>);
        _handler.Add((ushort)MsgId.SEnterdungeon, PacketHandler.S_EnterDungeonHandler);
        _onRecv.Add((ushort)MsgId.SLeavedungeon, MakePacket<S_LeaveDungeon>);
        _handler.Add((ushort)MsgId.SLeavedungeon, PacketHandler.S_LeaveDungeonHandler);
        _onRecv.Add((ushort)MsgId.SScreentext, MakePacket<S_ScreenText>);
        _handler.Add((ushort)MsgId.SScreentext, PacketHandler.S_ScreenTextHandler);
        _onRecv.Add((ushort)MsgId.SScreendone, MakePacket<S_ScreenDone>);
        _handler.Add((ushort)MsgId.SScreendone, PacketHandler.S_ScreenDoneHandler);
        _onRecv.Add((ushort)MsgId.SPartysearchresponse, MakePacket<S_PartySearchResponse>);
        _handler.Add((ushort)MsgId.SPartysearchresponse, PacketHandler.S_PartySearchHandler);
        _onRecv.Add((ushort)MsgId.SPartyresultresponse, MakePacket<S_PartyResultResponse>);
        _handler.Add((ushort)MsgId.SPartyresultresponse, PacketHandler.S_PartyResultHandler);
        _onRecv.Add((ushort)MsgId.SMarketlist, MakePacket<S_MarketList>);
        _handler.Add((ushort)MsgId.SMarketlist, PacketHandler.S_MarketListHandler);
        _onRecv.Add((ushort)MsgId.SMarketmylist, MakePacket<S_MarketMyList>);
        _handler.Add((ushort)MsgId.SMarketmylist, PacketHandler.S_MarketMyListHandler);
        _onRecv.Add((ushort)MsgId.SSellinmarket, MakePacket<S_SellInMarket>);
        _handler.Add((ushort)MsgId.SSellinmarket, PacketHandler.S_SellInMarketHandler);
        _onRecv.Add((ushort)MsgId.SBuyinmarket, MakePacket<S_BuyInMarket>);
        _handler.Add((ushort)MsgId.SBuyinmarket, PacketHandler.S_BuyInMarketHandler);
        _onRecv.Add((ushort)MsgId.SInventoryresponse, MakePacket<S_InventoryResponse>);
        _handler.Add((ushort)MsgId.SInventoryresponse, PacketHandler.S_InventoryHandler);
        _onRecv.Add((ushort)MsgId.SMarketselectbuyname, MakePacket<S_MarketSelectBuyName>);
        _handler.Add((ushort)MsgId.SMarketselectbuyname, PacketHandler.S_MarketSelectBuyNameHandler);
        _onRecv.Add((ushort)MsgId.SMatchresponse, MakePacket<S_MatchResponse>);
        _handler.Add((ushort)MsgId.SMatchresponse, PacketHandler.S_MatchResponse);
        _onRecv.Add((ushort)MsgId.SMatchstopresponse, MakePacket<S_MatchStopResponse>);
        _handler.Add((ushort)MsgId.SMatchstopresponse, PacketHandler.S_MatchStopResponse);
        _onRecv.Add((ushort)MsgId.SMatchingnotification, MakePacket<S_MatchingNotification>);
        _handler.Add((ushort)MsgId.SMatchingnotification, PacketHandler.S_MatchingNotification);
        _onRecv.Add((ushort)MsgId.SSellitemresponse, MakePacket<S_SellItemResponse>);
        _handler.Add((ushort)MsgId.SSellitemresponse, PacketHandler.S_SellItemHandler);
        _onRecv.Add((ushort)MsgId.SShopinventorylist, MakePacket<S_ShopInventoryList>);
        _handler.Add((ushort)MsgId.SShopinventorylist, PacketHandler.S_ShopInventoryListHandler);
        _onRecv.Add((ushort)MsgId.SMoveitemresponse, MakePacket<S_MoveItemResponse>);
        _handler.Add((ushort)MsgId.SMoveitemresponse, PacketHandler.S_MoveItemHandler);

        Debug.Log("핸들러 등록 완료");
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        //Debug.Log($"PacketManager.OnRecvPacket 호출: {BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count)}");

        ushort count = 0;

        // 크기를 4바이트로 읽음
        int size = BitConverter.ToInt32(buffer.Array, buffer.Offset);
        if (buffer.Count < size)
        {
            Debug.LogError("패킷 크기가 부족합니다. 데이터가 불완전합니다.");
            return;
        }

        Debug.Log($"패킷 크기: {size}");
        count += 4;

        // 아이디를 1바이트로 읽음
        byte id = buffer.Array[buffer.Offset + count];
        if (!_onRecv.ContainsKey(id))
        {
            Debug.LogError($"알 수 없는 패킷 ID: {id}");
            return;
        }

        Debug.Log($"패킷 ID: {id}");
        count += 1;

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;



        if (_onRecv.TryGetValue(id, out action))
        {
            Debug.Log($"패킷 핸들러 실행: {id}");
            action.Invoke(session, buffer, id);
        }
        else
        {
            Debug.LogError($"등록되지 않은 패킷 ID: {id}");
        }
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
    {
        try
        {
            T pkt = new T();
            pkt.MergeFrom(buffer.Array, buffer.Offset + 5, buffer.Count - 5);
            Debug.Log($"패킷 역직렬화 성공: {typeof(T).Name}");

            if (CustomHandler != null)
            {
                CustomHandler.Invoke(session, pkt, id);
            }
            else
            {
                Action<PacketSession, IMessage> action = null;
                if (_handler.TryGetValue(id, out action))
                {
                    Debug.Log($"핸들러 실행: {id}");
                    action.Invoke(session, pkt);
                }
                else
                {
                    Debug.LogError($"핸들러가 등록되지 않음: {id}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"패킷 역직렬화 실패: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
    {
        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            return action;
        return null;
    }
}