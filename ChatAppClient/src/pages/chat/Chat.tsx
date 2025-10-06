import { Sidebar } from "../../components/sidebar/Sidebar";
import { ChatWindow } from "../../components/chatWindow/ChatWindow";
import { CreateRoomModal } from "../../components/modals/createRoomModal/CreateRoomModal";
import { JoinRoomModal } from "../../components/modals/joinRoomModal/JoinRoomModal";
import "./Chat.css";
import { jwtDecode } from "jwt-decode";
import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import {
  getPublicRooms,
  joinChannel,
  getMyChannels,
} from "../../api/ChannelApi";

interface PublicRoom {
  id: number;
  name: string;
  description?: string;
  memberCount: number;
  isPrivate: boolean;
  isJoined?: boolean;
}

interface Channel {
  id: number;
  name: string;
  type: "public" | "private";
  unreadCount?: number;
}

export const Chat = () => {
  const [isCreateRoomOpen, setIsCreateRoomOpen] = useState(false);
  const [isJoinRoomOpen, setIsJoinRoomOpen] = useState(false);
  const [publicRooms, setPublicRooms] = useState<PublicRoom[]>([]);
  const [myChannels, setMyChannels] = useState<Channel[]>([]);
  const [currentChannel, setCurrentChannel] = useState<Channel | null>(null);
  const [currentUser, setCurrentUser] = useState<string>("");
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );

  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (token) {
      const decoded = jwtDecode<{ name: string }>(token);
      if (decoded?.name) setCurrentUser(decoded.name);
    }
  }, []);

  useEffect(() => {
    const conn = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5095/chathub")
      .withAutomaticReconnect()
      .build();

    conn
      .start()
      .then(() => console.log("Connection established"))
      .catch((err) => console.error("Connection failed: ", err));

    setConnection(conn);

    return () => {
      conn.stop().then(() => console.log("Connection stopped"));
    };
  }, []);

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

  const handleJoinRoom = async (roomId: number) => {
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
        onChannelSelect={(channel) => {
          setCurrentChannel(channel);
        }}
      />
      <ChatWindow
        connection={connection}
        currentChannel={currentChannel}
        currentUser={currentUser}
      />
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
