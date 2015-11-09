using UnityEngine;
using System.Collections;
using log4net;
using log4net.Core;
using log4net.Layout;
using log4net.Appender;
using log4net.Config;

namespace TeamStark.RobosubSimulator
{
    public class LoggingSystem : MonoBehaviour
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        ///  Configure logging to write to Logs\EventLog.txt and the Unity console output.
        /// </summary>
        public static void ConfigureAllLogging()
        {
            
            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date %-5level %logger - %message%newline"
            };
            patternLayout.ActivateOptions();

            // setup the appender that writes to Log\EventLog.txt
            var fileAppender = new RollingFileAppender
            {
                AppendToFile = false,
                File = @"Logs\EventLog.txt",
                Layout = patternLayout,
                MaxSizeRollBackups = 5,
                MaximumFileSize = "1GB",
                RollingStyle = RollingFileAppender.RollingMode.Size,
                StaticLogFileName = true
            };
            fileAppender.ActivateOptions();

            var unityLogger = new UnityAppender
            {
                Layout = new PatternLayout()
            };
            unityLogger.ActivateOptions();

            BasicConfigurator.Configure(unityLogger, fileAppender);
        }

        void Awake()
        {
            ConfigureAllLogging();
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    /// <summary> An appender which logs to the unity console. </summary>
    class UnityAppender : AppenderSkeleton
    {
        /// <inheritdoc />
        protected override void Append(LoggingEvent loggingEvent)
        {
            string message = RenderLoggingEvent(loggingEvent);

            if (Level.Compare(loggingEvent.Level, Level.Error) >= 0)
            {
                // everything above or equal to error is an error
                Debug.LogError(message);
            }
            else if (Level.Compare(loggingEvent.Level, Level.Warn) >= 0)
            {
                // everything that is a warning up to error is logged as warning
                Debug.LogWarning(message);
            }
            else
            {
                // everything else we'll just log normally
                Debug.Log(message);
            }
        }
    }
}