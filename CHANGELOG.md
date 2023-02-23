# Changelog

## Latest

#### Enhancements

- GP-API: Added validation method for X-GP-SIGNATURE that is received from GP-API on the notification URL.

## v5.1.1 (21/02/23)

#### Enhancements

- GP-API: Added Open Banking to let customers pay using their bank account.

## v5.1.0 (02/02/23)

#### Enhancements

- GP-API: Added Risk Assessments that represents an analysis to determine the risk of fraud associated with a transaction, payer or payment method.
- NWS: Gift Card Available Balance added to response

#### Bug Fixes:

- NWS: Out of balance issue related to gift cards

## v5.0.6 (01/31/23)

#### Enhancements

- GP-API: Click To Pay added for merchants can give customers an easier, seamless checkout experience while also gaining access to Secure Remote Commerce network tokens

## v5.0.5 (01/12/23)

#### Enhancements

- GP-API: Buy Now Pay Later payment method allows customers to repay the cost of their purchase over time instead of all at once
- GP-API: Added exemption status to help reduce payment friction

## v5.0.4 (01/10/23)

#### Bug Fixes:

- Portico Gateway: Fix for Level III data not being included in the CreditCPCEdit request

## v5.0.3 (01/05/23)

#### Enhancements

- Portico: SdkNameVersion: Name and Version of the SDK used for integration, where applicable
- Update device configuration to be generic
- Transaction API: Add transaction api support for US and CA region

## v5.0.2 (12/13/22)

#### Enhancements

- GP-API: Added brand_reference in the recurring transaction request
- GP-API: Added Decoupled authentication
- GP-API: Added onboarding merchant feature

## v5.0.1 (11/15/22)

#### Enhancements

- Portico Gateway: add Ecommerce tag to CreditReturn transactions
- Security enhancement in example application
- Sanitize phone numbers and zip codes

## v5.0.0 (11/01/22)

#### Enhancements

- Added phone country and retrieve the same by NumericCountryCode/ISO2/ISO3
- Removed 3DS1 for GP-API and GP-ECOM

## v4.2.4 (10/13/2022)

- GP-API: The Fraud Management solution passed the values sent in the risk_assessment object
- GP-API: Updated PayLink properties on the request
- Added isnotnull validation for subproperties in builders
- Updated OpenBaking service endpoints

## v4.2.3 (10/04/2022)

#### Enhancements

- Heartland/Portico Gateway: added method for voiding eCheck using ClientTxnId to CheckService

## v4.2.2 (9/27/2022)

#### Enhancements

- Heartland/Portico Gateway: add "ClientTxnId" to transaction response properties

#### Bug Fixes:

- Heartland/Portico Gateway: fix CreditReversal message structure

## v4.2.1 (9/22/2022)

#### Bug Fixes:

- UPA Devices: increased read buffer size to allow for larger message size from devices

## v4.2.0 (9/15/2022)

#### Enhancements

 GP-API: Add new mapping for card issuer avs/cvv result 

#### Bug Fixes:

- PAX Devices: - fixed 'verify' transactions not working
- GP-API: Fix issue with the ServiceUrl when switching to PRODUCTION env

## v4.1.6 (9/8/2022)

#### Enhancements

- UPA Devices: Improved connection timeout exception handling

#### Bug Fixes:

- UPA Devices: Corrected invoice number logic

## v4.1.5 (8/23/2022)

#### Enhancements

- Enhanced security on MVC end to end example application
- Increased the version number of Newtonsoft.Json in the GlobalPayments.API solution
- GP-API: Updated Misc Tests
- GP-ECOM: Add srd information on store a new card request 
- GP-API: Add mapping for "acs_reference_number", "acs_signed_content" and "server_trans_ref" in 3DS flow on initiate authentication step

#### Bug Fixes:

- UPA Devices: Added invoice number to Refund and Tip Adjust transactions
- GP-ECOM: Billing/Shipping country value should be ISO2 country code

## v4.1.4 (8/4/2022)

#### Bug Fixes:

- Added DE 25 Message Reason Code enum value for Failure to Dispense to replace incorrect, outdated values (NWS)
- Corrected an issue where EmvFallbackCondition was not being respected when set (NWS)

## v4.1.3 (8/2/2022)

#### Enhancements

- GP-API: Added PayLink API that allows you to generate single or multi-use unique payment links.

## v4.1.2 (7/26/2022)

#### Enhancements:

- UPA Devices: added tip-adjust support, open-tab report type, & expiration date transaction response property
- UPA Devices: added support for the new fields 'directMktInvoiceNbr', 'directMktShipMonth', 'directMktShipDay'

#### Bug Fixes:

- UPA Devices: corrected End-Of-Day response exception handling
- Wallet and 3DSecure: fix request 

## v4.1.1 (7/12/2022)

#### Enhancements:

- GP-ECOM: HPP_CHALLENGE_REQUEST_INDICATOR value from integer to string
- GP-ECOM: Added supplementary data elements for having extra data in the request and can be used in Authorization, Credit, Refund (Rebate) and Capture (Settle)

