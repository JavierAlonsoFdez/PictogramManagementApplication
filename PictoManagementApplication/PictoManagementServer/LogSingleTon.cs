using log4net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictoManagementServer
{
    /// <summary>
    /// Clase para el logger
    /// </summary>
    public class LogSingleTon
    {

        private static readonly Lazy<LogSingleTon> lazy = new Lazy<LogSingleTon>(() => new LogSingleTon());

        /// <summary>
        /// Obtiene una instancia del logger si existe, sino se genera una, patrón singleton
        /// </summary>
        public static LogSingleTon Instance { get { return lazy.Value; } }
        private ILog log;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        private LogSingleTon()
        {
            log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Loggea un mensaje de tipo info
        /// </summary>
        /// <param name="message">Mensaje a ser loggeado</param>
        public void LogMessage(string message)
        {
            Task.Factory.StartNew(() => log.Info(message));
        }

        /// <summary>
        /// Loggea un mensaje de tipo warning
        /// </summary>
        /// <param name="message">Mensaje a ser loggeado</param>
        public void LogWarning(string message)
        {
            Task.Factory.StartNew(() => log.Warn(message));
        }

        /// <summary>
        /// Loggea un mensaje de tipo error
        /// </summary>
        /// <param name="message">Mensaje a ser loggeado</param>
        public void LogError(string message)
        {
            Task.Factory.StartNew(() => log.Error(message));
        }
    }
}
