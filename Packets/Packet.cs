using System;
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
        public int ID;

        public ConnectPacket(string _userName, int _ID)
        {
            userName = _userName;
            ID = _ID;
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
}
