import { useState } from "react";
import { LoginForm } from "../components/auth/LoginForm";
import { RegisterForm } from "../components/auth/RegisterForm";
import { loginUser, registerUser } from "../api/AuthApi";

export const Auth = ({ setJwt }: { setJwt: (token: string) => void }) => {
  const [isLogin, setIsLogin] = useState(true);

  const handleLogin = async (username: string, password: string) => {
    await loginUser(username, password);
    const token = sessionStorage.getItem("token");
    if (token) {
      setJwt(token);
    }
  };

  const handleRegister = async (username: string, password: string) => {
    const result = await registerUser(username, password);
    if (result.success) {
      await handleLogin(username, password);
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
