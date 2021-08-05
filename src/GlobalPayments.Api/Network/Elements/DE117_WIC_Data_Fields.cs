using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE117_WIC_Data_Fields : IDataElement<DE117_WIC_Data_Fields> {
        public string DataSetIdentifier { get; set; }
        public int DataLength { get; set; }
        public DE117_WIC_Data_Field_EA EAData { get; set; }
        public DE117_WIC_Data_Field_PS PSData { get; set; }

        public DE117_WIC_Data_Fields FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            DataSetIdentifier = sp.ReadString(2);
            if (DataSetIdentifier.Equals("EA")) {
                NetworkMessage nm = NetworkMessage.Parse(buffer, Iso8583MessageType.SubElement_DE_0117_EA);
                EAData.UPCData = nm.GetString(DataElementId.DE_002);
                EAData.ItemDesciption = nm.GetString(DataElementId.DE_003);
                EAData.CategoryCode = nm.GetString(DataElementId.DE_004);
                EAData.CategoryDesciption = nm.GetString(DataElementId.DE_005);
                EAData.SubCategoryCode = nm.GetString(DataElementId.DE_006);
                EAData.SubCategoryDesciption = nm.GetString(DataElementId.DE_007);
                EAData.UnitOfMeasure = nm.GetString(DataElementId.DE_008);
                EAData.PackageSize = nm.GetString(DataElementId.DE_009);
                EAData.BenefitQuantity = nm.GetString(DataElementId.DE_011);
                EAData.BenefitUnitDescription = nm.GetString(DataElementId.DE_012);
                EAData.UPCDataLength = nm.GetString(DataElementId.DE_013);
            }
            else if (DataSetIdentifier.Equals("PS")) {
                NetworkMessage nm = NetworkMessage.Parse(buffer, Iso8583MessageType.SubElement_DE_0117_PS);
                PSData.UPCData = nm.GetString(DataElementId.DE_002);
                PSData.CategoryCode = nm.GetString(DataElementId.DE_003);
                PSData.SubCategoryCode = nm.GetString(DataElementId.DE_004);
                PSData.Units = nm.GetString(DataElementId.DE_005);
                PSData.ItemPrice = nm.GetString(DataElementId.DE_006);
                PSData.PurchaseQuantity = nm.GetString(DataElementId.DE_007);
                PSData.ItemActionCode = nm.GetString(DataElementId.DE_008);
                PSData.OriginalItemPrice = nm.GetString(DataElementId.DE_009);
                PSData.OriginalPurchaseQuantity = nm.GetString(DataElementId.DE_010);
                PSData.UPCDataLength = nm.GetString(DataElementId.DE_011);
            }
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = "";

            if (EAData != null) {
                string eaData = string.Concat(EAData.UPCData
                    , EAData.ItemDesciption
                    , StringUtils.PadLeft(EAData.CategoryCode + "", 2, '0')
                    , EAData.CategoryDesciption
                    , StringUtils.PadLeft(EAData.SubCategoryCode + "", 3, '0')
                    , EAData.SubCategoryDesciption
                    , EAData.UnitOfMeasure
                    , EAData.PackageSize
                    , StringUtils.PadLeft(EAData.BenefitQuantity + "", 5, '0')
                    , EAData.BenefitUnitDescription
                    , EAData.UPCDataLength
                    );                

                NetworkMessage message = new NetworkMessage(Iso8583MessageType.SubElement_DE_0117_EA)
                    .Set(DataElementId.DE_002, EAData.UPCData)
                    .Set(DataElementId.DE_003, EAData.ItemDesciption)
                    .Set(DataElementId.DE_004, StringUtils.PadLeft(EAData.CategoryCode + "", 2, '0'))
                    .Set(DataElementId.DE_005, EAData.CategoryDesciption)
                    .Set(DataElementId.DE_006, StringUtils.PadLeft(EAData.SubCategoryCode + "", 3, '0'))
                    .Set(DataElementId.DE_007, EAData.SubCategoryDesciption)
                    .Set(DataElementId.DE_008, EAData.UnitOfMeasure)
                    .Set(DataElementId.DE_009, EAData.PackageSize != null ? Convert.ToString(EAData.PackageSize) : null)
                    .Set(DataElementId.DE_011, StringUtils.PadLeft(EAData.BenefitQuantity + "", 5, '0'))
                    .Set(DataElementId.DE_012, EAData.BenefitUnitDescription)
                    .Set(DataElementId.DE_013, EAData.UPCDataLength != null ? Convert.ToString(EAData.UPCDataLength) : null);
                var bitmap = message.BuildMessage();

                rvalue = string.Concat("EA", message.Bitmap.ToBinaryString(), eaData);

            }
            if (PSData != null) {
                string psData = string.Concat(StringUtils.PadLeft(PSData.UPCData + "", 17, '0')
                    , PSData.CategoryCode
                    , PSData.SubCategoryCode
                    , PSData.Units
                    , StringUtils.PadLeft(PSData.ItemPrice + "", 6, '0')
                    , StringUtils.PadLeft(PSData.PurchaseQuantity + "", 5, '0')
                    , StringUtils.PadLeft(PSData.ItemActionCode + "", 2, '0')
                    , PSData.OriginalItemPrice
                    , PSData.OriginalPurchaseQuantity
                    , StringUtils.PadLeft(PSData.UPCData.Length, 2, '0')
                    );

                NetworkMessage message = new NetworkMessage(Iso8583MessageType.SubElement_DE_0117_PS)
                    .Set(DataElementId.DE_002, StringUtils.PadLeft(PSData.UPCData + "", 17, '0'))
                    .Set(DataElementId.DE_003, PSData.CategoryCode != null ? Convert.ToString(PSData.CategoryCode) : null)
                    .Set(DataElementId.DE_004, PSData.SubCategoryCode != null ? Convert.ToString(PSData.SubCategoryCode) : null)
                    .Set(DataElementId.DE_005, PSData.Units != null ? Convert.ToString(PSData.Units) : null)
                    .Set(DataElementId.DE_006, StringUtils.PadLeft(PSData.ItemPrice + "", 6, '0'))
                    .Set(DataElementId.DE_007, StringUtils.PadLeft(PSData.PurchaseQuantity + "", 5, '0'))
                    .Set(DataElementId.DE_008, StringUtils.PadLeft(PSData.ItemActionCode + "", 2, '0'))
                    .Set(DataElementId.DE_009, PSData.OriginalItemPrice != null ? Convert.ToString(PSData.OriginalItemPrice) : null)
                    .Set(DataElementId.DE_010, PSData.OriginalPurchaseQuantity != null ? Convert.ToString(PSData.OriginalPurchaseQuantity) : null)
                    .Set(DataElementId.DE_011, StringUtils.PadLeft(PSData.UPCDataLength + "", 2, '0'));
                var bitmap = message.BuildMessage();

                rvalue = string.Concat("PS", message.Bitmap.ToBinaryString(), psData);
            }

            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
