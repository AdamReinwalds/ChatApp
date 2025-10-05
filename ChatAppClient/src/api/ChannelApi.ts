const API_BASE = import.meta.env.VITE_API_BASE;

function getAuthHeaders() {
  const token = sessionStorage.getItem("token");
  return {
    "Content-Type": "application/json",
    Authorization: token ? `Bearer ${token}` : "",
  };
}

export const getPublicRooms = async () => {
  const res = await fetch(`${API_BASE}/channel/public`, {
    headers: getAuthHeaders(),
  });
  if (!res.ok) throw new Error("Failed to fetch public rooms");
  return await res.json();
};

export const getMyChannels = async () => {
  const res = await fetch(`${API_BASE}/channel/my`, {
    headers: getAuthHeaders(),
  });
  if (!res.ok) throw new Error("Failed to fetch my channels");
  return await res.json();
};

export const joinChannel = async (channelId: string) => {
  const res = await fetch(`${API_BASE}/channel/join/${channelId}`, {
    method: "POST",
    headers: getAuthHeaders(),
  });
  const data = await res.json();
  if (!res.ok) throw new Error(data.message || "Failed to join channel");
  return data;
};
