using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GlobalPayments.Api.Entities.Reporting;

namespace GlobalPayments.Api.Entities {
    /// <summary>
    /// Holds 3-D Secure authentication data and associated merchant data.
    /// </summary>
    public class ThreeDSecure {
        /// <summary>
        /// Gets or sets the complete GP-API authentication response.
        /// </summary>
        public AuthenticationSummary AuthenticationDetails { get; set; }

        /// <summary>
        /// The transaction identifier assigned by the Access Control Server.
        /// </summary>
        public string AcsTransactionId { get; set; }

        /// <summary>
        /// The highest 3-D Secure version supported by the Access Control Server.
        /// </summary>
        public string AcsEndVersion { get; set; }

        /// <summary>
        /// The lowest 3-D Secure version supported by the Access Control Server.
        /// </summary>
        public string AcsStartVersion { get; set; }

        /// <summary>
        /// Information indicators provided by the Access Control Server.
        /// </summary>
        public IEnumerable<string> AcsInfoIndicator { get; set; }

        /// <summary>
        /// The Access Control Server interface used for a mobile challenge.
        /// </summary>
        public string AcsInterface { get; set; }

        /// <summary>
        /// The Access Control Server UI template used for a mobile challenge.
        /// </summary>
        public string AcsUiTemplate { get; set; }

        /// <summary>
        /// The reference number identifying the Access Control Server.
        /// </summary>
        public string AcsReferenceNumber { get; set; }

        /// <summary>
        /// The provider's 3-D Secure server transaction reference.
        /// </summary>
        public string ProviderServerTransRef { get; set; }

        /// <summary>
        /// The algorithm used.
        /// </summary>
        public int Algorithm { get; set; }

        /// <summary>
        /// The transaction amount retained for merchant data serialization.
        /// </summary>
        private decimal? _amount;

        /// <summary>
        /// Gets or sets the transaction amount retained in merchant data.
        /// </summary>
        internal decimal? Amount {
            get { return _amount; }
            set {
                _amount = value;
                MerchantData.Add("_amount", _amount.ToString(), false);
            }
        }

        /// <summary>
        /// The source of the authentication request.
        /// </summary>
        public string AuthenticationSource { get; set; }

        /// <summary>
        /// The type of 3-D Secure authentication.
        /// </summary>
        public string AuthenticationType { get; set; }

        /// <summary>
        /// The type of authentication request sent to the authentication service.
        /// </summary>
        public string AuthenticationRequestType { get; set; }

        public string AuthenticationValue { get; set; }

        /// <summary>
        /// The authentication value returned under the GP-API authentication_value field.
        /// </summary>
        public string AuthenticationValueData { get; set; }

        /// <summary>
        /// Cardholder response information returned by the authentication service.
        /// </summary>
        public string CardHolderResponseInfo { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) verification value.
        /// </summary>
        public string Cavv { get; set; }

        /// <summary>
        /// Indicates whether the issuer mandates a challenge.
        /// </summary>
        public bool ChallengeMandated { get; set; }

        /// <summary>
        /// The currency retained for merchant data serialization.
        /// </summary>
        private string _currency;

        /// <summary>
        /// Gets or sets the currency retained in merchant data.
        /// </summary>
        internal string Currency {
            get { return _currency; }
            set {
                _currency = value;
                MerchantData.Add("_currency", _currency, false);
            }
        }

        /// <summary>
        /// The indicator describing the decoupled authentication response.
        /// </summary>
        public string DecoupledResponseIndicator { get; set; }

        /// <summary>
        /// The transaction identifier assigned by the Directory Server.
        /// </summary>
        public string DirectoryServerTransactionId { get; set; }

        /// <summary>
        /// The highest 3-D Secure version supported by the Directory Server.
        /// </summary>
        public string DirectoryServerEndVersion { get; set; }

        /// <summary>
        /// The lowest 3-D Secure version supported by the Directory Server.
        /// </summary>
        public string DirectoryServerStartVersion { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) electronic commerce indicator.
        /// </summary>
        public string Eci { get; set; }

        /// <summary>
        /// Recognized enrollment values are matched without regard to case.
        /// </summary>
        private static readonly HashSet<string> EnrolledValues =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "True", "Y", "Enrolled"
        };

