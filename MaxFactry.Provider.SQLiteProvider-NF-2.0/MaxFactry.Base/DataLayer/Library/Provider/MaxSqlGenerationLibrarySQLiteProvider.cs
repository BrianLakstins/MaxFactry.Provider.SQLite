// <copyright file="MaxSqlGenerationLibrarySQLiteProvider.cs" company="Lakstins Family, LLC">
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
// <change date="4/22/2016" author="Brian A. Lakstins" description="Updated to support altering a table.">
// </changelog>
#endregion

namespace MaxFactry.Base.DataLayer.Library.Provider
{
    using System;
    using System.Text;
    using MaxFactry.Base.DataLayer.Library.Provider;

    /// <summary>
    /// Generates Sql specific to any database
    /// http://www.sqlite.org/datatype3.html
    /// 
    /// </summary>
    public class MaxSqlGenerationLibrarySQLiteProvider : MaxSqlGenerationLibraryDefaultProvider
    {
        /// <summary>
        /// Initializes a new instance of the MaxSqlGenerationLibraryMySql56Provider class
        /// </summary>
        public MaxSqlGenerationLibrarySQLiteProvider()
        {
            this.AddReplacement("#DatabaseList", "show databases");
            this.AddReplacement("#SchemaTable", "sqlite_master");
            this.AddReplacement("#TableNameField", "name");
            this.AddReplacement("#TableExistFilter", " AND type = 'table'");
            this.AddReplacement("#COUNT", "count(*)");

            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(byte[]), "."), "BLOB");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(bool), "."), "NUMERIC");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(decimal), "."), "NUMERIC");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(DateTime), "."), "NUMERIC");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(double), "."), "REAL");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(Guid), "."), "TEXT");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(short), "."), "INTEGER");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(int), "."), "INTEGER");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(long), "."), "INTEGER");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(float), "."), "REAL");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(string), "."), "TEXT");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(MaxLongString), "."), "TEXT");
            this.AddReplacement(string.Concat("MaxDefinitionType.", typeof(MaxShortString), "."), "TEXT");

            //this.AddReplacement("AUTOINCREMENT", "AUTOINCREMENT ");

            //http://www.sqlite.org/lang_keywords.html
            //"keyword"		A keyword in double-quotes is an identifier.
            this.AddReplacement("[", "\"");
            this.AddReplacement("]", "\"");
        }

        public override string GetColumnList(string lsTable)
        {
            return "pragma table_info(\"" + lsTable + "\")";
        }

        public override string GetTableAlter(MaxDataModel loDataModel, MaxDataList loDataList)
        {
            StringBuilder loCommandText = new StringBuilder();
            string[] laKeyList = loDataModel.GetKeyList();
            int lnAdded = 0;
            for (int lnK = 0; lnK < laKeyList.Length; lnK++)
            {
                string lsKey = laKeyList[lnK];
                if (this.IsStored(loDataModel, lsKey))
                {
                    bool lbExists = false;
                    for (int lnD = 0; lnD < loDataList.Count; lnD++)
                    {
                        MaxData loData = loDataList[lnD];
                        if (loData.Get("name").ToString() == lsKey)
                        {
                            lbExists = true;
                        }
                    }

                    if (!lbExists)
                    {
                        string lsType = string.Concat("MaxDefinitionType.", loDataModel.GetValueType(lsKey), ".");
                        loCommandText.Append(string.Concat("ALTER TABLE [", loDataModel.DataStorageName, "] ADD COLUMN [", lsKey, "] ", lsType, ";"));
                        lnAdded++;
                    }
                }
            }

            if (lnAdded > 0)
            {
                return loCommandText.ToString();
            }

            return string.Empty;
        }
    }
}
