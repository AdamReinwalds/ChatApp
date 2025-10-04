import { useEffect, useState } from "react";
import { Auth } from "./pages/Auth";
import { Chat } from "./pages/chat/Chat";
const App = () => {
  const [jwt, setJwt] = useState<string | null>(
    sessionStorage.getItem("token") ? sessionStorage.getItem("token") : null
  );
  console.log("JWT Token:", jwt);
  useEffect(() => {
    const handleStorageChange = () => {
      setJwt(sessionStorage.getItem("token"));
    };
    window.addEventListener("storage", handleStorageChange);
    // Cleanup listener on unmount(even though app never unmounts currently xD)
    return () => {
      window.removeEventListener("storage", handleStorageChange);
    };
  }, []);

  if (!jwt) {
    return <Auth setJwt={setJwt} />;
  }
  return <Chat />;
};

export default App;
