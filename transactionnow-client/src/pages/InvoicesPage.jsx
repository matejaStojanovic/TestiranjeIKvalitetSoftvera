import { useEffect, useState } from "react";
import { api } from "../api/Api";
import Header from "../components/Header";
import Footer from "../components/Footer";
//import TransactionItem from "../components/TransactionItem";
import "../App.css";

function InvoicesPage() {
  const [invoices, setInvoices] = useState([]);
  const [receiverEmail, setReceiverEmail] = useState("");
  const [amount, setAmount] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const loadInvoices = async () => {
    try {
      setLoading(true);
      const data = await api.getInvoices();
      setInvoices(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadInvoices();
  }, []);

  const handleSendInvoice = async (e) => {
    e.preventDefault();

    if (!receiverEmail.trim() || !amount) {
      setError("Email and amount are required.");
      return;
    }

    try {
      setError("");
      await api.sendInvoice({
        receiverEmail,
        amount: parseFloat(amount),
      });

      setReceiverEmail("");
      setAmount("");
      loadInvoices();
    } catch (err) {
      setError(err.message);
    }
  };

  const handlePay = async (id) => {
    try {
      setError("");
      await api.payInvoice(id);
      loadInvoices();
    } catch (err) {
      setError(err.message);
    }
  };

  const handleDelete = async (id) => {
    try {
      setError("");
      await api.deleteInvoice(id);
      loadInvoices();
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <>
      <Header />

      <div className="container">
        <h2>Invoices</h2>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        {/* SEND INVOICE FORM */}
        <div className="card">
          <h3>Send Invoice</h3>

          <form onSubmit={handleSendInvoice}>
            <input
              className="input"
              placeholder="Recipient Email"
              value={receiverEmail}
              onChange={(e) => setReceiverEmail(e.target.value)}
            />

            <input
              className="input"
              placeholder="Amount"
              type="number"
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
            />

            <button className="button button-primary" type="submit">
              Send Invoice
            </button>
          </form>
        </div>

        {/* INVOICE LIST */}
        <div className="card">
          <h3>Incoming Invoices</h3>

          {loading ? (
            <p>Loading...</p>
          ) : invoices.length === 0 ? (
            <p>No invoices.</p>
          ) : (
            invoices.map((inv) => (
              <div key={inv.id} className="invoice-item">
                <p>
                  <strong>Amount:</strong> {inv.amount} RSD
                </p>
                <p>
                  <small>
                    Created: {new Date(inv.createdAt).toLocaleString()}
                  </small>
                </p>

                <button
                  className="button button-primary"
                  onClick={() => handlePay(inv.id)}
                >
                  Pay
                </button>

                <button
                  className="button danger"
                  onClick={() => handleDelete(inv.id)}
                >
                  Delete
                </button>
              </div>
            ))
          )}
        </div>
      </div>

      <Footer />
    </>
  );
}

export default InvoicesPage;
