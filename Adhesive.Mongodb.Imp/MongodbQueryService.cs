
using System;
using System.Collections.Generic;
using Adhesive.Common;
//using Adhesive.DistributedService;
using Adhesive.Mongodb.Server;

namespace Adhesive.Mongodb.Imp
{
    public class MongodbQueryService : AbstractService, IMongodbQueryService
    {

        private static IMongodbServer service = MongodbServiceLocator.GetService();

        public Dictionary<MongodbServerUrl, ServerInfo> GetServerInfo()
        {
            return service.GetServerInfo();
        }

        public MongodbAdminConfigurationItem GetAdminConfiguration(string username, string password)
        {
            return service.GetAdminConfiguration(username, password);
        }

        public MongodbAdminConfigurationItem GetAdminConfigurationInternal(string username)
        {
            return service.GetAdminConfigurationInternal(username);
        }

        public List<Category> GetCategoryData()
        {
            return service.GetCategoryData();
        }

        public int GetDataCount(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters)
        {
            return service.GetDataCount(databasePrefix, tableName, beginTime, endTime, filters);
        }

        public FilterData GetFilterData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime)
        {
            return service.GetFilterData(databasePrefix, tableNames, beginTime, endTime);
        }

        public List<TableData> GetTableData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, int pageIndex, int pageSize, Dictionary<string, object> filters)
        {
            return service.GetTableData(databasePrefix, tableNames, beginTime, endTime, pageIndex, pageSize, filters);
        }

        public List<TableData> GetTableDataByContextId(string contextId)
        {
            return service.GetTableDataByContextId(contextId);
        }

        public DetailData GetDetailData(string databasePrefix, string databaseName, string tableName, string pkcolumnName, string Id)
        {
            return service.GetDetailData(databasePrefix, databaseName, tableName, pkcolumnName, Id);
        }

        public DetailData GetDetailDataOnlyById(string databasePrefix, string Id)
        {
            return service.GetDetailDataOnlyById(databasePrefix, Id);
        }

        public List<Statistics> GetStatisticsData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, TimeSpan span, Dictionary<string, object> filters)
        {
            return service.GetStatisticsData(databasePrefix, tableNames, beginTime, endTime, span, filters);
        }

        public List<Group> GetGroupData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters)
        {
            return service.GetGroupData(databasePrefix, tableNames, beginTime, endTime, filters);
        }

        public DetailData GetStateData(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters)
        {
            return service.GetStateData(databasePrefix, tableName, beginTime, endTime, filters);
        }
    }
}
