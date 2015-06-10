
using System;

namespace Adhesive.AppInfoCenter
{
    public interface IExceptionService : IDisposable
    {
        void Handle(Exception exception);

        void Handle(string moduleName, Exception exception);

        void Handle(string categoryName, string subcategoryName, Exception exception);

        void Handle(string moduleName, string categoryName, string subcategoryName, Exception exception);

        void Handle(Exception exception, string description);

        void Handle(string moduleName, Exception exception, string description);

        void Handle(string categoryName, string subcategoryName, Exception exception, string description);

        void Handle(string moduleName, string categoryName, string subcategoryName, Exception exception, string description);


        void Handle(Exception exception, ExtraInfo extraInfo);

        void Handle(string moduleName, Exception exception, ExtraInfo extraInfo);

        void Handle(string categoryName, string subcategoryName, Exception exception, ExtraInfo extraInfo);

        void Handle(string moduleName, string categoryName, string subcategoryName, Exception exception, ExtraInfo extraInfo);

        void Handle(Exception exception, string description, ExtraInfo extraInfo);

        void Handle(string moduleName, Exception exception, string description, ExtraInfo extraInfo);

        void Handle(string categoryName, string subcategoryName, Exception exception, string description, ExtraInfo extraInfo);

        void Handle(string moduleName, string categoryName, string subcategoryName, Exception exception, string description, ExtraInfo extraInfo);  

    }
}
