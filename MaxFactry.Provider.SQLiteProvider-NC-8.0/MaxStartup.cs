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
// <change date="7/21/2023" author="Brian A. Lakstins" description="Added new method to create database and added setting provider configuration">
// <change date="3/31/2024" author="Brian A. Lakstins" description="Update for Namespace and naming convention changes">
// </changelog>
#endregion

namespace MaxFactry.Provider.SQLiteProvider
{
    using System;
    using MaxFactry.General;
    using MaxFactry.Base.DataLayer.Library.Provider;
    using MaxFactry.Core;

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
        /// To be run after providers have been registered
        /// </summary>
        /// <param name="loConfig">The configuration for the default repository provider.</param>
        public virtual void SetProviderConfiguration(MaxIndex loConfig)
        {
            loConfig.Add(typeof(MaxDataContextLibrarySQLiteProvider).Name, typeof(MaxDataContextLibrarySQLiteProvider));
        }

        /// <summary>
        /// To be run first, before anything else in the application.
        /// </summary>
        public virtual void RegisterProviders()
        {
        }

        /// <summary>
        /// To be run after providers have been configured.
        /// </summary>
        public virtual void ApplicationStartup()
        {
            InitializeDatabase("Data");
        }

        public static void InitializeDatabase(string lsBaseName)
        {
            string lsProductFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory") as string;
            string lsVersion = MaxAppLibrary.ProductVersion;
            string lsDbFileName = lsBaseName + "V-" + lsVersion + ".db";

            //// SQLite Database Configuration
            string lsDataFolder = System.IO.Path.Combine(lsProductFolder, "data");
            string lsDataFile = System.IO.Path.Combine(lsDataFolder, lsDbFileName);
            string lsProviderName = typeof(System.Data.SQLite.SQLiteFactory).Namespace;

            string lsConnectionString = "Data Source=" + lsDataFile + ";";
            lsConnectionString += String.Format(MaxDataContextLibrarySQLiteProvider.OptionVersion, "3") + ";";
            lsConnectionString += String.Format(MaxDataContextLibrarySQLiteProvider.OptionBinaryGUID, "false") + ";";
            lsConnectionString += String.Format(MaxDataContextLibrarySQLiteProvider.OptionDateTimeKind, "utc") + ";";

            string lsClass = typeof(System.Data.SQLite.SQLiteFactory).FullName; // System.Data.SQLite.SQLiteFactory
            string lsAssemblyFile = typeof(System.Data.SQLite.SQLiteFactory).Assembly.Location; //File system path to System.Data.SQLite.dll
            string lsName = lsDataFile;

            MaxIndex loDbConfig = MaxDataContextLibraryADODbProvider.AddConfig(
                lsName,
                lsConnectionString,
                lsProviderName,
                lsClass,
                lsAssemblyFile,
                typeof(MaxSqlGenerationLibrarySQLiteProvider),
                null);

            loDbConfig.Add("MaxDataFile", lsDataFile);
            loDbConfig.Add("MaxDataFileConnection", lsConnectionString);
            MaxFactryLibrary.SetValue(typeof(MaxDataContextLibrarySQLiteProvider) + "-Config", loDbConfig);
        }
    }
}
