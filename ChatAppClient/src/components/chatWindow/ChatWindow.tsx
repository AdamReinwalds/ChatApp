import { useEffect, useRef, useState } from "react";
import DOMPurify from "dompurify";
import * as SignalR from "@microsoft/signalr";

interface ChatWindowProps {
  connection: SignalR.HubConnection | null;
  currentUser: string;
  currentChannel: {
    id: number;
    name: string;
    type: "public" | "private";
  } | null;
}

export const ChatWindow = ({
  connection,
  currentChannel,
  currentUser,
}: ChatWindowProps) => {
  const [messages, setMessages] = useState<{ text: string; user: string }[]>(
    []
  );
  const [message, setMessage] = useState<string>("");
  const messagesEndRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (!connection) return;

    const receiveHandler = (msgDto: { username: string; text: string }) => {
      const cleanText = DOMPurify.sanitize(msgDto.text);
      const cleanUser = DOMPurify.sanitize(msgDto.username);
      if (!cleanText || !cleanUser) return;
      setMessages((prevMessages) => [
        ...prevMessages,
        { text: cleanText, user: cleanUser },
      ]);
    };
    connection.on("ReceiveMessage", receiveHandler);
    return () => {
      connection.off("ReceiveMessage", receiveHandler);
    };
  }, [connection]);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages]);

  const sendMessage = async () => {
    if (!connection || !message || !currentChannel) return;
    try {
      await connection.invoke("SendMessage", currentChannel.id, message);
      setMessage("");
    } catch (err) {
      console.error("Send message failed: ", err);
    }
  };

  return (
    <div className="p-6 max-w-lg mx-auto mt-10 lock-scroll">
      <div className="h-96 overflow-y-scroll border rounded-lg p-4 bg-primary">
        {messages && messages.length <= 0 && (
          <small className="text-gray-400">Inga meddelanden ännu 😞</small>
        )}
        {messages.length > 0 &&
          messages.map((m, i) => (
            <div
              key={i}
              className={`chat ${
                m.user === currentUser ? "chat-end" : "chat-start"
              }`}
            >
              <div className="chat-header">{m.user}</div>
              <div
                className={`chat-bubble ${
                  m.user === currentUser ? "bg-gray-700" : ""
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
