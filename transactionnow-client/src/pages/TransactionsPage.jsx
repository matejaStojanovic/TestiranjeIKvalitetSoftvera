import { useEffect, useState } from "react";
import { api } from "../api/Api";
import Header from "../components/Header";
import Footer from "../components/Footer";
import TransactionItem from "../components/TransactionItem";
import "../App.css";

function TransactionsPage() {
  const [transactions, setTransactions] = useState([]);

  const [recipientEmail, setRecipientEmail] = useState("");
  const [sendAmount, setSendAmount] = useState("");

  const [cardNumber, setCardNumber] = useState("");
  const [depositAmount, setDepositAmount] = useState("");

  const [error, setError] = useState("");

  const load = async () => {
    try {
      const data = await api.getTransactions();
      setTransactions(data);
    } catch (err) {
      setError(err.message);
    }
  };

  useEffect(() => {
    load();
  }, []);


  const send = async (e) => {
    e.preventDefault();

    try {
      setError("");

      await api.sendMoney({
        recipientEmail: recipientEmail,
        iznos: Number(sendAmount),
      });

      setRecipientEmail("");
      setSendAmount("");
      load();
    } catch (err) {
      setError(err.message);
    }
  };


  const deposit = async (e) => {
    e.preventDefault();

    try {
      setError("");

      await api.deposit({
        cardNumber: cardNumber,
        amount: Number(depositAmount),
      });

      setCardNumber("");
      setDepositAmount("");
      load();
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <>
      <Header />

      <div className="container">
        <h2 className="page-title">Transactions</h2>

        {error && (
          <div style={{ color: "#ef4444", marginBottom: "20px" }}>
            {error}
          </div>
        )}

        {/* SEND MONEY */}
        <div className="card">
          <h3>Send Money</h3>

          <form onSubmit={send} style={{ marginTop: "15px" }}>
            <input
              className="input"
              placeholder="Recipient Email"
              value={recipientEmail}
              onChange={(e) => setRecipientEmail(e.target.value)}
              required
            />

            <input
              className="input"
              placeholder="Amount"
              type="number"
              value={sendAmount}
              onChange={(e) => setSendAmount(e.target.value)}
              required
            />

            <button className="button button-primary" type="submit">
              Send
            </button>
          </form>
        </div>

        {/* DEPOSIT */}
        <div className="card">
          <h3>Deposit (Add Money from Card)</h3>

          <form onSubmit={deposit} style={{ marginTop: "15px" }}>
            <input
              className="input"
              placeholder="Card Number"
              value={cardNumber}
              onChange={(e) => setCardNumber(e.target.value)}
              required
            />

            <input
              className="input"
              placeholder="Amount"
              type="number"
              value={depositAmount}
              onChange={(e) => setDepositAmount(e.target.value)}
              required
            />

            <button className="button button-primary" type="submit">
              Deposit
            </button>
          </form>
        </div>

        {/* HISTORY */}
        <h3 style={{ marginTop: "40px" }}>History</h3>

        {transactions.length === 0 && (
          <p style={{ color: "#94a3b8" }}>No transactions yet.</p>
        )}

        {transactions.map((t) => (
          <TransactionItem key={t.id} transaction={t} />
        ))}
      </div>

      <Footer />
    </>
  );
}

export default TransactionsPage;
