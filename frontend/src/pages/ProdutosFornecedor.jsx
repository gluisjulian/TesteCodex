import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { api, mensagemErro, moeda, dataHora } from "../services/api";
import {
  Erro,
  Carregando,
  Status,
  Paginacao,
  Modal,
  Campo,
} from "../components/Comuns";
export default function ProdutosFornecedor() {
  const { id } = useParams();
  const [fornecedor, setFornecedor] = useState(null),
    [lista, setLista] = useState(null),
    [pagina, setPagina] = useState(1),
    [versao, setVersao] = useState(0),
    [erro, setErro] = useState("");
  const [modal, setModal] = useState(null),
    [form, setForm] = useState({}),
    [ocupado, setOcupado] = useState(false),
    [erroForm, setErroForm] = useState("");
  const [insumos, setInsumos] = useState([]),
    [busca, setBusca] = useState(""),
    [historico, setHistorico] = useState(null),
    [paginaHistoria, setPaginaHistoria] = useState(1);
  const path = "/fornecedores/" + id + "/produtos";
  useEffect(() => {
    let ativo = true;
    setLista(null);
    setErro("");
    Promise.all([
      api.get("/fornecedores/" + id),
      api.get(path, { params: { pagina } }),
    ])
      .then(([f, p]) => {
        if (ativo) {
          setFornecedor(f.data);
          setLista(p.data);
        }
      })
      .catch((e) => {
        if (ativo) setErro(mensagemErro(e));
      });
    return () => {
      ativo = false;
    };
  }, [id, path, pagina, versao]);
  useEffect(() => {
    if (modal?.tipo !== "novo") return;
    let ativo = true;
    const timer = setTimeout(() => {
      api
        .get("/insumos", { params: { ativo: true, busca, tamanhoPagina: 20 } })
        .then((r) => {
          if (ativo) setInsumos(r.data.itens);
        })
        .catch((e) => {
          if (ativo) setErroForm(mensagemErro(e));
        });
    }, 250);
    return () => {
      ativo = false;
      clearTimeout(timer);
    };
  }, [modal?.tipo, busca]);
  useEffect(() => {
    if (modal?.tipo !== "historico") return;
    let ativo = true;
    setHistorico(null);
    api
      .get("/fornecedor-produtos/" + modal.item.id + "/precos", {
        params: { pagina: paginaHistoria },
      })
      .then((r) => {
        if (ativo) setHistorico(r.data);
      })
      .catch((e) => {
        if (ativo) setErroForm(mensagemErro(e));
      });
    return () => {
      ativo = false;
    };
  }, [modal, paginaHistoria]);
  function abrir(tipo, item) {
    setErroForm("");
    setPaginaHistoria(1);
    setBusca("");
    setInsumos([]);
    setModal({ tipo, item });
    setForm(
      item
        ? { ...item, preco: "" }
        : {
            insumoId: "",
            codigoProdutoFornecedor: "",
            quantidadeEmbalagem: "",
            unidadeEmbalagem: "ML",
            precoInicial: "",
            ativo: true,
          },
    );
  }
  const mudar = (key, value) => setForm((old) => ({ ...old, [key]: value }));
  async function salvar(e) {
    e.preventDefault();
    setErroForm("");
    setOcupado(true);
    try {
      if (modal.tipo === "preco")
        await api.post("/fornecedor-produtos/" + modal.item.id + "/precos", {
          preco: Number(form.preco),
        });
      else {
        const payload = {
          insumoId: Number(form.insumoId),
          codigoProdutoFornecedor: form.codigoProdutoFornecedor || null,
          quantidadeEmbalagem: Number(form.quantidadeEmbalagem),
          unidadeEmbalagem: form.unidadeEmbalagem,
          ativo: form.ativo,
        };
        if (modal.tipo === "novo") {
          payload.precoInicial = Number(form.precoInicial);
          await api.post(path, payload);
        } else await api.put(path + "/" + modal.item.id, payload);
      }
      setModal(null);
      setVersao((v) => v + 1);
    } catch (e) {
      setErroForm(mensagemErro(e));
    } finally {
      setOcupado(false);
    }
  }
  async function status(item) {
    try {
      await api.patch(path + "/" + item.id + "/status", { ativo: !item.ativo });
      setVersao((v) => v + 1);
    } catch (e) {
      setErro(mensagemErro(e));
    }
  }
  const titulos = {
    novo: "Adicionar produto fornecido",
    editar: "Editar oferta",
    preco: "Alterar preço",
    historico: "Histórico de preços",
  };
  return (
    <>
      <Link to="/fornecedores" className="d-inline-block mb-3">
        ← Fornecedores
      </Link>
      <div className="eyebrow">PRODUTOS FORNECIDOS</div>
      <h1>
        {fornecedor?.nomeFantasia || fornecedor?.razaoSocial || "Fornecedor"}
      </h1>
      {fornecedor && (
        <div className="card p-3 my-4">
          <div className="d-flex gap-3 align-items-center">
            <strong>{fornecedor.razaoSocial}</strong>
            <Status ativo={fornecedor.ativo} />
          </div>
          <p className="small text-secondary mt-2 mb-0">
            {[fornecedor.cpfCnpj, fornecedor.telefone, fornecedor.email]
              .filter(Boolean)
              .join(" · ") || "Sem contatos informados"}
          </p>
          {fornecedor.site && (
            <a href={fornecedor.site} target="_blank" rel="noreferrer">
              {fornecedor.site}
            </a>
          )}
          {fornecedor.observacao && (
            <p className="mt-2 mb-0">{fornecedor.observacao}</p>
          )}
        </div>
      )}
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h2 className="h4">Apresentações e preços</h2>
        <button
          className="btn btn-primary"
          disabled={!fornecedor?.ativo}
          onClick={() => abrir("novo")}
        >
          Adicionar produto
        </button>
      </div>
      <Erro texto={erro} />
      {!lista && !erro && <Carregando />}
      {lista && (
        <>
          <div className="card table-responsive">
            <table className="table align-middle mb-0">
              <thead>
                <tr>
                  {[
                    "Produto",
                    "Apresentação",
                    "Preço atual",
                    "Custo por unidade base",
                    "Última atualização",
                    "Status",
                    "Ações",
                  ].map((h) => (
                    <th key={h}>{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {lista.itens.map((item) => (
                  <tr key={item.id}>
                    <td>
                      <strong>{item.insumoNome}</strong>
                      <div className="small text-secondary">
                        {item.codigoProdutoFornecedor || "—"}
                      </div>
                    </td>
                    <td>
                      {item.quantidadeEmbalagem} {item.unidadeEmbalagem}
                    </td>
                    <td>{moeda(item.precoAtual)}</td>
                    <td>
                      {moeda(item.custoUnidadeBase, 6)}/{item.unidadeBase}
                    </td>
                    <td>{dataHora(item.dataAtualizacao)}</td>
                    <td>
                      <Status ativo={item.ativo} />
                    </td>
                    <td>
                      <div className="d-flex gap-2 flex-wrap">
                        <button
                          className="btn btn-sm btn-outline-secondary"
                          onClick={() => abrir("editar", item)}
                        >
                          Editar
                        </button>
                        <button
                          className="btn btn-sm btn-outline-primary"
                          disabled={!item.ativo || !fornecedor?.ativo}
                          onClick={() => abrir("preco", item)}
                        >
                          Alterar preço
                        </button>
                        <button
                          className="btn btn-sm btn-outline-secondary"
                          onClick={() => abrir("historico", item)}
                        >
                          Histórico
                        </button>
                        <button
                          className="btn btn-sm btn-outline-secondary"
                          onClick={() => status(item)}
                        >
                          {item.ativo ? "Desativar" : "Ativar"}
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {!lista.itens.length && (
              <p className="p-4 mb-0 text-secondary">
                Nenhum produto fornecido cadastrado.
              </p>
            )}
          </div>
          <Paginacao pagina={pagina} total={lista.total} onChange={setPagina} />
        </>
      )}
      {modal && (
        <Modal
          titulo={titulos[modal.tipo]}
          fechar={() => setModal(null)}
          ocupado={ocupado}
        >
          <Erro texto={erroForm} />
          {modal.tipo === "historico" ? (
            <>
              <h3 className="h6">
                {modal.item.insumoNome} · {modal.item.quantidadeEmbalagem}{" "}
                {modal.item.unidadeEmbalagem}
              </h3>
              {!historico && !erroForm && <Carregando />}
              {historico && (
                <>
                  <table className="table">
                    <thead>
                      <tr>
                        <th>Preço</th>
                        <th>Início</th>
                        <th>Fim</th>
                      </tr>
                    </thead>
                    <tbody>
                      {historico.itens.map((p) => (
                        <tr key={p.id}>
                          <td>{moeda(p.preco)}</td>
                          <td>{dataHora(p.dataInicio)}</td>
                          <td>
                            {p.dataFim ? (
                              dataHora(p.dataFim)
                            ) : (
                              <span className="badge text-bg-success">
                                Vigente
                              </span>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                  {!historico.itens.length && <p>Nenhum preço registrado.</p>}
                  <Paginacao
                    pagina={paginaHistoria}
                    total={historico.total}
                    onChange={setPaginaHistoria}
                  />
                </>
              )}
            </>
          ) : (
            <form onSubmit={salvar}>
              <fieldset disabled={ocupado}>
                {modal.tipo === "preco" ? (
                  <>
                    <p>
                      {modal.item.insumoNome} · preço atual:{" "}
                      <strong>{moeda(modal.item.precoAtual)}</strong>
                    </p>
                    <Campo
                      nome="preco"
                      label="Novo preço (R$)"
                      type="number"
                      min="0.01"
                      step="0.01"
                      required
                      valor={form.preco}
                      mudar={mudar}
                    />
                    <p className="small text-secondary">
                      O preço vigente será encerrado e um novo registro será
                      criado no histórico.
                    </p>
                  </>
                ) : (
                  <>
                    {modal.tipo === "novo" ? (
                      <>
                        <Campo
                          nome="buscarInsumo"
                          label="Pesquisar insumo ativo"
                          valor={busca}
                          mudar={(_, value) => setBusca(value)}
                        />
                        <label htmlFor="insumoId" className="form-label">
                          Insumo
                        </label>
                        <select
                          id="insumoId"
                          className="form-select mb-3"
                          required
                          value={form.insumoId}
                          onChange={(e) => mudar("insumoId", e.target.value)}
                        >
                          <option value="">Selecione um insumo</option>
                          {insumos.map((i) => (
                            <option key={i.id} value={i.id}>
                              {i.nome} ({i.unidadeMedida})
                            </option>
                          ))}
                        </select>
                        <p className="small text-secondary">
                          A busca exibe até 20 resultados. Refine pelo nome para
                          localizar o insumo.
                        </p>
                      </>
                    ) : (
                      <p>
                        <strong>{modal.item.insumoNome}</strong> ·{" "}
                        {modal.item.quantidadeEmbalagem}{" "}
                        {modal.item.unidadeEmbalagem}
                        <br />
                        <small>
                          Para mudar a apresentação, crie outra oferta e
                          desative esta.
                        </small>
                      </p>
                    )}
                    <Campo
                      nome="codigoProdutoFornecedor"
                      label="Código no fornecedor"
                      maxLength={100}
                      valor={form.codigoProdutoFornecedor}
                      mudar={mudar}
                    />
                    {modal.tipo === "novo" && (
                      <>
                        <Campo
                          nome="quantidadeEmbalagem"
                          label="Quantidade da embalagem"
                          type="number"
                          min="0.000001"
                          step="0.000001"
                          required
                          valor={form.quantidadeEmbalagem}
                          mudar={mudar}
                        />
                        <label
                          htmlFor="unidadeEmbalagem"
                          className="form-label"
                        >
                          Unidade da embalagem
                        </label>
                        <select
                          id="unidadeEmbalagem"
                          className="form-select mb-3"
                          value={form.unidadeEmbalagem}
                          onChange={(e) =>
                            mudar("unidadeEmbalagem", e.target.value)
                          }
                        >
                          {["ML", "L", "G", "KG", "UN"].map((u) => (
                            <option key={u}>{u}</option>
                          ))}
                        </select>
                        <Campo
                          nome="precoInicial"
                          label="Preço inicial (R$)"
                          type="number"
                          min="0.01"
                          step="0.01"
                          required
                          valor={form.precoInicial}
                          mudar={mudar}
                        />
                      </>
                    )}
                    <label className="form-check">
                      <input
                        className="form-check-input"
                        type="checkbox"
                        checked={form.ativo}
                        onChange={(e) => mudar("ativo", e.target.checked)}
                      />
                      Ativo
                    </label>
                  </>
                )}
              </fieldset>
              <div className="d-flex justify-content-end gap-2 mt-4">
                <button
                  type="button"
                  className="btn btn-outline-secondary"
                  disabled={ocupado}
                  onClick={() => setModal(null)}
                >
                  Cancelar
                </button>
                <button className="btn btn-primary" disabled={ocupado}>
                  {ocupado ? "Salvando…" : "Salvar"}
                </button>
              </div>
            </form>
          )}
        </Modal>
      )}
    </>
  );
}