#### Bug Fixes:

- GP-ECOM ApplePay: Fixed amount less than $1

## v4.1.0 (6/28/2022)

#### Enhancements:

- GP-API: Added Dynamic Descriptor
- GP-ECOM: Added flag EnableExemptionOptimization to identify customer exemptions and avoid unnecessary authentications
- GP-ECOM: Added Open Banking as new payment method

## v4.0.2 (5/26/2022)

#### Enhancements:

- GP-API: Features (Adjust, Stored Payment Methods - POST Search and disputes- GET Document)
- GP-API: Update payment token (Added missing keys in the request to update the payment token)
- GP-ECOM: Added New HPP Fields in the SDKs for HPP Capture Billing enhancement

#### Bug Fixes:

- PAX devices: Fix refund validation logic
- TransIt Gateway: Fix pin debit message handling

## v4.0.0 (5/17/2022)

#### Enhancements:

- GpApiService refactor
- GP-API: Add increment an Auth

#### Bug Fixes:

- PAX Devices: Fixed issue with EBT processing using recent application versions

## v3.0.5 (4/28/2022)

#### Bug Fixes:

- Fixed connection failover to secondary endpoints (NWS)

## v3.0.4 (4/21/2022)

#### Enhancements:

- Added support for device ordering during account creation (ProPay)
- CountryUtils class expanded to convert from any ISO 3166 value to any other ISO 3166 value
- GP-ECOM: add 3DS tests to the suite

## v3.0.3 (4/7/2022)

#### Enhancements:

- Added additional batch details to End of Day response object (UPA)
- Updated Digital Wallet and 3DSecure support to use latest Portico data fields 

#### Bug Fixes:

- Fixed HPP verify with 3DSecure
- Fixed an issue when setting DeviceType to HPA_LANE3000 during configuration

## v3.0.2 (3/31/2022)

#### Enhancements:

- Card Brand Transaction ID is mapped with the response when token is requested (UPA)

#### Bug Fixes:

- Fixed index out of bounds exception with Cardholder Name
- Fixed processor configuration error with birth year
- Addressed null reference exceptions with response mappings (UPA)
- Corrected an issue with a HEX conversion helper method (UPA)
- Corrected a boolean conversion error on request building (UPA)
- Corrected an incorrect internal TransactionType setting (UPA)

## v3.0.1 (3/29/2022)

#### Enhancements:

- Added challenge request indicator on 3DS2 initiate step on Gp3DSProvider

#### Bug Fixes:

- Fixed Multicapture information on the response of Authorize transaction (GpApi)
- Fixed for card information not to be sent into request when using tokenized card (GpApi)

## v3.0.0 (3/24/2022)

#### Enhancements:

- Added Authorization (UPA)
- Added DeletePreAuth (UPA)
- Added Tokenization (UPA)
- Added HSA/FSA support (UPA)
- Support alphanumerical ECI values on 3DS

#### Bug Fixes:

- Fixed mapping issue with Payment Method details (GPEcom)

## v2.0.6 (3/22/2022)

#### Enhancements:

- Added direct Base64 string support for Document/Document Chargeback Upload image parameters (ProPay)
- Added ProPay error code to GatewayException object (ProPay)

## v2.0.5 (03/15/2022)

#### Enhancements:

- Added fingerprint_mode in the create transaction request
- MOBILE_SDK source in the 3DS flow
- Alternative Payment Method (PayPal) for GPEcom
- Added expMonth and expYear on test classes
- DCC tests updates
- Added more tests for fingerprint
- Added test for expiry card

## v2.0.4 (03/02/2022)

#### Bug Fixes:

- fix for set attributes exp month and exp year when updating a token

## v2.0.3 (2/22/2022)

#### Enhancements:

- use "IN_APP" entry_mode when creating a transaction with digital wallets (GP-API)
- Refactor reporting unit test (GP-API)
- Pass X509 Certificate by Base64 string (ProPay)
- Added ProPay transaction type 19, Get Enhanced Account Details (ProPay)

## v2.0.2 (02/17/2022)

#### Enhancements:

- Add Payment Link Id in the request for authorize (GP-API)
- Add Dynamic Currency Conversion feature for GP-API

#### Bug Fixes:

- Fix issue for Diners card type (GP-ECOM)

## v2.0.1 (01/20/2022)

- move RequestLogger to new structure

#### Bug Fixes:

- fix FLEET card types on GP-ECOM

---

## v2.0.0 (12/16/2021)

#### Enhancements:

- Add Paypal alternative payment method to GpApi

---

## v1.10.0 (12/07/2021)

#### Bug Fixes:

- Fix Mater Card automatic card type detection
- Fix thread safe hash generation

---

## v1.10.0 (11/18/2021)

#### Enhancements:

- Added initial support for Unified Payments Application

---

## v1.9.0 (11/10/2021)

#### Enhancements:

- Update default GpApi version header
- Add Eci field to DigitalWallet
- Add Throw Exception on error to UpdateTokenExpiry and DeleteToken methods on Credit class
- Removed funding and cvv_indicator fields when not needed when calling GpApi

