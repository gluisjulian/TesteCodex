# Gestão de perfumes

ASP.NET Core 10 + EF Core 10 + SQL Server, com frontend separado em React, Vite, JavaScript, Axios, React Router e Bootstrap 5.

Implementados os arquivos reais de fornecedores, insumos, apresentações e histórico de preços. Conversões e custos pertencem ao backend. Preços anteriores são preservados; transações e índices protegem ofertas e vigências.

Consulte ARQUITETURA.md para o modelo e ETAPAS.md para arquivos e validação por etapa.

## Executar com LocalDB

Requisitos: SDK .NET 10, Node 22.12+ (validado com Node 24), npm e LocalDB. Execute da raiz do projeto:

```powershell
dotnet restore
dotnet tool restore
dotnet build tests/Perfumes.Verificacoes/Perfumes.Verificacoes.csproj --no-restore
dotnet run --project tests/Perfumes.Verificacoes --no-build
SqlLocalDB start MSSQLLocalDB
$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=PerfumesCodexTeste;Trusted_Connection=True;TrustServerCertificate=True'
dotnet ef database update --project backend
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project backend --no-launch-profile --urls http://localhost:5080
```

Autenticação integrada local, sem senha no código. Para outro banco, configure ConnectionStrings__DefaultConnection externamente. A factory de migrations usa essa variável. Migration não é aplicada automaticamente.

Em outro terminal:

```powershell
cd frontend
npm ci
npm run dev
```

Abra http://localhost:5173. A API padrão é http://localhost:5080/api; VITE_API_URL permite configurar outra URL. CORS aceita http://localhost:5173; configure Cors:Origens para outras origens.

## API

Documento OpenAPI JSON: http://localhost:5080/openapi/v1.json em Development. Não há Swagger UI nesta entrega. /health verifica execução, sem testar banco. Sem conexão configurada, cadastros retornam 503 com ProblemDetails.

- GET/POST /api/fornecedores e /api/insumos; GET/PUT /{id}; PATCH /{id}/status.
- GET/POST /api/fornecedores/{id}/produtos; GET/PUT /api/fornecedores/{id}/produtos/{produtoId}; PATCH /status da oferta.
- GET/POST /api/fornecedor-produtos/{id}/precos; GET /api/fornecedor-produtos/{id}/preco-atual.

Listas usam pagina, tamanhoPagina (1–100) e filtros busca/ativo nos cadastros. Enums JSON usam strings: MateriaPrima/Embalagem e ML/G/UN/L/KG. Insumos aceitam somente ML/G/UN como base. Densidade opcional em g/ml não provoca conversão implícita entre massa e volume.

Oferta com histórico não muda insumo/apresentação; crie outra e desative a anterior. Documento CPF/CNPJ é opcional, normalizado e validado por formato de 11/14 dígitos; não verifica dígitos verificadores. Não existem endpoints de exclusão física.

## Verificar

Com API executando:

```powershell
./scripts/Verificar-Api.ps1
# Somente banco de teste: cria registros e preserva-os.
./scripts/Verificar-Integracao.ps1
npm run build --prefix frontend
```

Vite usa carregamento nativo da configuração e preserveSymlinks para evitar o subprocesso de descoberta de unidades de rede do Windows no ambiente restrito.

## Validação em 06/10/2026

- API e projeto de verificações compilados sem erros ou avisos.
- 20 verificações passaram: conversões, custo, precisão, documento, índices e delete restrito.
- Migration Inicial, designer, snapshot e script SQL gerados.
- Health e OpenAPI verificados por HTTP; cadastro inválido e enum inválido retornaram 400. Sem conexão, confirmado 503 com ProblemDetails.
- Frontend instalado e build de produção concluído.

O cliente .NET falhou ao acessar NuGet neste ambiente. Obtivemos os pacotes diretamente do NuGet por HTTPS com certificados validados, em feed temporário fora do repositório. A validação TLS não foi desabilitada.

**Integração pendente:** o ambiente restrito não consegue iniciar/acessar a configuração do LocalDB no registro do Windows. database update falhou antes de conectar. Nenhum banco foi confirmado como criado. CRUD persistido, rollback e concorrência real ainda não foram verificados. O script de integração está pronto, mas não foi executado com sucesso. Inicie LocalDB no PowerShell do usuário e execute os comandos acima.

Estoque, compras, fórmulas, produção, vendas, financeiro e autenticação seguem fora do escopo. A API não tem controle de acesso nesta fase; execute localmente até adicionar autenticação/autorização.
