using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE72_DataRecord : IDataElement<DE72_DataRecord> {
        public string RecordFormat { get; set; }
        public RecordDataEntry RecordData { get; set; }

        public DE72_DataRecord() { }
        public DE72_DataRecord(string recordDataFormat, RecordDataEntry dataRecord)
        {
            RecordFormat = recordDataFormat;
            RecordData = dataRecord;
        }

        public DE72_DataRecord FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);
            RecordFormat = sp.ReadString(4);
            RecordData.MessageVersion = sp.ReadString(3);
            RecordData.TransactionDate = sp.ReadString(8);
            RecordData.TransactionTime = sp.ReadString(6);
            RecordData.CompanyName = sp.ReadString(15);
            RecordData.HeartlandCompanyId = sp.ReadString(5);
            RecordData.MerchantName = sp.ReadString(20);
            RecordData.MerchantId = sp.ReadString(15);
            RecordData.MerchantStreet = sp.ReadString(30);
            RecordData.MerchantCity = sp.ReadString(20);
            RecordData.MerchantState = sp.ReadString(2);
            RecordData.MerchantZIP = sp.ReadString(5);
            RecordData.MerchantPhoneNumber = sp.ReadString(12);
            RecordData.SiteBrand = sp.ReadString(15);
            RecordData.MerchantType = sp.ReadString(4);
            RecordData.POSApplicationType = sp.ReadString(1);
            RecordData.OperationMethod = sp.ReadString(1);
            RecordData.POSVendor = sp.ReadString(15);
            RecordData.POSModel = sp.ReadString(15);
            RecordData.POSTerminalType = sp.ReadString(3);
            RecordData.POSSoftwareVersion = sp.ReadString(8);
            RecordData.POSSpecification = sp.ReadString(1);
            RecordData.POSSpecificationVersion = sp.ReadString(4);
            RecordData.PaymentEngine = sp.ReadString(1);
            RecordData.PaymentVertical = sp.ReadString(1);
            RecordData.HardwareVersion = sp.ReadString(4);
            RecordData.SoftwareVersion = sp.ReadString(8);
            RecordData.FirmwareLevel = sp.ReadString(8);
            RecordData.MiddlewareVendor = sp.ReadString(15);
            RecordData.MiddlewareModel = sp.ReadString(15);
            RecordData.MiddlewareType = sp.ReadString(1);
            RecordData.MiddlewareSoftwareVersion = sp.ReadString(8);
            RecordData.ReceiptPrinterType = sp.ReadString(1);
            RecordData.ReceiptPrinterModel = sp.ReadString(15);
            RecordData.JournalPrinterType = sp.ReadString(1);
            RecordData.JournalPrinterModel = sp.ReadString(15);
            RecordData.MultiLaneDeviceType = sp.ReadString(1);
            RecordData.MultiLaneDeviceVendor = sp.ReadString(15);
            RecordData.MultiLaneDeviceModel = sp.ReadString(15);
            RecordData.InsideKeyManagementScheme = sp.ReadString(1);
            RecordData.InsidePINEncryption = sp.ReadString(1);
            RecordData.OutsidePEDType = sp.ReadString(1);
            RecordData.OutsidePEDVendor = sp.ReadString(15);
            RecordData.OutsidePEDModel = sp.ReadString(15);
            RecordData.OutsideKeyManagementScheme = sp.ReadString(1);
            RecordData.OutsidePINEncryption = sp.ReadString(1);
            RecordData.CheckReaderVendor = sp.ReadString(15);
            RecordData.CheckReaderModel = sp.ReadString(15);
            RecordData.InsideContactlessReaderType = sp.ReadString(1);
            RecordData.InsideContactlessReaderVendor = sp.ReadString(15);
            RecordData.InsideContactlessReaderModel = sp.ReadString(15);
            RecordData.OutsideContactlessReaderType = sp.ReadString(1);
            RecordData.OutsideContactlessReaderVendor = sp.ReadString(15);
            RecordData.OutsideContactlessReaderModel = sp.ReadString(15);
            RecordData.CommunicationMedia = sp.ReadString(1);
            RecordData.CommunicationProtocol = sp.ReadString(1);
            RecordData.BroadbandUse = sp.ReadString(1);
            RecordData.DataWireAccess = sp.ReadString(1);
            RecordData.MicroNodeModelNumber = sp.ReadString(8);
            RecordData.MicroNodeSoftwareVersion = sp.ReadString(8);
            RecordData.RouterType = sp.ReadString(1);
            RecordData.RouterVendor = sp.ReadString(15);
            RecordData.RouterProductModel = sp.ReadString(15);
            RecordData.ModemPhoneNumber = sp.ReadString(12);
            RecordData.PrimaryDialNumberOrIPAddress = sp.ReadString(21);
            RecordData.SecondaryDialNumberOrIPAddress = sp.ReadString(21);
            RecordData.DispenserInterfaceVendor = sp.ReadString(15);
            RecordData.DispenserInterfaceModel = sp.ReadString(15);
            RecordData.DispenserInterfaceSoftwareVersion = sp.ReadString(8);
            RecordData.DispenserVendor = sp.ReadString(15);
            RecordData.DispenserModel = sp.ReadString(15);
            RecordData.DispenserSoftwareVersion = sp.ReadString(8);
            RecordData.DispenserQuantity = sp.ReadString(2);
            RecordData.NumberOfScannersOrPeripherals = sp.ReadString(2);
            RecordData.Scanner1Vendor = sp.ReadString(15);
            RecordData.Scanner1Model = sp.ReadString(15);
            RecordData.Scanner1SoftwareVersion = sp.ReadString(8);
            RecordData.Peripheral2Vendor = sp.ReadString(15);
            RecordData.Peripheral2Model = sp.ReadString(15);
            RecordData.Peripheral2SoftwareVersion = sp.ReadString(8);
            RecordData.Peripheral3Vendor = sp.ReadString(15);
            RecordData.Peripheral3Model = sp.ReadString(15);
            RecordData.Peripheral3SoftwareVersion = sp.ReadString(8);
            RecordData.Peripheral4Vendor = sp.ReadString(15);
            RecordData.Peripheral4Model = sp.ReadString(15);
            RecordData.Peripheral4SoftwareVersion = sp.ReadString(8);
            RecordData.Peripheral5Vendor = sp.ReadString(15);
            RecordData.Peripheral5Model = sp.ReadString(15);
            RecordData.Peripheral5SoftwareVersion = sp.ReadString(8);
            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(
                StringUtils.PadRight(RecordFormat, 4, ' '),
                StringUtils.PadRight(RecordData.MessageVersion, 3, ' '),
                StringUtils.PadRight(RecordData.TransactionDate, 8, ' '),
                StringUtils.PadRight(RecordData.TransactionTime, 6, ' '),
                StringUtils.PadRight(RecordData.CompanyName, 15, ' '),
                StringUtils.PadRight(RecordData.HeartlandCompanyId, 5, ' '),
                StringUtils.PadRight(RecordData.MerchantName, 20, ' '),
                StringUtils.PadRight(RecordData.MerchantId, 15, ' '),
                StringUtils.PadRight(RecordData.MerchantStreet, 30, ' '),
                StringUtils.PadRight(RecordData.MerchantCity, 20, ' '),
                StringUtils.PadRight(RecordData.MerchantState, 2, ' '),
                StringUtils.PadRight(RecordData.MerchantZIP, 5, ' '),
                StringUtils.PadRight(RecordData.MerchantPhoneNumber, 12, ' '),
                StringUtils.PadRight(RecordData.SiteBrand, 15, ' '),
                StringUtils.PadRight(RecordData.MerchantType, 4, ' '),
                StringUtils.PadRight(RecordData.POSApplicationType, 1, ' '),
                StringUtils.PadRight(RecordData.OperationMethod, 1, ' '),
                StringUtils.PadRight(RecordData.POSVendor, 15, ' '),
                StringUtils.PadRight(RecordData.POSModel, 15, ' '),
                StringUtils.PadRight(RecordData.POSTerminalType, 3, ' '),
                StringUtils.PadRight(RecordData.POSSoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.POSSpecification, 1, ' '),
                StringUtils.PadRight(RecordData.POSSpecificationVersion, 4, ' '),
                StringUtils.PadRight(RecordData.PaymentEngine, 1, ' '),
                StringUtils.PadRight(RecordData.PaymentVertical, 1, ' '),
                StringUtils.PadRight(RecordData.HardwareVersion, 4, ' '),
                StringUtils.PadRight(RecordData.SoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.FirmwareLevel, 8, ' '),
                StringUtils.PadRight(RecordData.MiddlewareVendor, 15, ' '),
                StringUtils.PadRight(RecordData.MiddlewareModel, 15, ' '),
                StringUtils.PadRight(RecordData.MiddlewareType, 1, ' '),
                StringUtils.PadRight(RecordData.MiddlewareSoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.ReceiptPrinterType, 1, ' '),
                StringUtils.PadRight(RecordData.ReceiptPrinterModel, 15, ' '),
                StringUtils.PadRight(RecordData.JournalPrinterType, 1, ' '),
                StringUtils.PadRight(RecordData.JournalPrinterModel, 15, ' '),
                StringUtils.PadRight(RecordData.MultiLaneDeviceType, 1, ' '),
                StringUtils.PadRight(RecordData.MultiLaneDeviceVendor, 15, ' '),
                StringUtils.PadRight(RecordData.MultiLaneDeviceModel, 15, ' '),
                StringUtils.PadRight(RecordData.InsideKeyManagementScheme, 1, ' '),
                StringUtils.PadRight(RecordData.InsidePINEncryption, 1, ' '),
                StringUtils.PadRight(RecordData.OutsidePEDType, 1, ' '),
                StringUtils.PadRight(RecordData.OutsidePEDVendor, 15, ' '),
                StringUtils.PadRight(RecordData.OutsidePEDModel, 15, ' '),
                StringUtils.PadRight(RecordData.OutsideKeyManagementScheme, 1, ' '),
                StringUtils.PadRight(RecordData.OutsidePINEncryption, 1, ' '),
                StringUtils.PadRight(RecordData.CheckReaderVendor, 15, ' '),
                StringUtils.PadRight(RecordData.CheckReaderModel, 15, ' '),
                StringUtils.PadRight(RecordData.InsideContactlessReaderType, 1, ' '),
                StringUtils.PadRight(RecordData.InsideContactlessReaderVendor, 15, ' '),
                StringUtils.PadRight(RecordData.InsideContactlessReaderModel, 15, ' '),
                StringUtils.PadRight(RecordData.OutsideContactlessReaderType, 1, ' '),
                StringUtils.PadRight(RecordData.OutsideContactlessReaderVendor, 15, ' '),
                StringUtils.PadRight(RecordData.OutsideContactlessReaderModel, 15, ' '),
                StringUtils.PadRight(RecordData.CommunicationMedia, 1, ' '),
                StringUtils.PadRight(RecordData.CommunicationProtocol, 1, ' '),
                StringUtils.PadRight(RecordData.BroadbandUse, 1, ' '),
                StringUtils.PadRight(RecordData.DataWireAccess, 1, ' '),
                StringUtils.PadRight(RecordData.MicroNodeModelNumber, 8, ' '),
                StringUtils.PadRight(RecordData.MicroNodeSoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.RouterType, 1, ' '),
                StringUtils.PadRight(RecordData.RouterVendor, 15, ' '),
                StringUtils.PadRight(RecordData.RouterProductModel, 15, ' '),
                StringUtils.PadRight(RecordData.ModemPhoneNumber, 12, ' '),
                StringUtils.PadRight(RecordData.PrimaryDialNumberOrIPAddress, 21, ' '),
                StringUtils.PadRight(RecordData.SecondaryDialNumberOrIPAddress, 21, ' '),
                StringUtils.PadRight(RecordData.DispenserInterfaceVendor, 15, ' '),
                StringUtils.PadRight(RecordData.DispenserInterfaceModel, 15, ' '),
                StringUtils.PadRight(RecordData.DispenserInterfaceSoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.DispenserVendor, 15, ' '),
                StringUtils.PadRight(RecordData.DispenserModel, 15, ' '),
                StringUtils.PadRight(RecordData.DispenserSoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.DispenserQuantity, 2, ' '),
                StringUtils.PadRight(RecordData.NumberOfScannersOrPeripherals, 2, ' '),
                StringUtils.PadRight(RecordData.Scanner1Vendor, 15, ' '),
                StringUtils.PadRight(RecordData.Scanner1Model, 15, ' '),
                StringUtils.PadRight(RecordData.Scanner1SoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.Peripheral2Vendor, 15, ' '),
                StringUtils.PadRight(RecordData.Peripheral2Model, 15, ' '),
                StringUtils.PadRight(RecordData.Peripheral2SoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.Peripheral3Vendor, 15, ' '),
                StringUtils.PadRight(RecordData.Peripheral3Model, 15, ' '),
                StringUtils.PadRight(RecordData.Peripheral3SoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.Peripheral4Vendor, 15, ' '),
                StringUtils.PadRight(RecordData.Peripheral4Model, 15, ' '),
                StringUtils.PadRight(RecordData.Peripheral4SoftwareVersion, 8, ' '),
                StringUtils.PadRight(RecordData.Peripheral5Vendor, 15, ' '),
                StringUtils.PadRight(RecordData.Peripheral5Model, 15, ' '),
                StringUtils.PadRight(RecordData.Peripheral5SoftwareVersion, 8, ' ')
                );

            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
