import { Sidebar } from "../../components/sidebar/Sidebar";
import { ChatWindow } from "../../components/chatWindow/ChatWindow";
import { CreateRoomModal } from "../../components/modals/createRoomModal/CreateRoomModal";
import "./Chat.css";
import { useState } from "react";
import { JoinRoomModal } from "../../components/modals/joinRoomModal/JoinRoomModal";

interface PublicRoom {
  id: string;
  name: string;
  description?: string;
  memberCount: number;
  isPrivate: boolean;
  isJoined?: boolean;
}

export const Chat = () => {
  const [isCreateRoomOpen, setIsCreateRoomOpen] = useState(false);
  const [isJoinRoomOpen, setIsJoinRoomOpen] = useState(false);
  const [publicRooms, setPublicRooms] = useState<PublicRoom[]>([]);

  const fetchPublicRooms = async () => {
    try {
      const res = await fetch("http://localhost:5095/api/channel/public", {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${sessionStorage.getItem("token")}`,
        },
      });
      if (!res.ok) throw new Error("Failed to fetch public rooms");
      const data = await res.json();
      setPublicRooms(data);
    } catch (err) {
      console.error(err);
    }
  };

  const handleJoinRoom = async (roomId: string) => {
    try {
      const res = await fetch(
        `http://localhost:5095/api/channel/join/${roomId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${sessionStorage.getItem("token")}`,
          },
        }
      );
      if (!res.ok) throw new Error("Failed to join room");
      console.log("Joined room:", roomId);

      // Optional: refresh the sidebar channel list
    } catch (err) {
      console.error(err);
    }
  };

  const handleModalClose = () => {
    setIsCreateRoomOpen(false);
    setIsJoinRoomOpen(false);

    document.documentElement.classList.remove("lock-scroll");
  };

  const handleCreateModalOpen = () => {
    setIsCreateRoomOpen(true);
    document.documentElement.classList.add("lock-scroll");
  };
  const handleJoinModalOpen = () => {
    setIsJoinRoomOpen(true);
    fetchPublicRooms();
    document.documentElement.classList.add("lock-scroll");
  };

  return (
    <div className="chat-layout">
      <Sidebar
        onCreateRoom={handleCreateModalOpen}
        onJoinRoom={handleJoinModalOpen}
      />
      <ChatWindow />
      <CreateRoomModal isOpen={isCreateRoomOpen} onClose={handleModalClose} />
      <JoinRoomModal
        isOpen={isJoinRoomOpen}
        onClose={handleModalClose}
        publicRooms={publicRooms}
        onJoinRoom={handleJoinRoom}
      />
    </div>
  );
};

export default Chat;
