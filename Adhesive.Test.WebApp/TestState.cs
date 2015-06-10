using Adhesive.AppInfoCenter;
using Adhesive.Mongodb;

namespace Adhesive.Test.WebApp
{
    [MongodbPersistenceEntity("Test", DisplayName = "测试状态", ExpireDays = 1)]
    public class TestState : BaseInfo
    {
        [MongodbPresentationItem(DisplayName = "测试状态值", ShowInTableView = true)]
        public int StateValue { get; set; }
    }
}