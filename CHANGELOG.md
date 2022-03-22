# Changelog

## Latest

#### Enhancements

- Added direct Base64 string support for Document/Document Chargeback Upload image parameters (ProPay)
- Added ProPay error code to GatewayException object (ProPay)

## v2.0.5 (03/15/2022)

#### Enhancements

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

#### Enhancements

- use "IN_APP" entry_mode when creating a transaction with digital wallets (GP-API)
- Refactor reporting unit test (GP-API)
- Pass X509 Certificate by Base64 string (ProPay)
- Added ProPay transaction type 19, Get Enhanced Account Details (ProPay)

## v2.0.2 (02/17/2022)

#### Enhancements

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

#### Enhancements

- Add Paypal alternative payment method to GpApi

---

## v1.10.0 (12/07/2021)

#### Bug Fixes:

- Fix Mater Card automatic card type detection
- Fix thread safe hash generation

---

## v1.10.0 (11/18/2021)

#### Enhancements

- Added initial support for Unified Payments Application

---

## v1.9.0 (11/10/2021)

#### Enhancements

- Update default GpApi version header
- Add Eci field to DigitalWallet
- Add Throw Exception on error to UpdateTokenExpiry and DeleteToken methods on Credit class
- Removed funding and cvv_indicator fields when not needed when calling GpApi

---

## v1.7.45 (10/28/2021)

#### Enhancements

- Add Contactless Swipe entry mode
- Add ACH functionality
- Add Thread safe add/remove configuration on ServicesContainer

#### Bug Fixes:

- Fix issue in v1.7.31 and up for eCheck auths on Portico/Heartland

---

## v1.7.44 (09/28/2021)

#### Enhancements

- Add Multiple merchants to GpApi

---

## v1.7.43 (09/23/2021)

#### Enhancements

- Add Entry Mode to GpApi

---

## v1.7.42 (09/09/2021)

#### Enhancements

- Add Dynamic Headers to GpApi
- Add Digital Wallet to GpApi
- Add AvsResponse mappings to GpApi
- Add Fraud rules response to RealexHppClient

---

## v1.7.41 (08/26/2021)

#### Enhancements

- Add Fraud Dynamic Rules to GpEcom
- Add amount and currency into Apple Pay hash generation for GpEcom

---

## v1.7.40 (08/19/2021)

#### Enhancements

- Add Alternative payment method response mapping to GpEcom

---

## v1.7.39.1 (08/12/2021)

- Add recurring payment with stored credentials functionallity to GpApi
- Add MerchantContactUrl to GpApiConfig

---

## v1.7.38 (08/10/2021)

#### Enhancements

- Add "Netherlands Antilles" country codes
- Add phone and subscriber number validation for 3DS2
- Add search by deposit ID for settlement disputes on GpApi
- Add PAY_BY_BANK_APP as an alternative payment type

#### Bug Fixes:

- Fix alternative payment method for charge on GPEcom

---

## v1.7.37 (08/05/2021)

#### Enhancements

- NWS second phase:
	- NWS Credit card transactions
	- Giftcard and Fleet tested
	- Changes specifically for Purchase cards, EWIC, ECheck, ReadyLink and Fleet
	- Added DE 72 for POS site config
	- Fixed issues with formatting and test data
	- Update AVS tests

---

## v1.7.36 (08/03/2021)

#### Enhancements

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
