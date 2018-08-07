<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Response.aspx.cs" Inherits="end_to_end.Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="http://maxcdn.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css" rel="stylesheet">
    <link href="css/response.css" rel="stylesheet" />
</head>
<body>
    <div class="container">
        <table>
            <tr>
                <td>
                    <h1 id="header" runat="server"></h1>
                    <asp:Label ID="lblStatus" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <a href="Default.aspx" class="btn btn-lg">Do Another Transaction</a>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
