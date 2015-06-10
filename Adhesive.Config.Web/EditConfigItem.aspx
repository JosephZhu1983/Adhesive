<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditConfigItem.aspx.cs" Inherits="Adhesive.Config.Web.EditConfigItem" ValidateRequest="false"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="Stylesheet" type="text/css" href="/css/MyCss.css" />
</head>
<body>
    <form id="form1" runat="server">
  <TABLE class="gray-12" id="Table1" style="BORDER-COLLAPSE: collapse" borderColor="#1c517b"
				cellSpacing="0" cellPadding="0" width="100%" align="left" border="1">
                <TR class="black-12" bgColor="#c7ddf8">
					<TD colSpan="2" height="22"><span id="labWorkName" class="black-14">参数项</span></TD>
				</TR>
  <tr><td colspan="2">
      <asp:Button ID="btnSave" runat="server" onclick="btnSave_Click" Text="确定" style="font-size:9pt;height:25px;width:52px;BACKGROUND-IMAGE: url(../Images/button-2.gif); BORDER-TOP-STYLE: none; BACKGROUND-REPEAT: no-repeat; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none"/>
      &nbsp;&nbsp;
      <asp:Button ID="btnCancel" runat="server" Text="取消" style="font-size:9pt;height:25px;width:52px;BACKGROUND-IMAGE: url(../Images/button-2.gif); BORDER-TOP-STYLE: none; BACKGROUND-REPEAT: no-repeat; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" 
          onclick="btnCancel_Click"/>
      &nbsp;&nbsp;
      <asp:Button ID="btnDelete" runat="server" Text="删除" style="font-size:9pt;height:25px;width:52px;BACKGROUND-IMAGE: url(../Images/button-2.gif); BORDER-TOP-STYLE: none; BACKGROUND-REPEAT: no-repeat; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none" onclick="btnDelete_Click"  OnClientClick="return confirm('确认要删除此配置项吗？删除后不可恢复。');"/>
      &nbsp;</td>
</tr>
  <tr>
  <td style="WIDTH: 90px; HEIGHT: 19px" align="right"><FONT face="宋体"><span id="Label5" class="label">名称</span></FONT></td>

  <td style="HEIGHT: 19px">
      <asp:TextBox ID="txtName" runat="server" Width="720px"></asp:TextBox><label style="color:Red">*</label></td></tr>
  <tr>
  <td style="WIDTH: 90px; HEIGHT: 19px" align="right"><FONT face="宋体"><span id="Label6" class="label">友好名称</span></FONT></td>
  <td style="HEIGHT: 19px">
  <asp:TextBox ID="txtFriendlyName" runat="server" Width="720px"></asp:TextBox>
  </td></tr>
    <tr>
    <td style="WIDTH: 90px; HEIGHT: 19px" align="right"><FONT face="宋体"><span id="Span1" class="label">描述</span></FONT></td>
  <td style="HEIGHT: 19px"><asp:TextBox ID="txtDesc" runat="server" Width="720px" TextMode="MultiLine"></asp:TextBox></td></tr>
  <tr>
  <td style="WIDTH: 90px; HEIGHT: 19px" align="right"><FONT face="宋体"><span id="Span2" class="label">类型</span></FONT></td>
  <td style="HEIGHT: 19px;"><asp:DropDownList ID="ddlAssembly" runat="server"
          Width="226px" AutoPostBack="True" 
          onselectedindexchanged="ddlAssembly_SelectedIndexChanged" ToolTip="根据程序集过滤"></asp:DropDownList><asp:DropDownList ID="ddlSource" runat="server"
          Width="500px" AutoPostBack="True" 
          onselectedindexchanged="ddlSource_SelectedIndexChanged"></asp:DropDownList></td></tr>

  <tr><td style="WIDTH: 90px; HEIGHT: 19px" align="right"><FONT face="宋体"><span id="Label2" class="label">值</span></FONT></td>
  <td style="HEIGHT: 19px"><asp:TextBox ID="txtValue" runat="server" Rows="30"  Columns="40" TextMode="MultiLine"  Height="480px" Width="720px" ></asp:TextBox>
  <asp:DropDownList ID="ddlSourceItem" runat="server" Width="726px" Visible="false"></asp:DropDownList>
  <asp:CheckBoxList ID="chklSourceItem" runat="server" Width="726px"  SelectionMode ="Multiple"  RepeatLayout="Flow" RepeatDirection="Vertical" Visible="false"></asp:CheckBoxList>
  </td></tr>
  
  </table>
    </form>
</body>
</html>
