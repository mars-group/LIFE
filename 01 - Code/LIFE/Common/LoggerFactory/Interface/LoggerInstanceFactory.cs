// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System.Collections.Generic;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace LoggerFactory.Interface {
    public class LoggerInstanceFactory {
        private static ILog Logger = null;
        private static string defaultLayout = "%d [%t] %-5p %m%n";


        public static ILog GetLoggerInstance<T>() {
            if (Logger == null) initLogger<T>();

            return Logger;
        }

        private static void initLogger<T>() {
            Hierarchy hierarchy = (Hierarchy) LogManager.GetRepository();
            TraceAppender tracer = new TraceAppender();
            PatternLayout patternLayout = GetPatternLayout();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);
            foreach (IAppender appender in CreateAppenders()) {
                hierarchy.Root.AddAppender(appender);
            }

            //Set Debug Level
            hierarchy.Root.Level = Level.Off;
            hierarchy.Configured = true;

            Logger = LogManager.GetLogger(typeof (T));
        }

        private static List<IAppender> CreateAppenders() {
            List<IAppender> appenderList = new List<IAppender>();

            appenderList.Add(CreateConsoleAppender());
            //appenderList.Add(CreateRollingFileAppender());

            return appenderList;
        }

        private static IAppender CreateConsoleAppender() {
            ConsoleAppender consoleAppender = new ConsoleAppender();

            consoleAppender.Layout = GetPatternLayout();

            return consoleAppender;
        }

        private static IAppender CreateRollingFileAppender() {
            RollingFileAppender roller = new RollingFileAppender();
            roller.Layout = GetPatternLayout();
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 4;
            roller.MaximumFileSize = "100KB";
            roller.StaticLogFileName = true;
            roller.File = "dnservices.txt";
            roller.ActivateOptions();
            return roller;
        }


        private static PatternLayout GetPatternLayout() {
            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = defaultLayout;
            patternLayout.ActivateOptions();

            return patternLayout;
        }
    }
}