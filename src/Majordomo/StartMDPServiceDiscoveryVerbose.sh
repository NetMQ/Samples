exec dotnet run --framework net5.0 -p MDPBrokerProcess/MDPBrokerProcess.csproj & 
exec dotnet run --framework net5.0 -p MDPWorkerExample/MDPWorkerExample.csproj -v &
exec dotnet run --framework net5.0 -p MDPServiceDiscoveryClientExample/MDPServiceDiscoveryClientExample.csproj -v