# DDD

## Running locally

docker run -d -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Sample123$' -p 1433:1433 --name local-sql samuelmarks/mssql-server-fts-sqlpackage-linux

docker run -d --hostname my-rabbit --name local-rabbit -p 5672:5672 -p 8080:15672 rabbitmq:3-management

docker run -d --name local-jaeger -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 -p 5775:5775/udp -p 6831:6831/udp -p 6832:6832/udp -p 5778:5778 -p 16686:16686 -p 14268:14268 -p 14250:14250 -p 9411:9411 jaegertracing/all-in-one:latest

echo 'admin' | docker run --rm -i datalust/seq config hash

mkdir -p seq

docker run --name local-seq -d --restart unless-stopped -e ACCEPT_EULA=Y -e SEQ_FIRSTRUN_ADMINPASSWORDHASH="FJQJVTSpRudsDE9JslbRhkIJKf5wvUtkjQY0JnvVC7y2+f/IjA==" -v seq:/data -p 80:80 -p 5341:5341 datalust/seq