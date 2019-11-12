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
		public void Error(Exception e, string message) => logger.Error(e, message);
		public void Error(string format, params object[] args) => logger.Error(format, args);
		public void Error(Exception e, string format, params object[] args) => logger.Error(e, format, args);
	}
}
