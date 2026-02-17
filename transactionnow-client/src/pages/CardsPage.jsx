import { useEffect, useState } from "react";
import { api } from "../api/Api";
import Header from "../components/Header";
import Footer from "../components/Footer";
import CardItem from "../components/CardItem";
import "../App.css";

function CardsPage() {
  const [cards, setCards] = useState([]);
  const [cardNumber, setCardNumber] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const loadCards = async () => {
    try {
      setLoading(true);
      const data = await api.getCards();
      setCards(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCards();
  }, []);

  const addCard = async (e) => {
    e.preventDefault();

    if (!cardNumber.trim()) {
      setError("Card number is required.");
      return;
    }

    try {
      setError("");
      await api.addCard({ brojKartice: cardNumber });
      setCardNumber("");
      loadCards();
    } catch (err) {
      setError(err.message);
    }
  };

  const deleteCard = async (id) => {
    try {
      setError("");
      await api.deleteCard(id);
      loadCards();
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <>
      <Header />

      <div className="container">
        <h2 className="page-title">My Cards</h2>

        {error && (
          <div style={{ color: "#ef4444", marginBottom: "20px" }}>
            {error}
          </div>
        )}

        {/* ADD CARD */}
        <div className="card">
          <h3>Add New Card</h3>

          <form onSubmit={addCard} style={{ marginTop: "15px" }}>
            <input
              className="input"
              placeholder="Card number"
              value={cardNumber}
              onChange={(e) => {
                setCardNumber(e.target.value);
                setError("");
              }}
              required
            />

            <button className="button button-primary" type="submit">
              Add Card
            </button>
          </form>
        </div>


        {/* CARD LIST */}
        <h3 style={{ marginTop: "40px" }}>Your Cards</h3>

        {loading && <p style={{ color: "#94a3b8" }}>Loading...</p>}

        {!loading && cards.length === 0 && (
          <p style={{ color: "#94a3b8" }}>
            No cards added yet.
          </p>
        )}

        {cards.map((c) => (
          <CardItem
            key={c.id}
            card={c}
            onDelete={deleteCard}
          />
        ))}
      </div>

      <Footer />
    </>
  );
}

export default CardsPage;
