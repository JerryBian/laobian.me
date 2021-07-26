﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Laobian.Share.Helper;
using Laobian.Share.Logger;

namespace Laobian.Api.Logger
{
    public class GitFileLoggerProcessor : ILaobianLoggerProcessor, IDisposable
    {
        private readonly ApiConfig _config;
        private readonly string _logDir;
        private readonly IGitFileLogQueue _messageQueue;
        private readonly GitFileLoggerOptions _options;
        private readonly Thread _underlingThread;

        private bool _stop;

        public GitFileLoggerProcessor(GitFileLoggerOptions options, ApiConfig config, IGitFileLogQueue messageQueue)
        {
            _config = config;
            _options = options;

            _logDir = Path.Combine(config.AssetLocation, "log");
            Directory.CreateDirectory(_logDir);
            _messageQueue = messageQueue;

            _underlingThread = new Thread(Process)
            {
                IsBackground = true,
                Name = "Git file logs processing thread"
            };
            _underlingThread.Start();
        }

        public void Dispose()
        {
            _stop = true;
            try
            {
                _underlingThread.Join(TimeSpan.FromSeconds(15));
            }
            catch (ThreadStateException)
            {
            }
        }

        public void Add(LaobianLog log)
        {
            if (!_stop)
            {
                try
                {
                    _messageQueue.Add(log);
                    return;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            try
            {
                ProcessLogs(log);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void Process()
        {
            try
            {
                while (true)
                {
                    var logs = new List<LaobianLog>();
                    while (_messageQueue.TryDequeue(out var log))
                    {
                        logs.Add(log);
                    }

                    if (logs.Any())
                    {
                        ProcessLogs(logs.ToArray());
                    }

                    if (_stop)
                    {
                        return;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception)
            {
                _stop = true;
            }
        }

        private void ProcessLogs(params LaobianLog[] logs)
        {
            if (logs == null || !logs.Any())
            {
                return;
            }

            foreach (var log in logs)
            {
                try
                {
                    var loggerName = string.IsNullOrEmpty(log.LoggerName) ? _options.LoggerName : log.LoggerName;
                    if (string.IsNullOrEmpty(loggerName))
                    {
                        loggerName = "undefined";
                    }

                    var dir = Path.Combine(_logDir, loggerName, log.TimeStamp.Year.ToString(),
                        log.TimeStamp.Month.ToString("D2"));
                    Directory.CreateDirectory(dir);
                    File.AppendAllLines(Path.Combine(dir, $"{log.TimeStamp:yyyy-MM-dd}.log"),
                        new[] {JsonHelper.Serialize(log)});
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Process Logs failed. ==> {JsonHelper.Serialize(log)}{Environment.NewLine}{ex}");
                }
            }
        }
    }
}