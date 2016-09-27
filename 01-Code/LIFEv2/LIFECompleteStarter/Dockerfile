FROM artifactory.mars.haw-hamburg.de:5000/microsoft/dotnet-nightly:rel-1.0.0-sdk


RUN mkdir -p /dotnetapp
WORKDIR /dotnetapp

ENTRYPOINT ["dotnet", "run"]

ONBUILD COPY . /dotnetapp
ONBUILD RUN dotnet restore -s https://artifactory.mars.haw-hamburg.de/artifactory/api/nuget/nuget-virtual