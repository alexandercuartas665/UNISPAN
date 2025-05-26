<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RptSalesInvoiceTrackings.aspx.cs" Inherits="adesoft.adeposx.report.RptSalesInvoiceTrackings" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="height:calc(100vh - 20px)">
            <asp:ScriptManager runat="server"></asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Height="100%" Width="100%" >
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
