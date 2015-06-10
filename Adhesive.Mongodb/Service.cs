//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MongoDB.Driver;
//using System.Collections;
//using System.Reflection;
//using MongoDB.Bson;

//namespace Adhesive.Mongodb
//{
//    public class ColmnMeta
//    {
//        public string ColumnName { get; set; }

//        public bool Index { get; set; }
//    }

//    public class Service
//    {
//        public static void Add<T>(T item)
//        {
//            var server = MongoServer.Create("mongodb://192.168.129.173:20000");
//            var meta = server.GetDatabase("TableMeta");
//            var metacol = meta.GetCollection<ColmnMeta>(typeof(T).Name);
//            var metaList = new List<ColmnMeta>();
//            var database = server.GetDatabase("zhuye");
//            var col = database.GetCollection(typeof(T).Name);
//            col.RemoveAll();
//            var doc = new BsonDocument();
//            var data = new Dictionary<string, object>();
//            foreach (var property in typeof(T).GetProperties().ToList())
//            {
//                if (property.GetCustomAttributes(typeof(MongodbItemAttribute), true).Count() == 1)
//                {
//                    var attr = (MongodbItemAttribute)property.GetCustomAttributes(typeof(MongodbItemAttribute), true).Single();
//                    metaList.Add(new ColmnMeta
//                    {
//                        ColumnName = property.Name,
//                        Index = attr.IsIndex,
//                    });
//                }
//                InternalAdd(item, data, property);
//            }
//            doc.Add(data as IDictionary);
//            col.Insert(doc);
//            metacol.RemoveAll();
//            //metacol.Insert(metaList);
//        }

//        private static void InternalAdd<T>(T item, Dictionary<string, object> data, PropertyInfo pi)
//        {
//            if (pi.PropertyType.Assembly.GlobalAssemblyCache)
//            {
//                var val = pi.GetValue(item, null);
//                data.Add(pi.Name, val);
//            }
//            else
//            {
//                var subdata = new Dictionary<string, object>();
//                var subitem = pi.GetValue(item, null);
//                foreach (var property in subitem.GetType().GetProperties().ToList())
//                {
//                    InternalAdd(subitem, subdata, property);
//                }
//                data.Add(pi.Name, subdata);
//            }
//        }
//    }
//}
