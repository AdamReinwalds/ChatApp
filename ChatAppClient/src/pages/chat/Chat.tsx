import { Sidebar } from "../../components/sidebar/Sidebar";
import { ChatWindow } from "../../components/chatWindow/ChatWindow";
import { CreateRoomModal } from "../../components/modals/createRoomModal/CreateRoomModal";
import { JoinRoomModal } from "../../components/modals/joinRoomModal/JoinRoomModal";
import "./Chat.css";
import { useEffect, useState } from "react";
import {
  getPublicRooms,
  joinChannel,
  getMyChannels,
} from "../../api/ChannelApi";

interface PublicRoom {
  id: string;
  name: string;
  description?: string;
  memberCount: number;
  isPrivate: boolean;
  isJoined?: boolean;
}

interface Channel {
  id: string;
  name: string;
  type: "public" | "private";
  unreadCount?: number;
}

export const Chat = () => {
  const [isCreateRoomOpen, setIsCreateRoomOpen] = useState(false);
  const [isJoinRoomOpen, setIsJoinRoomOpen] = useState(false);
  const [publicRooms, setPublicRooms] = useState<PublicRoom[]>([]);
  const [myChannels, setMyChannels] = useState<Channel[]>([]);

  // Fetch the user's own channels
  const fetchMyChannels = async () => {
    try {
      const channels = await getMyChannels();
      setMyChannels(channels);
    } catch (err) {
      console.error("Failed to fetch my channels:", err);
    }
  };

  // Fetch public channels only when modal opens
  const fetchPublicRooms = async () => {
    try {
      const rooms = await getPublicRooms();
      setPublicRooms(rooms);
    } catch (err) {
      console.error("Failed to fetch public rooms:", err);
    }
  };

  useEffect(() => {
    fetchMyChannels(); // fetch only once on mount
  }, []);

  const handleJoinRoom = async (roomId: string) => {
    try {
      await joinChannel(roomId);
      await fetchMyChannels(); // refresh user's channels after joining

      setPublicRooms((prevRooms) =>
        prevRooms.filter((room) => room.id !== roomId)
      );
      console.log("Joined room:", roomId);
    } catch (err) {
      console.error("Failed to join room:", err);
    }
  };

  const handleCreateModalOpen = () => {
    setIsCreateRoomOpen(true);
    document.documentElement.classList.add("lock-scroll");
  };

  const handleJoinModalOpen = async () => {
    setIsJoinRoomOpen(true);
    if (publicRooms.length === 0) {
      await fetchPublicRooms(); // fetch only once per session
    }
    document.documentElement.classList.add("lock-scroll");
  };

  const handleModalClose = () => {
    setIsCreateRoomOpen(false);
    setIsJoinRoomOpen(false);
    document.documentElement.classList.remove("lock-scroll");
  };

  return (
    <div className="chat-layout">
      <Sidebar
        channels={myChannels}
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
