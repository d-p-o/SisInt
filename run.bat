docker compose down
timeout /t 1
docker compose up --build -d
timeout /t 1
docker compose ps
timeout /t 1
docker ps -a
timeout /t 20
.\keycloak8080.url
.\rabbitmq15672.url
.\frontend5173.url