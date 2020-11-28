using System;
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
        ClientName
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
}
