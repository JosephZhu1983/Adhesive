
using System;
using System.Collections.Generic;
using System.Linq;
using Adhesive.Common;

namespace Adhesive.Mongodb.Server.Imp
{
    public partial class MongodbServer : AbstractService, IMongodbServer
    {
        #region State

        public DetailData GetStateData(string databasePrefix, string tableName, DateTime beginTime, DateTime endTime, Dictionary<string, object> filters)
        {
            try
            {
                var columnDescriptions = MongodbServerMaintainceCenter.GetMongodbColumnDescriptions(databasePrefix);
                var pkColumn = columnDescriptions.FirstOrDefault(desc => desc.IsPrimaryKey);
                if (pkColumn != null)
                {
                    var pkColumnName = pkColumn.DisplayName;
                    var table = GetTableData(databasePrefix, new List<string> { tableName }, beginTime, endTime, 0, 1, filters);
                    if (table == null || table.Count == 0) return null;
                    var tab = table.First();
                    if (tab.Tables == null || tab.Tables.Count == 0) return null;
                    var t = tab.Tables.First();
                    if (t.Data == null || t.Data.Count == 0) return null;
                    var row = t.Data.First();
                    var id = "";
                    if (row.ContainsKey(pkColumnName))
                        id = row[pkColumnName];
                    if (!string.IsNullOrEmpty(id))
                    {
                        var detail = GetDetailData(databasePrefix, t.DatabaseName, tab.TableName, tab.PkColumnName, id);
                        return detail;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_State", "GetStateData", ex.Message);
                throw;
            }
        }
        #endregion
    }
}
