dotnet test .\DddEfteling.Tests\DddEfteling.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
dotnet build-server shutdown
dotnet sonarscanner /k:"EF" /d:sonar.host.url="http://192.168.1.246:9000" /d:sonar.coverage.exclusions="**Test*.cs,**/Program.cs,**/Startup.cs"  /d:sonar.cs.opencover.reportsPaths=DddEfteling.Tests/coverage.opencover.xml
dotnet build .
dotnet sonarscanner end
