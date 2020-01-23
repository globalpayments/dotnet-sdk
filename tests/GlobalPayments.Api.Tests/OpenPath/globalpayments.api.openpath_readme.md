# Global Payments OpenPath API
## Configuring OpenPath with Global Payments
Adding the OpenPath platform for multiple platform routing, fruad, reporting and alerts to your existing Global Payments API is simple. In order to initalize OpenPath you will need an OpenPath API Key and a OpenPath API URL.

## Testing values
OpenPath API Key: ZFQ4CTapPpZAEmjFAGeZfJsRaaFsafuZepCzV9TY
OpenPath API URL: https://unittest-api.openpath.io/v1/globalpayments

## Adding the credentials to your Services Container
Using any of the available service container attributes available for any of the Global Payment feature, just add the credentials to the Services Container as specified below.
```
ServicesContainer.ConfigureService(new GatewayConfig {

    // global payment portico attributes
    SecretApiKey = "skapi_cert_MTeSAQAfG1UA9qQDrzl-kz4toXvARyieptFwSKP24w",
    ServiceUrl = "https://cert.api2.heartlandportico.com",

    // openpath attributes
    OpenPathApiKey = "ZFQ4CTapPpZAEmjFAGeZfJsRaaFsafuZepCzV9TY",
    OpenPathApiUrl = "https://unittest-api.openpath.io/v1/globalpayments"

});
```

# Package developer notes
## Code additions
### GlobalPayments.Api.Entities
`GlobalPayments.Api.Entities.OpenPathResponse` Entity for receiving responses from OpenPath

`GlobalPayments.Api.Entities.OpenpathResultModel` Child transaction results for the Response

`GlobalPayments.Api.Entities.OpenPathTransaction` 

### GlobalPayments.Api.Gateways
`GlobalPayments.Api.Gateways.IOpenPathGateway` Interface for `GlobalPayments.Api.Gateways.OpenPathGateway`

`GlobalPayments.Api.Gateways.OpenPathGateway` Methods for making calls to the OpenPath platform

## Code changes
### GlobalPayments.Api.Builders
`GlobalPayments.Api.Builders.AuthorizationBuilder` Added OpenPath attributes at line: 62

### GlobalPayments.Api.Gateways
`GlobalPayments.Api.Gateways.AmaryllisConnector` Added OpenPath credentials at line: 12

`GlobalPayments.Api.Gateways.PorticoConnector` Added OpenPath credentials at line: 21

`GlobalPayments.Api.Gateways.RealexConnector` Added OpenPath credentials at line: 24

## Unit tests
### Portico Tests
`GlobalPayments.Api.Tests.OpenPath.PorticoCreditTests`

`GlobalPayments.Api.Tests.OpenPath.OpenPathRealexCreditTests`

## Revision History
Version 3179 - Initial release candidate.