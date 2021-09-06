exec dotnet run --framework net5.0 -p MDPBrokerProcess/MDPBrokerProcess.csproj -v & 
exec dotnet run --framework net5.0 -p MDPWorkerExample/MDPWorkerExample.csproj -v &
exec dotnet run --framework net5.0 -p MDPClientExample/MDPClientExample.csproj -r100 