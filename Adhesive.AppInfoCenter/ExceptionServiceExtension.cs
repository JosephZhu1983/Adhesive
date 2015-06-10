/*****************************************************************************************************************                                                                                                               *
* Copyright (C) 2011 5173.com                                                                                   *
* This project may be copied only under the terms of the Apache License 2.0.                                    *
* Please visit the project Home Page http://adhesive.codeplex.com/ for more detail.                             *
*                                                                                                               *
****************************************************************************************************************/
using System;

namespace Adhesive.AppInfoCenter
{
    public static class ExceptionServiceExtension
    {
        public static void Handle(this Exception exception)
        {
            AppInfoCenterService.ExceptionService.Handle(exception);
        }

        public static void Handle(this Exception exception, string moduleName, string categoryName, string subcategoryName)
        {
            AppInfoCenterService.ExceptionService.Handle(moduleName, categoryName, subcategoryName, exception);
        }

        public static void Handle(this Exception exception, string description)
        {
            AppInfoCenterService.ExceptionService.Handle(exception, description);
        }

        public static void Handle(this Exception exception, string moduleName, string description)
        {
            AppInfoCenterService.ExceptionService.Handle(moduleName, exception, description);
        }

        public static void Handle(this Exception exception, string moduleName, string categoryName, string subcategoryName, string description)
        {
            AppInfoCenterService.ExceptionService.Handle(moduleName, categoryName, subcategoryName, exception, description);
        }


        public static void Handle(this Exception exception, ExtraInfo extraInfo)
        {
            AppInfoCenterService.ExceptionService.Handle(exception, extraInfo);
        }

        public static void Handle(this Exception exception, string moduleName, string categoryName, string subcategoryName, ExtraInfo extraInfo)
        {
            AppInfoCenterService.ExceptionService.Handle(moduleName, categoryName, subcategoryName, exception, extraInfo);
        }

        public static void Handle(this Exception exception, string description, ExtraInfo extraInfo)
        {
            AppInfoCenterService.ExceptionService.Handle(exception, description, extraInfo);
        }

        public static void Handle(this Exception exception, string moduleName, string description, ExtraInfo extraInfo)
        {
            AppInfoCenterService.ExceptionService.Handle(moduleName, exception, description, extraInfo);
        }

        public static void Handle(this Exception exception, string moduleName, string categoryName, string subcategoryName, string description, ExtraInfo extraInfo)
        {
            AppInfoCenterService.ExceptionService.Handle(moduleName, categoryName, subcategoryName, exception, description, extraInfo);
        }
    }
}
