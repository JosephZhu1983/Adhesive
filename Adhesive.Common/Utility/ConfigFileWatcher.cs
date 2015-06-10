
using System;
using System.IO;
using System.Threading;

namespace Adhesive.Common
{
    public delegate void ConfigFileChangedEventHandler(object configFile);
    public sealed class ConfigFileWatcher : IDisposable
    {
        #region Fields
        /// <summary>
        /// The timer used to compress the notification events.
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// The list of FileSystemWatcher.
        /// </summary>
        private readonly FileSystemWatcher _fsw = new FileSystemWatcher();

        /// <summary>
        /// The default amount of time to wait after receiving notification
        /// before reloading the config file.
        /// </summary>
        private const int TimeoutMilliseconds = 1000;
        #endregion

        #region Constructor (s) / Destructor
        /// <summary>
        ///-
        /// </summary>
        ///<param name="configFile"></param>
        ///<param name="configFileChangedEventHandler"></param>
        public ConfigFileWatcher(string configFile, ConfigFileChangedEventHandler configFileChangedEventHandler)
        {
            AttachWatcher(new FileInfo(configFile));
            _timer = new Timer(new TimerCallback(configFileChangedEventHandler), configFile, Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region Methods

        private void AttachWatcher(FileInfo configFile)
        {
            _fsw.Path = configFile.DirectoryName;
            _fsw.Filter = configFile.Name;

            // Set the notification filters
            _fsw.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;

            // Add event handlers. OnChanged will do for all event handlers that fire a FileSystemEventArgs
            _fsw.Changed += new FileSystemEventHandler(ConfigWatcherHandler_OnChanged);
            _fsw.Created += new FileSystemEventHandler(ConfigWatcherHandler_OnChanged);
            _fsw.Deleted += new FileSystemEventHandler(ConfigWatcherHandler_OnChanged);
            _fsw.Renamed += new RenamedEventHandler(ConfigWatcherHandler_OnRenamed);

            // Begin watching.
            _fsw.EnableRaisingEvents = true;
        }
        /// <summary>
        /// Event handler used by <see cref="ConfigFileWatcher"/>.
        /// </summary>
        /// <param name="source">The <see cref="FileSystemWatcher"/> firing the event.</param>
        /// <param name="e">The argument indicates the file that caused the event to be fired.</param>
        /// <remarks>
        /// This handler reloads the configuration from the file when the event is fired.
        /// </remarks>
        private void ConfigWatcherHandler_OnChanged(object source, FileSystemEventArgs e)
        {

            //Console.WriteLine("ConfigFileWatcher : " + e.ChangeType + " [" + e.Name + "]");


            // timer will fire only once
            _timer.Change(TimeoutMilliseconds, Timeout.Infinite);
        }

        /// <summary>
        /// Event handler used by <see cref="ConfigFileWatcher"/>.
        /// </summary>
        /// <param name="source">The <see cref="FileSystemWatcher"/> firing the event.</param>
        /// <param name="e">The argument indicates the file that caused the event to be fired.</param>
        /// <remarks>
        /// This handler reloads the configuration from the file when the event is fired.
        /// </remarks>
        private void ConfigWatcherHandler_OnRenamed(object source, RenamedEventArgs e)
        {

            //Console.WriteLine("ConfigFileWatcher : " + e.ChangeType + " [" + e.OldName + "/" + e.Name + "]");


            // timer will fire only once
            _timer.Change(TimeoutMilliseconds, Timeout.Infinite);
        }
        #endregion

        public void Dispose()
        {
            _fsw.EnableRaisingEvents = false;
            _fsw.Dispose();
        }
    }
}