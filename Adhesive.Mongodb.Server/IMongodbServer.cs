
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Adhesive.Mongodb.Server
{
    [ServiceContract(Namespace = "Adhesive.Mongodb")]
    public interface IMongodbServer : IDisposable
    {
        [OperationContract]
        void SubmitData(IList<MongodbData> dataList, MongodbDatabaseDescription databaseDescription);

        [OperationContract]
        MongodbAdminConfigurationItem GetAdminConfiguration(string username, string password);

        [OperationContract]
        MongodbAdminConfigurationItem GetAdminConfigurationInternal(string username);

        [OperationContract]
        Dictionary<MongodbServerUrl, ServerInfo> GetServerInfo();

        [OperationContract]
        List<Category> GetCategoryData();

        [OperationContract]
        int GetDataCount(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters);

        [OperationContract]
        FilterData GetFilterData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime);

        [OperationContract]
        List<TableData> GetTableData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, int pageIndex, int pageSize, Dictionary<string, object> filters);

        [OperationContract]
        List<TableData> GetTableDataByContextId(string contextId);

        [OperationContract]
        DetailData GetDetailData(string databasePrefix, string databaseName, string tableName, string pkcolumnName, string Id);

        [OperationContract]
        DetailData GetDetailDataOnlyById(string databasePrefix, string Id);

        [OperationContract]
        List<Statistics> GetStatisticsData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, TimeSpan span, Dictionary<string, object> filters);

        [OperationContract]
        List<Group> GetGroupData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters);

        [OperationContract]
        DetailData GetStateData(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters);
    }
}
