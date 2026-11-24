# Mediasphere - социальная сеть

Микросервисная архитектура на ASP.NET Core с фронтендом на React. 5 независимых сервисов, API Gateway, своя БД у каждого, аутентификация и авторизация с помощью Keycloak, изображения в MinIO, взаимодействие между микросервисами через MassTransit и gRPC.

## Стек

**Backend:** C# 14, ASP.NET Core 10, Entity Framework Core, MediatR, JWT, YARP, MassTransit, Keycloak
**Database:** PostgreSQL 
**Storage:** MinIO
**Frontend:** React  
**DevOps:** Docker Compose  

## Архитектура

5 микросервисов + API Gateway + Shared:
```
Backend/
├── Comment/
├── Follow/
├── User/
├── Like/
├── Post/
├── Gateway/
├── Shared/
├── docker-compose.yml
└── .env
Frontend/frontend.client
```

## Быстрый старт

### Предварительные требования

- .NET 10 SDK
- Node.js 18+
- Docker Desktop

### Установка

1. Клонировать репозиторий:

```bash
git clone https://github.com/daniilfars/Mediasphere
```

2. Переименовать файл `.env.example` в `.env` и заменить на свои данные:

```env
POSTGRES_PASSWORD=your-password
MINIO_ROOT_USER=your-minioadmin
MINIO_ROOT_PASSWORD=your-minioadmin-password
```

3. Запустить все микросервисы:

```bash
docker compose up
```

4. Вставить настройки Keycloak:
   http://localhost:8080/ -> Вход через админа(по умолчанию admin/admin) -> Create realm -> Browse и выбрать файл keycloak-settings.json из корня проекта

5. Запустить фронтенд локально:

```bash
cd Frontend/frontend.client
npm install
npm run dev -- --port 63250
```

6. Открыть в браузере:

- Frontend: `http://localhost:63250`
- Swagger Post: `http://localhost:8081/swagger`
- Swagger User: `http://localhost:8082/swagger`
- Swagger Follow: `http://localhost:8083/swagger`
- Swagger Like: `http://localhost:8084/swagger`
- Swagger Comment: `http://localhost:8085/swagger`

## Сервисы

| Сервис | Порт |
|--------|------|
| API Gateway | 5000 |
| Post | 8081 |
| User | 8082 |
| Follow | 8083 |
| Like | 8084 |
| Comment | 8085 |
