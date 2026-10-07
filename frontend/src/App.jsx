import { NavLink, Routes, Route, Link } from "react-router-dom";
import Dashboard from "./pages/Dashboard.jsx";
import Cadastros from "./pages/Cadastros.jsx";
import ProdutosFornecedor from "./pages/ProdutosFornecedor.jsx";
export default function App() {
  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          Ateliê<span>Gestão de perfumes</span>
        </div>
        <nav aria-label="Menu principal">
          <NavLink to="/" end>
            Dashboard
          </NavLink>
          <div className="nav-caption">Cadastros</div>
          <NavLink to="/fornecedores">Fornecedores</NavLink>
          <NavLink to="/insumos">Insumos</NavLink>
        </nav>
        <p className="small text-white-50 mt-5">
          Fornecedores, insumos e histórico de preços
        </p>
      </aside>
      <main className="main">
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route
            path="/fornecedores"
            element={<Cadastros tipo="fornecedores" />}
          />
          <Route path="/insumos" element={<Cadastros tipo="insumos" />} />
          <Route
            path="/fornecedores/:id/produtos"
            element={<ProdutosFornecedor />}
          />
          <Route
            path="*"
            element={
              <>
                <h1>Página não encontrada</h1>
                <Link to="/">Ir para dashboard</Link>
              </>
            }
          />
        </Routes>
      </main>
    </div>
  );
}
