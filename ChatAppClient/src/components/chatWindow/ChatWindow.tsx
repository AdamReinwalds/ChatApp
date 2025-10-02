import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import DOMPurify from "dompurify";

export const ChatWindow = () => {
  const [messages, setMessages] = useState<{ user: string; text: string }[]>(
    []
  );
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );
  const [user, setUser] = useState<string>("");
  const [message, setMessage] = useState<string>("");
  const messagesEndRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    const conn = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5095/chathub")
      .withAutomaticReconnect()
      .build();

    conn
      .start()
      .then(() => console.log("Connection established"))
      .catch((err) => console.error("Connection failed: ", err));

    conn.on("ReceiveMessage", (u, msg) => {
      const safeUser = DOMPurify.sanitize(u);
      const safeMessage = DOMPurify.sanitize(msg);
      setMessages((prev) => [...prev, { user: safeUser, text: safeMessage }]);
    });
    setConnection(conn);
    return () => {
      conn.stop().then(() => console.log("Connection stopped"));
    };
  }, []);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const sendMessage = async () => {
    if (connection && message && user) {
      try {
        await connection.invoke("SendMessage", user, message);
        setMessage("");
      } catch (err) {
        console.error("Send message failed: ", err);
      }
    }
  };

  return (
    <div className="p-6 max-w-lg mx-auto mt-10">
      <div className="h-96 overflow-y-scroll border rounded-lg p-4 bg-primary">
        {messages && messages.length <= 0 && (
          <small className="text-gray-400">Inga meddelanden ännu 😞</small>
        )}
        {messages.length > 0 &&
          messages.map((m, i) => (
            <div
              key={i}
              className={`chat ${m.user === user ? "chat-end" : "chat-start"}`}
            >
              <div className="chat-header">{m.user}</div>
              <div
                className={`chat-bubble ${
                  m.user === user ? "bg-gray-700" : ""
                }`}
              >
                {m.text}
              </div>
            </div>
          ))}
        <div ref={messagesEndRef} />
      </div>

      <div className="mt-4 flex gap-2">
        <input
          type="text"
          placeholder="Name"
          value={user}
          onChange={(e) => setUser(e.target.value)}
          className="input input-bordered w-1/3"
        />
        <input
          type="text"
          placeholder="Message"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") sendMessage();
          }}
          className="input input-bordered flex-1"
        />
        <button className="btn btn-primary" onClick={() => sendMessage()}>
          Send
        </button>
      </div>
    </div>
  );
};
