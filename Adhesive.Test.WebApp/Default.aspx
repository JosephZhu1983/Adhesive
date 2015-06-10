<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Adhesive.Test.WebApp._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .aa
        {
            font-family: 宋体, Arial, Helvetica, sans-serif;
            font-size: 10pt;
        }
    </style>
</head>
<body class="aa">
    <form id="form1" runat="server">
    <div>
        <strong>通用信息</strong><br />
        <br />
        <strong>模拟数据量</strong>
        <asp:TextBox ID="Count" runat="server" Width="102px">1</asp:TextBox>
        <br />
        模块名
        <asp:TextBox ID="ModuleName" runat="server"></asp:TextBox>
        &nbsp;
        大类
        <asp:TextBox ID="Category" runat="server"></asp:TextBox>
        &nbsp;小类
        <asp:TextBox ID="SubCategory" runat="server"></asp:TextBox>
        &nbsp;<br />
        多选过滤1
        <asp:TextBox ID="MFilter1" runat="server"></asp:TextBox>
        &nbsp;单选过滤1
        <asp:TextBox ID="SFilter1" runat="server"></asp:TextBox>
        &nbsp;文本过滤1
        <asp:TextBox ID="TFilter1" runat="server"></asp:TextBox>
        <br />
        多选过滤2
        <asp:TextBox ID="MFilter2" runat="server"></asp:TextBox>
        &nbsp;单选过滤2
        <asp:TextBox ID="SFilter2" runat="server"></asp:TextBox>
        &nbsp;文本过滤2
        <asp:TextBox ID="TFilter2" runat="server"></asp:TextBox>
        <br />
        <br />
        <strong>日志服务测试</strong><br />
        <br />
        消息
        <asp:TextBox ID="LogMessage" runat="server" Width="347px"></asp:TextBox>
        &nbsp;<asp:Button ID="Log" runat="server" OnClick="Log_Click" Text="提交日志" />
        <br />
        <br />
        <strong>异常服务测试</strong><br />
        <br />
        异常信息
        <asp:TextBox ID="ExceptionMessage" runat="server" Width="334px"></asp:TextBox>
        &nbsp;内部异常信息
        <asp:TextBox ID="InnerExceptionMessage" runat="server"></asp:TextBox>
        &nbsp;描述
        <asp:TextBox ID="ExceptionDescription" runat="server"></asp:TextBox>
        &nbsp;<asp:Button ID="HandleException" runat="server" OnClick="HandleException_Click"
            Text="处理异常" />
        <asp:Button ID="UnHandleException" runat="server" OnClick="UnHandleException_Click"
            Text="未处理异常" />
        <br />
        <br />
        <strong>性能服务测试</strong><br />
        <br />
        休眠时间 
        <asp:TextBox ID="SleepTime" runat="server" Width="70px">100</asp:TextBox>
        &nbsp;循环次数
        <asp:TextBox ID="CpuTime" runat="server" Width="70px">100000</asp:TextBox>
        &nbsp;<asp:Button ID="Performance" runat="server" OnClick="Performance_Click"
            Text="测试性能" />
        <br />
        <br />
        <strong>状态服务测试</strong><br />
        <br />
        状态汇报值 
        <asp:TextBox ID="StateValue" runat="server" Width="70px">101</asp:TextBox>
        &nbsp;<asp:Button ID="UpdateState" runat="server" OnClick="UpdateState_Click"
            Text="修改状态汇报值" />
        <br />
        <br />
        <strong>自定义实体测试<br />
        </strong>
        <br />
        部门
        <asp:DropDownList ID="DeptName" runat="server">
            <asp:ListItem>Tech</asp:ListItem>
            <asp:ListItem>Sales</asp:ListItem>
            <asp:ListItem>Market</asp:ListItem>
        </asp:DropDownList>
        &nbsp;状态
        <asp:DropDownList ID="Status" runat="server">
            <asp:ListItem>借出</asp:ListItem>
            <asp:ListItem>归还</asp:ListItem>
            <asp:ListItem>丢失</asp:ListItem>
        </asp:DropDownList>
        &nbsp;分类
        <asp:TextBox ID="BookCategory" runat="server">技术书</asp:TextBox>
        &nbsp;书名
        <asp:TextBox ID="BookName" runat="server" Width="247px"></asp:TextBox>
        &nbsp;借书者
        <asp:TextBox ID="UserName" runat="server">朱晔</asp:TextBox>
        &nbsp;备注
        <asp:TextBox ID="Memo" runat="server">是一本好书</asp:TextBox>
        &nbsp;
        <asp:Button ID="SubmitData" runat="server" Text="写入数据" OnClick="SubmitData_Click" />
        <br />
        <br />
        <strong>服务调用测试 </strong>
        <br />
        <br />
        参数
        <asp:TextBox ID="FuckCount" runat="server" Width="39px">1</asp:TextBox>
        &nbsp;<asp:CheckBox ID="Safe" runat="server" Text="安全方式" />
        &nbsp;<asp:Button ID="InvokeService" runat="server" OnClick="InvokeService_Click"
            Text="调用服务" />
        <br />
        <br />
        <asp:Button ID="AllTest" runat="server" OnClick="AllTest_Click" Text="测试所有" />
        &nbsp;<br />
        <br />
        <asp:Label ID="Result" runat="server" ForeColor="Red" Style="font-weight: 700"></asp:Label>
        <br />
    </div>
    </form>
</body>
</html>
