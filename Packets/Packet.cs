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
        UserListPacket,
        GameConnect,
        GameDisconnect,
        GameSetWord,
        GameEnterCharacter,
        GameResult
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
    public class PrivateMessagePacket : Packet
    {
        public string sender;
        public string receiver;
        public string message;

        public PrivateMessagePacket(string _sender, string _receiver, string _message)
        {
            sender = _sender;
            receiver = _receiver;
            message = _message;
            packetType = PacketType.PrivateMessage;
        }
    }
    [Serializable]
    public class NewNamePacket : Packet
    {
        public string newName;

        public NewNamePacket(string _newName)
        {
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
        public List<string> userList;

        public UserListPacket(List<string> _userList)
        {
            userList = _userList;
            packetType = PacketType.UserListPacket;
        }
    }
    [Serializable]
    public class GameConnectPacket : Packet
    {
        public int ID;

        public GameConnectPacket(int _ID)
        {
            ID = _ID;
            packetType = PacketType.GameConnect;
        }
    }
    [Serializable]
    public class GameDisconnectPacket : Packet
    {
        public int ID;

        public GameDisconnectPacket(int _ID)
        {
            ID = _ID;
            packetType = PacketType.GameDisconnect;
        }
    }
    [Serializable]
    public class GameSetWordPacket : Packet
    {
        public string correctWord;

        public GameSetWordPacket(string _correctWord)
        {
            correctWord = _correctWord;
            packetType = PacketType.GameSetWord;
        }
    }
    [Serializable]
    public class GameEnterCharacterPacket : Packet
    {
        public char character;

        public GameEnterCharacterPacket(char _character)
        {
            character = _character;
            packetType = PacketType.GameEnterCharacter;
        }
    }
    [Serializable]
    public class GameResultPacket : Packet
    {
        public bool win;

        public GameResultPacket(bool _win)
        {
            win = _win;
            packetType = PacketType.GameResult;
        }
    }
}
