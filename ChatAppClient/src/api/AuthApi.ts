const API_BASE = import.meta.env.VITE_API_BASE;

export const registerUser = async (username: string, password: string) => {
  try {
    const res = await fetch(`${API_BASE}/auth/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        username,
        password,
      }),
    });
    if (!res.ok) {
      throw new Error("Registration failed");
    }
    const data = await res.json();
    console.log("Registration successful:", data);
  } catch (error) {
    console.log("Error:", error);
  }
};

export const loginUser = async (username: string, password: string) => {
  try {
    const res = await fetch(`${API_BASE}/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ username, password }),
    });
    if (!res.ok) {
      throw new Error("Login failed");
    }
    const data = await res.json();
    sessionStorage.setItem("token", data.token);
    console.log("Login successful:", data);
  } catch (error) {
    console.error("Error:", error);
  }
};
