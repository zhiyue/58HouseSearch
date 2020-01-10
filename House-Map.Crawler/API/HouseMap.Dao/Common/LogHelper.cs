﻿using NLog;
using System;
using System.Threading.Tasks;

namespace HouseMap.Common
{
    public static class LogHelper
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();


        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Error(string message, Exception ex, object oj = null)
        {
            Logger.Error(message + ",Exception:" + ex.ToString(), ex, oj);
        }


        public static void RunActionNotThrowEx(Action action, string functionName = "default", Object oj = null)
        {
            try
            {
                action.Invoke();

            }
            catch (Exception ex)
            {
                if (oj != null)
                {
                    Logger.Info("关键数据:" + Newtonsoft.Json.JsonConvert.SerializeObject(oj));
                }
                Error(functionName, ex);
                Console.WriteLine($"[{DateTime.Now}]|{functionName}执行失败, 关键数据:{Newtonsoft.Json.JsonConvert.SerializeObject(oj)}, 异常信息:{ex.ToString()},stackTrace:{ex.StackTrace}");
            }
        }


        public static void RunActionTaskNotThrowEx(Action action, string functionName = "default", Object oj = null)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    if (oj != null)
                    {
                        Logger.Info("关键数据:" + Newtonsoft.Json.JsonConvert.SerializeObject(oj));
                        Console.WriteLine($"{functionName} error,ex:{ex.StackTrace},data:{Newtonsoft.Json.JsonConvert.SerializeObject(oj)}");
                    }
                    Error(functionName, ex);
                }
            });
        }

    }
}
