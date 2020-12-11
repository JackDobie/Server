﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packets
{
    public enum PacketType
    {
        ChatMessage,
        PrivateMessage,
        NewName,
        Connect,
        Disconnect,
        UserListPacket
    }

    [Serializable]
    public class Packet
    {
        public PacketType packetType
        {
            get;
            set;
        }
    }

    [Serializable]
    public class ChatMessagePacket : Packet
    {
        public string message;

        public ChatMessagePacket(string _message)
        {
            message = _message;
            packetType = PacketType.ChatMessage;
        }
    }
    [Serializable]
    public class NewNamePacket : Packet
    {
        public string oldName, newName;

        public NewNamePacket(string _oldName, string _newName)
        {
            oldName = _oldName;
            newName = _newName;
            packetType = PacketType.NewName;
        }
    }
    [Serializable]
    public class ConnectPacket : Packet
    {
        public string userName;

        public ConnectPacket(string _userName)
        {
            userName = _userName;
            packetType = PacketType.Connect;
        }
    }
    [Serializable]
    public class DisconnectPacket : Packet
    {
        public string userName;

        public DisconnectPacket(string _userName)
        {
            userName = _userName;
            packetType = PacketType.Disconnect;
        }
    }
    [Serializable]
    public class UserListPacket : Packet
    {
        public ConcurrentDictionary<int, string> userList;

        public UserListPacket(ConcurrentDictionary<int, string> _userList)
        {
            userList = _userList;
            packetType = PacketType.UserListPacket;
        }
    }
}
