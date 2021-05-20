# Changelog

## Latest

#### Enhancements

- Add support for Tokenize transaction type with Portico Gateway
- Add GP ECOM dynamic descriptor field

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