        /// <summary>
        /// The enrollment status value.
        /// </summary>
        private string _enrolled;
        /// <summary>
        /// The enrollment status:
        /// </summary>
        public string Enrolled {
            get { return _enrolled; }
            set {
                _enrolled = value;
                IsEnrolled = EnrolledValues.Contains(value ?? string.Empty);
            }
        }

        /// <summary>
        /// IsEnrolled is true or false based on the Enrolled status.
        /// </summary>
        public bool IsEnrolled { get; private set; }

        /// <summary>
        /// The exempt status
        /// </summary>
        public ExemptStatus? ExemptStatus { get; set; }

        /// <summary>
        /// The exemption optimization service reason
        /// </summary>
        public string ExemptReason { get; set; }

        /// <summary>
        /// The URL of the Issuing Bank's ACS.
        /// </summary>
        public string IssuerAcsUrl { get; set; }

        /// <summary>
        /// The URL that receives the result after an ACS challenge.
        /// </summary>
        public string ChallengeReturnUrl { get; set; }

        /// <summary>
        /// The issuer's challenge model.
        /// </summary>
        public string ChallengeModel { get; set; }

        /// <summary>
        /// The value submitted to the ACS when presenting a challenge.
        /// </summary>
        public string ChallengeValue { get; set; }

        /// <summary>
        /// The URL used to redirect the payer to the ACS.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// The ACS challenge request URL.
        /// </summary>
        public string AcsChallengeRequestUrl { get; set; }

        /// <summary>
        /// The ACS method URL used to gather device data.
        /// </summary>
        public string MethodUrl { get; set; }

        /// <summary>
        /// Gets or sets data used by the ACS method URL.
        /// </summary>
        public ThreeDSecureMethodData MethodData { get; set; }

        /// <summary>
        /// Gets or sets mobile challenge response data.
        /// </summary>
        public ThreeDSecureMobileData MobileData { get; set; }

        /// <summary>
        /// Gets or sets the ACS decoupled-response indicators.
        /// </summary>
        public IEnumerable<string> AcsDecoupledResponseIndicator { get; set; }

        /// <summary>
        /// The name of the field containing session data.
        /// </summary>
        public string SessionDataFieldName { get; set; }

        /// <summary>
        /// The 3-D Secure protocol message type.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The collection of merchant-supplied data, including hidden SDK values.
        /// </summary>
        private MerchantDataCollection _merchantData;
        /// <summary>
        /// A key-value collection of merchant-supplied data.
        /// </summary>
        public MerchantDataCollection MerchantData {
            get {
                if (_merchantData == null)
                    _merchantData = new MerchantDataCollection();
                return _merchantData;
            }
            set {
                if (_merchantData != null)
                    value.MergeHidden(_merchantData);

                _merchantData = value;
                if (_merchantData.HasKey("_amount")) {
                    _amount = _merchantData.GetValue<decimal>("_amount");
                }
                if (_merchantData.HasKey("_currency")) {
                    _currency = _merchantData.GetValue<string>("_currency");
                }
                if (_merchantData.HasKey("_orderid")) {
                    _orderId = _merchantData.GetValue<string>("_orderid");
                }
                if (_merchantData.HasKey("_version")) {
                    Secure3dVersion version;
                    if (Enum.TryParse(_merchantData.GetValue<string>("_version"), out version)) {
                        _version = version;
                    }
                }
            }
        }

        /// <summary>
        /// The 3-D Secure message category.
        /// </summary>
        public string MessageCategory { get; set; }

        /// <summary>
        /// The result from transaction risk analysis exemption optimization.
        /// </summary>
        public string ExemptionOptimizationResult { get; set; }

        /// <summary>
        /// Structured message extensions associated with the authentication.
        /// </summary>
        public List<MessageExtension> MessageExtensions { get; set; }

        /// <summary>
        /// Gets or sets raw message-extension values returned by GP-API.
        /// </summary>
        public IEnumerable<string> MessageExtensionValues { get; set; }

        /// <summary>
        /// The 3-D Secure protocol message version.
        /// </summary>
        public string MessageVersion { get; set; }

