import { useEffect, useState } from "react";
import "./JoinRoomModal.css";

interface PublicRoom {
  id: number;
  name: string;
  description?: string;
  memberCount: number;
  isPrivate: boolean;
  isJoined?: boolean;
}

interface JoinRoomModalProps {
  isOpen: boolean;
  onClose: () => void;
  onJoinRoom: (roomId: number) => void;
  publicRooms: PublicRoom[];
}

export const JoinRoomModal = ({
  isOpen,
  onClose,
  onJoinRoom,
  publicRooms,
}: JoinRoomModalProps) => {
  const [searchQuery, setSearchQuery] = useState("");
  const [joinCode, setJoinCode] = useState("");
  const [activeTab, setActiveTab] = useState<"browse" | "code">("browse");
  const [filteredRooms, setFilteredRooms] = useState<PublicRoom[]>(publicRooms);

  useEffect(() => {
    setFilteredRooms(
      publicRooms.filter(
        (room) =>
          room.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
          (room.description &&
            room.description.toLowerCase().includes(searchQuery.toLowerCase()))
      )
    );
  }, [searchQuery, publicRooms]);

  const handleJoin = (roomId: number) => {
    onJoinRoom(roomId);
    onClose();
  };

  const handleJoinByCode = (e: React.FormEvent) => {
    e.preventDefault();
    if (!joinCode.trim()) return;
    alert("Join by code not implemented yet: " + joinCode);
  };

  if (!isOpen) return null;

  return (
    <>
      <div className="modal-overlay" onClick={onClose}></div>

      <div className="modal-content">
        <div className="modal-header">
          <h2>Join a Room</h2>
          <button className="close-button" onClick={onClose}>
            &times;
          </button>
        </div>

        {/* Tabs */}
        <div className="modal-tabs">
          <button
            className={activeTab === "browse" ? "active" : ""}
            onClick={() => setActiveTab("browse")}
          >
            Browse Rooms
          </button>
          <button
            className={activeTab === "code" ? "active" : ""}
            onClick={() => setActiveTab("code")}
          >
            Join by Code
          </button>
        </div>

        {activeTab === "browse" ? (
          <div className="modal-body">
            <input
              type="text"
              placeholder="Search rooms..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="search-input"
            />
            <ul className="room-list">
              {filteredRooms.length > 0 ? (
                filteredRooms.map((room) => (
                  <li key={room.id} className="room-item">
                    <div className="room-info">
                      <strong>{room.name}</strong>
                      <p className="room-description">{room.description}</p>
                      <span className="member-count">
                        <img
                          src="members-icon.png"
                          alt="Members"
                          className="icon"
                        />
                        {room.memberCount}{" "}
                        {room.memberCount === 1 ? "member" : "members"}
                      </span>
                    </div>
                    <button
                      className="join-button"
                      onClick={() => handleJoin(room.id)}
                      disabled={room.isJoined}
                    >
                      Join
                    </button>
                  </li>
                ))
              ) : (
                <p className="no-rooms">No rooms found</p>
              )}
            </ul>
          </div>
        ) : (
          <form className="modal-body" onSubmit={handleJoinByCode}>
            <input
              type="text"
              placeholder="Enter room invite code"
              value={joinCode}
              onChange={(e) => setJoinCode(e.target.value)}
              required
            />
            <button type="submit">Join</button>
          </form>
        )}
      </div>
    </>
  );
};
