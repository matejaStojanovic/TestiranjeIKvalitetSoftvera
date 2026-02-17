function CardItem({ card, onDelete }) {
  return (
    <div className="card">
      <p><strong>Card:</strong> {card.cardNumber}</p>
      <p><small>Added: {new Date(card.addedAt).toLocaleString()}</small></p>
      <button
        className="danger"
        onClick={() => onDelete(card.id)}
      >
        Delete
      </button>
    </div>
  );
}

export default CardItem;
