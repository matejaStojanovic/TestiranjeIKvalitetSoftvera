import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import Footer from "../components/Footer";
import { api } from "../api/Api";
import "../App.css";

function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();

    try {
      setError("");

      await api.login({ email, password });

      navigate("/dashboard");
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <>
      <div className="form-card card">
        <h2 className="page-title">Welcome Back</h2>

        {error && (
          <div style={{ color: "#ef4444", marginBottom: "15px" }}>
            {error}
          </div>
        )}

        <form onSubmit={handleLogin}>
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
            Login
          </button>
        </form>

        <p style={{ marginTop: "15px", fontSize: "14px" }}>
          Don't have an account?{" "}
          <Link to="/register" style={{ color: "#22c55e" }}>
            Register
          </Link>
        </p>
      </div>

      <Footer />
    </>
  );
}

export default LoginPage;
