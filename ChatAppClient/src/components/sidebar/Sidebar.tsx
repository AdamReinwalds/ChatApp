import { useEffect, useState } from "react";
import "./Sidebar.css";

interface Channel {
  id: string;
  name: string;
  type: "public" | "private";
  unreadCount?: number;
}

interface SidebarProps {
  onChannelSelect?: (channel: Channel) => void;
  onCreateRoom?: () => void;
  onJoinRoom?: () => void;
}

export const Sidebar = ({
  onChannelSelect,
  onCreateRoom,
  onJoinRoom,
}: SidebarProps) => {
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedChannel, setSelectedChannel] = useState<string>("");
  const [channels, setChannels] = useState<Channel[]>([]);

  const getChannels = async () => {
    try {
      const response = await fetch("http://localhost:5095/api/channel/my", {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${sessionStorage.getItem("token")}`,
        },
      });
      if (!response.ok) {
        throw new Error("Failed to fetch channels");
      }
      const data = await response.json();
      return data as Channel[];
    } catch (error) {
      console.error("Error fetching channels:", error);
      return [];
    }
  };

  useEffect(() => {
    const fetchChannels = async () => {
      const channelList = await getChannels();
      console.log("Fetched channels:", channelList);
      setChannels(channelList);
    };
    fetchChannels();
  }, []);

  const handleChannelClick = (channel: Channel) => {
    setSelectedChannel(channel.id);
    onChannelSelect?.(channel);
  };

  const handleJoinRoom = () => {
    onJoinRoom?.();
  };

  return (
    <div className="sidebar bg-base-200 w-64 min-h-screen p-4 flex flex-col">
      {/* Header */}
      <div className="mb-6">
        <h2 className="text-xl font-bold text-base-content mb-4">ChatApp</h2>

        {/* Search Bar */}
        <div className="relative">
          <input
            type="text"
            placeholder="Search channels..."
            className="input input-bordered input-sm w-full pl-8"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <svg
            className="absolute left-2 top-2.5 h-4 w-4 text-base-content/50"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
            />
          </svg>
        </div>
      </div>

      {/* Action Buttons */}
      <div className="mb-6 space-y-2">
        <button
          onClick={onCreateRoom}
          className="btn btn-primary btn-sm w-full"
        >
          <svg
            className="w-4 h-4 mr-2"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 4v16m8-8H4"
            />
          </svg>
          Create Room
        </button>

        <button
          onClick={handleJoinRoom}
          className="btn btn-outline btn-sm w-full"
        >
          <svg
            className="w-4 h-4 mr-2"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 6v6m0 0v6m0-6h6m-6 0H6"
            />
          </svg>
          Join Room
        </button>
      </div>

      {/* Channels Section */}
      <div className="flex-1">
        <div className="flex items-center mb-3">
          <h3 className="text-sm font-semibold text-base-content/70 uppercase tracking-wide">
            Text Channels
          </h3>
        </div>

        <ul className="space-y-1">
          {channels.map((channel) => (
            <li key={channel.id}>
              <button
                onClick={() => handleChannelClick(channel)}
                className={`w-full text-left px-3 py-2 rounded-md text-sm flex items-center justify-between hover:bg-base-300 transition-colors ${
                  selectedChannel === channel.id
                    ? "bg-primary text-primary-content"
                    : "text-base-content"
                }`}
              >
                <div className="flex items-center">
                  <span className="mr-2">#</span>
                  <span className="truncate">{channel.name}</span>
                </div>
                {channel.unreadCount && channel.unreadCount > 0 && (
                  <span className="badge badge-sm badge-error text-xs">
                    {channel.unreadCount}
                  </span>
                )}
              </button>
            </li>
          ))}
        </ul>

        {channels.length === 0 && searchQuery && (
          <div className="text-center py-4 text-base-content/50 text-sm">
            No channels found matching "{searchQuery}"
          </div>
        )}
      </div>

      {/* User Info at bottom */}
      <div className="mt-auto pt-4 border-t border-base-300">
        <div className="flex items-center space-x-3">
          <div className="avatar placeholder">
            <div className="bg-primary text-primary-content rounded-full w-8">
              <span className="text-xs">U</span>
            </div>
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium text-base-content truncate">
              Username
            </p>
            <p className="text-xs text-base-content/50 truncate">Online</p>
          </div>
        </div>
      </div>
    </div>
  );
};
