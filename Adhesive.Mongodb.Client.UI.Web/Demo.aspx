<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Demo.aspx.cs" Inherits="Adhesive.Mongodb.Client.UI.Web.Demo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:DropDownList ID="Category" runat="server" AutoPostBack="True" 
            onselectedindexchanged="Category_SelectedIndexChanged">
        </asp:DropDownList>
        <asp:DropDownList ID="Database" runat="server" AutoPostBack="True" 
            onselectedindexchanged="Database_SelectedIndexChanged">
        </asp:DropDownList>
    
        <asp:TextBox ID="StartTime" runat="server"></asp:TextBox>
        <asp:TextBox ID="EndTime" runat="server"></asp:TextBox>
        <asp:DropDownList ID="TableNames" runat="server" AutoPostBack="True" 
            onselectedindexchanged="TableNames_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <br />
        <asp:PlaceHolder ID="PlaceHolder2" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
        <br />
        <br />
        <asp:Button ID="Query" runat="server" onclick="Query_Click" Text="查询表数据" />
        <br />
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
    
    </div>
    </form>
</body>
</html>
