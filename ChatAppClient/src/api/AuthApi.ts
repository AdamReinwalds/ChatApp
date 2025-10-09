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
      return { success: false, message: "Registration failed" };
    }
    const data = await res.json();
    console.log("Registration successful:", data);
    return { success: true };
  } catch (error) {
    console.log("Error:", error);
    return { success: false, message: "Registration failed" };
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
      return { success: false, message: "Login failed" };
    }
    const data = await res.json();
    sessionStorage.setItem("token", data.token);
    return { success: true };
  } catch (error) {
    console.error("Error:", error);
    return { success: false, message: "Login failed" };
  }
};
