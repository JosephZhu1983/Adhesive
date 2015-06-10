using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Adhesive.AppInfoCenter;
using System.Web.Compilation;
using System.Reflection;
using System.Diagnostics;
using Adhesive.Mongodb;
using System.Threading;

namespace Adhesive.Test.WebApp
{
    public partial class _Default : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var msg = Guid.NewGuid().ToString().Substring(0, 5) + " " + DateTime.Now.ToString();
                LogMessage.Text = "日志" + msg;
                ExceptionMessage.Text = "异常" + msg;
                InnerExceptionMessage.Text ="内部异常" +  msg;
                ExceptionDescription.Text = "描述" + msg;
                BookName.Text = "书名" + msg;
            }           
        }

        protected void Log_Click(object sender, EventArgs e)
        {
            var extra = new ExtraInfo
            {
                CheckBoxListFilterItem1 = MFilter1.Text,
                CheckBoxListFilterItem2 = MFilter2.Text,
                DropDownListFilterItem1 = SFilter1.Text,
                DropDownListFilterItem2 = SFilter2.Text,
                TextBoxFilterItem1 = TFilter1.Text,
                TextBoxFilterItem2 = TFilter2.Text,
            };
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < int.Parse(Count.Text); i++)
            {
                AppInfoCenterService.LoggingService.Debug(ModuleName.Text, Category.Text, SubCategory.Text, "Debug" + LogMessage.Text, extra);
                AppInfoCenterService.LoggingService.Info(ModuleName.Text, Category.Text, SubCategory.Text, "Info" + LogMessage.Text, extra);
                AppInfoCenterService.LoggingService.Warning(ModuleName.Text, Category.Text, SubCategory.Text, "Warning" + LogMessage.Text, extra);
                AppInfoCenterService.LoggingService.Error(ModuleName.Text, Category.Text, SubCategory.Text, "Error" + LogMessage.Text, extra);
            }
            Result.Text = sw.ElapsedMilliseconds.ToString();
        }

        protected void HandleException_Click(object sender, EventArgs e)
        {
            var extra = new ExtraInfo
            {
                CheckBoxListFilterItem1 = MFilter1.Text,
                CheckBoxListFilterItem2 = MFilter2.Text,
                DropDownListFilterItem1 = SFilter1.Text,
                DropDownListFilterItem2 = SFilter2.Text,
                TextBoxFilterItem1 = TFilter1.Text,
                TextBoxFilterItem2 = TFilter2.Text,
            };
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < int.Parse(Count.Text); i++)
            {
                if (string.IsNullOrEmpty(InnerExceptionMessage.Text))
                    new Exception(ExceptionMessage.Text).Handle(ModuleName.Text, Category.Text, SubCategory.Text, ExceptionDescription.Text, extra);
                else
                    new Exception(ExceptionMessage.Text, new NullReferenceException(InnerExceptionMessage.Text)).Handle(ModuleName.Text, Category.Text, SubCategory.Text, ExceptionDescription.Text, extra);
            }
            Result.Text = sw.ElapsedMilliseconds.ToString();
        }

        protected void AllTest_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            Log_Click(this, null);
            HandleException_Click(this, null);
            Performance_Click(this, null);
            InvokeService_Click(this, null);
            SubmitData_Click(this, null);
            Result.Text = sw.ElapsedMilliseconds.ToString();
        }

        protected void InvokeService_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();


            Result.Text += sw.ElapsedMilliseconds.ToString();
        }

        protected void SubmitData_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < int.Parse(Count.Text); i++)
            {
                var book = new Book()
                {
                    DeptName = DeptName.SelectedValue,
                    ID = Guid.NewGuid().ToString(),
                    Memo = Memo.Text,
                    UserName = UserName.Text,
                    ServerTime = DateTime.Now,
                    Name = BookName.Text,
                    Status = (Status)Enum.Parse(typeof(Status), Status.SelectedValue),
                    Category = BookCategory.Text
                };
                MongodbService.MongodbInsertService.Insert(book);
            }
            Result.Text = sw.ElapsedMilliseconds.ToString();
        }

        protected void Performance_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            AppInfoCenterService.PerformanceService.StartPerformanceMeasure("性能测试1");
            for (int i = 0; i < 5; i++)
            {
                MethodA();
                MethodB();
            }
            Result.Text = sw.ElapsedMilliseconds.ToString();
        }

        private void MethodA()
        {
            Thread.Sleep(int.Parse(SleepTime.Text));
            AppInfoCenterService.PerformanceService.SetPerformanceMeasurePoint("性能测试1", "MethodA");
        }

        private void MethodB()
        {
            for (int i = 0; i < int.Parse(CpuTime.Text); i++)
            {
                var d = 10000000;
                Math.Cos(d);
                Math.Sin(d);
                Math.Log10(d);
                Math.Sqrt(d);
            }
            AppInfoCenterService.PerformanceService.SetPerformanceMeasurePoint("性能测试1", "MethodB");
        }

        protected void UpdateState_Click(object sender, EventArgs e)
        {
            //Global.stateValue = int.Parse(StateValue.Text);
        }

        protected void UnHandleException_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InnerExceptionMessage.Text))
                throw new Exception(ExceptionMessage.Text);
            else
                throw new Exception(ExceptionMessage.Text, new NullReferenceException(InnerExceptionMessage.Text));
        }
    }
}