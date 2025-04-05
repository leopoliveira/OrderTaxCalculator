# Order Tax Calculator

## Visão Geral

Esta aplicação, feita em .NET, gerencia e calcula impostos sobre pedidos recebidos de um sistema externo.

Após o processamento, os pedidos são disponibilizados para para integração com outro sistema externo.

O principal objetivo é fornecer uma solução robusta e escalável para lidar com um alto volume de pedidos diários, aplicando regras de cálculo de impostos configuráveis.

## Funcionalidades

* **Recepção de Pedidos:** Recebe novos pedidos via API REST.
* **Validação de Duplicidade:** Garante que pedidos com o mesmo `pedidoId` não sejam processados mais de uma vez.
* **Cálculo de Imposto:** Calcula o valor total do imposto do pedido baseado no valor total dos itens.
* **Consulta de Pedidos:** Consulta detalhada dos pedidos por seu Id e listagem dos pedidos com filtro por status..

## Requisitos Técnicos

* **Framework:** .NET 8 (LTS)
* **ORM:** Entity Framework
* **Arquitetura:** Aplicação dividida em camadas (API, Domain, Data).
* **Logging:** Serilog para registro de logs detalhados.
* **Banco de Dados:** Flexível (hoje é In-Memory para desenvolvimento/testes).
* **Qualidade de Código:** Segue princípios Clean Code, SOLID, DRY, YAGNI e Object Calisthenics.
* **Testes:**
    * **Unitários:** xUnit, FluentAssertions, Bogus, NSubstitute.
    * **Integração:** Testcontainers e SQL Server, ambos via Docker.
* **API:** RESTful.

## Pré-requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) ou superior.
* [Git](https://git-scm.com/).
* [Docker](https://www.docker.com/) **(Obrigatório se for executar os testes de integração com Testcontainers ou rodar a aplicação em contêiner).**
* Um IDE de sua preferência (Visual Studio, VS Code com C# Dev Kit, Rider).

## Instalação e Configuração

1.  **Clone o repositório:**
    ```bash
    git clone <URL_DO_SEU_REPOSITORIO>
    cd OrderTaxCalculator
    ```

2.  **Restaure as dependências:**
    ```bash
    dotnet restore
    ```

3.  **Configure o Banco de Dados:**
    * Como a aplicação usa um banco de dados em memória, não é necessária nenhuma configuração adicional nem execução de Migrations.
    * Caso você queira usar um banco de dados convencional, é necessário algumas configurações adicionais, tais como:
      * Configurar as variáveis de ambiente para receber uma ConnectionString;
      * Realizar a configuração do banco de dados escolhido em `OrderTaxCalculator.Data.ConfigurarServicos.ConfigurePedidoDbContext(this IServiceCollection service)`;
      * Criar as migrations e executá-las (caso opte pela abordagem code first).

4.  **Configure a Feature Flag:**
    * Localize a configuração da feature flag no arquivo `appsettings.development.json` (ou via variáveis de ambiente/outra fonte de configuração, caso deseje).
    * Ajuste o valor para habilitar/desabilitar o cálculo da reforma tributária conforme necessário. Exemplo:
        ```json
        {
          "FeatureManagement": {
            "AtivarCalculoImpostoReformaTributaria": false // ou true
          }
        }
        ```

## Executando a Aplicação

1.  **Navegue até o diretório do projeto da API:**
    ```bash
    cd src/OrderTaxCalculator.API
    ```

2.  **Execute a aplicação:**
    ```bash
    dotnet run
    ```

A API estará disponível nos endereços configurados (geralmente `http://localhost:5107` e `https://localhost:7104`, conforme configurado no arquivo `launchSettings.json`).

Verifique o output do console para os endereços exatos.

Se o Swagger estiver habilitado, você pode acessá-lo em `/swagger`.

## Usando a API

A API segue os padrões RESTful.

**Endpoints:**

1.  **Criar um novo Pedido**
    * **Endpoint:** `POST /api/v1/pedidos`
    * **Content-Type:** `application/json`
    * **Request Body:**
        ```json
        {
          "pedidoId": 12345,
          "clienteId": 678,
          "itens": [
            {
              "produtoId": 1001,
              "quantidade": 2,
              "valor": 52.70
            }
          ]
        }
        ```
    * **Response (Sucesso - 201 Created):**
        * **Headers:** `Location: /api/v1/pedidos/{pedidoId}`
        * **Body:**
            ```json
            {
              "id": 1, // ID interno gerado pela aplicação
              "status": "Criado"
            }
            ```
    * **Response (Erro - 400 Bad Request):** Retorna se os dados forem inválidos ou se o `pedidoId` já existir.
        ```json
        {
          "title": "Pedido Duplicado",
          "status": 400,
          "detail": "Pedido com ID 12345 já existe." // Mensagem detalhada
        }
        ```

2.  **Consultar um Pedido pelo ID Interno**
    * **Endpoint:** `GET /api/v1/pedidos/{pedidoId}`
    * **Exemplo:** `GET /api/v1/pedidos/12345`
    * **Response (Sucesso - 200 OK):**
        ```json
        {
          "id": 1,
          "pedidoId": 12345,
          "clienteId": 678,
          "imposto": 15.81,
          "itens": [
            {
              "produtoId": 1001,
              "quantidade": 2,
              "valor": 52.70
            }
          ],
          "status": "Criado"
        }
        ```
    * **Response (Erro - 404 Not Found):** Retorna se o pedido com o `pedidoId` fornecido não for encontrado.

3.  **Listar Pedidos (com filtro por status)**
    * **Endpoint:** `GET /api/v1/pedidos?status={status}`
    * **Exemplo:** `GET /api/v1/pedidos?status=Criado`
    * **Response (Sucesso - 200 OK):** Retorna uma lista de pedidos que correspondem ao status.
        ```json
        [
          {
            "id": 1,
            "pedidoId": 12345,
            "clienteId": 678,
            "imposto": 15.81,
            "itens": [ /* ... */ ],
            "status": "Criado"
          },
          // ... outros pedidos com status "Criado"
        ]
        ```
    * **Response (Erro - 400 Bad Request):** Se o status fornecido for inválido.

## Executando Testes

1.  **Navegue até o diretório raiz da solução (onde está o arquivo `.sln`) ou o diretório do projeto de teste (`OrderTaxCalculator.Test`).**

2.  **Execute os testes unitários e de integração:**
    * **Importante:** Para executar os testes de integração que utilizam `Testcontainers`, é **necessário ter o Docker instalado e em execução** no seu ambiente.
    ```bash
    dotnet test
    ```

## Práticas de Desenvolvimento

* **Git Flow:** Siga o modelo Git Flow para gerenciamento de branches (main, develop, feature/*, release/*, hotfix/*).
* **Commits Semânticos:** Utilize commits semânticos (ex: `feat: Adiciona endpoint de consulta`, `fix: Corrige cálculo de imposto`, `refactor: Melhora performance da validação`).
