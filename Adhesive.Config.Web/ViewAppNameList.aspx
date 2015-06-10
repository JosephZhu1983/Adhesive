<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewAppNameList.aspx.cs" Inherits="Adhesive.Config.Web.ViewAppNameList" %>

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
					<td colSpan="7"><FONT face="宋体"><span id="lbList" class="black-14"></span><span id="Label1" class="black-14">应用程序列表</span></FONT></td>
				</tr>

    <tr>
    <td colSpan="7" style="height: 35px">
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span id="lblEntryId" class="label">应用程序名称</span>&nbsp;&nbsp;
        <asp:DropDownList ID="ddlTolLevelConfigItems" runat="server">
        </asp:DropDownList>
        &nbsp;&nbsp;

        <asp:Button ID="btnViewConfigItems" runat="server" Text="查询" style="height:25px;width:52px;BACKGROUND-IMAGE: url(../Images/button-2.gif); BORDER-TOP-STYLE: none; BACKGROUND-REPEAT: no-repeat; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none"
                onclick="btnViewConfigItems_Click" />
        </td>
    </tr>
    </table>
    </form>
</body>
</html>
