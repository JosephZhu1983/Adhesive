<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestMongodbService.aspx.cs" Inherits="Adhesive.Test.WebApp.TestMongodbService" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        分类:<asp:DropDownList ID="DropDownList1" runat="server" 
            AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" 
            AppendDataBoundItems="True">
            <asp:ListItem Selected="True" Value="-1">请选择</asp:ListItem>
        </asp:DropDownList>
        数据库:<asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" AppendDataBoundItems="True" OnSelectedIndexChanged="DropDownList2_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        表:<asp:RadioButtonList ID="RadioButtonList1" runat="server" RepeatDirection="Horizontal"
            RepeatLayout="Flow">
        </asp:RadioButtonList>
        <br />
        时间:最近<asp:TextBox ID="TextBox1" runat="server" Width="37px">3</asp:TextBox>天
        <asp:Button ID="Button1" runat="server" Text="Select" OnClick="Button1_Click" />
        <br />
        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <br />数据:
        <br />
        <asp:Literal ID="Literal2" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
