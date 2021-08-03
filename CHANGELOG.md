# Changelog

## Latest

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