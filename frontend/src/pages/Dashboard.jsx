import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { api, mensagemErro } from "../services/api";
import { Erro, Carregando } from "../components/Comuns";
export default function Dashboard() {
  const [dados, setDados] = useState(null),
    [erro, setErro] = useState("");
  useEffect(() => {
    let ativo = true;
    Promise.all([
      api.get("/fornecedores", { params: { tamanhoPagina: 1 } }),
      api.get("/insumos", { params: { tamanhoPagina: 1 } }),
    ])
      .then(([f, i]) => {
        if (ativo)
          setDados({ fornecedores: f.data.total, insumos: i.data.total });
      })
      .catch((e) => {
        if (ativo) setErro(mensagemErro(e));
      });
    return () => {
      ativo = false;
    };
  }, []);
  return (
    <>
      <div className="eyebrow">VISÃO GERAL</div>
      <h1>Seu ateliê, organizado.</h1>
      <p className="text-secondary mb-4">
        Acompanhe os cadastros da sua produção de perfumes.
      </p>
      <Erro texto={erro} />
      {!dados && !erro && <Carregando />}
      {dados && (
        <div className="row g-4">
          {["fornecedores", "insumos"].map((key) => (
            <div className="col-md-6" key={key}>
              <div className="card p-4">
                <h2 className="h5 text-capitalize">{key}</h2>
                <div className="display-5 my-3">{dados[key]}</div>
                <Link to={"/" + key}>Gerenciar {key} →</Link>
              </div>
            </div>
          ))}
        </div>
      )}
      <div className="card p-4 mt-4">
        <h2 className="h5">Preços com histórico</h2>
        <p className="mb-0 text-secondary">
          Cadastre as apresentações de cada fornecedor e consulte o custo por
          unidade base. Cada alteração de preço mantém os registros anteriores.
        </p>
      </div>
    </>
  );
}
