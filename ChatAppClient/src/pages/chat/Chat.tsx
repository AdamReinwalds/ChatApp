import { Sidebar } from "../../components/sidebar/Sidebar";
import { ChatWindow } from "../../components/chatWindow/ChatWindow";
import { CreateRoomModal } from "../../components/modals/createRoomModal/CreateRoomModal";
import { JoinRoomModal } from "../../components/modals/joinRoomModal/JoinRoomModal";
import "./Chat.css";
import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import {
  getPublicRooms,
  joinChannel,
  getMyChannels,
} from "../../api/ChannelApi";
import { getMessages } from "../../api/MessageApi";

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

export const Chat = ({ currentUser }: { currentUser: string }) => {
  const [isCreateRoomOpen, setIsCreateRoomOpen] = useState(false);
  const [isJoinRoomOpen, setIsJoinRoomOpen] = useState(false);
  const [publicRooms, setPublicRooms] = useState<PublicRoom[]>([]);
  const [myChannels, setMyChannels] = useState<Channel[]>([]);
  const [currentChannel, setCurrentChannel] = useState<Channel | null>(null);
  const [messages, setMessages] = useState<
    { text: string; username: string }[]
  >([]);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );

  useEffect(() => {
    const conn = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7145/chathub", {
        accessTokenFactory: () => sessionStorage.getItem("token") || "",
      })
      .withAutomaticReconnect()
      .build();

    setConnection(conn);

    conn
      .start()
      .then(() => console.log("Connection established"))
      .catch((err) => console.error("Connection failed: ", err));

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
      if (connection) await connection.invoke("JoinChannel", roomId);
      await fetchMyChannels(); // refresh user's channels after joining

      setPublicRooms((prevRooms) =>
        prevRooms.filter((room) => room.id !== roomId)
      );
      console.log("Joined room:", roomId);
    } catch (err) {
      console.error("Failed to join room:", err);
    }
  };

  const onChannelSelect = async (channel: Channel) => {
    setCurrentChannel(channel);
    try {
      const msgs: { text: string; username: string }[] =
        (await getMessages(channel.id)) || [];
      const formattedMsgs = msgs.map((msg) => ({
        text: msg.text,
        username: msg.username,
      }));
      setMessages(formattedMsgs);
    } catch (err) {
      console.error("Failed to fetch messages for channel:", err);
    }
  };

  const handleCreateModalOpen = () => {
    setIsCreateRoomOpen(true);
    document.documentElement.classList.add("lock-scroll");
  };

  const handleJoinModalOpen = async () => {
    setIsJoinRoomOpen(true);
    if (publicRooms.length === 0) {
      await fetchPublicRooms();
    }
    document.documentElement.classList.add("lock-scroll");
  };

  const handleModalClose = async () => {
    setIsCreateRoomOpen(false);
    setIsJoinRoomOpen(false);
    document.documentElement.classList.remove("lock-scroll");
    await fetchMyChannels();
  };

  return (
    <div className="chat-layout">
      <Sidebar
        channels={myChannels}
        onCreateRoom={handleCreateModalOpen}
        onJoinRoom={handleJoinModalOpen}
        onChannelSelect={onChannelSelect}
      />
      <ChatWindow
        connection={connection}
        currentChannel={currentChannel}
        currentUser={currentUser}
        messages={messages}
        setMessages={setMessages}
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
