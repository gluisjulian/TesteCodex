import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api, mensagemErro } from "../services/api";
import {
  Erro,
  Carregando,
  Status,
  Paginacao,
  Modal,
  Campo,
} from "../components/Comuns";
const vazioFornecedor = {
  razaoSocial: "",
  nomeFantasia: "",
  cpfCnpj: "",
  telefone: "",
  email: "",
  site: "",
  observacao: "",
  ativo: true,
};
const vazioInsumo = {
  nome: "",
  descricao: "",
  tipoInsumo: "MateriaPrima",
  unidadeMedida: "ML",
  densidade: "",
  observacao: "",
  ativo: true,
};
export default function Cadastros({ tipo }) {
  const fornecedor = tipo === "fornecedores";
  const [lista, setLista] = useState(null),
    [busca, setBusca] = useState(""),
    [termo, setTermo] = useState(""),
    [pagina, setPagina] = useState(1),
    [filtro, setFiltro] = useState(""),
    [erro, setErro] = useState(""),
    [versao, setVersao] = useState(0);
  const [form, setForm] = useState(null),
    [leitura, setLeitura] = useState(false),
    [erroForm, setErroForm] = useState(""),
    [salvando, setSalvando] = useState(false);
  useEffect(() => {
    setBusca("");
    setTermo("");
    setPagina(1);
    setForm(null);
  }, [tipo]);
  useEffect(() => {
    let ativo = true;
    setLista(null);
    setErro("");
    api
      .get("/" + tipo, {
        params: {
          busca: termo,
          pagina,
          ativo: filtro === "" ? undefined : filtro === "true",
        },
      })
      .then((r) => {
        if (ativo) setLista(r.data);
      })
      .catch((e) => {
        if (ativo) setErro(mensagemErro(e));
      });
    return () => {
      ativo = false;
    };
  }, [tipo, termo, pagina, filtro, versao]);
  const mudar = (key, value) => setForm((old) => ({ ...old, [key]: value }));
  const abrir = (item, apenasLer = false) => {
    setForm(
      item
        ? { ...item, densidade: item.densidade ?? "" }
        : { ...(fornecedor ? vazioFornecedor : vazioInsumo) },
    );
    setLeitura(apenasLer);
    setErroForm("");
  };
  async function salvar(e) {
    e.preventDefault();
    setSalvando(true);
    setErroForm("");
    try {
      if (
        fornecedor &&
        form.cpfCnpj &&
        !/^(?:\d{11}|\d{14}|\d{3}\.\d{3}\.\d{3}-\d{2}|\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2})$/.test(
          form.cpfCnpj,
        )
      )
        throw new Error(
          "Informe CPF/CNPJ com 11 ou 14 dígitos, com ou sem pontuação.",
        );
      const fields = fornecedor
        ? Object.keys(vazioFornecedor)
        : Object.keys(vazioInsumo);
      const payload = Object.fromEntries(
        fields.map((key) => [key, form[key] === "" ? null : form[key]]),
      );
      if (!fornecedor)
        payload.densidade =
          form.densidade === "" ? null : Number(form.densidade);
      if (form.id) await api.put("/" + tipo + "/" + form.id, payload);
      else await api.post("/" + tipo, payload);
      setForm(null);
      setVersao((v) => v + 1);
    } catch (e) {
      setErroForm(e.response ? mensagemErro(e) : e.message);
    } finally {
      setSalvando(false);
    }
  }
  async function status(item) {
    setErro("");
    try {
      await api.patch("/" + tipo + "/" + item.id + "/status", {
        ativo: !item.ativo,
      });
      setVersao((v) => v + 1);
    } catch (e) {
      setErro(mensagemErro(e));
    }
  }
  return (
    <>
      <div className="eyebrow">CADASTROS</div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>{fornecedor ? "Fornecedores" : "Insumos"}</h1>
        <button className="btn btn-primary" onClick={() => abrir(null)}>
          Novo {fornecedor ? "fornecedor" : "insumo"}
        </button>
      </div>
      <form
        className="d-flex gap-2 mb-4 flex-wrap"
        onSubmit={(e) => {
          e.preventDefault();
          setPagina(1);
          setTermo(busca);
        }}
      >
        <input
          aria-label="Buscar"
          className="form-control flex-grow-1 w-auto"
          placeholder={fornecedor ? "Nome ou CPF/CNPJ" : "Nome do insumo"}
          value={busca}
          onChange={(e) => setBusca(e.target.value)}
        />
        <select
          aria-label="Filtrar status"
          className="form-select w-auto"
          value={filtro}
          onChange={(e) => {
            setFiltro(e.target.value);
            setPagina(1);
          }}
        >
          <option value="">Todos os status</option>
          <option value="true">Ativos</option>
          <option value="false">Inativos</option>
        </select>
        <button className="btn btn-outline-primary">Buscar</button>
      </form>
      <Erro texto={erro} />
      {!lista && !erro && <Carregando />}
      {lista && (
        <>
          <div className="card table-responsive">
            <table className="table align-middle mb-0">
              <thead>
                <tr>
                  {(fornecedor
                    ? ["Nome fantasia", "Razão social", "CPF/CNPJ", "Telefone"]
                    : ["Nome", "Tipo", "Unidade"]
                  ).map((h) => (
                    <th key={h}>{h}</th>
                  ))}
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                {lista.itens.map((item) => (
                  <tr key={item.id}>
                    {fornecedor ? (
                      <>
                        <td>{item.nomeFantasia || "—"}</td>
                        <td>{item.razaoSocial}</td>
                        <td>{item.cpfCnpj || "—"}</td>
                        <td>{item.telefone || "—"}</td>
                      </>
                    ) : (
                      <>
                        <td>{item.nome}</td>
                        <td>
                          {item.tipoInsumo === "MateriaPrima"
                            ? "Matéria-prima"
                            : "Embalagem"}
                        </td>
                        <td>{item.unidadeMedida}</td>
                      </>
                    )}
                    <td>
                      <Status ativo={item.ativo} />
                    </td>
                    <td>
                      <div className="d-flex gap-2 flex-wrap">
                        <button
                          className="btn btn-sm btn-outline-secondary"
                          onClick={() => abrir(item, true)}
                        >
                          Visualizar
                        </button>
                        <button
                          className="btn btn-sm btn-outline-secondary"
                          onClick={() => abrir(item)}
                        >
                          Editar
                        </button>
                        {fornecedor && (
                          <Link
                            className="btn btn-sm btn-outline-primary"
                            to={"/fornecedores/" + item.id + "/produtos"}
                          >
                            Produtos
                          </Link>
                        )}
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
                Nenhum registro encontrado.
              </p>
            )}
          </div>
          <Paginacao pagina={pagina} total={lista.total} onChange={setPagina} />
        </>
      )}
      {form && (
        <Modal
          titulo={
            leitura
              ? "Visualizar cadastro"
              : form.id
                ? "Editar cadastro"
                : "Novo cadastro"
          }
          fechar={() => setForm(null)}
          ocupado={salvando}
        >
          <Erro texto={erroForm} />
          <form onSubmit={salvar}>
            <fieldset disabled={leitura || salvando}>
              {(fornecedor
                ? [
                    ["razaoSocial", "Razão social", "text", 200, true],
                    ["nomeFantasia", "Nome fantasia", "text", 200],
                    ["cpfCnpj", "CPF/CNPJ", "text", 18],
                    ["telefone", "Telefone", "tel", 30],
                    ["email", "E-mail", "email", 254],
                    ["site", "Site", "url", 500],
                  ]
                : [
                    ["nome", "Nome", "text", 200, true],
                    ["descricao", "Descrição", "text", 2000],
                  ]
              ).map(([key, label, type, maxLength, required]) => (
                <Campo
                  key={key}
                  nome={key}
                  label={label}
                  valor={form[key]}
                  mudar={mudar}
                  {...{ type, maxLength, required }}
                />
              ))}
              {!fornecedor && (
                <>
                  <label className="form-label" htmlFor="tipoInsumo">
                    Tipo
                  </label>
                  <select
                    id="tipoInsumo"
                    className="form-select mb-3"
                    value={form.tipoInsumo}
                    onChange={(e) => mudar("tipoInsumo", e.target.value)}
                  >
                    <option value="MateriaPrima">Matéria-prima</option>
                    <option value="Embalagem">Embalagem</option>
                  </select>
                  <label htmlFor="unidadeMedida" className="form-label">
                    Unidade base
                  </label>
                  <select
                    id="unidadeMedida"
                    className="form-select mb-3"
                    value={form.unidadeMedida}
                    onChange={(e) => mudar("unidadeMedida", e.target.value)}
                  >
                    {["ML", "G", "UN"].map((u) => (
                      <option key={u}>{u}</option>
                    ))}
                  </select>
                  <Campo
                    nome="densidade"
                    label="Densidade (g/ml, opcional)"
                    type="number"
                    step="0.000001"
                    min="0.000001"
                    valor={form.densidade}
                    mudar={mudar}
                  />
                </>
              )}
              <label htmlFor="observacao" className="form-label">
                Observação
              </label>
              <textarea
                id="observacao"
                className="form-control mb-3"
                maxLength={2000}
                value={form.observacao || ""}
                onChange={(e) => mudar("observacao", e.target.value)}
              />
              <label className="form-check">
                <input
                  className="form-check-input"
                  type="checkbox"
                  checked={form.ativo}
                  onChange={(e) => mudar("ativo", e.target.checked)}
                />
                Ativo
              </label>
            </fieldset>
            <div className="d-flex justify-content-end gap-2 mt-4">
              <button
                type="button"
                className="btn btn-outline-secondary"
                disabled={salvando}
                onClick={() => setForm(null)}
              >
                {leitura ? "Fechar" : "Cancelar"}
              </button>
              {!leitura && (
                <button className="btn btn-primary" disabled={salvando}>
                  {salvando ? "Salvando…" : "Salvar"}
                </button>
              )}
            </div>
          </form>
        </Modal>
      )}
    </>
  );
}
