rem Package the library for Nuget
copy ..\MaxFactry.Provider.SQLiteProvider-NF-2.0\bin\Release\MaxFactry.Provider.SQLite*.dll lib\net20\
copy ..\MaxFactry.Provider.SQLiteProvider-NF-2.0\bin\Release\x86\*.* lib\net20\x86
copy ..\MaxFactry.Provider.SQLiteProvider-NF-2.0\bin\Release\x64\*.* lib\net20\x64
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.5.2\bin\Release\MaxFactry.Provider.SQLite*.dll lib\net452\
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.5.2\bin\Release\x86\*.* lib\net452\x86
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.5.2\bin\Release\x64\*.* lib\net452\x64
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.7.2\bin\Release\MaxFactry.Provider.SQLite*.dll lib\net472\
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.7.2\bin\Release\x86\*.* lib\net472\x86
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.7.2\bin\Release\x64\*.* lib\net472\x64
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.8\bin\Release\MaxFactry.Provider.SQLite*.dll lib\net48\
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.8\bin\Release\x86\*.* lib\net48\x86
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.8\bin\Release\x64\*.* lib\net48\x64
copy ..\MaxFactry.Provider.SQLiteProvider-NC-3.1\bin\Release\netcoreapp3.1\MaxFactry.Provider.SQLite*.dll lib\netcoreapp3.1\
copy ..\MaxFactry.Provider.SQLiteProvider-NC-6.0\bin\Release\net6.0\MaxFactry.Provider.SQLite*.dll lib\net6.0\
copy ..\MaxFactry.Provider.SQLiteProvider-NC-8.0\bin\Release\net8.0\MaxFactry.Provider.SQLite*.dll lib\net8.0\

c:\install\nuget\nuget.exe pack MaxFactry.Provider.SQLite.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 