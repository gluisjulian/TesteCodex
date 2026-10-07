# Arquivos e validação por etapa — 06/10/2026

Nenhum erro de compilação foi deixado na entrega. O único bloqueio restante é executar operações persistidas no LocalDB dentro do ambiente restrito.

| Etapa | Arquivos criados ou alterados | Resultado / comando |
| --- | --- | --- |
| 1 — estrutura | Alterados Perfumes.slnx e .gitignore; criado .config/dotnet-tools.json | Solution mantém backend separado; exclusão de bin/obj/node_modules/dist/.vs. dotnet build Perfumes.slnx -m:1 |
| 2 — EF/SQL | Alterados backend/Perfumes.Api.csproj e Program.cs; criados Data/ApplicationDbContext.cs e ApplicationDbContextFactory.cs | EF Core SQL Server 10.0.11 restaurado; configuração externa. dotnet restore |
| 3 — entidades | Entities/Registro.cs, Enums.cs, Fornecedor.cs, Insumo.cs, FornecedorProduto.cs, FornecedorProdutoPreco.cs | Tipos decimal, datas UTC, relações explícitas e status lógico; compilação aprovada |
| 4 — Fluent API | Configurations/FornecedorConfiguration.cs, InsumoConfiguration.cs, FornecedorProdutoConfiguration.cs, FornecedorProdutoPrecoConfiguration.cs | Precisões, índices filtrados, apresentação normalizada única, restrições e delete restrito |
| 5 — migration | Migrations/20261006192106_Inicial.cs, respectivo Designer.cs, ApplicationDbContextModelSnapshot.cs e Inicial.sql | Gerados pelo EF; criação de banco ainda pendente. dotnet tool restore; dotnet ef database update --project backend |
| 6 — DTOs | DTOs/FornecedorDto.cs, InsumoDto.cs, FornecedorProdutoDto.cs, PrecoDto.cs, ComunsDto.cs, Mapeamentos.cs | DTOs de gravação compartilhados entre create/update quando os campos são iguais; entidades não expostas por endpoints |
| 7 — regras | Services/FornecedorService.cs, InsumoService.cs, FornecedorProdutoService.cs, PrecoService.cs, UnidadeService.cs, RegraException.cs | Cadastros, precisão, apresentações, transações e vigências; conversão L/ML e KG/G centralizada |
| 8 — endpoints | Controllers/FornecedoresController.cs, InsumosController.cs, FornecedorProdutosController.cs e PrecosController.cs | REST com async/await, paginação, 201/204 e validação automática |
| 9 — OpenAPI | Alterados Program.cs e csproj | Documento /openapi/v1.json em Development testado; sem UI Swagger |
| 10 — React | frontend/package.json, package-lock.json, index.html e vite.config.js | Dependências instaladas; npm ci; npm run build |
| 11 — Axios/Router | frontend/.env.example, src/main.jsx, App.jsx e services/api.js | Rotas separadas e mensagens ProblemDetails; endereço da API configurável |
| 12 — layout | src/styles.css e components/Comuns.jsx | Menu responsivo, loading, erros, diálogo nativo acessível, paginação e badges |
| 13 — fornecedores | src/pages/Cadastros.jsx | Cadastro, edição, visualização, filtro, busca e ativação; usa API real |
| 14 — insumos | src/pages/Cadastros.jsx | Tipo, unidade base, densidade opcional e status; usa API real |
| 15 — ofertas | src/pages/ProdutosFornecedor.jsx | Seleção de insumo com busca, apresentações, preço inicial e código; backend grava oferta/preço em uma transação |
| 16 — histórico | ProdutosFornecedor.jsx e Services/PrecoService.cs | Novo preço encerra o anterior; histórico paginado exibe início, fim e vigente; backend bloqueia escritores por oferta |
| 17 — erros | Services/ApiExceptionHandler.cs, DTOs e componentes React | ProblemDetails, HTTP 400/404/409/503 e erros visíveis; sem detalhes internos nas respostas 500 |
| 18 — revisão | tests/Perfumes.Verificacoes/*, scripts/Verificar-Api.ps1, Verificar-Integracao.ps1, README.md, ARQUITETURA.md, frontend/README.md | 20 verificações unitárias/metadados passaram; health/OpenAPI e 400 testados; build do frontend passou. Integração SQL real pendente |

## Limites da verificação

O script de integração cria dados em banco de teste e verifica duplicações equivalentes, histórico, intervalos contíguos, preço inválido, proteção da unidade base e desativação. Está preparado, mas não executado com sucesso devido ao bloqueio do LocalDB. A atomicidade e serialização foram implementadas, porém ainda não comprovadas por testes com o servidor real. A interface foi compilada, mas os fluxos completos de gravação dependem dessa integração.

Para rodar as verificações sem banco: dotnet run --project tests/Perfumes.Verificacoes. Para verificar API executando: ./scripts/Verificar-Api.ps1. Para integração após iniciar LocalDB e aplicar migration: ./scripts/Verificar-Integracao.ps1.
