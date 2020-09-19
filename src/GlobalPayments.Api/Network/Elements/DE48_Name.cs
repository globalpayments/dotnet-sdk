using GlobalPayments.Api.Network.Abstractions;
using GlobalPayments.Api.Network.Entities;
using GlobalPayments.Api.Utils;
using System.Text;

namespace GlobalPayments.Api.Network.Elements {
    public class DE48_Name : IDataElement<DE48_Name> {
        public DE48_NameType NameType { get; set; }
        public DE48_NameFormat NameFormat { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string PositionalTitle { get; set; }
        public string FunctionalTitle { get; set; }

        public DE48_Name FromByteArray(byte[] buffer) {
            StringParser sp = new StringParser(buffer);

            //NameType = sp.ReadStringConstant<DE48_NameType>(1);
            //NameFormat = sp.ReadStringConstant<DE48_NameFormat>(1);
            NameType = EnumConverter.FromMapping<DE48_NameType>(Target.NWS, sp.ReadString(1));
            NameFormat = EnumConverter.FromMapping<DE48_NameFormat>(Target.NWS, sp.ReadString(1));

            switch (NameFormat) {
                case DE48_NameFormat.Delimited_FirstMiddleLast:
                    FirstName = sp.ReadToChar('\\');
                    MiddleName = sp.ReadToChar('\\');
                    LastName = sp.ReadRemaining();
                    break;
                case DE48_NameFormat.Delimited_Title:
                    Prefix = sp.ReadToChar('\\');
                    FirstName = sp.ReadToChar('\\');
                    MiddleName = sp.ReadToChar('\\');
                    LastName = sp.ReadToChar('\\');
                    Suffix = sp.ReadToChar('\\');
                    PositionalTitle = sp.ReadToChar('\\');
                    FunctionalTitle = sp.ReadRemaining();
                    break;
                default:
                    Name = sp.ReadRemaining();
                    break;                
            }

            return this;
        }

        public byte[] ToByteArray() {
            string rvalue = string.Concat(EnumConverter.GetMapping(Target.NWS, NameType)
                    , EnumConverter.GetMapping(Target.NWS, NameFormat));

            switch (NameFormat) {
                case DE48_NameFormat.Delimited_FirstMiddleLast:
                    rvalue = string.Concat(rvalue,(FirstName + '\\')
                            ,(MiddleName + '\\')
                            ,LastName);
                            break;
                case DE48_NameFormat.Delimited_Title:
                    rvalue = string.Concat(rvalue,(Prefix + '\\')
                            ,(FirstName + '\\')
                            ,(LastName + '\\')
                            ,(Suffix + '\\')
                            ,(PositionalTitle + '\\')
                            ,FunctionalTitle);
                            break;
                default:
                    rvalue = string.Concat(rvalue,Name);
                    break;                
            }

            return Encoding.ASCII.GetBytes(rvalue);
        }

        public new string ToString() {
            return Encoding.UTF8.GetString(ToByteArray());
        }
        }
    }
