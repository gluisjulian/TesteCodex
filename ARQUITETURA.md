# Sistema de gestão de perfumes

## Escopo

Primeira entrega funcional: fornecedores, insumos, ofertas por apresentação e histórico de preços. Estoque, compras, fórmulas, produção, vendas, financeiro e autenticação ficam para futuras entregas. A base inicial não contém esses módulos nem dados fictícios.

## Arquitetura

Monólito modular simples, com uma API ASP.NET Core 10 e uma aplicação React/Vite independente. Controllers traduzem HTTP; Services validam e executam casos de uso; DTOs definem contratos; Entities e Configurations definem persistência. EF Core acessa SQL Server diretamente, sem repositório genérico ou camadas adicionais. Novos módulos poderão adicionar serviços e entidades mantendo os identificadores atuais.

```text
Perfumes.slnx
backend/
  Perfumes.Api.csproj
  Program.cs
  Controllers/
  Services/
  DTOs/
  Entities/
  Data/
  Configurations/
  Migrations/
frontend/                    # React será criado na etapa 10
  src/                      # Estrutura futura
    components/
    pages/
    services/
    layouts/
```

## Modelo proposto do banco

Todos os IDs serão bigint/long. Datas serão datetimeoffset em UTC, controladas pelo servidor. DataAtualizacao será inicialmente igual a DataCadastro.

| Tabela | Campos principais | Restrições |
| --- | --- | --- |
| Fornecedor | Id, RazaoSocial(200), NomeFantasia(200), CpfCnpj(14), Telefone(30), Email(254), Site(500), Observacao(2000), Ativo, DataCadastro, DataAtualizacao | RazaoSocial obrigatória; documento opcional, somente dígitos; índice único filtrado para documento informado |
| Insumo | Id, Nome(200), Descricao(2000), TipoInsumo, UnidadeMedida, Densidade, Observacao(2000), Ativo, DataCadastro, DataAtualizacao | Nome obrigatório; densidade opcional decimal(18,6), positiva |
| FornecedorProduto | Id, FornecedorId, InsumoId, CodigoProdutoFornecedor(100), QuantidadeEmbalagem, UnidadeEmbalagem, QuantidadeBase, UnidadeBase, Ativo, DataCadastro, DataAtualizacao | FKs obrigatórias; quantidade decimal(18,6) positiva; índice único fornecedor/insumo/quantidade base/unidade base |
| FornecedorProdutoPreco | Id, FornecedorProdutoId, Preco, DataInicio, DataFim, DataCadastro | Preço decimal(18,2) positivo; DataFim opcional; índice único por oferta filtrado por DataFim IS NULL; DataFim > DataInicio |

Relacionamentos: Fornecedor 1:N FornecedorProduto; Insumo 1:N FornecedorProduto; FornecedorProduto 1:N FornecedorProdutoPreco. Todos com DeleteBehavior.Restrict. Nenhum endpoint de exclusão física será exposto. Registros inativos permanecem consultáveis e seus históricos são preservados.

## Regras e ambiguidades resolvidas

1. O enum inicial incluirá ML, G, UN, L e KG, pois os exemplos exigem litros e quilogramas. Insumo usa apenas ML/G/UN como unidade base. O serviço de unidades converte L para ML e KG para G e rejeita dimensões incompatíveis. Densidade não será usada implicitamente para converter massa e volume; sua unidade será g/ml.
2. QuantidadeBase e UnidadeBase são campos adicionais internos necessários para reconhecer apresentações equivalentes, como 1 L e 1000 ML. Código do fornecedor não torna uma apresentação duplicada válida. Ofertas inativas também conservam sua identidade; deverão ser reativadas.
3. Custo unitário será calculado no backend com decimal, preservando precisão no cálculo e retornando até seis casas decimais. O frontend apenas formata valores. Valores com precisão superior à permitida serão rejeitados, evitando arredondamentos silenciosos ao persistir.
4. Oferta com histórico não poderá trocar insumo ou apresentação. Uma apresentação diferente cria outra oferta; a anterior pode ser desativada. Isso evita reinterpretar o histórico antigo ao editar a quantidade. Código e status continuam editáveis. A unidade base do insumo também não muda quando há ofertas vinculadas.
5. Vigências serão intervalos [DataInicio, DataFim), com início definido pelo servidor, sem agendamento ou retroatividade nesta etapa. Atualização de preço encerra o vigente e insere outro na mesma transação. A transação serializa a alteração por oferta; o índice filtrado garante a integridade adicionalmente. Conflitos de gravação serão retornados como HTTP 409.
6. Oferta e preço inicial serão persistidos juntos. Não haverá oferta criada pela interface sem preço. Preço zero não será permitido nesta etapa.
7. CPF/CNPJ é opcional. Quando informado, será normalizado e validado ao menos pelo formato de 11/14 dígitos, conforme o requisito. A chave única também protege contra gravações concorrentes. Documentos novos com formato distinto exigem revisão explícita dessa regra.
8. Listas terão paginação limitada, busca e filtros de status. Respostas usarão DTOs; falhas usarão ProblemDetails, com 400 para validação, 404 para ausência e 409 para conflitos.
9. Configuração do banco virá de ConnectionStrings__DefaultConnection ou user-secrets; nenhuma senha será incluída. CORS terá origens explícitas. Autenticação futura poderá usar ASP.NET Core Identity/OIDC sem inventar gestão de senhas.
10. Dashboard inicial será apenas um resumo dos cadastros existentes. Nenhum indicador de estoque, vendas ou financeiro será apresentado como implementado.

## API planejada

- GET/POST /api/fornecedores; GET/PUT /api/fornecedores/{id}; PATCH /api/fornecedores/{id}/status.
- GET/POST /api/insumos; GET/PUT /api/insumos/{id}; PATCH /api/insumos/{id}/status.
- GET/POST /api/fornecedores/{fornecedorId}/produtos; PUT /api/fornecedores/{fornecedorId}/produtos/{id}; PATCH equivalente para status.
- GET/POST /api/fornecedor-produtos/{id}/precos; GET /api/fornecedor-produtos/{id}/preco-atual.
- Consulta futura de comparação usará InsumoId e custo normalizado sem reformular o modelo.

## Fontes técnicas

- OpenAPI oficial ASP.NET Core: https://learn.microsoft.com/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-10.0
- Índices EF Core: https://learn.microsoft.com/ef/core/modeling/indexes

Esses mecanismos serão configurados nas etapas correspondentes, após restaurar as dependências.
