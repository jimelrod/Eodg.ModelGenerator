### Eodg.ModelCreator

This is simple command line utility to auto-generate POCOs from a given database connection.

From the provided connection string, it scans all tables in the database specified and generates POCOs to the specified output directory. Each class is encapsulated in the specified namespace.

Example usage:

`dotnet Eodg.ModelCreator.dll -c <connection string> -o <output directory> -n <namespace>`

All paramaters are required.

**Note**: This does not take into account best practices for C# naming conventions - it just reproduces exactly what is in the database. Also, the `property-script.sql` may not include cases for datatypes necessary for your project. The output will reflect that and not generate a POCO. If this is the case, don't hesitate to modify the script and submit a pull request.