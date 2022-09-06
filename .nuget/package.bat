rem Package the library for Nuget
copy ..\MaxFactry.Provider.SQLiteProvider-NF-2.0\bin\Release\MaxFactry.Provider.SQLite*.dll lib\net20\
copy ..\MaxFactry.Provider.SQLiteProvider-NF-4.5.2\bin\Release\MaxFactry.Provider.SQLite*.dll lib\net452\

c:\install\nuget\nuget.exe pack MaxFactry.Provider.SQLite.nuspec -OutputDirectory "packages" -IncludeReferencedProjects -properties Configuration=Release 