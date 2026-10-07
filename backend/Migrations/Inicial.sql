IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Fornecedores] (
    [Id] bigint NOT NULL IDENTITY,
    [RazaoSocial] nvarchar(200) NOT NULL,
    [NomeFantasia] nvarchar(200) NULL,
    [CpfCnpj] nvarchar(14) NULL,
    [Telefone] nvarchar(30) NULL,
    [Email] nvarchar(254) NULL,
    [Site] nvarchar(500) NULL,
    [Observacao] nvarchar(2000) NULL,
    [Ativo] bit NOT NULL,
    [DataCadastro] datetimeoffset NOT NULL,
    [DataAtualizacao] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Fornecedores] PRIMARY KEY ([Id])
);

CREATE TABLE [Insumos] (
    [Id] bigint NOT NULL IDENTITY,
    [Nome] nvarchar(200) NOT NULL,
    [Descricao] nvarchar(2000) NULL,
    [TipoInsumo] int NOT NULL,
    [UnidadeMedida] int NOT NULL,
    [Densidade] decimal(18,6) NULL,
    [Observacao] nvarchar(2000) NULL,
    [Ativo] bit NOT NULL,
    [DataCadastro] datetimeoffset NOT NULL,
    [DataAtualizacao] datetimeoffset NOT NULL,
    CONSTRAINT [PK_Insumos] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Insumo_Densidade] CHECK ([Densidade] IS NULL OR [Densidade] > 0),
    CONSTRAINT [CK_Insumo_Tipo] CHECK ([TipoInsumo] IN (1,2)),
    CONSTRAINT [CK_Insumo_Unidade] CHECK ([UnidadeMedida] IN (1,2,3))
);

CREATE TABLE [FornecedorProdutos] (
    [Id] bigint NOT NULL IDENTITY,
    [FornecedorId] bigint NOT NULL,
    [InsumoId] bigint NOT NULL,
    [CodigoProdutoFornecedor] nvarchar(100) NULL,
    [QuantidadeEmbalagem] decimal(18,6) NOT NULL,
    [UnidadeEmbalagem] int NOT NULL,
    [QuantidadeBase] decimal(18,6) NOT NULL,
    [UnidadeBase] int NOT NULL,
    [Ativo] bit NOT NULL,
    [DataCadastro] datetimeoffset NOT NULL,
    [DataAtualizacao] datetimeoffset NOT NULL,
    CONSTRAINT [PK_FornecedorProdutos] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Oferta_Quantidade] CHECK ([QuantidadeEmbalagem] > 0 AND [QuantidadeBase] > 0),
    CONSTRAINT [CK_Oferta_Unidades] CHECK ([UnidadeEmbalagem] IN (1,2,3,4,5) AND [UnidadeBase] IN (1,2,3)),
    CONSTRAINT [FK_FornecedorProdutos_Fornecedores_FornecedorId] FOREIGN KEY ([FornecedorId]) REFERENCES [Fornecedores] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FornecedorProdutos_Insumos_InsumoId] FOREIGN KEY ([InsumoId]) REFERENCES [Insumos] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [FornecedorProdutoPrecos] (
    [Id] bigint NOT NULL IDENTITY,
    [FornecedorProdutoId] bigint NOT NULL,
    [Preco] decimal(18,2) NOT NULL,
    [DataInicio] datetimeoffset NOT NULL,
    [DataFim] datetimeoffset NULL,
    [DataCadastro] datetimeoffset NOT NULL,
    CONSTRAINT [PK_FornecedorProdutoPrecos] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Preco_Valor] CHECK ([Preco] > 0),
    CONSTRAINT [CK_Preco_Vigencia] CHECK ([DataFim] IS NULL OR [DataFim] > [DataInicio]),
    CONSTRAINT [FK_FornecedorProdutoPrecos_FornecedorProdutos_FornecedorProdutoId] FOREIGN KEY ([FornecedorProdutoId]) REFERENCES [FornecedorProdutos] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Fornecedores_CpfCnpj] ON [Fornecedores] ([CpfCnpj]) WHERE [CpfCnpj] IS NOT NULL;

CREATE INDEX [IX_Fornecedores_RazaoSocial] ON [Fornecedores] ([RazaoSocial]);

CREATE UNIQUE INDEX [IX_FornecedorProdutoPrecos_FornecedorProdutoId] ON [FornecedorProdutoPrecos] ([FornecedorProdutoId]) WHERE [DataFim] IS NULL;

CREATE UNIQUE INDEX [IX_FornecedorProdutoPrecos_FornecedorProdutoId_DataInicio] ON [FornecedorProdutoPrecos] ([FornecedorProdutoId], [DataInicio]);

CREATE UNIQUE INDEX [IX_FornecedorProdutos_FornecedorId_InsumoId_QuantidadeBase_UnidadeBase] ON [FornecedorProdutos] ([FornecedorId], [InsumoId], [QuantidadeBase], [UnidadeBase]);

CREATE INDEX [IX_FornecedorProdutos_InsumoId] ON [FornecedorProdutos] ([InsumoId]);

CREATE INDEX [IX_Insumos_Nome] ON [Insumos] ([Nome]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261006192106_Inicial', N'10.0.11');

COMMIT;
GO

