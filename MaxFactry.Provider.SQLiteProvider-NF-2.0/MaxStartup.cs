// <copyright file="MaxStartup.cs" company="Lakstins Family, LLC">
// Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// </copyright>

#region License
// <license>
// This software is provided 'as-is', without any express or implied warranty. In no 
// event will the author be held liable for any damages arising from the use of this 
// software.
//  
// Permission is granted to anyone to use this software for any purpose, including 
// commercial applications, and to alter it and redistribute it freely, subject to the 
// following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the following) in the product documentation is required.
// 
// Portions Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// 
// 2. Altered source versions must be plainly marked as such, and must not be 
// misrepresented as being the original software.
// 
// 3. This notice may not be removed or altered from any source distribution.
// </license>
#endregion

#region Change Log
// <changelog>
// <change date="2/19/2019" author="Brian A. Lakstins" description="Initial creation">
// <change date="2/21/2019" author="Brian A. Lakstins" description="Update to allow different file name for default database.  Add speed options.">
// </changelog>
#endregion

namespace MaxFactry.Provider.SQLiteProvider
{
    using System;
    using System.IO;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using MaxFactry.Core;
    using MaxFactry.Base.DataLayer.Provider;

    /// <summary>
    /// Class used to define initialization tasks for a module or provider.
    /// </summary>
    public class MaxStartup
    {
        /// <summary>
        /// Internal storage of single object
        /// </summary>
        private static object _oInstance = null;

        /// <summary>
        /// Lock object for multi-threaded access.
        /// </summary>
        private static object _oLock = new object();

        protected static object CreateInstance(System.Type loType, object loCurrent)
        {
            if (null == loCurrent)
            {
                lock (_oLock)
                {
                    if (null == loCurrent)
                    {
                        loCurrent = MaxFactry.Core.MaxFactryLibrary.CreateSingleton(loType);
                    }
                }
            }

            return loCurrent;
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static MaxStartup Instance
        {
            get
            {
                _oInstance = CreateInstance(typeof(MaxStartup), _oInstance);
                return _oInstance as MaxStartup;
            }
        }

        /// <summary>
        /// To be run first, before anything else in the application.
        /// </summary>
        public virtual void RegisterProviders()
        {
        }

        /// <summary>
        /// To be run after providers have been registered
        /// </summary>
        /// <param name="loConfig">The configuration for the default repository provider.</param>
        public virtual void SetProviderConfiguration(MaxIndex loConfig)
        {
        }

        /// <summary>
        /// To be run after providers have been configured.
        /// </summary>
        public virtual void ApplicationStartup()
        {
            SetupDatabase(string.Empty);
        }

        public void SetupDatabase(string lsDbFileName)
        {
            string lsProductFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory") as string;
            string lsVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyVersion").ToString();

            if (string.IsNullOrEmpty(lsDbFileName))
            {
                lsDbFileName = "DataV" + lsVersion + ".db";
            }

            //// SQLite Database Configuration
            string lsDataFolder = System.IO.Path.Combine(lsProductFolder, "data");
            string lsDataFile = System.IO.Path.Combine(lsDataFolder, lsDbFileName);
            string lsProviderName = "System.Data.SQLite";
            string lsConnectionString = "Data Source=" + lsDataFile + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionVersion, "3") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionBinaryGUID, "false") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionDateTimeKind, "utc") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionPooling, "true") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionMaxPoolSize, "300") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionCacheSize, "20000") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionJournalMode, "Delete") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionSynchronous, "Full") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionDefaultTimeout, "30") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionBusyTimeout, "30") + ";";

            string lsClass = "System.Data.SQLite.SQLiteFactory, System.Data.SQLite";
            string lsAssemblyFile = string.Empty;
            string lsName = lsDataFile;

            MaxIndex loDbConfig = MaxFactry.Base.DataLayer.Provider.MaxDataContextADODbProvider.AddConfig(
                lsName,
                lsConnectionString,
                lsProviderName,
                lsClass,
                lsAssemblyFile,
                typeof(MaxFactry.Base.DataLayer.Library.Provider.MaxSqlGenerationLibrarySQLiteProvider),
                null);

            MaxFactryLibrary.SetValue(typeof(MaxFactry.Base.DataLayer.Provider.MaxDataContextSQLiteProvider) + "-Config", loDbConfig);
        }
    }
}
