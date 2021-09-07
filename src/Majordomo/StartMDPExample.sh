exec dotnet run --framework net5.0 -p MDPBrokerProcess/MDPBrokerProcess.csproj & 
exec dotnet run --framework net5.0 -p MDPWorkerExample/MDPWorkerExample.csproj &
exec dotnet run --framework net5.0 -p MDPClientExample/MDPClientExample.csproj -r10000 