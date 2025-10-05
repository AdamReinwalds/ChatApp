import React, { useState } from "react";

interface RegisterFormProps {
  onSwitchToLogin: () => void;
  onRegister: (username: string, password: string) => void;
}

export const RegisterForm = ({
  onSwitchToLogin,
  onRegister,
}: RegisterFormProps) => {
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    onRegister(username, password);
    setIsLoading(false);
  };

  return (
    <div>
      <form onSubmit={handleSubmit} className="flex flex-col gap-4">
        <input
          type="text"
          placeholder="Set Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
        <input
          type="email"
          placeholder="Set Email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <input
          type="password"
          placeholder="Set Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <button type="submit">Register</button>
      </form>
      <div>
        <p>Already have an account? Login here!</p>
        <button
          onClick={onSwitchToLogin}
          disabled={isLoading}
          className="bg-blue-500 text-white py-2 px-4 rounded"
        >
          {isLoading ? "Loading..." : "Login"}
        </button>
      </div>
    </div>
  );
};
