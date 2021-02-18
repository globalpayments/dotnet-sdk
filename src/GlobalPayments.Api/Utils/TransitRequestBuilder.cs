using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;
using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Utils {
    internal class TransitRequestBuilder {
        private string _batchDeviceId;
        private string _batchUserId;
        private ElementTree _et;
        private int? _paymentCount;
        private string _root;
        private int? _sequenceNumber;
        private List<CommercialLineItem> _productDetails;
        private Dictionary<string, string> _values;
        private AdditionalTaxDetails _additionalTaxDetails;

        private string this[string key] {
            get {
                if (_values.ContainsKey(key)) {
                    return _values[key];
                }
                return null;
            }
        }

        private bool HasBatchParams {
            get {
                return (!string.IsNullOrEmpty(_batchDeviceId) || !string.IsNullOrEmpty(_batchUserId));
            }
        }
        private bool HasPartialShipmentData {
            get {
                return (_sequenceNumber != null || _paymentCount != null);
            }
        }
        private bool HasProductDetails {
            get {
                return _productDetails != null && _productDetails.Count > 0;
            }
        }

        public TransitRequestBuilder(string root) {
            _root = root;
            _values = new Dictionary<string, string>();
            _et = new ElementTree();
        }

        public TransitRequestBuilder Set(string key, string value) {
            if (!string.IsNullOrEmpty(value)) {
                _values.Add(key, value);
            }
            return this;
        }

        public TransitRequestBuilder Set(string key, int? value, int length = 1) {
            if (value != null) {
                string strValue = value.ToString().PadLeft(length, '0');
                _values.Add(key, strValue);
            }
            return this;
        }

        public TransitRequestBuilder Set(string key, Enum value) {
            if (value != null) {
                _values.Add(key, value.ToString());
            }

            return this;
        }

        public TransitRequestBuilder SetAdditionalTaxDetails(AdditionalTaxDetails details) {
            _additionalTaxDetails = details;
            return this;
        }

        public TransitRequestBuilder SetBatchParams(string deviceId = null, string userId = null) {
            _batchDeviceId = deviceId;
            _batchUserId = userId;
            return this;
        }

        public TransitRequestBuilder SetProductDetails(List<CommercialLineItem> productData) {
            _productDetails = productData;
            return this;
        }

        public TransitRequestBuilder SetPartialShipmentData(int? sequenceNumber, int? paymentCount) {
            _sequenceNumber = sequenceNumber;
            _paymentCount = paymentCount;
            return this;
        }

        public string BuildRequest<T>(T builder) where T : TransactionBuilder<Transaction> {
            Element transaction = _et.Element(_root);
            foreach (var element in BuildRequestMap(builder)) {
                if (element.Equals("productDetails") && HasProductDetails) {
                    foreach (var item in _productDetails) {
                        var productElement = _et.SubElement(transaction, "productDetails");

                        _et.SubElement(productElement, "productCode", item.ProductCode);
                        _et.SubElement(productElement, "productName", item.Name);
                        _et.SubElement(productElement, "price", item.UnitCost);
                        _et.SubElement(productElement, "quantity", item.Quantity);
                        _et.SubElement(productElement, "measurementUnit", item.UnitOfMeasure);

                        // discount details
                        if (item.DiscountDetails != null) {
                            var discountDetails = _et.SubElement(productElement, "productDiscountDetails");
                            _et.SubElement(discountDetails, "productDiscountName", item.DiscountDetails.DiscountName);
                            _et.SubElement(discountDetails, "productDiscountAmount", item.DiscountDetails.DiscountAmount);
                            _et.SubElement(discountDetails, "productDiscountPercentage", item.DiscountDetails.DiscountAmount);
                            _et.SubElement(discountDetails, "productDiscountType", item.DiscountDetails.DiscountAmount);
                            _et.SubElement(discountDetails, "priority", item.DiscountDetails.DiscountPriority);
                            _et.SubElement(discountDetails, "stackable", item.DiscountDetails.DiscountIsStackable ? "YES" : "NO");
                        }

                        // tax details
                        if (item.TaxAmount != null) {
                            var taxDetails = _et.SubElement(productElement, "productTaxDetails");
                            _et.SubElement(taxDetails, "productTaxName", item.TaxName);
                            _et.SubElement(taxDetails, "productTaxAmount", item.TaxAmount.ToCurrencyString());
                            _et.SubElement(taxDetails, "productTaxPercentage", item.TaxPercentage);
                        }

                        _et.SubElement(productElement, "productNotes", item.Description);
                        //_et.SubElement(productElement, "productDicsountIndicator", item);
                        _et.SubElement(productElement, "productCommodityCode", item.CommodityCode);
                        _et.SubElement(productElement, "alternateTaxID", item.AlternateTaxId);
                        if (item.CreditDebitIndicator.Equals(CreditDebitIndicator.Credit)) {
                            _et.SubElement(productElement, "creditIndicator",  "YES");
                        }
                    }
                }
                else if (element.Equals("additionalTaxDetails") && _additionalTaxDetails != null) {
                    Element taxDetails = _et.SubElement(transaction, "additionalTaxDetails");
                    _et.SubElement(taxDetails, "taxType", _additionalTaxDetails.TaxType);
                    _et.SubElement(taxDetails, "taxAmount", _additionalTaxDetails.TaxAmount.ToCurrencyString());
                    _et.SubElement(taxDetails, "taxRate", _additionalTaxDetails.TaxRate.ToCurrencyString());
                    _et.SubElement(taxDetails, "taxCategory", EnumConverter.GetMapping(Target.Transit, _additionalTaxDetails.TaxCategory));
                }
                else if (element.Equals("partialShipmentData") && HasPartialShipmentData) {
                    Element paymentDetails = _et.SubElement(transaction, "partialShipmentData");
                    _et.SubElement(paymentDetails, "currentPaymentSequenceNumber", _sequenceNumber);
                    _et.SubElement(paymentDetails, "totalPaymentCount", _paymentCount);
                }
                else if (element.Equals("batchCloseParameter") && HasBatchParams) {
                    Element batchParams = _et.SubElement(transaction, "batchCloseParameter");
                    _et.SubElement(batchParams, "currentPaymentSequenceNumber", _batchDeviceId);
                    _et.SubElement(batchParams, "totalPaymentCount", _batchUserId);
                }
                else _et.SubElement(transaction, element, this[element]);
            }

            return _et.ToString(transaction);
        }

        private LinkedList<string> BuildRequestMap<T>(T builder) where T : TransactionBuilder<Transaction> {
            IPaymentMethod paymentMethod = builder.PaymentMethod;

            switch (builder.TransactionType) {
                case TransactionType.Auth:
                case TransactionType.Sale: {
                        if (paymentMethod is Debit) {
                            return BuildList("deviceID|transactionKey|manifest|cardDataSource|transactionAmount|tip|salesTax|additionalTaxDetails|currencyCode|track2Data|track3Data|emulatedTrackData|emvTags|emvFallbackCondition|lastChipRead|paymentAppVersion|emcContactlessToContactChip|pin|pinKsn|secureCode|digitalPaymentCryptogram|programProtocol|directoryServerTransactionID|paymentAccountReference|panReferenceIdentifier|nfcTags|ksn|transactionMID|externalReferenceID|operatorID|orderNumber|cardOnFile|merchantReportID|encryptionType|tokenRequired|healthCareAccountType|prescriptionAmount|visionAmount|dentalAmount|clinicAmount|isQualifiedIIAS|rxNumber|couponID|providerID|providerToken|locationID|notifyEmailID|customerCode|firstName|lastName|transTotalDiscountAmount|transDiscountName|transDiscountAmount|transDiscountPercentage|priority|stackable|productDetails|productDiscountName|productDiscountAmount|productDiscountPercentage|productDiscountType|priority|stackable|productTaxName|productTaxAmount|productTaxPercentage|productTaxType|productVariation|modifierName|modifierValue|modifierPrice|productNotes|softDescriptor|developerID|registeredUserIndicator|lastRegisteredChangeDate|laneID|authorizationIndicator|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|splitTenderPayment|splitTenderID|splitTenderConsolidatedReceipt|noIndividualTransactionReceipt");
                        }
                        return BuildList("deviceID|transactionKey|manifest|cardDataSource|transactionAmount|tip|salesTax|additionalTaxDetails|shippingCharges|dutyCharges|surcharge|additionalAmountType|additionalAmount|additionalAmountSign|currencyCode|cardNumber|expirationDate|cvv2|track1Data|track2Data|track3Data|emulatedTrackData|cardHolderName|secureCode|securityProtocol|ucafCollectionIndicator|digitalPaymentCryptogram|programProtocol|directoryServerTransactionID|paymentAccountReference|panReferenceIdentifier|eciIndicator|cardOnFileTransactionIdentifier|emvTags|pin|pinKsn|emvFallbackCondition|lastChipRead|paymentAppVersion|emvContactlessToContactChip|nfcTags|walletSource|checkOutID|addressLine1|zip|transactionMID|externalReferenceID|operatorID|orderNumber|cardOnFile|merchantReportID|encryptionType|ksn|tokenRequired|healthCareAccountType|prescriptionAmount|visionAmount|dentalAmount|clinicAmount|isQualifiedIIAS|rxNumber|couponID|providerID|providerToken|locationID|notifyEmailID|orderID|customerCode|firstName|lastName|customerPhone|transTotalDiscountAmount|transDiscountName|transDiscountAmount|transDiscountPercentage|priority|stackable|productDetails|productDiscountName|productDiscountAmount|productDicsountPercentage|productDiscountType|priority|stackable|productTaxName|productTaxAmount|productTaxPercentage|productTaxType|productVariation|modifierName|modifierValue|modifierPrice|productNotes|productDiscountIndicator|orderNotes|orderServiceTimestamp|commercialCardLevel|purchaseOrder|chargeDescriptor|customerVATNumber|customerRefID|orderDate|summaryCommodityCode|vatInvoice|chargeDescriptor2|chargeDescriptor3|chargeDescriptor4|supplierReferenceNumber|shipFromZip|shipToZip|destinationCountryCode|orderID|tokenRequesterID|softDescriptor|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|mPosAcceptanceDeviceType|developerID|paymentFacilitatorIdentifier|paymentFacilitatorName|subMerchantIdentifier|subMerchantName|subMerchantCountryCode|subMerchantStateCode|subMerchantCity|subMerchantPostalCode|subMerchantEmailId|subMerchantPhone|isoIdentifier|isRecurring|billingType|paymentCount|currentPaymentCount|isoIdentifier|registeredUserIndicator|lastRegisteredChangeDate|laneID|authorizationIndicator|splitTenderPayment|splitTenderID|splitTenderConsolidatedReceipt|noIndividualTransactionReceipt");
                    }
                case TransactionType.Balance:
                    return BuildList("deviceID|transactionKey|manifest|cardDataSource|currencyCode|track1Data|track2Data|track3Data|emulatedTrackData|cardNumber|expirationDate|cvv2|cardHolderName|secureCode|securityProtocol|ucafCollectionIndicator|digitalPaymentCryptogram|programProtocol|directoryServerTransactionID|paymentAccountReference|panReferenceIdentifier|eciIndicator|cardOnFileTransactionIdentifier|nfcTags|walletSource|checkOutID|dtvv|addressLine1|zip|transactionMID|externalReferenceID|operatorID|orderNumber|cardOnFile|encryptionType|ksn|tokenRequired|customerCode|firstName|lastName|customerPhone|tokenRequesterID|softDescriptor|developerID|laneID|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|mPosAcceptanceDeviceType");
                case TransactionType.BatchClose:
                    return BuildList("deviceID|transactionKey|manifest|operatingUserID|batchCloseParameter");
                case TransactionType.Capture:
                    return BuildList("deviceID|transactionKey|manifest|transactionAmount|tip|salesTax|additionalTaxDetails|shippingCharges|dutyCharges|surcharge|additionalAmountType|additionalAmount|additionalAmountSign|transactionID|externalReferenceID|operatorID|isPartialShipment|partialShipmentData|softDescriptor|merchantReportID|customerCode|firstName|lastName|transTotalDiscountAmount|transDiscountName|transDiscountAmount|transDiscountPercentage|priority|stackable|productDetails|productDiscountName|productDiscountAmount|productDicsountPercentage|productDiscountType|priority|stackable|productTaxName|productTaxAmount|productTaxPercentage|productTaxType|productVariation|modifierName|modifierValue|modiferPrice|productNotes|productDiscountIndicator|orderNotes|orderServiceTimestamp|commercialCardLevel|purchaseOrder|chargeDescriptor|customerVATNumber|customerRefID|orderDate|summaryCommodityCode|vatInvoice|chargeDescriptor2|chargeDescriptor3|chargeDescriptor4|supplierReferenceNumber|shipFromZip|shipToZip|destinationCountryCode|developerID|paymentFacilitatorIdentifier|paymentFacilitatorName|subMerchantIdentifier|subMerchantName|subMerchantCountryCode|subMerchantStateCode|subMerchantCity|subMerchantPostalCode|subMerchantEmailId|subMerchantPhone");
                case TransactionType.Refund:
                case TransactionType.Reversal:
                case TransactionType.Void:
                    return BuildList("deviceID|transactionKey|manifest|transactionAmount|tip|salesTax|additionalTaxDetails|shippingCharges|surcharge|currencyCode|transactionID|externalReferenceID|operatorID|tokenRequired|productDetails|productTaxName|productTaxAmount|productTaxPercentage|productVariation|modifierName|modifierValue|modifierPrice|productNotes|developerID|voidReason|laneID|achCancelNote");
                case TransactionType.Verify: {
                        /* This is the list for the zero dollar authorization */
                        return BuildList("deviceID|transactionKey|manifest|cardDataSource|currencyCode|track1Data|track2Data|track3Data|emulatedTrackData|cardNumber|expirationDate|cvv2|cardHolderName|secureCode|securityProtocol|ucafCollectionIndicatordigitalPaymentCryptogram|programProtocol|directoryServerTransactionID||paymentAccountReference|panReferenceIdentifier|eciIndicator|nfcTags|walletSource|checkOutID|addressLine1|zip|transactionMID|externalReferenceID|operatorID|orderNumber|cardOnFile|merchantReportID|encryptionType|ksn|tokenRequired|customerCode|firstName|lastName|tokenRequesterID|softDescriptor|developerID|laneID|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|mPosAcceptanceDeviceType");

                        /* This is the list for the card AVS and CVV check */
                        //return BuildList("deviceID|transactionKey|manifest|cardDataSource|emulatedTrackData|cardNumber|expirationDate|cvv2|walletSource|checkOutID|cardHolderName|secureCode|securityProtocol|ucafCollectionIndicator|paymentAccountReference|panReferenceIdentifier|eciIndicator|nfcTags|addressLine1|zip|externalReferenceID|operatorID|orderNumber|cardOnFile|merchantReportID|encryptionType|ksn|tokenRequired|customerCode|firstName|lastName|tokenRequesterID|softDescriptor|developerID|laneID|terminalCapability|terminalOperatingEnvironment|cardholderAuthenticationMethod|terminalAuthenticationCapability|terminalOutputCapability|maxPinLength|terminalCardCaptureCapability|cardholderPresentDetail|cardPresentDetail|cardDataInputMode|cardholderAuthenticationEntity|cardDataOutputCapability|mPosAcceptanceDeviceType");
                    }
                case TransactionType.Tokenize: {
                        return BuildList("deviceID|transactionKey|cardDataSource|cardNumber|expirationDate|cardHolderName|cardVerification|developerID");
                    }
                default: throw new UnsupportedTransactionException();
            }
        }

        private LinkedList<string> BuildList(string str) {
            var list = new LinkedList<string>();

            string[] values = str.Split('|');
            foreach (string value in values) {
                if (!string.IsNullOrEmpty(value)) {
                    list.AddLast(value);
                }
            }

            return list;
        }
    }
}
