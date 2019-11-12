using Debtors.Core.Interfaces;
using MvvmCross;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Services
{
	public class LogService : ILogService
	{
		private ILogConfiguration logConfiguration;
		private ILogger logger;

		public LogService(ILogConfiguration logConfiguration)
		{
			this.logConfiguration = logConfiguration;

			LogManager.Configuration = new XmlLoggingConfiguration(logConfiguration.ConfigFilePath);
			logger = LogManager.GetLogger("NLogSample");
		}

		#region Error
		public void Error(string message) => logger.Error(message);
		public void Error(Exception ex) => logger.Error(ex, ex.Message);
		public void Error(Exception e, string message) => logger.Error(e, message);
		public void Error(string format, params object[] args) => logger.Error(format, args);
		public void Error(Exception e, string format, params object[] args) => logger.Error(e, format, args);
		#endregion

		#region Fatal
		public void Fatal(string message) => logger.Fatal(message);
		public void Fatal(string format, params object[] args) => logger.Fatal(format, args);

		public void Fatal(Exception e, string message) => logger.Fatal(e, message);
		public void Fatal(Exception e, string format, params object[] args) => logger.Fatal(e, format, args);
		#endregion

		#region Debug
		public void Debug(string message) => logger.Debug(message);
		public void Debug(string format, params object[] args) => logger.Debug(format, args);
		#endregion

		#region Info
		public void Info(string message) => logger.Info(message);
		public void Info(string message, params object[] args) => logger.Info(message, args);
		#endregion

		#region Trace
		public void Trace(string message) => logger.Trace(message);
		public void Trace(string format, params object[] args) => logger.Trace(format, args);
		#endregion

		#region Warn
		public void Warn(string message) => logger.Warn(message);
		public void Warn(string format, params object[] args) => logger.Warn(format, args);
		#endregion
	}
}
