
using System;
namespace Adhesive.AppInfoCenter
{
    public interface ILoggingService : IDisposable
    {
        void Debug(string message);

        void Debug(string moduleName, string message);

        void Debug(string categoryName, string subcategoryName, string message);

        void Debug(string moduleName, string categoryName, string subcategoryName, string message);

        void Debug(string message, ExtraInfo extraInfo);

        void Debug(string moduleName, string message, ExtraInfo extraInfo);

        void Debug(string categoryName, string subcategoryName, string message, ExtraInfo extraInfo);

        void Debug(string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo);


        void Info(string message);

        void Info(string moduleName, string message);

        void Info(string categoryName, string subcategoryName, string message);

        void Info(string moduleName, string categoryName, string subcategoryName, string message);

        void Info(string message, ExtraInfo extraInfo);

        void Info(string moduleName, string message, ExtraInfo extraInfo);

        void Info(string categoryName, string subcategoryName, string message, ExtraInfo extraInfo);

        void Info(string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo);


        void Warning(string message);

        void Warning(string moduleName, string message);

        void Warning(string categoryName, string subcategoryName, string message);

        void Warning(string moduleName, string categoryName, string subcategoryName, string message);

        void Warning(string message, ExtraInfo extraInfo);

        void Warning(string moduleName, string message, ExtraInfo extraInfo);

        void Warning(string categoryName, string subcategoryName, string message, ExtraInfo extraInfo);

        void Warning(string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo);


        void Error(string message);

        void Error(string moduleName, string message);

        void Error(string categoryName, string subcategoryName, string message);

        void Error(string moduleName, string categoryName, string subcategoryName, string message);

        void Error(string message, ExtraInfo extraInfo);

        void Error(string moduleName, string message, ExtraInfo extraInfo);

        void Error(string categoryName, string subcategoryName, string message, ExtraInfo extraInfo);

        void Error(string moduleName, string categoryName, string subcategoryName, string message, ExtraInfo extraInfo);    

    }
}
