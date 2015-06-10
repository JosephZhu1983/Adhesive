using System;
using System.Collections.Generic;
using System.ServiceModel;
using Adhesive.Common;

namespace Adhesive.Mongodb.Silverlight.Web
{
    [ServiceContract]
    [ServiceBehavior(Namespace = "Adhesive", InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    public partial class DataService
    {
        public static readonly DataService Instance = new DataService();
        private IMongodbQueryService service = MongodbService.MongodbQueryService;

        [OperationContract]
        public void Log(OperationLog log)
        {
            log.ID = Guid.NewGuid().ToString();
            log.ServerTime = DateTime.Now;
            MongodbService.MongodbInsertService.Insert(log);
        }

        [OperationContract]
        public List<Category> GetCategoryData()
        {
            return service.GetCategoryData();
        }

        [OperationContract]
        public Dictionary<MongodbServerUrl, ServerInfo> GetServerInfo()
        {
            return service.GetServerInfo();
        }

        [OperationContract]
        public DetailData GetDetailDataOnlyById(string databasePrefix, string Id)
        {
            return service.GetDetailDataOnlyById(databasePrefix, Id);
        }

        [OperationContract]
        public MongodbAdminConfigurationItem GetAdminConfiguration(string username, string password)
        {
            var r = service.GetAdminConfiguration(username, password);
            if (r!=null)
                r.IP = CommonConfiguration.MachineIP;
            return r;
        }

        [OperationContract]
        public FilterData GetFilterData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime)
        {
            return service.GetFilterData(databasePrefix, tableNames, beginTime, endTime);
        }

        [OperationContract]
        public List<TableData> GetTableData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, int pageIndex, int pageSize, Dictionary<string, object> filters)
        {
            return service.GetTableData(databasePrefix, tableNames, beginTime, endTime, pageIndex, pageSize, filters);
        }

        [OperationContract]
        public List<TableData> GetTableDataByContextId(string contextId)
        {
            return service.GetTableDataByContextId(contextId);
        }

        [OperationContract]
        public List<Statistics> GetStatisticsData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, TimeSpan span, Dictionary<string, object> filters)
        {
            return service.GetStatisticsData(databasePrefix, tableNames, beginTime, endTime, span, filters);
        }

        [OperationContract]
        public DetailData GetDetailData(string databasePrefix, string databaseName, string tableName, string pkcolumnName, string Id)
        {
            return service.GetDetailData(databasePrefix, databaseName, tableName, pkcolumnName, Id);
        }

        [OperationContract]
        public List<Group> GetGroupData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters)
        {
            return service.GetGroupData(databasePrefix, tableNames, beginTime, endTime, filters);
        }

        [OperationContract]
        public DetailData GetStateData(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters)
        {
            return service.GetStateData(databasePrefix, tableName, beginTime, endTime, filters);
        }
    }
}
