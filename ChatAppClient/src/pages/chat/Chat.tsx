import { Sidebar } from "../../components/sidebar/Sidebar";
import { ChatWindow } from "../../components/chatWindow/ChatWindow";
import "./Chat.css";

export const Chat = () => {
  return (
    <div className="chat-layout">
      <Sidebar />
      <ChatWindow />
    </div>
  );
};

export default Chat;