        /// <summary>
        /// The transaction order identifier.
        /// </summary>
        private string _orderId;

        /// <summary>
        /// The order ID used for the initial transaction
        /// </summary>
        public string OrderId {
            get { return _orderId; }
            set {
                _orderId = value;
                MerchantData.Add("_orderid", _orderId, false);
            }
        }

        /// <summary>
        /// The Payer Authentication Request returned by the Enrolment Server. Must be sent to the Issuing Bank's ACS (Access Control Server) URL.
        /// </summary>
        public string PayerAuthenticationRequest { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) source.
        /// </summary>
        public string PaymentDataSource { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) type.
        /// </summary>
        /// <remarks>
        /// Default value is `"3DSecure"`.
        /// </remarks>
        public string PaymentDataType { get; set; }

        /// <summary>
        /// The SDK interface used for the authentication.
        /// </summary>
        public string SdkInterface { get; set; }

        /// <summary>
        /// The UI types supported by the 3-D Secure SDK.
        /// </summary>
        public IEnumerable<string> SdkUiType { get; set; }

        /// <summary>
        /// The secure code associated with the authentication.
        /// </summary>
        public string SecureCode { get; set; }

        /// <summary>
        /// The transaction identifier assigned by the 3-D Secure Server.
        /// </summary>
        public string ServerTransactionId { get; set; }

        /// <summary>
        /// The authentication status reported by the provider.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The reason associated with the authentication status.
        /// </summary>
        public string StatusReason { get; set; }

        /// <summary>
        /// The challenge status returned by the authentication service.
        /// </summary>
        public string ChallengeStatus { get; set; }

        /// <summary>
        /// The Universal Cardholder Authentication Field indicator.
        /// </summary>
        public UCAFIndicator UCAFIndicator { get; set; }

        /// <summary>
        /// The 3-D Secure protocol version retained by this object.
        /// </summary>
        public Secure3dVersion? _version;

        /// <summary>
        /// Gets or sets the 3-D Secure protocol version.
        /// </summary>
        public Secure3dVersion? Version {
            get { return _version; }
            set {
                _version = value;
                MerchantData.Add("_version", value.ToString(), false);
            }
        }

        /// <summary>
        /// The cardholder's trusted-beneficiary whitelist status.
        /// </summary>
        public string WhitelistStatus { get; set; }

        /// <summary>
        /// Consumer authentication (3DSecure) transaction ID.
        /// </summary>
        public string Xid { get; set; }

        /// <summary>
        /// Indicates the liability-shift result for the authentication.
        /// </summary>
        public string LiabilityShift { get; set; }

        /// <summary>
        /// Creates a 3-D Secure data object with the default payment data type.
        /// </summary>
        public ThreeDSecure() {
            PaymentDataType = "3DSecure";
        }

