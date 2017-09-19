# Akka.NET Persistence Fundamentals

This repository is my run through of Jason Robert's [Pluralsight](http://www.pluralsight.com) course on [Akka.NET Persistence](https://app.pluralsight.com/library/courses/akka-dotnet-persistence-fundamentals/table-of-contents).

I have pretty much followed the course material except for these points:

- Use the .NET Core 2.0 platform
- Updated to version 1.3.1 of `Akka` and `Akka.Persistence.SqlServer`
- Configured and used `Akka.Serialization.Hyperion` latest pre-release. This got rid of the annoying deprecation warning.
- Used Serilog for console logging rather than the custom logging used in the course.
- Configure to use a full-blown SqlServer, but you can easily change the connection string in the hocon file.