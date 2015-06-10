﻿
﻿using System;
﻿using System.Data.Entity.Infrastructure;
﻿using Adhesive.Common;
using Microsoft.Practices.Unity;
using System.Linq;
namespace Adhesive.Persistence.Imp
{
    public class InitServiceTask : InitServiceBootstrapperTask
    {
        public InitServiceTask(IUnityContainer container)
            : base(container)
        {
        }
        public override TaskContinuation Execute()
        {
            LocalLoggingService.Info("开始初始化持久化服务");
            try
            {
                BuildManagerWrapper.Current.PublicTypes.Where(e => e != null && e.IsSubclassOf(typeof(StorageContext)) && !e.IsAbstract)
                                                       .Each(e =>
                                                                 {
                                                                     using (StorageContext context = LocalServiceLocator.GetService<IDbContextFactory>().CreateContext(e))
                                                                     {

                                                                         context.Database.CreateIfNotExists();
                                                                         var script = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
                                                                         if (!string.IsNullOrEmpty(script))
                                                                         {
                                                                             try
                                                                             {
                                                                                 context.Database.ExecuteSqlCommand(script);
                                                                             }
                                                                             catch (Exception ex)
                                                                             {
                                                                                 LocalLoggingService.Warning(ex.ToString());
                                                                             }
                                                                         }
                                                                         //Console.WriteLine(script);
                                                                     }

                                                                 });
                LocalLoggingService.Info("初始化持久化服务完成");

            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("开始初始化持久化服务失败，异常信息：{0}", ex);
                return TaskContinuation.Break;
            }
            return TaskContinuation.Continue;
        }
    }
}
