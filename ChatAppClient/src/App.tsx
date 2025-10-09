import { useEffect, useState } from "react";
import { Auth } from "./pages/Auth";
import { Chat } from "./pages/chat/Chat";
import { jwtDecode } from "jwt-decode";

interface JwtClaims {
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
  exp: number;
  iss: string;
  aud: string;
}

const App = () => {
  const [jwt, setJwt] = useState<string | null>(
    sessionStorage.getItem("token") ? sessionStorage.getItem("token") : null
  );
  const [currentUser, setCurrentUser] = useState<string>("");

  useEffect(() => {
    if (jwt) {
      try {
        const decoded = jwtDecode<JwtClaims>(jwt);
        const username =
          decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
        if (username) setCurrentUser(username);
      } catch (err) {
        console.error("Failed to decode JWT: ", err);
        setJwt(null);
        sessionStorage.removeItem("token");
      }
    } else {
      setCurrentUser("");
    }
  }, [jwt]);

  if (!jwt) {
    return <Auth setJwt={setJwt} />;
  }
  return <Chat currentUser={currentUser} />;
};

export default App;
