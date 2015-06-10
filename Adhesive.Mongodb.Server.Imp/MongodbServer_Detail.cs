
using System;
using System.Collections.Generic;
using System.Linq;
using Adhesive.Common;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Adhesive.Mongodb.Server.Imp
{
    public partial class MongodbServer : AbstractService, IMongodbServer
    {
        #region Detail

        public DetailData GetDetailDataOnlyById(string databasePrefix, string Id)
        {
            try
            {
                var databaseInfos = MongodbServerMaintainceCenter.GetDatabaseInfos(databasePrefix);
                var columnDescriptions = MongodbServerMaintainceCenter.GetMongodbColumnDescriptions(databasePrefix);
                var pkColumn = columnDescriptions.FirstOrDefault(desc => desc.IsPrimaryKey);
                if (pkColumn == null) return null;

                foreach (var databaseInfo in databaseInfos)
                {
                    foreach (var collectionInfo in databaseInfo.Collections)
                    {
                        var data = GetDetailData(databasePrefix, databaseInfo.DatabaseName, collectionInfo.CollectionName, pkColumn.ColumnName, Id);
                        if (data != null)
                        {
                            return data;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Detail", "GetDetailDataOnlyById", ex.Message);
                throw;
            }
        }

        public DetailData GetDetailData(string databasePrefix, string databaseName, string tableName, string pkColumnName, string Id)
        {
            try
            {
                var detailData = new DetailData()
                {
                    DatabasePrefix = databasePrefix,
                    DatabaseName = databaseName,
                    PkColumnName = pkColumnName,
                    TableName = tableName,
                };
                var data = new List<Detail>();
                var typeFullName = MongodbServerMaintainceCenter.GetTypeFullName(databasePrefix);
                var server = CreateSlaveMongoServer(typeFullName);
                var database = server.GetDatabase(databaseName);
                var collection = database.GetCollection(tableName);
                var row = collection.FindOne(Query.EQ(pkColumnName, Id));
                if (row != null)
                {
                    var columnDescriptions = MongodbServerMaintainceCenter.GetMongodbColumnDescriptions(databasePrefix);
                    var enumColumnDescriptions = MongodbServerMaintainceCenter.GetMongodbEnumColumnDescriptions(databasePrefix);

                    foreach (var col in row)
                        InternalGetDetailData(string.Empty, data, col, columnDescriptions, enumColumnDescriptions, pkColumnName);
                    detailData.Data = data;
                    return detailData;
                }
                return null;
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", MongodbServerConfiguration.ModuleName, "MongodbServer_Detail", "GetDetailData", ex.Message);
                throw;
            }
        }

        private void InternalGetDetailData(string prefix, List<Detail> data, BsonElement element, List<MongodbColumnDescription> descriptions, List<MongodbEnumColumnDescription> enumDescriptions, string pkColumnName)
        {

            if (element.Value.IsObjectId || element.Value.IsBsonNull) return;

            var columnName = string.IsNullOrEmpty(prefix) ? element.Name : string.Format("{0}.{1}", prefix, element.Name);
            var entityColumnName = columnName;
            if (columnName.Contains("__") && columnName.Contains(".") && columnName.Split('.').Length > 2)
            {
                var parts = columnName.Split('.');
                entityColumnName = "";
                foreach (var part in parts)
                {
                    if (part.IndexOf("__") >= 0)
                        entityColumnName += part.Substring(0, part.IndexOf("__"));
                    else
                        entityColumnName += part;
                    entityColumnName += ".";
                }
                entityColumnName = entityColumnName.TrimEnd('.');
            }
            try
            {

                var description = descriptions.FirstOrDefault(d => d.ColumnName == entityColumnName) 
                    ?? new MongodbColumnDescription
                    {
                        Description = "",
                        DisplayName = element.Name,
                        ColumnName = columnName,
                    };

                if (element.Value.IsBsonDocument)
                {
                    var subData = new List<Detail>();
                    foreach (var subDocument in element.Value.AsBsonDocument.Elements)
                    {
                        InternalGetDetailData(columnName, subData, subDocument, descriptions, enumDescriptions, pkColumnName);
                    }
                    data.Add(new Detail
                    {
                        Description = description.Description,
                        DisplayName = description.DisplayName.Replace('.', '_'),
                        ColumnName = columnName,
                        Value = "",
                        SubDetails = subData,
                    });
                }
                else
                {
                    string value = "";

                    if (element.Value.IsBsonDateTime)
                    {
                        value = element.Value.AsDateTime.ToLocalTime().ToString();
                    }
                    else
                    {
                        var enumColumnDescription = enumDescriptions.FirstOrDefault(e => e.Name == columnName);
                        if (enumColumnDescription == null)
                        {
                            value = element.Value.RawValue.ToString();
                        }
                        else
                        {
                            var enumValue = 0;
                            if (int.TryParse(element.Value.RawValue.ToString(), out enumValue) && enumColumnDescription.EnumItems.ContainsKey(enumValue.ToString()))
                            {
                                value = enumColumnDescription.EnumItems[enumValue.ToString()];
                            }
                            else
                            {
                                value = element.Value.RawValue.ToString();
                            }
                        }
                    }

                    var detail = new Detail
                    {
                        Description = description.Description,
                        DisplayName = description.DisplayName.Replace('.', '_'),
                        ColumnName = columnName,
                        Value = value,
                    };
                    data.Add(detail);
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3} {4}", MongodbServerConfiguration.ModuleName, "MongodbServer_Detail", "InternalGetDetailData",
                    string.Format("参数：{0}", entityColumnName), ex.Message);
                throw;
            }
        }

        #endregion
    }
}
