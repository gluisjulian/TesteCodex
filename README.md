# Gestão de perfumes — entrega da etapa 1

A arquitetura, modelo de banco, relacionamentos e decisões estão em [ARQUITETURA.md](ARQUITETURA.md). Apenas a estrutura inicial foi implementada. Os cadastros, persistência, migration, histórico e frontend ainda não foram implementados.

## Etapa 1 — concluída

Criados: Perfumes.slnx, backend/Perfumes.Api.csproj, backend/Program.cs, backend/appsettings.json, backend/appsettings.Development.json, backend/Properties/launchSettings.json, backend/Perfumes.Api.http, diretórios Controllers, Services, DTOs, Entities, Data, Configurations e Migrations, frontend/README.md, .gitignore, ARQUITETURA.md e este README.

Arquivos gerados ajustados: Program.cs recebeu ProblemDetails e /health; Perfumes.Api.csproj mantém net10.0 e nullable reference types, sem pacotes externos nesta etapa. Removidos os exemplos WeatherForecast.cs e Controllers/WeatherForecastController.cs do template. O arquivo HTTP foi adaptado para /health. Não existia aplicação anterior no diretório.

Verificação: dotnet build Perfumes.slnx terminou com zero erros e zero avisos no SDK .NET 10 instalado. OpenAPI será acrescentado na etapa 9, conforme a ordem solicitada.

Execute no diretório outputs:

```powershell
dotnet build Perfumes.slnx
dotnet run --project backend/Perfumes.Api.csproj --no-launch-profile --urls http://localhost:5080
```

Consulte GET http://localhost:5080/health. Testado com resposta {"status":"ok"}. Esse endpoint confirma somente que a API está executando; ainda não verifica banco. O logging usa console para não depender de permissão de escrita no Event Log do Windows.

## Etapa 2 — bloqueada antes de alterar a aplicação

O teste de restauração de Microsoft.EntityFrameworkCore.SqlServer 10.0.11 foi feito em um projeto temporário fora da entrega. Falhou ao resolver dependências Microsoft.Extensions.Logging, Microsoft.Extensions.Caching.Memory e Microsoft.Extensions.Configuration.Abstractions, com NU1301 e falha HTTPS do Windows: SEC_E_NO_CREDENTIALS / Credenciais não disponíveis no pacote de segurança. A falha persistiu após conceder acesso à rede e também ocorreu no curl. O cache local não resolveu a restauração normal.

A aplicação entregue permanece compilável. Nenhuma etapa posterior foi iniciada. Será necessário corrigir o acesso HTTPS do NuGet no ambiente para restaurar os pacotes. Não foi desabilitada a validação de certificados.

Depois de corrigir esse acesso, a continuação começará por configurar EF Core/SQL Server e uma connection string via user-secrets ou variável ConnectionStrings__DefaultConnection. Será necessário indicar a instância SQL Server desejada antes de criar o banco. Não há credenciais armazenadas no projeto e nenhum banco foi criado.

Etapas pendentes: 2 configuração EF/SQL Server; 3 entidades; 4 Fluent API; 5 migration/banco; 6 DTOs; 7 serviços; 8 endpoints; 9 OpenAPI; 10 React; 11 Axios/Router; 12 layout; 13 fornecedores; 14 insumos; 15 ofertas; 16 histórico; 17 validações; 18 revisão.
