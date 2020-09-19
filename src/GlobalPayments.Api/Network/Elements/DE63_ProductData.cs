using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Collections.Generic;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE63_ProductData : IDataElement<DE63_ProductData> {
        //private int productCount;
        public ProductDataFormat ProductDataFormat { get; set; } = ProductDataFormat.HeartlandStandardFormat;
        public ProductCodeSet ProductCodeSet { get; set; } = ProductCodeSet.Heartland;
        public ServiceLevel ServiceLevel { get; set; } = ServiceLevel.SelfServe;
        public Dictionary<string, DE63_ProductDataEntry> ProductDataEntries { get; set; }
        public int ProductCount { get { return ProductDataEntries.Count; } set { } }

        public DE63_ProductData() {
            ProductDataEntries = new Dictionary<string, DE63_ProductDataEntry>();
        }

        public void Add(DE63_ProductDataEntry entry) {
            ProductDataEntries[entry.Code] = entry;
        }

       public DE63_ProductData FromByteArray(byte[] buffer) {
           StringParser sp = new StringParser(buffer);

            //productDataFormat = sp.ReadStringConstant<ProductDataFormat>(1);            
            //productCodeSet = sp.ReadStringConstant<ProductCodeSet>(1);
            //serviceLevel = sp.ReadStringConstant<ServiceLevel>(1);
            ProductDataFormat = EnumConverter.FromMapping<ProductDataFormat>(Target.NWS, sp.ReadString(1));
            ProductCodeSet = EnumConverter.FromMapping<ProductCodeSet>(Target.NWS, sp.ReadString(1));
            ServiceLevel = EnumConverter.FromMapping<ServiceLevel>(Target.NWS, sp.ReadString(1));

            switch (ProductDataFormat) {
                case ProductDataFormat.HeartlandStandardFormat:
                    ProductCount = sp.ReadInt(3);
                    for (int i = 0; i < ProductCount; i++) {
                        string code = sp.ReadToChar('\\');
                        string quantity = sp.ReadToChar('\\');
                        string price = sp.ReadToChar('\\');
                        string amount = sp.ReadToChar('\\');

                        DE63_ProductDataEntry entry = new DE63_ProductDataEntry {
                            Code = code,
                            Price = StringUtils.ToFractionalAmount(price),
                            Amount = StringUtils.ToAmount(amount)
                        };

                        if (!string.IsNullOrEmpty(quantity)) {
                            entry.UnitOfMeasure = EnumConverter.FromMapping<UnitOfMeasure>(Target.NWS, quantity.Substring(0, 1));
                            entry.Quantity = StringUtils.ToFractionalAmount(quantity.Substring(1));
                        }

                        ProductDataEntries[code] = entry;
                    }
                    break;
                case ProductDataFormat.ANSI_X9_TG23_Format:
                    ProductCount = sp.ReadInt(2);
                    for (int i = 0; i < ProductCount; i++) {
                        string code = sp.ReadString(3);
                        string quantity = sp.ReadToChar('\\');
                        string price = sp.ReadToChar('\\');
                        string amount = sp.ReadToChar('\\');

                        DE63_ProductDataEntry entry = new DE63_ProductDataEntry {
                            Code = code,
                            Price = StringUtils.ToFractionalAmount(price),
                            Amount = StringUtils.ToAmount(amount)
                        };

                        if (!string.IsNullOrEmpty(quantity)) {
                            entry.UnitOfMeasure = EnumConverter.FromMapping<UnitOfMeasure>(Target.NWS, quantity.Substring(0, 1));
                            entry.Quantity = StringUtils.ToFractionalAmount(quantity.Substring(1));
                        }

                        ProductDataEntries[code] = entry;
                    }
                    break;
                case ProductDataFormat.Heartland_ProductCoupon_Format:
                    ProductCount = sp.ReadInt(2);
                    for (int i = 0; i < ProductCount; i++) {
                        //ProductCodeSet set = sp.ReadStringConstant<ProductCodeSet>(1);
                        ProductCodeSet set = EnumConverter.FromMapping<ProductCodeSet>(Target.NWS, sp.ReadString(1));
                        string code = sp.ReadToChar('\\');
                        string quantity = sp.ReadToChar('\\');
                        string price = sp.ReadToChar('\\');
                        string amount = sp.ReadToChar('\\');
                        string couponStatus = sp.ReadToChar('\\');
                        string couponCode = sp.ReadToChar('\\');
                        string serialNumber = sp.ReadToChar('\\');

                        DE63_ProductDataEntry entry = new DE63_ProductDataEntry {
                            CodeSet = set,
                            Code = code,
                            Price = StringUtils.ToFractionalAmount(price),
                            Amount = StringUtils.ToAmount(amount)
                        };

                        if (!string.IsNullOrEmpty(quantity)) {
                            //entry.UnitOfMeasure = ReverseStringEnumMap<UnitOfMeasure>.Parse<UnitOfMeasure>(quantity.Substring(0, 1));
                            //entry.UnitOfMeasure = (UnitOfMeasure)Enum.Parse(typeof(UnitOfMeasure),quantity.Substring(0, 1));
                            entry.UnitOfMeasure = EnumConverter.FromMapping<UnitOfMeasure>(Target.NWS, quantity.Substring(0, 1));
                            entry.Quantity = StringUtils.ToFractionalAmount(quantity.Substring(1));
                        }

                        if (!string.IsNullOrEmpty(couponStatus)) {
                            string status = couponStatus.Substring(0, 1);
                            string markdownType = couponStatus.Substring(1, 2);
                            decimal? value = StringUtils.ToAmount(couponStatus.Substring(2));

                            entry.CouponStatus = status;
                            entry.CouponMarkdownType = markdownType;
                            entry.CouponValue = value;
                        }

                        if (!string.IsNullOrEmpty(couponCode)) {
                            //ProductCodeSet psc = ReverseStringEnumMap<ProductCodeSet>.Parse<ProductCodeSet>(couponCode.Substring(0, 1));
                            //ProductCodeSet psc = (ProductCodeSet)Enum.Parse(typeof(ProductCodeSet),couponCode.Substring(0, 1));
                            ProductCodeSet psc = EnumConverter.FromMapping<ProductCodeSet>(Target.NWS, couponCode.Substring(0, 1));
                            couponCode = couponCode.Substring(1);

                            entry.CouponProductSetCode = psc;
                            entry.CouponCode = couponCode;
                        }

                        entry.CouponExtendedCode = serialNumber;

                        ProductDataEntries[code] = entry;
                    }
                    break;
            }

           return this;
       }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(EnumConverter.GetMapping(Target.NWS, ProductDataFormat)
                    , EnumConverter.GetMapping(Target.NWS, ProductCodeSet)
                    , EnumConverter.GetMapping(Target.NWS, ServiceLevel));

            switch(ProductDataFormat) {
                case ProductDataFormat.HeartlandStandardFormat: {
                    rvalue = string.Concat(rvalue,StringUtils.PadLeft(ProductCount, 3, '0'));
                    foreach (DE63_ProductDataEntry entry in ProductDataEntries.Values) {
                        rvalue = string.Concat(rvalue,entry.Code + "\\");

                        if(entry.UnitOfMeasure != null) {
                            rvalue = string.Concat(rvalue, EnumConverter.GetMapping(Target.NWS, entry.UnitOfMeasure));
                        }
                        if(entry.Quantity != 0) {
                            rvalue = string.Concat(rvalue,StringUtils.ToFractionalNumeric(entry.Quantity));
                        }
                        rvalue = string.Concat(rvalue,("\\")
                                ,(StringUtils.ToFractionalNumeric(entry.Price) + "\\")
                                ,(StringUtils.ToNumeric(entry.Amount) + "\\"));
                    }
                } break;
                case ProductDataFormat.ANSI_X9_TG23_Format: {
                    rvalue = string.Concat(rvalue,StringUtils.PadLeft(ProductCount, 2, '0'));
                    foreach(DE63_ProductDataEntry entry in ProductDataEntries.Values) {
                        rvalue = string.Concat(rvalue,entry.Code);

                        if(entry.UnitOfMeasure != null) {
                            rvalue = string.Concat(rvalue, EnumConverter.GetMapping(Target.NWS, entry.UnitOfMeasure));
                        }
                        if(entry.Quantity != 0) {
                            rvalue = string.Concat(rvalue,StringUtils.ToFractionalNumeric(entry.Quantity));
                        }
                        rvalue = string.Concat(rvalue,("\\")
                                ,(StringUtils.ToFractionalNumeric(entry.Price) + "\\")
                                ,(StringUtils.ToNumeric(entry.Amount) + "\\"));
                    }
                } break;
                case ProductDataFormat.Heartland_ProductCoupon_Format: {
                    rvalue = string.Concat(rvalue,StringUtils.PadLeft(ProductCount, 3, '0'));
                    foreach(DE63_ProductDataEntry entry in ProductDataEntries.Values) {
                        rvalue = string.Concat(rvalue,entry.Code);

                        if(entry.Quantity != 0) {
                            rvalue = string.Concat(rvalue, EnumConverter.GetMapping(Target.NWS, entry.UnitOfMeasure));
                        }
                        if(entry.Quantity != 0) {
                            rvalue = string.Concat(rvalue,StringUtils.ToFractionalNumeric(entry.Quantity));
                        }
                        rvalue = string.Concat(rvalue,("\\")
                                ,(StringUtils.ToFractionalNumeric(entry.Price) + "\\")
                                ,(StringUtils.ToNumeric(entry.Amount) + "\\")
                                ,(entry.CouponStatus)
                                ,(entry.CouponMarkdownType)
                                ,(StringUtils.ToNumeric(entry.CouponValue) + "\\")
                                ,(EnumConverter.GetMapping(Target.NWS, entry.CouponProductSetCode))
                                ,(entry.CouponCode + "\\")
                                ,(entry.CouponExtendedCode + "\\"));
                    }
                } break;
            }            
            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
    }
}
