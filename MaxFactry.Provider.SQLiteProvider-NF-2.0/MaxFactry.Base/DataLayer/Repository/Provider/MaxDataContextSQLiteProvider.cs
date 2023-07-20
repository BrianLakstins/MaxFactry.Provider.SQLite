// <copyright file="MaxDataContextSQLiteProvider.cs" company="Lakstins Family, LLC">
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
// <change date="4/1/2016" author="Brian A. Lakstins" description="Initial Release">
// <change date="7/22/2016" author="Brian A. Lakstins" description="Update to automatically create the data file.">
// <change date="12/29/2016" author="Brian A. Lakstins" description="Updated to create database folder before creating database file.">
// <change date="7/20/2023" author="Brian A. Lakstins" description="Add methods to generate configuration.">
// </changelog>
#endregion

namespace MaxFactry.Base.DataLayer.Provider
{
	using System;
    using System.IO;
    using MaxFactry.Core;
    using MaxFactry.Core.Provider;
    using MaxFactry.General;
    using MaxFactry.Provider.CoreProvider.DataLayer;

	/// <summary>
	/// Data Context used to work with SQLite databases through ADO.NET (Connection, Command, Parameter)
	/// </summary>
    public class MaxDataContextSQLiteProvider : MaxDataContextADODbProvider
	{
        private static MaxIndex _oDataFileCreated = new MaxIndex();

        private static object _oLock = new object();

        public static MaxIndex GetDefaultContextConfig()
        {
            MaxIndex loR = new MaxIndex();
            string lsProviderType = "DefaultContextProviderTypeSQLite";
            loR.Add(lsProviderType, typeof(MaxDataContextSQLiteProvider));
            string lsBase = typeof(MaxFactry.Core.MaxProvider).ToString();
            string lsKey = "DefaultContextProviderName";
            loR.Add(lsBase + "-" + lsKey, lsProviderType);
            return loR;
        }

        public static MaxIndex GetDefaultProviderConfig(string lsDbFileNameBase)
        {
            MaxIndex loR = new MaxIndex();
            string lsProductFolder = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory") as string;
            string lsDbFileName = lsDbFileNameBase + "V" + MaxAppLibrary.ProductVersion + ".db";

            //// SQLite Database Configuration
            string lsDataFolder = System.IO.Path.Combine(lsProductFolder, "data");
            string lsDataFile = System.IO.Path.Combine(lsDataFolder, lsDbFileName);
            string lsProviderName = typeof(System.Data.SQLite.SQLiteFactory).Namespace; //System.Data.SQLite

            string lsConnectionString = "Data Source=" + lsDataFile + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionVersion, "3") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionBinaryGUID, "false") + ";";
            lsConnectionString += String.Format(MaxDataContextSQLiteProvider.OptionDateTimeKind, "utc") + ";";

            string lsClass = typeof(System.Data.SQLite.SQLiteFactory).FullName; // System.Data.SQLite.SQLiteFactory
            string lsAssemblyFile = typeof(System.Data.SQLite.SQLiteFactory).Module.ToString(); //System.Data.SQLite.dll
            string lsName = lsDataFile;

            loR = MaxDataContextADODbProvider.AddConfig(
                lsName,
                lsConnectionString,
                lsProviderName,
                lsClass,
                lsAssemblyFile,
                typeof(MaxFactry.Base.DataLayer.Library.Provider.MaxSqlGenerationLibrarySQLiteProvider),
                null);

