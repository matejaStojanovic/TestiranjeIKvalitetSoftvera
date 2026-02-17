import { BrowserRouter, Routes, Route } from "react-router-dom";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import Dashboard from "./pages/Dashboard";
import CardsPage from "./pages/CardsPage";
import InvoicesPage from "./pages/InvoicesPage.jsx";
import TransactionsPage from "./pages/TransactionsPage";
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/cards" element={<CardsPage />} />
        <Route path="/transactions" element={<TransactionsPage />} />
        <Route path="/invoices" element={<InvoicesPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