        /// <summary>
        /// Merges non-null values from another 3-D Secure data object into this object.
        /// </summary>
        /// <param name="secureEcom">The 3-D Secure data to merge.</param>
        public void Merge(ThreeDSecure secureEcom) {
            if (secureEcom != null) {
                AuthenticationDetails = MergeValue(AuthenticationDetails, secureEcom.AuthenticationDetails);
                AcsTransactionId = MergeValue(AcsTransactionId, secureEcom.AcsTransactionId);
                AcsEndVersion = MergeValue(AcsEndVersion, secureEcom.AcsEndVersion);
                AcsStartVersion = MergeValue(AcsStartVersion, secureEcom.AcsStartVersion);
                AcsInterface = MergeValue(AcsInterface, secureEcom.AcsInterface);
                AcsUiTemplate = MergeValue(AcsUiTemplate, secureEcom.AcsUiTemplate);
                AcsReferenceNumber = MergeValue(AcsReferenceNumber, secureEcom.AcsReferenceNumber);
                ProviderServerTransRef = MergeValue(ProviderServerTransRef, secureEcom.ProviderServerTransRef);
                Algorithm = MergeValue(Algorithm, secureEcom.Algorithm);
                Amount = MergeValue(Amount, secureEcom.Amount);
                AuthenticationSource = MergeValue(AuthenticationSource, secureEcom.AuthenticationSource);
                AuthenticationType = MergeValue(AuthenticationType, secureEcom.AuthenticationType);
                AuthenticationRequestType = MergeValue(AuthenticationRequestType, secureEcom.AuthenticationRequestType);
                AuthenticationValue = MergeValue(AuthenticationValue, secureEcom.AuthenticationValue);
                AuthenticationValueData = MergeValue(AuthenticationValueData, secureEcom.AuthenticationValueData);
                CardHolderResponseInfo = MergeValue(CardHolderResponseInfo, secureEcom.CardHolderResponseInfo);
                Cavv = MergeValue(Cavv, secureEcom.Cavv);
                ChallengeMandated = MergeValue(ChallengeMandated, secureEcom.ChallengeMandated);
                ChallengeStatus = MergeValue(ChallengeStatus, secureEcom.ChallengeStatus);
                ChallengeModel = MergeValue(ChallengeModel, secureEcom.ChallengeModel);
                ChallengeValue = MergeValue(ChallengeValue, secureEcom.ChallengeValue);
                RedirectUrl = MergeValue(RedirectUrl, secureEcom.RedirectUrl);
                AcsChallengeRequestUrl = MergeValue(AcsChallengeRequestUrl, secureEcom.AcsChallengeRequestUrl);
                MethodUrl = MergeValue(MethodUrl, secureEcom.MethodUrl);
                MethodData = MergeValue(MethodData, secureEcom.MethodData);
                MobileData = MergeValue(MobileData, secureEcom.MobileData);
                AcsDecoupledResponseIndicator = MergeValue(AcsDecoupledResponseIndicator, secureEcom.AcsDecoupledResponseIndicator);
                ExemptionOptimizationResult = MergeValue(ExemptionOptimizationResult, secureEcom.ExemptionOptimizationResult);
                MessageExtensionValues = MergeValue(MessageExtensionValues, secureEcom.MessageExtensionValues);
                MessageExtensions = MergeValue(MessageExtensions, secureEcom.MessageExtensions);
                Currency = MergeValue(Currency, secureEcom.Currency);
                DecoupledResponseIndicator = MergeValue(DecoupledResponseIndicator, secureEcom.DecoupledResponseIndicator);
                DirectoryServerTransactionId = MergeValue(DirectoryServerTransactionId, secureEcom.DirectoryServerTransactionId);
                DirectoryServerEndVersion = MergeValue(DirectoryServerEndVersion, secureEcom.DirectoryServerEndVersion);
                DirectoryServerStartVersion = MergeValue(DirectoryServerStartVersion, secureEcom.DirectoryServerStartVersion);
                Eci = MergeValue(Eci, secureEcom.Eci);
                Enrolled = MergeValue(Enrolled, secureEcom.Enrolled);
                IssuerAcsUrl = MergeValue(IssuerAcsUrl, secureEcom.IssuerAcsUrl);
                MessageCategory = MergeValue(MessageCategory, secureEcom.MessageCategory);
                MessageVersion = MergeValue(MessageVersion, secureEcom.MessageVersion);
                OrderId = MergeValue(OrderId, secureEcom.OrderId);
                PayerAuthenticationRequest = MergeValue(PayerAuthenticationRequest, secureEcom.PayerAuthenticationRequest);
                PaymentDataSource = MergeValue(PaymentDataSource, secureEcom.PaymentDataSource);
                PaymentDataType = MergeValue(PaymentDataType, secureEcom.PaymentDataType);
                SdkInterface = MergeValue(SdkInterface, secureEcom.SdkInterface);
                SdkUiType = MergeValue(SdkUiType, secureEcom.SdkUiType);
                ServerTransactionId = MergeValue(ServerTransactionId, secureEcom.ServerTransactionId);
                Status = MergeValue(Status, secureEcom.Status);
                StatusReason = MergeValue(StatusReason, secureEcom.StatusReason);
                Version = MergeValue(Version, secureEcom.Version);
                WhitelistStatus = MergeValue(WhitelistStatus, secureEcom.WhitelistStatus);
                ExemptStatus = MergeValue(ExemptStatus, secureEcom.ExemptStatus);
                ExemptReason = MergeValue(ExemptReason, secureEcom.ExemptReason);
                Xid = MergeValue(Xid, secureEcom.Xid);
                LiabilityShift = MergeValue(LiabilityShift, secureEcom.LiabilityShift);
                //this.merchantData = mergeValue(merchantData, secureEcom.getMerchantData());
            }
        }

