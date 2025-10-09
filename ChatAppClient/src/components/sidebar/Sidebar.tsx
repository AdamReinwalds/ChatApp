import "./Sidebar.css";
import { useState } from "react";

interface Channel {
  id: number;
  name: string;
  type: "public" | "private";
  unreadCount?: number;
}

interface SidebarProps {
  channels: Channel[];
  onChannelSelect?: (channel: Channel) => void;
  onCreateRoom?: () => void;
  onJoinRoom?: () => void;
}

export const Sidebar = ({
  channels,
  onChannelSelect,
  onCreateRoom,
  onJoinRoom,
}: SidebarProps) => {
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedChannel, setSelectedChannel] = useState<number | null>(null);

  const handleChannelClick = (channel: Channel) => {
    setSelectedChannel(channel.id);
    onChannelSelect?.(channel);
  };

  return (
    <div className="sidebar bg-base-200 w-64 min-h-screen p-4 flex flex-col">
      {/* Header */}
      <div className="mb-6">
        <h2 className="text-xl font-bold text-base-content mb-4">ChatApp</h2>
        <div className="relative">
          <input
            type="text"
            placeholder="Search channels..."
            className="input input-bordered input-sm w-full pl-8"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
      </div>

      {/* Action Buttons */}
      <div className="mb-6 space-y-2">
        <button
          onClick={onCreateRoom}
          className="btn btn-primary btn-sm w-full"
        >
          Create Room
        </button>
        <button onClick={onJoinRoom} className="btn btn-outline btn-sm w-full">
          Join Room
        </button>
      </div>

      {/* Channels */}
      <div className="flex-1">
        <h3 className="text-sm font-semibold mb-2">Text Channels</h3>
        <ul className="space-y-1">
          {channels
            .filter((c) =>
              c.name.toLowerCase().includes(searchQuery.toLowerCase())
            )
            .map((channel) => (
              <li key={channel.id}>
                <button
                  onClick={() => handleChannelClick(channel)}
                  className={`w-full text-left px-3 py-2 rounded-md text-sm flex items-center justify-between hover:bg-base-300 ${
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
      </div>
    </div>
  );
};
