using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using UnityEngine;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
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
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_LoginResponse>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_LoginHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_Enter>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_EnterHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_Spawn>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_SpawnHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_Despawn>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_DespawnHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_Move>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_MoveHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_Animation>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_AnimationHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_Chat>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_ChatHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_BuyItemResponse>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_BuyItemHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_EquipItemResponse>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_EquipItemHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_DisrobeItemResponse>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_DisrobeItemHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_ActiveItemRequest>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_ActiveItemHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_PartyResponse>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_PartyHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_EnterDungeon>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_EnterDungeonHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_LeaveDungeon>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_LeaveDungeonHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_ScreenText>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_ScreenTextHandler);
        _onRecv.Add((ushort)MsgId.SRegisterresponse, MakePacket<S_ScreenDone>);
        _handler.Add((ushort)MsgId.SRegisterresponse, PacketHandler.S_ScreenDoneHandler);
        Debug.Log("핸들러 등록 완료");
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        Debug.Log($"PacketManager.OnRecvPacket 호출: {BitConverter.ToString(buffer.Array, buffer.Offset, buffer.Count)}");

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