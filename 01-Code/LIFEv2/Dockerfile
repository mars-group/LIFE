FROM artifactory.mars.haw-hamburg.de:5000/microsoft/dotnet:1.0.0-preview2.1-sdk

MAINTAINER Christian Hüning <christian.huening@haw-hamburg.de>

RUN mkdir -p /life

COPY . /life

RUN dotnet restore -s https://artifactory.mars.haw-hamburg.de/artifactory/api/nuget/nuget-virtual/

WORKDIR /life/LIFECompleteStarter

RUN rm -rf ./config

RUN dotnet build

ENTRYPOINT ["dotnet", "run", "-m", "./model", "--sc", "88840273-3c90-421e-8ff5-25cf952e672b", "--mca", "dock-three.mars.haw-hamburg.de:8080"]



#dotnet run -m ./model --sc 88840273-3c90-421e-8ff5-25cf952e672b --mca dock-three.mars.haw-hamburg.de:8080