# Itaú Backend Test

Projeto criado para o teste de Backend ([https://github.com/cairano/backend-test](https://github.com/cairano/backend-test)).


## Execução

Você poderá testar e executar o projeto através dos arquivos **Run.cmd & Test.cmd**, que estão na pasta da solução.

## Stack

 - C# & .NET Core 2.1
 - Microsoft Visual Studio 2019
 - NuGet Package Newtonsoft.Json(Utilizado para fazer a leitura dos JSON retornados pela API)

## Estrutura do Projeto
### Core
 - Projeto contendo toda a base que será utilizada entre os componentes, como Models, Services, Helpers, etc.
### Application
 - Contém a maior parte da lógica da aplicação. É a que recebe os dados, processa os requisitos, e exibe no Console.
### Data.Transactions
 - Obtém os dados das transações locais(transactions.log)
### Data.Transactions.API
 - Obtém as transações da API, através do Gateway informado, processa e valida os dados.
### Services.Transactions
 - Executa o carregamento das transações, e combina os dados para processamento.
### Tests
 - Solução contendo todos os testes da aplicação.

## Lógica
### Core
Começando pelos Models, temos o **TransactionData**, que é o modelo que representa uma transação. 
Na pasta **Services**, temos as principais interfaces da Aplicação, sendo elas:

 1. **ITransactionReader**: Interface para ler transações. Todas as classes responsáveis por carregar transações(seja da API ou Local), devem herdar dessa interface.
 2. **ITransactionService**: Interface para realizar o carregamento e a combinação das transações. Recebe uma lista de *ITransactionReader*, e executa.

Na pasta *ActionsResult*, temos a classe **Result**. Essa classe, representa o resultado de uma ação. É usada para indicar que uma ação, pode ou não, ter sucesso. 

### Application
Inicializa nossa ApiGateway, passando como parâmetro a struct ApiConfig, que contém dados de configurações do nosso Gateway.

Após inicializar o Gateway, criamos uma **TransactionService**, e passamos como parâmetro uma lista de *ITransactionReader*, nessa lista temos o *TransactionApiReader* (ler as transações da Api), e o *TransactionLogReader*(Ler as transações locais, do arquivo transactions.log). 

Após inicializarmos o *TransactionService*, nós inicializamos o serviço **TransactionApplicationService**, que contém a maior parte da lógica da nossa aplicação. 

Para fins de organização, a exibição de dados no console está separada em métodos, que são uma **Extension** da **TransactionApplicationService**, e são chamados no método Main, do *Program.cs*.