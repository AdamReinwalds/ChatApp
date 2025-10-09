import { useState } from "react";

interface LoginFormProps {
  onLogin: (username: string, password: string) => void;
  onSwitchToRegister: () => void;
}

export const LoginForm = ({ onLogin, onSwitchToRegister }: LoginFormProps) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    onLogin(username, password);
    setIsLoading(false);
  };

  return (
    <>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <div>
          <label>Username:</label>
          <input
            type="text"
            placeholder="Enter your username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
        </div>
        <div>
          <label>Password:</label>
          <input
            placeholder="Enter your password"
            type={showPassword ? "text" : "password"}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <button type="button" onClick={() => setShowPassword(!showPassword)}>
            {showPassword ? "Hide" : "Show"}
          </button>
        </div>
        <button
          type="submit"
          disabled={isLoading}
          className="bg-blue-500 text-white py-2 px-4 rounded"
        >
          {isLoading ? "Loading..." : "Login"}
        </button>
      </form>
      <div>
        <p>Don't have an account? Register now!</p>
        <button onClick={onSwitchToRegister}>Register</button>
      </div>
    </>
  );
};
