import { useEffect, useState } from "react";
import { api } from "../api/Api";
import Footer from "../components/Footer";
import Header from "../components/Header";
import { useNavigate } from "react-router-dom";
import "../App.css";

function Dashboard() {
  const [user, setUser] = useState(null);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    api.getMe()
      .then(setUser)
      .catch(() => navigate("/"));
  }, []);

  const handleLogout = async () => {
    try {
      await api.logout();
      navigate("/");
    } catch (err) {
      setError(err.message);
    }
  };

  if (!user) return null;

  return (
    <>
      <Header />

      <div className="container">
        <div className="card">
          <h2>
            Welcome {user.ime} {user.prezime}
          </h2>

          <h3 style={{ marginTop: "15px", color: "#22c55e" }}>
            Balance: {user.balance} RSD
          </h3>

          {error && (
            <div style={{ color: "#ef4444", marginTop: "10px" }}>
              {error}
            </div>
          )}

          <div style={{ marginTop: "25px" }}>
            <button
              className="button button-primary"
              onClick={() => navigate("/cards")}
            >
              My Cards
            </button>

            <button
              className="button button-primary"
              style={{ marginLeft: 10 }}
              onClick={() => navigate("/transactions")}
            >
              Transactions
            </button>

            <button
              className="button button-danger"
              style={{ marginLeft: 10 }}
              onClick={handleLogout}
            >
              Logout
            </button>
          </div>
        </div>
      </div>

      <Footer />
    </>
  );
}

export default Dashboard;
