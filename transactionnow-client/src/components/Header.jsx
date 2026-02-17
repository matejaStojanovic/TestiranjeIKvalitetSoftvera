import { Link } from "react-router-dom";

export default function Header() {
  return (
    <header style={{
      background: "#0f172a",
      padding: "20px 40px",
      display: "flex",
      justifyContent: "space-between",
      alignItems: "center",
      borderBottom: "1px solid #1e293b"
    }}>
      <h2 style={{ color: "#22c55e" }}>TransactionNow</h2>

      <nav style={{ display: "flex", gap: "20px" }}>
        <Link to="/dashboard">Dashboard</Link>
        <Link to="/cards">Cards</Link>
        <Link to="/transactions">Transactions</Link>
        <Link to="/invoices">Invoices</Link>
      </nav>
    </header>
  );
}
