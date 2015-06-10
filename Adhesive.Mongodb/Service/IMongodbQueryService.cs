
using System;
using System.Collections.Generic;

namespace Adhesive.Mongodb
{
    public interface IMongodbQueryService : IDisposable
    {
        /// <summary>
        /// 获取类别数据
        /// </summary>
        /// <returns></returns>
        List<Category> GetCategoryData();

        /// <summary>
        /// 获取管理信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        MongodbAdminConfigurationItem GetAdminConfiguration(string username, string password);

        /// <summary>
        /// 报警服务获取管理信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        MongodbAdminConfigurationItem GetAdminConfigurationInternal(string username);

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <returns></returns>
        Dictionary<MongodbServerUrl, ServerInfo> GetServerInfo();

        /// <summary>
        /// 获取数据量
        /// </summary>
        /// <param name="databasePrefix"></param>
        /// <param name="tableName"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        int GetDataCount(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters);

        /// <summary>
        /// 获取过滤信息
        /// </summary>
        /// <param name="databasePrefix"></param>
        /// <param name="tableName"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        FilterData GetFilterData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime);

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="databasePrefix"></param>
        /// <param name="tableName"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<TableData> GetTableData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, int pageIndex, int pageSize, Dictionary<string, object> filters);

        /// <summary>
        /// 根据上下文Id或者所有类型的列表数据
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="contextId"></param>
        /// <returns></returns>
        List<TableData> GetTableDataByContextId(string contextId);

        /// <summary>
        /// 获取单条记录
        /// </summary>
        /// <param name="databasePrefix"></param>
        /// <param name="databaseName"></param>
        /// <param name="tableName"></param>
        /// <param name="pkcolumnName"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        DetailData GetDetailData(string databasePrefix, string databaseName, string tableName, string pkcolumnName, string Id);

        /// <summary>
        /// 直接根据Id获取单条记录
        /// </summary>
        /// <param name="databasePrefix"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        DetailData GetDetailDataOnlyById(string databasePrefix, string Id);

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <param name="databasePrefix"></param>
        /// <param name="tableNames"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="span"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<Statistics> GetStatisticsData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, TimeSpan span, Dictionary<string, object> filters);

        /// <summary>
        /// 获取分组统计数据
        /// </summary>
        /// <param name="databasePrefix"></param>
        /// <param name="tableNames"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<Group> GetGroupData(string databasePrefix, List<string> tableNames, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters);

        /// <summary>
        /// 获取状态数据
        /// </summary>
        /// <param name="databasePrefix"></param>
        /// <param name="tableName"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        DetailData GetStateData(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters);
    }
}
