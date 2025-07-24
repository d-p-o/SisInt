# SisInt - Sistema Integrado de Planejamento de Recursos Empresariais

---

O **SisInt** será um **ERP (Enterprise Resource Planning)** moderno, desenvolvido do zero com uma **arquitetura de microsserviços** robusta. Sua concepção visa **flexibilidade, escalabilidade e resiliência**, características essenciais para ambientes de negócios complexos.

---

## 🚀 Visão Geral da Arquitetura e Tecnologias

A base do SisInt é sua arquitetura de **microsserviços**, que promove modularidade e facilita o desenvolvimento e a manutenção. Para gerenciar as requisições entre esses serviços, o sistema utilizará o **API Gateway Kong**.

### 🛠️ Tecnologias Principais

As principais tecnologias que impulsionam o SisInt incluem:

* **Back-end:**
    * **.NET 8**
    * **Entity Framework Core** (ORM)
    * **JwtBearer** (autenticação)
    * **Swashbuckle** (documentação de API)
* **Front-end:**
    * **React**
    * **Vite**
    * **Node.js** e **npm**
* **Contêineres e Orquestração:**
    * **Docker**
    * **Docker Compose**
    * **Docker Desktop**
* **Autenticação e Autorização:**
    * **Keycloak** (gestão de identidade e acesso)
* **Banco de Dados Relacional:**
    * **Microsoft SQL Server 2022** (operando em Docker)
* **Mensageria:**
    * **RabbitMQ** (comunicação assíncrona)
* **Comunicação em Tempo Real:**
    * **SignalR** (atualizações instantâneas)
* **Banco de Dados NoSQL:**
    * **MongoDB**

---

## ⚙️ Configuração e Ambiente de Desenvolvimento

As ferramentas atualmente configuradas e operacionais no ambiente **Windows 11 Home (64 bits, Intel x64)** são:

* **Banco de Dados:** **Microsoft SQL Server 2022** (banco 'SisInt') acessível via SSMS 21 em `localhost,1433` (usuário: `sa` / senha: `5@L0ca1h`).
* **Contêineres:** **Docker** (v28.3.0), **Docker Compose** (v2.38.2) e **Docker Desktop** (4.43.2).
* **IDE:** **Visual Studio 2022 Community** (v17.14.8) com pacotes '.NET' (ASP/Web, Multiplataforma, Desktop).
* **Back-end:** **.NET Core 8 LTS** (SDK v9.0.302).
* **Front-end:** **Node.js** (v22.17.0) e **npm** (v11.4.2).
* **Segurança:** **Keycloak** (realm: `sisint-realm`, client: `sisint-auth-service`, segredo: `ylpwaJVLL0Ya3VeoSHtfPQUhVDbsy2F2`, user: `admin` / `@dm1N`).
* **Mensageria:** **RabbitMQ**.
* **Testes:** Atualmente, testes manuais são realizados por **HTTP** na solução e via **Postman**.

---

## 🌐 Serviços Ativos e Acessíveis via Docker Compose

Os seguintes serviços estão configurados e podem ser acessados após levantar o `docker-compose.yml`:

* **auth-service (Back-end):** `0.0.0.0:5000` (HTTP) e `0.0.0.0:5001` (HTTPS)
* **frontend (Front-end):** `0.0.0.0:5173`
* **keycloak:** `0.0.0.0:8080` (imagem: `quay.io/keycloak/keycloak:latest`)
* **rabbitmq:** `0.0.0.0:15672` (imagem: `rabbitmq:3-management-alpine`, interface de gerenciamento)
* **sql:** `0.0.0.0:1433` (imagem: `mcr.microsoft.com/mssql/server:2022-latest`)

### Endereços para Acesso:

* **Keycloak:** [http://keycloak:8080/](http://keycloak:8080/) (ou via 127.0.0.1 no `hosts` para 'keycloak')
    * Usuário/Senha de Admin: `admin` / `admin`
* **RabbitMQ (Gerenciamento):** [http://localhost:15672/](http://localhost:15672/)
    * Usuário/Senha Padrão: `guest` / `guest`
* **Front-end:** [http://localhost:5173/](http://localhost:5173/)

---

## ✅ Testes Atuais Validados

Os seguintes testes foram validados com sucesso:

* **Autenticação Keycloak:** `POST http://localhost:8080/realms/sisint-realm/protocol/openid-connect/token` (OK)
* **Endpoint Público (AuthService):** `GET http://localhost:5000/TestAuth/public` (OK)
* **Endpoint Protegido (AuthService):** `GET http://localhost:5000/TestAuth` (OK)

---

## 🛣️ Próximos Passos e Roadmap

O roadmap do SisInt foca na integração e configuração de componentes essenciais para a sua funcionalidade completa:

* **Integração de Segurança:** Estabelecer a comunicação completa e segura entre **Keycloak, Front-end e Back-end**.
* **Testes Automatizados:** Implementar **xUnit** para testes unitários/integrados no Back-end e **Vitest** para o Front-end.
* **API Gateway:** Configurar e implantar o **Kong** como o ponto de entrada central para todos os microsserviços.
* **Integração de Mensageria:** Aprofundar a integração com **RabbitMQ** para comunicação assíncrona.
* **Comunicação em Tempo Real:** Incorporar o **SignalR** para funcionalidades interativas e atualizações instantâneas.
* **NoSQL:** Integrar o **MongoDB** para necessidades específicas de armazenamento.

O SisInt avança como um futuro ERP robusto e eficiente, com uma base tecnológica sólida e um plano claro para sua evolução.

---

Sinta-se à vontade para explorar o código e contribuir!
