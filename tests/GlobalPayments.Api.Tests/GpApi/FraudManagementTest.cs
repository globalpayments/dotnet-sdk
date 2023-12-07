using System;
using System.Collections.Generic;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPayments.Api.Tests.GpApi
{
    [TestClass]
    public class FraudManagementTest : BaseGpApiTests
    {
        private static CreditCardData card;
        private static Address address;
        private const string CURRENCY = "USD";

        [TestInitialize]
        public void TestInitialize() {
            var gpApiConfig = GpApiConfigSetup(AppId, AppKey, Channel.CardNotPresent);
            ServicesContainer.ConfigureService(gpApiConfig);

            card = new CreditCardData {
                Number = "4263970000005262",
                ExpMonth = DateTime.Now.Month,
                ExpYear = DateTime.Now.Year + 1,
                Cvn = "131",
                CardHolderName = "James Mason"
            };

            address = new Address {
                StreetAddress1 = "123 Main St.",
                City = "Downtown",
                State = "NJ",
                Country = "US",
                PostalCode = "12345"
            };
        }

        //TODO - For filter set OFF the response fraud result is set incorrectly as Hold
        [TestMethod]
        public void FraudManagementDataSubmissions() {
            var fraudFilters = new Dictionary<FraudFilterMode, string> {
                { FraudFilterMode.ACTIVE, FraudFilterResult.PASS.ToString() },
                { FraudFilterMode.PASSIVE, FraudFilterResult.PASS.ToString() },
                { FraudFilterMode.OFF, "" }
            };
            
            foreach (var items in fraudFilters) {
                var response = card.Charge(98.10m)
                    .WithCurrency(CURRENCY)
                    .WithAddress(address)
                    .WithFraudFilter(items.Key)
                    .Execute();

                Assert.IsNotNull(response);
                Assert.AreEqual("SUCCESS", response.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage);
                Assert.IsNotNull(response.FraudFilterResponse);
                Assert.AreEqual(items.Key.ToString(), response.FraudFilterResponse.FraudResponseMode);
                Assert.AreEqual(items.Value, response.FraudFilterResponse.FraudResponseResult);
            }
        }

        [TestMethod]
        public void FraudManagementDataSubmissionWithRules() {
            const string rule1 = "0c93a6c9-7649-4822-b5ea-1efa356337fd";
            const string rule2 = "a539d51a-abc1-4fff-a38e-b34e00ad0cc3";

            var rules = new FraudRuleCollection();
            rules.AddRule(rule1, FraudFilterMode.ACTIVE);
            rules.AddRule(rule2, FraudFilterMode.OFF);

            var response = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE, rules)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage);
            Assert.IsNotNull(response.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), response.FraudFilterResponse.FraudResponseMode);

            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), response.FraudFilterResponse.FraudResponseResult);
            foreach (var fraudResponseRule in response.FraudFilterResponse.FraudResponseRules)
            {
                if (fraudResponseRule.Key == rule1)
                {
                    Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), fraudResponseRule.result);
                }
                if (fraudResponseRule.Key == rule2)
                {
                    Assert.AreEqual(FraudFilterResult.NOT_EXECUTED.ToString().ToUpper(), fraudResponseRule.result);
                }
            }
        }
        
        [TestMethod]
        public void FraudManagementDataSubmissionWith_AllRulesActive() {
            var ruleList = new List<string> {
                "0c93a6c9-7649-4822-b5ea-1efa356337fd",
                "a539d51a-abc1-4fff-a38e-b34e00ad0cc3",
                "d023a19e-6985-4fda-bb9b-5d4e0dedbb1e"
            };
            
            var rules = new FraudRuleCollection();
            foreach (var rule in ruleList) {
                rules.AddRule(rule, FraudFilterMode.ACTIVE);
            }

            var response = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE, rules)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage);
            Assert.IsNotNull(response.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), response.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), response.FraudFilterResponse.FraudResponseResult);

            foreach (var fraudResponseRule in response.FraudFilterResponse.FraudResponseRules) {
                Assert.IsTrue(ruleList.Contains(fraudResponseRule.Key));
            }
        }
        
        [TestMethod]
        public void FraudManagementDataSubmissionWith_AllRulesOff() {
            var ruleList = new List<string> {
                "0c93a6c9-7649-4822-b5ea-1efa356337fd",
                "a539d51a-abc1-4fff-a38e-b34e00ad0cc3",
                "d023a19e-6985-4fda-bb9b-5d4e0dedbb1e"
            };
            
            var rules = new FraudRuleCollection();
            foreach (var rule in ruleList) {
                rules.AddRule(rule, FraudFilterMode.OFF);
            }

            var response = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE, rules)
                .Execute();

            Assert.IsNotNull(response);
            Assert.AreEqual("SUCCESS", response.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), response.ResponseMessage);
            Assert.IsNotNull(response.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), response.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.NOT_EXECUTED.ToString().ToUpper(), response.FraudFilterResponse.FraudResponseResult);

            foreach (var fraudResponseRule in response.FraudFilterResponse.FraudResponseRules) {
                Assert.IsTrue(ruleList.Contains(fraudResponseRule.Key));
                Assert.AreEqual(FraudFilterResult.NOT_EXECUTED.ToString().ToUpper(), fraudResponseRule.result);
                Assert.AreEqual(FraudFilterMode.OFF, fraudResponseRule.Mode);
            }
        }

        [TestMethod]
        public void ReleaseTransactionAfterFraudResultHold() {
            card.CardHolderName = "Lenny Bruce";
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.HOLD.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);
            
            var trn2 = trn.Release()
                .WithReasonCode(ReasonCode.FALSEPOSITIVE)
                .Execute();

            Assert.IsNotNull(trn2);
            Assert.AreEqual("SUCCESS", trn2.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn2.ResponseMessage);
            Assert.IsNotNull(trn2.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.RELEASE_SUCCESSFUL.ToString().ToUpper(), trn2.FraudFilterResponse.FraudResponseResult);
        }

        [TestMethod]
        public void FraudManagementDataSubmissionFullCycle() {
            var trn = card.Authorize(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Hold()
               .WithReasonCode(ReasonCode.FRAUD)
               .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.HOLD_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Release()
               .WithReasonCode(ReasonCode.FALSEPOSITIVE)
               .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.RELEASE_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Capture()
                    .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
        }
        
         [TestMethod]
        public void FraudManagementDataSubmissionFullCycle_HoldAndReleaseWithoutReasonCode() {
            var trn = card.Authorize(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Hold()
               .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.HOLD_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Release()
               .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.RELEASE_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Capture()
                    .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
        }

        [TestMethod]
        public void CaptureTransactionAfterFraudResultHold() {
            card.CardHolderName = "Lenny Bruce";
            var trn = card.Authorize(10.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.HOLD.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            var errorFound = false;
            try {
                trn.Capture()
                        .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("50020", e.ResponseMessage);
                Assert.AreEqual("Status Code: BadRequest - This transaction has been held", e.Message);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void RefundTransactionAfterFraudResultHold() {
            card.CardHolderName = "Lenny Bruce";
            var trn = card.Charge(10.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.HOLD.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            var errorFound = false;
            try {
                trn.Refund()
                    .WithCurrency(CURRENCY)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - You can't refund a delayed transaction that has not been sent for settlement You are refunding money to a customer that has not been and never will be charged! ", e.Message);
                Assert.AreEqual("40087", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void FraudManagementDataSubmissionFullCycle_Charge() {
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Hold()
               .WithReasonCode(ReasonCode.FRAUD)
               .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.HOLD_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Release()
               .WithReasonCode(ReasonCode.FALSEPOSITIVE)
               .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.RELEASE_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);
        }
        
        [TestMethod]
        public void FraudManagementDataSubmissionFullCycle_Charge_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();
            
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);
            
            var exceptionCaught = false;
            try {
                card.Charge(1)
                    .WithCurrency(CURRENCY)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={trn.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void FraudManagementDataSubmissionFullCycle_ChargeThenRefund() {
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Hold()
                .WithReasonCode(ReasonCode.FRAUD)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.HOLD_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Release()
                .WithReasonCode(ReasonCode.FALSEPOSITIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.RELEASE_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            var refund = trn.Refund()
                .WithCurrency(CURRENCY)
                .Execute();

            Assert.IsNotNull(refund);
            Assert.AreEqual("SUCCESS", refund.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), refund.ResponseMessage);
        }
        
        [TestMethod]
        public void FraudManagementDataSubmissionFullCycle_ChargePassive() {
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.PASSIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.PASSIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Hold()
                .WithReasonCode(ReasonCode.FRAUD)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.HOLD_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Release()
                .WithReasonCode(ReasonCode.FALSEPOSITIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.RELEASE_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);
        }
        
        [TestMethod]
        public void FraudManagementDataSubmissionFullCycle_Charge_ThenReleaseWithoutHold() {
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.PASSIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.PASSIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            var errorFound = false;
            try {
                trn.Release()
                    .WithReasonCode(ReasonCode.FALSEPOSITIVE)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Cant release transaction that is not held", e.Message);
                Assert.AreEqual("50020", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void FraudManagementDataSubmissionFullCycle_Authorize_ThenReleaseWithoutHold() {
            var trn = card.Authorize(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.PASSIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Preauthorized.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.PASSIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            var errorFound = false;
            try {
                trn.Release()
                    .WithReasonCode(ReasonCode.FALSEPOSITIVE)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - Cant release transaction that is not held", e.Message);
                Assert.AreEqual("50020", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void Release_AllReasonCodes() {
            foreach (ReasonCode reasonCode in Enum.GetValues(typeof(ReasonCode))) {
                if (reasonCode == ReasonCode.FRAUD || reasonCode == ReasonCode.OUTOFSTOCK) {
                    continue;
                }

                var trn = card.Charge(98.10m)
                    .WithCurrency(CURRENCY)
                    .WithAddress(address)
                    .WithFraudFilter(FraudFilterMode.ACTIVE)
                    .Execute();

                Assert.IsNotNull(trn);
                Assert.AreEqual("SUCCESS", trn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
                Assert.IsNotNull(trn.FraudFilterResponse);
                Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(),
                    trn.FraudFilterResponse.FraudResponseMode);
                Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(),
                    trn.FraudFilterResponse.FraudResponseResult);
                
                trn = trn.Hold()
                    .WithReasonCode(ReasonCode.FRAUD)
                    .Execute();

                Assert.IsNotNull(trn);
                Assert.AreEqual("SUCCESS", trn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
                Assert.IsNotNull(trn.FraudFilterResponse);
                Assert.AreEqual(FraudFilterResult.HOLD_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

                trn = trn.Release()
                    .WithReasonCode(reasonCode)
                    .Execute();

                Assert.IsNotNull(trn);
                Assert.AreEqual("SUCCESS", trn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
                Assert.IsNotNull(trn.FraudFilterResponse);
                Assert.AreEqual(FraudFilterResult.RELEASE_SUCCESSFUL.ToString().ToUpper(),
                    trn.FraudFilterResponse.FraudResponseResult);
            }
        }
        
        [TestMethod]
        public void Release_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();
            
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            trn = trn.Hold()
                .WithReasonCode(ReasonCode.FRAUD)
                .Execute();
            
            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterResult.HOLD_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);
            
            var exceptionCaught = false;
            try {
                trn.Release()
                    .WithReasonCode(ReasonCode.FALSEPOSITIVE)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={trn.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }

        [TestMethod]
        public void Release_RandomTransaction() {
            var trn = new Transaction {
                TransactionId = Guid.NewGuid().ToString()
            };

            var errorFound = false;
            try {
                trn.Release()
                    .WithReasonCode(ReasonCode.OTHER)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual($"Status Code: NotFound - Transaction {trn.TransactionId} not found at this location.", e.Message);
                Assert.AreEqual("40008", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void Release_InvalidReason() {
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.PASSIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.PASSIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            var errorFound = false;
            try {
                trn.Release()
                    .WithReasonCode(ReasonCode.FRAUD)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - reason_code value is invalid. Please check the reason_code is entered correctly", e.Message);
                Assert.AreEqual("40259", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void HoldTransactionAfterFraudResultHold() {
            card.CardHolderName = "Lenny Bruce";
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.HOLD.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            var errorFound = false;
            try {
                trn.Hold()
                    .WithReasonCode(ReasonCode.FRAUD)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - This transaction is already held", e.Message);
                Assert.AreEqual("50020", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void Hold_AllReasonCodes() {
            foreach (ReasonCode reasonCode in Enum.GetValues(typeof(ReasonCode))) {
                if (reasonCode == ReasonCode.FALSEPOSITIVE || reasonCode == ReasonCode.INSTOCK) {
                    continue;
                }

                var trn = card.Charge(98.10m)
                    .WithCurrency(CURRENCY)
                    .WithAddress(address)
                    .WithFraudFilter(FraudFilterMode.ACTIVE)
                    .Execute();

                Assert.IsNotNull(trn);
                Assert.AreEqual("SUCCESS", trn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
                Assert.IsNotNull(trn.FraudFilterResponse);
                Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(),
                    trn.FraudFilterResponse.FraudResponseMode);
                Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(),
                    trn.FraudFilterResponse.FraudResponseResult);
                
                trn = trn.Hold()
                    .WithReasonCode(reasonCode)
                    .Execute();

                Assert.IsNotNull(trn);
                Assert.AreEqual("SUCCESS", trn.ResponseCode);
                Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
                Assert.IsNotNull(trn.FraudFilterResponse);
                Assert.AreEqual(FraudFilterResult.HOLD_SUCCESSFUL.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);
            }
        }
        
        [TestMethod]
        public void Hold_WithIdempotencyKey() {
            var idempotencyKey = Guid.NewGuid().ToString();
            
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.ACTIVE)
                .WithIdempotencyKey(idempotencyKey)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.ACTIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);
            
            var exceptionCaught = false;
            try {
                trn.Hold()
                    .WithReasonCode(ReasonCode.FRAUD)
                    .WithIdempotencyKey(idempotencyKey)
                    .Execute();
            }
            catch (GatewayException ex) {
                exceptionCaught = true;
                Assert.AreEqual("DUPLICATE_ACTION", ex.ResponseCode);
                Assert.AreEqual("40039", ex.ResponseMessage);
                Assert.AreEqual($"Status Code: Conflict - Idempotency Key seen before: id={trn.TransactionId}",
                    ex.Message);
            } finally {
                Assert.IsTrue(exceptionCaught);
            }
        }
        
        [TestMethod]
        public void Hold_RandomTransaction() {
            var trn = new Transaction {
                TransactionId = Guid.NewGuid().ToString()
            };

            var errorFound = false;
            try {
                trn.Hold()
                    .WithReasonCode(ReasonCode.OTHER)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual($"Status Code: NotFound - Transaction {trn.TransactionId} not found at this location.", e.Message);
                Assert.AreEqual("40008", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }
        
        [TestMethod]
        public void Hold_InvalidReason() {
            var trn = card.Charge(98.10m)
                .WithCurrency(CURRENCY)
                .WithAddress(address)
                .WithFraudFilter(FraudFilterMode.PASSIVE)
                .Execute();

            Assert.IsNotNull(trn);
            Assert.AreEqual("SUCCESS", trn.ResponseCode);
            Assert.AreEqual(TransactionStatus.Captured.ToString().ToUpper(), trn.ResponseMessage);
            Assert.IsNotNull(trn.FraudFilterResponse);
            Assert.AreEqual(FraudFilterMode.PASSIVE.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseMode);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trn.FraudFilterResponse.FraudResponseResult);

            var errorFound = false;
            try {
                trn.Hold()
                    .WithReasonCode(ReasonCode.FALSEPOSITIVE)
                    .Execute();
            }
            catch (GatewayException e) {
                errorFound = true;
                Assert.AreEqual("Status Code: BadRequest - reason_code value is invalid. Please check the reason_code is entered correctly", e.Message);
                Assert.AreEqual("40259", e.ResponseMessage);
            } finally {
                Assert.IsTrue(errorFound);
            }
        }

        [TestMethod]
        public void GetTransactionWithFraudCheck()
        {
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now.AddDays(-3);
            
            var response = ReportingService.FindTransactionsPaged(1, 10)
                    .OrderBy(TransactionSortProperty.TimeCreated)
                    .Where(SearchCriteria.StartDate, startDate)
                    .And(SearchCriteria.EndDate, endDate)
                    .And(SearchCriteria.RiskAssessmentResult, FraudFilterResult.PASS)
                    .Execute();

            Assert.IsTrue(response.Results.Count > 0);
            
            var trnSummary = response.Results[new Random().Next(1, response.Results.Count)];
            Assert.IsNotNull(trnSummary.FraudManagementResponse);
            Assert.AreEqual(FraudFilterResult.PASS.ToString().ToUpper(), trnSummary.FraudManagementResponse.FraudResponseResult);
        }
    }
}
