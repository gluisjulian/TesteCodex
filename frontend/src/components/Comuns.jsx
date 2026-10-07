import { useEffect, useRef } from "react";
export function Erro({ texto }) {
  return texto ? (
    <div role="alert" className="alert alert-danger">
      {texto}
    </div>
  ) : null;
}
export function Carregando() {
  return (
    <div role="status" className="py-4">
      <span className="spinner-border spinner-border-sm me-2" />
      Carregando…
    </div>
  );
}
export function Status({ ativo }) {
  return (
    <span
      className={`badge ${ativo ? "text-bg-success" : "text-bg-secondary"}`}
    >
      {ativo ? "Ativo" : "Inativo"}
    </span>
  );
}
export function Paginacao({ pagina, total, tamanho = 20, onChange }) {
  const paginas = Math.max(1, Math.ceil(total / tamanho));
  return (
    <div className="d-flex gap-3 align-items-center mt-3">
      <button
        className="btn btn-outline-secondary"
        disabled={pagina <= 1}
        onClick={() => onChange(pagina - 1)}
      >
        Anterior
      </button>
      <span>
        Página {pagina} de {paginas} · {total} registros
      </span>
      <button
        className="btn btn-outline-secondary"
        disabled={pagina >= paginas}
        onClick={() => onChange(pagina + 1)}
      >
        Próxima
      </button>
    </div>
  );
}
export function Modal({ titulo, children, fechar, ocupado = false }) {
  const dialog = useRef(null);
  useEffect(() => {
    const el = dialog.current;
    el.showModal();
    return () => {
      if (el.open) el.close();
    };
  }, []);
  return (
    <dialog
      ref={dialog}
      className="app-dialog"
      aria-label={titulo}
      onCancel={(e) => {
        e.preventDefault();
        if (!ocupado) fechar();
      }}
    >
      <div className="p-4">
        <div className="d-flex justify-content-between mb-3">
          <h2 className="h4">{titulo}</h2>
          <button
            className="btn-close"
            aria-label="Fechar"
            disabled={ocupado}
            onClick={fechar}
          />
        </div>
        {children}
      </div>
    </dialog>
  );
}
export function Campo({
  nome,
  label,
  valor,
  mudar,
  type = "text",
  required = false,
  maxLength,
  step,
  min,
}) {
  return (
    <div className="mb-3">
      <label htmlFor={nome} className="form-label">
        {label}
      </label>
      <input
        id={nome}
        className="form-control"
        value={valor ?? ""}
        onChange={(e) => mudar(nome, e.target.value)}
        {...{ type, required, maxLength, step, min }}
      />
    </div>
  );
}