            loR.Add("MaxDataFile", lsDataFile);
            loR.Add("MaxDataFileConnection", lsConnectionString);
            return loR;
        }

        public override void Initialize(string lsName, MaxIndex loConfig)
        {
            base.Initialize(lsName, loConfig);
        }

        protected override bool HasTable(MaxDataModel loDataModel, System.Data.Common.DbConnection loConnection)
        {
            if (!_oDataFileCreated.Contains(loConnection.ConnectionString))
            {
                lock (_oLock)
                {
                    if (!_oDataFileCreated.Contains(loConnection.ConnectionString))
                    {
                        //// Create the database file if it does not exist.
                        string[] laConnectionString = loConnection.ConnectionString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string lsConfig in laConnectionString)
                        {
                            if (lsConfig.ToLower().Contains("data source"))
                            {
                                string[] laConfig = lsConfig.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                                if (laConfig.Length == 2)
                                {
                                    string lsDataFile = laConfig[1].Trim();
                                    if (String.Compare(lsDataFile, ":memory:", StringComparison.OrdinalIgnoreCase) != 0 && !File.Exists(lsDataFile))
                                    { 
                                        //// Create the directory if it does not exist
                                        new FileInfo(lsDataFile).Directory.Create();
                                        System.Data.SQLite.SQLiteConnection.CreateFile(lsDataFile);
                                    }
                                }
                            }
                        }

                        _oDataFileCreated.Add(loConnection.ConnectionString, true);
                    }
                }
            }

            return base.HasTable(loDataModel, loConnection);
        }

        /// <summary>
        /// 3
        /// </summary>
        public const string OptionVersion = "Version={0}";

        /// <summary>
        /// If true - GUID columns are stored in binary form; otherwise GUID columns are stored as text.
        /// Use False for text so can be queried.
        /// </summary>
        public const string OptionBinaryGUID = "BinaryGUID={0}";

        /// <summary>
        /// Unspecified: Not specified as either UTC or local time.
        /// Utc: The time represented is UTC.
        /// Local: The time represented is local time.
        /// Defaults to Unspecified
        /// </summary>
        public const string OptionDateTimeKind = "DateTimeKind={0}";

        /// <summary>
        /// Ticks: Use the value of DateTime.Ticks.
        /// ISO8601:  Use the ISO-8601 format.  Uses the "yyyy-MM-dd HH:mm:ss.FFFFFFFK" format for UTC
        /// DateTime values and "yyyy-MM-dd HH:mm:ss.FFFFFFF" format for local DateTime values).
        /// JulianDay: The interval of time in days and fractions of a day since January 1, 4713 BC.
        /// UnixEpoch: The whole number of seconds since the Unix epoch (January 1, 1970).
        /// InvariantCulture: Any culture-independent string value that the .NET Framework can interpret as a valid DateTime.
        /// CurrentCulture: Any string value that the .NET Framework can interpret as a valid DateTime using the current culture.
        /// Defaults to ISO8601
        /// </summary>
        public const string OptionDateTimeFormat = "DateTimeFormat={0}";

        /// <summary>
        /// If the argument N is positive then the suggested cache size is set to N.
        /// If the argument N is negative, then the number of cache pages is adjusted
        /// to use approximately abs(N*4096) bytes of memory. Backwards compatibility
        /// note: The behavior of cache_size with a negative N was different in SQLite
        /// versions prior to 3.7.10. In version 3.7.9 and earlier, the number of
        /// pages in the cache was set to the absolute value of N.
        /// Defaults to -2000
        /// </summary>
        public const string OptionCacheSize = "Cache Size={0}";


        /// <summary>
        /// Normal: Normal file flushing behavior
        /// Full: Full flushing after all writes
        /// Off: Underlying OS flushes I/O's
        /// Defaults to Full
        /// </summary>
        public const string OptionSynchronous = "Synchronous={0}";

        /// <summary>
        /// True: Use connection pooling.<br/>
        /// False: Do not use connection pooling.<br/><br/>
        /// WARNING -  When using the default connection pool implementation,
        /// setting this property to True should be avoided by applications that make
        /// use of COM (either directly or indirectly) due to possible deadlocks that
        /// can occur during the finalization of some COM objects.
        /// Defaults to False
        /// </summary>
        public const string OptionPooling = "Pooling={0}";

        /// <summary>
        /// {time in seconds}: The default command timeout
        /// Defaults to 30
        /// </summary>
        public const string OptionDefaultTimeout = "Default Timeout={0}";

        /// <summary>
        /// {time in milliseconds}: Sets the busy timeout for the core library.
        /// Defaults to 0
        /// </summary>
        public const string OptionBusyTimeout = "BusyTimeout={0}";

        /// <summary>
        /// Default: The default mode, this causes SQLite to use the existing journaling mode for the database.
        /// Delete: SQLite will create and destroy the journal file as-needed.
        /// Persist: When this is set, SQLite will keep the journal file even after a transaction has completed.  It's contents will be erased,
        /// and the journal re-used as often as needed.  If it is deleted, it will be recreated the next time it is needed.
        /// Off: This option disables the rollback journal entirely.  Interrupted transactions or a program crash can cause database
        /// corruption in this mode!
        /// Truncate: SQLite will truncate the journal file to zero-length instead of deleting it.
        /// Memory: SQLite will store the journal in volatile RAM.  This saves disk I/O but at the expense of database safety and integrity.
        /// If the application using SQLite crashes in the middle of a transaction when the MEMORY journaling mode is set, then the
        /// database file will very likely go corrupt.
        /// Wal: SQLite uses a write-ahead log instead of a rollback journal to implement transactions.  The WAL journaling mode is persistent;
        /// after being set it stays in effect across multiple database connections and after closing and reopening the database. A database
        /// in WAL journaling mode can only be accessed by SQLite version 3.7.0 or later.
        /// </summary>
        public const string OptionJournalMode = "Journal Mode={0}";

        /// <summary>
        /// The maximum number of connections for the given connection string that can be in the connection pool
        /// Defaults to 100
        /// </summary>
        public const string OptionMaxPoolSize = "Max Pool Size={0}";
    }
}
