FROM artifactory.mars.haw-hamburg.de:5000/microsoft/dotnet-nightly:rel-1.0.0-core-deps

MAINTAINER Christian Hüning <christian.huening@haw-hamburg.de>


RUN mkdir -p /life

COPY ./bin/Release/netcoreapp1.0/debian.8-x64/publish/ /life

WORKDIR /life

ENTRYPOINT ["./LIFECompleteStarter"]