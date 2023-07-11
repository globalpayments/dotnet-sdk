<%@ Page Title="Global Payments end-to-end with GP-API example" Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="end_to_end._Default" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <title>Global Payments end-to-end with GP-API example</title>
    <link rel="stylesheet" href="Content/styles.css" />
    <script src="https://js.globalpay.com/v1/globalpayments.js"></script>
    <script src="Scripts/globalpayments-3ds.js"></script>
    <script>
        let accessToken = '<%=accessToken%>';
    </script>
    <script defer src="Scripts/main.js"></script>
</head>
<body>
    <div class="container">
        <p>3DS test card with CHALLENGE_REQUIRED: 4012 0010 3848 8884</p> 
        <p>Frictionless card: 4263970000005262</p>
        <p>Amount: 100 EUR</p>
        <form id="payment-form" method="post">
            <!-- Target for the credit card form -->
            <div id="credit-card"></div>
        </form>
    </div>
</body>
</html>


