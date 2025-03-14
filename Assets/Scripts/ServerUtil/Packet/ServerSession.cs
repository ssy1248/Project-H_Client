﻿using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ServerSession : PacketSession
{
	public void Send(IMessage packet)
	{
		string msgName = packet.Descriptor.Name.Replace("_", String.Empty);
		MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName,true);
			
		ushort size = (ushort)packet.CalculateSize();
		// byte[] sendBuff = new byte[size + 4]; 
		// Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuff, 0, sizeof(ushort)); // 어느정도 크기의 데이터인지
		// Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuff, 2, sizeof(ushort)); // 프로토콜의 아이디
		// Array.Copy(packet.ToByteArray(), 0, sendBuff, 4, size); // 전달하려는 데이터
		
		byte[] sendBuff = new byte[size + 5]; // 크기(4바이트) + 아이디(1바이트) + 데이터 크기
		Array.Copy(BitConverter.GetBytes(size + 5), 0, sendBuff, 0, sizeof(int)); // 데이터 크기 (4바이트)
		sendBuff[4] = (byte)msgId; // 프로토콜의 아이디 (1바이트)
		Array.Copy(packet.ToByteArray(), 0, sendBuff, 5, size); // 전달하려는 데이터
		
		Send(new ArraySegment<byte>(sendBuff));
    }
	
	public override void OnConnected(EndPoint endPoint)
	{
		Debug.Log($"OnConnected : {endPoint}");
		IsConnected = true;
		
		
		TownManager.Instance.Connected();

        if (PacketManager.Instance == null)
        {
            Debug.LogError("PacketManager 초기화되지 않음!");
        }

        PacketManager.Instance.CustomHandler = (s, m, i) =>
		{
			PacketQueue.Instance.Push(i, m);
#if !UNITY_EDITOR
			Debug.Log($"Packet id : [{i}] {(MsgId)i}  -  msg : {m}");
#endif
		};

        Debug.Log($"클라이언트 연결 상태: {IsConnected}");
    }

	public override void OnDisconnected(EndPoint endPoint)
	{
		Debug.Log($"OnDisconnected : {endPoint}");
		IsConnected = false;
	}

	public override void OnRecvPacket(ArraySegment<byte> buffer)
	{
        //Debug.Log($"패킷 수신 크기: {buffer.Count}");
        PacketManager.Instance.OnRecvPacket(this, buffer);
	}

	public override void OnSend(int numOfBytes)
	{
		//Console.WriteLine($"Transferred bytes: {numOfBytes}");
	}
}