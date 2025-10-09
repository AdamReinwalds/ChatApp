import { useEffect, useRef, useState } from "react";
import DOMPurify from "dompurify";
import * as SignalR from "@microsoft/signalr";

interface ChatWindowProps {
  connection: SignalR.HubConnection | null;
  currentUser: string;
  messages: { text: string; username: string }[];
  setMessages: React.Dispatch<
    React.SetStateAction<{ text: string; username: string }[]>
  >;
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
  messages = [],
  setMessages,
}: ChatWindowProps) => {
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
        { text: cleanText, username: cleanUser },
      ]);
    };

    connection.on("ReceiveMessage", receiveHandler);
    return () => {
      connection.off("ReceiveMessage", receiveHandler);
    };
  }, [connection, setMessages]);

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
                m.username === currentUser ? "chat-end" : "chat-start"
              }`}
            >
              <div className="chat-header">{m.username}</div>
              <div
                className={`chat-bubble ${
                  m.username === currentUser ? "bg-gray-700" : ""
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