---

## v1.7.45 (10/28/2021)

#### Enhancements:

- Add Contactless Swipe entry mode
- Add ACH functionality
- Add Thread safe add/remove configuration on ServicesContainer

#### Bug Fixes:

- Fix issue in v1.7.31 and up for eCheck auths on Portico/Heartland

---

## v1.7.44 (09/28/2021)

#### Enhancements:

- Add Multiple merchants to GpApi

---

## v1.7.43 (09/23/2021)

#### Enhancements:

- Add Entry Mode to GpApi

---

## v1.7.42 (09/09/2021)

#### Enhancements:

- Add Dynamic Headers to GpApi
- Add Digital Wallet to GpApi
- Add AvsResponse mappings to GpApi
- Add Fraud rules response to RealexHppClient

---

## v1.7.41 (08/26/2021)

#### Enhancements:

- Add Fraud Dynamic Rules to GpEcom
- Add amount and currency into Apple Pay hash generation for GpEcom

---

## v1.7.40 (08/19/2021)

#### Enhancements:

- Add Alternative payment method response mapping to GpEcom

---

## v1.7.39.1 (08/12/2021)

- Add recurring payment with stored credentials functionallity to GpApi
- Add MerchantContactUrl to GpApiConfig

---

## v1.7.38 (08/10/2021)

#### Enhancements:

- Add "Netherlands Antilles" country codes
- Add phone and subscriber number validation for 3DS2
- Add search by deposit ID for settlement disputes on GpApi
- Add PAY_BY_BANK_APP as an alternative payment type

#### Bug Fixes:

- Fix alternative payment method for charge on GPEcom

---

## v1.7.37 (08/05/2021)

#### Enhancements:

- NWS second phase:
	- NWS Credit card transactions
	- Giftcard and Fleet tested
	- Changes specifically for Purchase cards, EWIC, ECheck, ReadyLink and Fleet
	- Added DE 72 for POS site config
	- Fixed issues with formatting and test data
	- Update AVS tests

---

## v1.7.36 (08/03/2021)

#### Enhancements:

- Upgrade GpApi to March version
- Add support for single and multiple usage mode for tokenized cards
- Remove detokenization endpoint
- 3DSecure mapping updated to support liability shift to GpApi
- Add deposit date filter for find settlement disputes search to GpApi
- Add EBT functionallity to GpApi

---

## v1.7.35 (07/15/2021)

#### Enhancements:

- Add ability to remove gateway configurations by config name

---

## v1.7.33 (07/01/2021)

#### Enhancements:

- Add "encoded_method_data" field mapping to PayerAuthenticationRequest in Map3DSecureData

#### Bug Fixes:

- Fix GpApi response mapping for null dates

---

## v1.7.31 (06/24/2021)

#### Enhancements:

- Add Itokenizable implementation to eCheck class

---

## v1.7.30 (06/17/2021)

#### Enhancements:

- Add MessageExtension property to ThreeDSecure class
- Add DepositDate and DepositReference to Settlement Dispute Summary response mapping

---

## v1.7.29 (06/15/2021)

#### Enhancements:

- Add SchemeReferenceData to GPEcom Transaction Detail Response mapping

---

## v1.7.28 (05/27/2021)

#### Enhancements:

- Add support for portico create customer functionallity with legacy credentials

---

## v1.7.27 (05/13/2021)

#### Enhancements:

- Add GP ECOM dynamic descriptor field

---

## v1.7.26 (05/11/2021)

#### Enhancements:

- Add support for Tokenize transaction type with Portico Gateway

---

## v1.7.24 (05/11/2021)

#### Enhancements:

- Update GP API access token not authenticated scenarios
- Add GP API 3DS builder stored credentials and properly map the data on each request
- Set global merchant country configuration where required
- Add enable exemption optimization on GP ECOM 3DS2 initiate authentication
- Add GP API close batch functionality
- Add GP API stored payment methods report
- Add GP API actions report
- Implement GP API transaction reauthorization
- Update GP API production url

#### Bug Fixes:

- Move GP API 3DS tests service container to class initialize to make sure we reuse the same access token
- Fix GP API get settlement dispute detail with wrong id unit test

---

## v1.7.23 (04/29/2021)

#### Enhancements:

- Enable use GpEcom query command to get transaction details report

---

## v1.7.22 (04/13/2021)

#### Enhancements:

- Update GP API tokenize payment method and verify flows
- Enable limit the specific permissions the GP API access token will have
- Update GP API 3DS authentication flows
- Add GP API 3DS check availability request body fields
- Add GP API 3DS initiate authentication request body fields
- Add additional GP API transaction summary mappings
- Remove GP API disputes and settled disputes filter by adjustment funding
- Remove GP API disputes and settled disputes filter by from adjustment time created and to adjustment time created
- Enhance GP API transactions reports and settled transactions reports

#### Bug Fixes:

- Check if GP API token is not set to create a tokenized payment method
- Clear GP API card token on detokenize to prevent error creating transactions from that card object
- Check GP API 3DS not enrolled response code and let the flow throw the exception in other case

---