        /// <summary>
        /// Returns the incoming value when present; otherwise, returns the current value.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="currentValue">The value already held by this object.</param>
        /// <param name="mergeValue">The incoming value.</param>
        /// <returns>The incoming value if non-null; otherwise, the current value.</returns>
        private T MergeValue<T>(T currentValue, T mergeValue) {
            if (mergeValue == null) {
                return currentValue;
            }
            return mergeValue;
        }
    }

    /// <summary>
    /// Represents 3DS method data returned by GP-API.
    /// </summary>
    public class ThreeDSecureMethodData {
        /// <summary>
        /// Gets or sets the 3DS server transaction identifier.
        /// </summary>
        public string ThreeDSServerTransactionId { get; set; }
        /// <summary>
        /// Gets or sets the method return URL.
        /// </summary>
        public string ThreeDSMethodReturnUrl { get; set; }
        /// <summary>
        /// Gets or sets the encoded method data.
        /// </summary>
        public string EncodedMethodData { get; set; }
    }

    /// <summary>
    /// Represents ACS rendering information for a mobile challenge.
    /// </summary>
    public class ThreeDSecureMobileData {
        /// <summary>
        /// Gets or sets the signed ACS challenge content.
        /// </summary>
        public string AcsSignedContent { get; set; }
        /// <summary>
        /// Gets or sets the scalar ACS rendering type returned by GP-API.
        /// </summary>
        public string AcsRenderingTypeValue { get; set; }
        /// <summary>
        /// Gets or sets the ACS interface returned directly in mobile data.
        /// </summary>
        public string AcsInterface { get; set; }
        /// <summary>
        /// Gets or sets the ACS UI template returned directly in mobile data.
        /// </summary>
        public string AcsUiTemplate { get; set; }
        /// <summary>
        /// Gets or sets the ACS rendering type details.
        /// </summary>
        public ThreeDSecureAcsRenderingType AcsRenderingType { get; set; }
    }

    /// <summary>
    /// Represents the ACS interface and UI template for a mobile challenge.
    /// </summary>
    public class ThreeDSecureAcsRenderingType {
        /// <summary>
        /// Gets or sets the ACS interface.
        /// </summary>
        public string AcsInterface { get; set; }
        /// <summary>
        /// Gets or sets the ACS UI template.
        /// </summary>
        public string AcsUiTemplate { get; set; }
    }

    /// <summary>
    /// Stores merchant-supplied key-value data and hidden SDK data.
    /// </summary>
    public class MerchantDataCollection : IEnumerable<KeyValuePair<string, string>> {
        /// <summary>
        /// The stored merchant data entries.
        /// </summary>
        private List<MerchantKVP> _collection;

        /// <summary>
        /// Gets the visible value for a key or sets the value of an existing key.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns>The visible entry value, or <c>null</c> if the key is not found.</returns>
        public string this[string key] {
            get {
                var kvp = _collection.FirstOrDefault(p => p.Key == key);
                if (kvp != null && kvp.Visible)
                    return kvp.Value;
                return null;
            }
            internal set {
                var kvp = _collection.FirstOrDefault(p => p.Key == key);
                if (kvp != null)
                    kvp.Value = value;
            }
        }

        /// <summary>
        /// Gets the number of visible entries.
        /// </summary>
        public int Count {
            get {
                return _collection.Where(p => p.Visible).Count();
            }
        }

        /// <summary>
        /// Gets the entries hidden from merchant-facing enumeration.
        /// </summary>
        internal IEnumerable<MerchantKVP> HiddenValues {
            get { return _collection.Where(p => p.Visible == false); }
        }

        /// <summary>
        /// Creates an empty merchant data collection.
        /// </summary>
        public MerchantDataCollection() {
            _collection = new List<MerchantKVP>();
        }

        /// <summary>
        /// Adds an entry with the specified visibility.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value.</param>
        /// <param name="visible">Whether the entry is visible to merchants.</param>
        internal void Add(string key, string value, bool visible) {
            if (HasKey(key)) {
                if (visible)
                    throw new ApiException(string.Format("Key {0} already exists in the collection.", key));
                else this[key] = value;
            }


            _collection.Add(new MerchantKVP {
                Key = key,
                Value = value,
                Visible = visible
            });
        }
        /// <summary>
        /// Adds a merchant-visible entry.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <param name="value">The entry value.</param>
        public void Add(string key, string value) {
            Add(key, value, true);
        }

        /// <summary>
        /// Gets an entry value and optionally converts it to the requested type.
        /// </summary>
        /// <typeparam name="T">The type to which the value is converted.</typeparam>
        /// <param name="key">The entry key.</param>
        /// <param name="converter">An optional conversion function.</param>
        /// <returns>The converted value, or the default value of <typeparamref name="T"/> when the key is absent.</returns>
        internal T GetValue<T>(string key, Func<string, T> converter = null) {
            var kvp = _collection.FirstOrDefault(p => p.Key == key);
            if (kvp != null) {
                if (converter != null)
                    return converter(kvp.Value);
                else return (T)Convert.ChangeType(kvp.Value, typeof(T));
            }
            return default(T);
        }

        /// <summary>
        /// Determines whether an entry with the specified key has a value.
        /// </summary>
        /// <param name="key">The entry key.</param>
        /// <returns><c>true</c> if the key has a value; otherwise, <c>false</c>.</returns>
        internal bool HasKey(string key) {
            return GetValue<string>(key) != null;
        }

        /// <summary>
        /// Copies hidden entries from another collection when their keys are absent.
        /// </summary>
        /// <param name="collection">The collection containing hidden entries.</param>
        internal void MergeHidden(MerchantDataCollection collection) {
            foreach (var item in collection.HiddenValues) {
                if (!this.HasKey(item.Key))
                    _collection.Add(item);
            }
        }

        /// <summary>
        /// Parses a Base64-encoded merchant data string into a collection.
        /// </summary>
        /// <param name="kvpString">The encoded key-value data.</param>
        /// <param name="decoder">An optional function used to decode the data after Base64 decoding.</param>
        /// <returns>The parsed merchant data collection.</returns>
        public static MerchantDataCollection Parse(string kvpString, Func<string, string> decoder = null) {
            var collection = new MerchantDataCollection();

            // decrypt the string
            var decryptedKvp = Encoding.UTF8.GetString(Convert.FromBase64String(kvpString));
            if (decoder != null) {
                decryptedKvp = decoder(decryptedKvp);
            }

            // build out the object
            var merchantData = decryptedKvp.Split('|');
            foreach (var kvp in merchantData) {
                var data = kvp.Split(':');
                collection.Add(data[0], data[1], bool.Parse(data[2]));
            }

            return collection;
        }

        /// <summary>
        /// Serializes the collection as a Base64-encoded string.
        /// </summary>
        /// <param name="encryption">An optional function used to transform the serialized data before Base64 encoding.</param>
        /// <returns>The serialized merchant data.</returns>
        public string ToString(Func<string, string> encryption = null) {
            var sb = new StringBuilder();

            _collection.ForEach((kvp) => {
                sb.Append(string.Format("{0}:{1}:{2}|", kvp.Key, kvp.Value, kvp.Visible));
            });

            var formatted = sb.ToString().TrimEnd('|');
            if (encryption != null)
                formatted = encryption(formatted);

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(formatted));
        }

        /// <summary>
        /// Returns an enumerator over the collection's key-value entries.
        /// </summary>
        /// <returns>An enumerator over the entries.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            var enumerator = new List<KeyValuePair<string, string>>(_collection.Select((kvp) => {
                return new KeyValuePair<string, string>(kvp.Key, kvp.Value);
            }));

            return enumerator.GetEnumerator();
        }

        /// <summary>
        /// Returns a non-generic enumerator over the collection's entries.
        /// </summary>
        /// <returns>An enumerator over the entries.</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// Represents one merchant data key-value entry and its visibility.
    /// </summary>
    internal class MerchantKVP {
        /// <summary>
        /// The entry key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The entry value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Indicates whether the entry is visible to merchants.
        /// </summary>
        internal bool Visible { get; set; }
    }
}
