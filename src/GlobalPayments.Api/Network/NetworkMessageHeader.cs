using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Network {
    public class NetworkMessageHeader {
        public NetworkTransactionType NetworkTransactionType{ get; set; }
        public MessageType MessageType{ get; set; }
        public CharacterSet CharacterSet{ get; set; }
        public NetworkResponseCode ResponseCode{ get; set; }
        public NetworkResponseCodeOrigin ResponseCodeOrigin{ get; set; }
        public NetworkProcessingFlag ProcessingFlag{ get; set; }
        public ProtocolType ProtocolType{ get; set; }
        public ConnectionType ConnectionType{ get; set; }
        public string NodeIdentification{ get; set; }
        public byte[] OriginCorrelation1{ get; set; }
        public string CompanyId{ get; set; }
        public byte[] OriginCorrelation2{ get; set; }
        public byte Version{ get; set; }

        public static NetworkMessageHeader Parse(byte[] buffer) {
            NetworkMessageHeader header = new NetworkMessageHeader();

            StringParser sp = new StringParser(buffer);
            //header.NetworkTransactionType = sp.ReadStringConstant<NetworkTransactionType>(2);
            header.NetworkTransactionType = EnumConverter.FromMapping<NetworkTransactionType>(Target.NWS, sp.ReadString(2));
            sp.ReadBytes(2);
            //header.MessageType = sp.ReadByteConstant<MessageType>();
            //header.CharacterSet = sp.ReadByteConstant<CharacterSet>();
            //header.ResponseCode = sp.ReadByteConstant<NetworkResponseCode>();
            //header.ResponseCodeOrigin = sp.ReadByteConstant<NetworkResponseCodeOrigin>();
            //header.ProcessingFlag = sp.ReadByteConstant<NetworkProcessingFlag>();
            header.MessageType = (MessageType)Enum.Parse(typeof(MessageType), (Encoding.UTF8.GetBytes(sp.ReadString(1))[0]).ToString());
            header.CharacterSet = (CharacterSet)Enum.Parse(typeof(CharacterSet), (Encoding.UTF8.GetBytes(sp.ReadString(1))[0]).ToString());
            header.ResponseCode = (NetworkResponseCode)Enum.Parse(typeof(NetworkResponseCode), (Encoding.UTF8.GetBytes(sp.ReadString(1))[0]).ToString());
            header.ResponseCodeOrigin = (NetworkResponseCodeOrigin)Enum.Parse(typeof(NetworkResponseCodeOrigin), (Encoding.UTF8.GetBytes(sp.ReadString(1))[0]).ToString());
            header.ProcessingFlag = (NetworkProcessingFlag)Enum.Parse(typeof(NetworkProcessingFlag), (Encoding.UTF8.GetBytes(sp.ReadString(1))[0]).ToString());

            // protocol type is special when async
            byte protocolType = sp.ReadByte();
            if (protocolType == 0x07) {
                protocolType = 0x05;
            }

            header.ProtocolType = (ProtocolType)(protocolType);
            //header.ConnectionType = sp.ReadByteConstant<ConnectionType>();
            header.ConnectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), (Encoding.UTF8.GetBytes(sp.ReadString(1))[0]).ToString());
            header.NodeIdentification = sp.ReadString(4);
            header.OriginCorrelation1 = sp.ReadBytes(2);
            header.CompanyId = sp.ReadString(4);
            header.OriginCorrelation2 = sp.ReadBytes(8);
            header.Version = sp.ReadByte();

            return header;
        }
    }
}
