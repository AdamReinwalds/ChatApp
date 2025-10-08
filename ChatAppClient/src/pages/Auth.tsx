import { useState } from "react";
import { LoginForm } from "../components/auth/LoginForm";
import { RegisterForm } from "../components/auth/RegisterForm";
const API_BASE = import.meta.env.VITE_API_BASE;
export const Auth = ({ setJwt }: { setJwt: (token: string) => void }) => {
  const [isLogin, setIsLogin] = useState(true);

  const handleLogin = async (username: string, password: string) => {
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
      setJwt(data.token);
      console.log("Login successful:", data);
    } catch (error) {
      console.error("Error:", error);
    }
  };

  const handleRegister = async (username: string, password: string) => {
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

  const handleSwitchToRegister = () => {
    setIsLogin(false);
  };

  const handleSwitchToLogin = () => {
    setIsLogin(true);
  };

  if (isLogin) {
    return (
      <LoginForm
        onSwitchToRegister={handleSwitchToRegister}
        onLogin={handleLogin}
      />
    );
  }
  return (
    <RegisterForm
      onSwitchToLogin={handleSwitchToLogin}
      onRegister={handleRegister}
    />
  );
};
