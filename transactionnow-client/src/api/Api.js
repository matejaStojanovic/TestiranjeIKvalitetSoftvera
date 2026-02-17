const BASE_URL = "http://localhost:5295";

async function request(url, options = {}) {
  const response = await fetch(BASE_URL + url, {
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
    ...options,
  });

  const data = await response.json().catch(() => ({}));

  if (!response.ok) {
    throw new Error(data.error || "Something went wrong");
  }

  return data;
}

export const api = {
  register: (data) =>
    request("/api/auth/register", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  login: (data) =>
    request("/api/auth/login", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  logout: () =>
    request("/api/auth/logout", {
      method: "POST",
    }),

  getMe: () => request("/api/user/me"),

  getCards: () => request("/api/cards"),
  addCard: (data) =>
    request("/api/cards", {
      method: "POST",
      body: JSON.stringify(data),
    }),
  deleteCard: (id) =>
    request(`/api/cards/${id}`, { method: "DELETE" }),

  getTransactions: () => request("/api/transactions"),
  sendMoney: (data) =>
    request("/api/transactions", {
      method: "POST",
      body: JSON.stringify(data),
    }),
  deposit: (data) =>
    request("/api/transactions/deposit", {
      method: "POST",
      body: JSON.stringify(data),
    }),
  getInvoices: () =>
    request("/api/invoices"),

  sendInvoice: (data) =>
    request("/api/invoices", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  payInvoice: (id) =>
    request(`/api/invoices/${id}/pay`, {
      method: "POST",
    }),

  deleteInvoice: (id) =>
    request(`/api/invoices/${id}`, {
      method: "DELETE",
    }),
};
