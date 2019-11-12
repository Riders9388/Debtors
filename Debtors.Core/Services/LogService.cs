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

		public void Error(string message) => logger.Error(message);
		public void Error(Exception ex) => logger.Error(ex, ex.Message);
		public void Error(Exception ex, string message) => logger.Error(ex, message);
		public void Error(string format, params object[] args) => logger.Error(format, args);
		public void Error(Exception ex, string format, params object[] args) => logger.Error(ex, format, args);
	}
}
