export default function TransactionItem({ transaction }) {
  return (
    <div className="card">
      <h3>{transaction.description}</h3>

      <p>Amount: ${transaction.amount}</p>

      <p style={{ color: "#94a3b8", marginTop: "5px" }}>
        {new Date(transaction.dateTime).toLocaleDateString()}
      </p>
    </div>
  );
}
