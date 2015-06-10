<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewTopLevelConfigItemList.aspx.cs" Inherits="Adhesive.Config.Web.ViewTopLevelConfigItemList" %>
<%@ Import Namespace="Adhesive.Config" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="../css/MyCss.css" />
</head>
<body>
    <form id="form1" runat="server">
    <TABLE id="Table1" style="BORDER-COLLAPSE: collapse" borderColor="#1c517b" cellSpacing="0"
				cellPadding="0" width="100%" align="center" border="1">
     <tr bgColor="#c7ddf8" height="22">
					<td colSpan="7"><FONT face="宋体"><asp:Label ID="lblLocation" runat="server" Text="Label" CssClass="black-14"></asp:Label></FONT></td>
				</tr>
     <tr>
    <td colSpan="7" style="height: 35px">
    <asp:Button runat="server" Text="上一级"  ID="btnGotoParent" style="height:25px;width:65px;BACKGROUND-IMAGE: url(../Images/button-3.gif); BORDER-TOP-STYLE: none; BACKGROUND-REPEAT: no-repeat; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none"
        onclick="btnGotoParent_Click"/>
        &nbsp;
<asp:Button ID="btnAdd" runat="server" Text="增加" onclick="btnAdd_Click" style="height:25px;width:52px;BACKGROUND-IMAGE: url(../Images/button-2.gif); BORDER-TOP-STYLE: none; BACKGROUND-REPEAT: no-repeat; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none"/>
        
    
    </td>
    </tr>
    <tr>
    <td colspan="7" >
    <asp:DataGrid ID="dgTopLevelConfigItems" runat="server" AutoGenerateColumns="False"
            HorizontalAlign="Left" Width="85%"   CellPadding = "3"
            onitemdatabound="dgTopLevelConfigItems_ItemDataBound" >
        <Columns>
        <asp:TemplateColumn>
            <ItemTemplate>
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#string.Format("ViewConfigItemList.aspx?appName={0}&parentPathItemNames={1}&prevParentId={2}&parentId={3}",Eval("AppName"),Eval("Name"),Eval("ParentId"),Eval("Id")) %>' Text="下一级" ></asp:HyperLink>
            </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" CssClass="gray-12" Wrap="false"/>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="名称">
            <ItemTemplate>
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl='<%#string.Format("EditConfigItem.aspx?appName={0}&parentPathItemNames={1}&id={2}&parentId={3}&type=edit",Eval("AppName"),Eval("Name"),Eval("Id"),Eval("ParentId")) %>' Text='<%#Eval("Name") %>' ToolTip="点击链接进行编辑操作"></asp:HyperLink>
            </ItemTemplate>
                <ItemStyle  CssClass="gray-12" Wrap="false"/>
            </asp:TemplateColumn>
            <asp:TemplateColumn HeaderText="类型">
            <ItemTemplate>
            <label><%#ConfigHelper.GetValueTypeEnumFriendlyName(Eval("ValueTypeEnum") as string)%></label>
            </ItemTemplate>
            <ItemStyle Wrap="false"/>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="FriendlyName" HeaderText="友好名称" ItemStyle-Wrap="false">
            </asp:BoundColumn>
            <asp:BoundColumn DataField="Description" HeaderText="描述">
            </asp:BoundColumn>
            <asp:TemplateColumn HeaderText="值">
            <ItemTemplate>
            <label><%#HttpUtility.HtmlEncode(Eval("Value")) %></label>
            </ItemTemplate>
            </asp:TemplateColumn>
            <asp:BoundColumn DataField="Id" HeaderText="Id">
            </asp:BoundColumn>
        </Columns>
     
        <HeaderStyle CssClass="black-12" />
    </asp:DataGrid>
    </td>
    </tr>
    </table>
   
	
   

    
    
    </form>
</body>
</html>
