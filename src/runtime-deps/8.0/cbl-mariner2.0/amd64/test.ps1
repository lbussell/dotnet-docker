docker build -t sizetest -f "C:\sources\dotnet-docker\src\runtime-deps\8.0\cbl-mariner2.0\amd64\Dockerfile" .
docker image inspect sizetest
