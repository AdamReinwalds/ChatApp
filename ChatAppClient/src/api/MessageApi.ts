const API_BASE = import.meta.env.VITE_API_BASE;

function getAuthHeaders() {
  const token = sessionStorage.getItem("token");
  return {
    "Content-Type": "application/json",
    Authorization: token ? `Bearer ${token}` : "",
  };
}

export const getMessages = async (channelId: number) => {
  const res = await fetch(`${API_BASE}/message/${channelId}`, {
    headers: getAuthHeaders(),
  });
  if (!res.ok) throw new Error("Failed to fetch messages");
  return await res.json();
};

export const sendMessage = async (channelId: number, message: string) => {
  const res = await fetch(`${API_BASE}/message/send`, {
    method: "POST",
    headers: getAuthHeaders(),
    body: JSON.stringify({ channelId, message }),
  });

  if (!res.ok) {
    throw new Error("Failed to send message");
  }
};
