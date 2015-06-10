
using System.Collections.Generic;
using Adhesive.Mongodb;

namespace Adhesive.AppInfoCenter.Imp
{
    public class MvcContextInfo
    {
        [MongodbPersistenceItem(ColumnName = "RD")]
        [MongodbPresentationItem(DisplayName = "路由信息")]
        public Dictionary<string, string> RouteData { get; set; }

        [MongodbPersistenceItem(ColumnName = "TD")]
        [MongodbPresentationItem(DisplayName = "临时数据")]
        public Dictionary<string, string> TempData { get; set; }

        [MongodbPersistenceItem(ColumnName = "VD")]
        [MongodbPresentationItem(DisplayName = "视图数据")]
        public Dictionary<string, string> ViewData { get; set; }

        [MongodbPersistenceItem(ColumnName = "PD")]
        [MongodbPresentationItem(DisplayName = "参数数据")]
        public Dictionary<string, string> ParameterData { get; set; }

        [MongodbPersistenceItem(ColumnName = "ART")]
        [MongodbPresentationItem(DisplayName = "输出类型")]
        public string ActionResultType { get; set; }

        [MongodbPersistenceItem(ColumnName = "CN")]
        [MongodbPresentationItem(DisplayName = "控制器名")]
        public string ControllerName { get; set; }

        [MongodbPersistenceItem(ColumnName = "AN")]
        [MongodbPresentationItem(DisplayName = "行为名")]
        public string ActionName { get; set; }

        [MongodbPersistenceItem(ColumnName = "CA")]
        [MongodbPresentationItem(DisplayName = "是否是子行为")]
        public bool? IsChildAction { get; set; }

        [MongodbPersistenceItem(ColumnName = "ST")]
        [MongodbPresentationItem(DisplayName = "执行阶段")]
        public MvcActionStage State { get; set; }
    }
}
