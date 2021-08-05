using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_MessageControl : IDataElement<DE48_MessageControl> {
        public DE48_1_CommunicationDiagnostics CommunicationDiagnostics { get; set; }
        public DE48_2_HardwareSoftwareConfig HardwareSoftwareConfig { get; set; }
        public string LanguageCode { get; set; }
        public int BatchNumber { get; set; }
        public string ShiftNumber { get; set; }
        public string ClerkId { get; set; }
        public DE48_8_CustomerData CustomerData { get; set; }
        public string Track2ForSecondCard { get; set; }
        public string Track1ForSecondCard { get; set; }
        public DE48_CardType? CardType { get; set; }
        public DE48_AdministrativelyDirectedTaskCode? AdministrativelyDirectedTaskCode { get; set; }
        public string RfidData { get; set; }
        public DE48_14_PinEncryptionMethodology PinEncryptionMethodology { get; set; }
        public DE48_33_PosConfiguration PosConfiguration { get; set; }
        public DE48_34_MessageConfiguration MessageConfiguration { get; set; }
        public DE48_Name Name1 { get; set; }
        public DE48_Name Name2 { get; set; }
        public string SecondaryAccountNumber { get; set; }
        public DE48_39_PriorMessageInformation PriorMessageInformation { get; set; }
        private Dictionary<DataElementId, DE48_Address> addresses;
        private int addressIndex = 0;
        private DataElementId[] addressElementIds = new DataElementId[] {
                DataElementId.DE_040,
                DataElementId.DE_041,
                DataElementId.DE_042,
                DataElementId.DE_043,
                DataElementId.DE_044,
                DataElementId.DE_045,
                DataElementId.DE_046,
                DataElementId.DE_047,
                DataElementId.DE_048,
                DataElementId.DE_049,
        };
        public int SequenceNumber { get; set; }

        public void AddAddress(DE48_Address address) {
            addresses[addressElementIds[addressIndex++]] = address;
        }

        public DE48_MessageControl() {
            addresses = new Dictionary<DataElementId, DE48_Address>();
            PriorMessageInformation = new DE48_39_PriorMessageInformation();
        }

        public DE48_MessageControl FromByteArray(byte[] buffer) {
            NetworkMessage nm = NetworkMessage.Parse(buffer, Iso8583MessageType.SubElement_DE_048);
            CommunicationDiagnostics = nm.GetDataElement<DE48_1_CommunicationDiagnostics>(DataElementId.DE_001);
            HardwareSoftwareConfig = nm.GetDataElement<DE48_2_HardwareSoftwareConfig>(DataElementId.DE_002);
            LanguageCode = nm.GetString(DataElementId.DE_003);
            string _batchNumber = nm.GetString(DataElementId.DE_004);
            if (!string.IsNullOrEmpty(_batchNumber))
            {
                SequenceNumber = int.Parse(_batchNumber.Substring(0, 6));
                BatchNumber = int.Parse(_batchNumber.Substring(6));
            }
            ShiftNumber = nm.GetString(DataElementId.DE_005);
            ClerkId = nm.GetString(DataElementId.DE_006);
            CustomerData = nm.GetDataElement<DE48_8_CustomerData>(DataElementId.DE_008);
            Track2ForSecondCard = nm.GetString(DataElementId.DE_009);
            Track1ForSecondCard = nm.GetString(DataElementId.DE_010);
            CardType = nm.GetStringConstant(DataElementId.DE_011);
            AdministrativelyDirectedTaskCode = nm.GetByteConstant(DataElementId.DE_012);
            RfidData = nm.GetString(DataElementId.DE_013);
            PinEncryptionMethodology = nm.GetDataElement<DE48_14_PinEncryptionMethodology>(DataElementId.DE_014);
            PosConfiguration = nm.GetDataElement<DE48_33_PosConfiguration>(DataElementId.DE_033);
            MessageConfiguration = nm.GetDataElement<DE48_34_MessageConfiguration>(DataElementId.DE_034);
            Name1 = nm.GetDataElement<DE48_Name>(DataElementId.DE_035);
            Name2 = nm.GetDataElement<DE48_Name>(DataElementId.DE_036);
            SecondaryAccountNumber = nm.GetString(DataElementId.DE_037);
            PriorMessageInformation = nm.GetDataElement<DE48_39_PriorMessageInformation>(DataElementId.DE_039);

            addressIndex = 0;
            foreach(DataElementId addressId in addressElementIds) {
                DE48_Address address = nm.GetDataElement<DE48_Address>(addressId);
                if(address != null) {
                    addresses[addressId] = address;
                }
            }

            return this;
        }

        public byte[] ToByteArray() {
            string _batchNumber = string.Concat(StringUtils.PadLeft(SequenceNumber, 6, '0')
                    ,(StringUtils.PadLeft(BatchNumber, 4, '0')));
            if(_batchNumber.Equals("0000000000")) {
                _batchNumber = null;
            }
            NetworkMessage message = new NetworkMessage(Iso8583MessageType.SubElement_DE_048)
                .Set(DataElementId.DE_001, CommunicationDiagnostics)
                .Set(DataElementId.DE_002, HardwareSoftwareConfig)
                .Set(DataElementId.DE_003, LanguageCode)
                .Set(DataElementId.DE_004, _batchNumber)
                .Set(DataElementId.DE_005, ShiftNumber)
                .Set(DataElementId.DE_006, ClerkId)
                .Set(DataElementId.DE_008, CustomerData)
                .Set(DataElementId.DE_009, Track2ForSecondCard)
                .Set(DataElementId.DE_010, Track1ForSecondCard)
                .Set(DataElementId.DE_011, CardType != null ? Encoding.UTF8.GetBytes(StringUtils.PadRight(Convert.ToString(EnumConverter.GetMapping(Target.NWS, CardType)), 4, ' ')) : null)
                .Set(DataElementId.DE_012, AdministrativelyDirectedTaskCode != null ? AdministrativelyDirectedTaskCode.ToString() : null)
                .Set(DataElementId.DE_013, RfidData)
                .Set(DataElementId.DE_014, PinEncryptionMethodology)
                .Set(DataElementId.DE_033, PosConfiguration)
                .Set(DataElementId.DE_034, MessageConfiguration)
                .Set(DataElementId.DE_035, Name1)
                .Set(DataElementId.DE_036, Name2)
                .Set(DataElementId.DE_037, SecondaryAccountNumber)
                .Set(DataElementId.DE_039, PriorMessageInformation);

            foreach(DataElementId addressId in addresses.Keys) {
                message.Set(addressId, addresses[addressId]);
            }

            return message.BuildMessage();
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
