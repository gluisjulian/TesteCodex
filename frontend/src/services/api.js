import axios from "axios";
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5080/api",
  timeout: 15000,
});
export function mensagemErro(error) {
  const data = error.response?.data;
  if (data?.errors) return Object.values(data.errors).flat().join(" ");
  return (
    data?.title ||
    (error.code === "ECONNABORTED"
      ? "A requisição demorou demais. Tente novamente."
      : "Não foi possível conectar à API.")
  );
}
export const moeda = (value, casas = 2) =>
  value == null
    ? "—"
    : new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL",
        minimumFractionDigits: casas,
        maximumFractionDigits: casas,
      }).format(value);
export const dataHora = (value) =>
  value ? new Date(value).toLocaleString("pt-BR") : "—";
