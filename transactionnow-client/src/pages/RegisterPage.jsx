import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { api } from "../api/Api";
import Footer from "../components/Footer";
import "../App.css";

function RegisterPage() {
  const [ime, setIme] = useState("");
  const [prezime, setPrezime] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const handleRegister = async (e) => {
    e.preventDefault();

    try {
      setError("");

      await api.register({
        ime,
        prezime,
        email,
        password,
      });

      navigate("/");
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <>
      <div className="form-card card">
        <h2 className="page-title">Create Account</h2>

        {error && (
          <div style={{ color: "#ef4444", marginBottom: "15px" }}>
            {error}
          </div>
        )}

        <form onSubmit={handleRegister}>
          <input
            className="input"
            placeholder="Ime"
            value={ime}
            onChange={(e) => setIme(e.target.value)}
            required
          />

          <input
            className="input"
            placeholder="Prezime"
            value={prezime}
            onChange={(e) => setPrezime(e.target.value)}
            required
          />

          <input
            className="input"
            type="email"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />

          <input
            className="input"
            type="password"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />

          <button className="button button-primary" type="submit">
            Register
          </button>
        </form>

        <p style={{ marginTop: "15px", fontSize: "14px" }}>
          Already have an account?{" "}
          <Link to="/" style={{ color: "#22c55e" }}>
            Login
          </Link>
        </p>
      </div>

      <Footer />
    </>
  );
}

export default RegisterPage;
