CREATE TABLE Room(
    RoomId VARCHAR(36) PRIMARY KEY,
    RoomName VARCHAR(10),
);

CREATE TABLE ChatUser(
    UserId VARCHAR(36) PRIMARY KEY,
    UserName VARCHAR(10),
    ConnectionId VARCHAR(36),
    RoomId VARCHAR(36) FOREIGN KEY REFERENCES Room(RoomId),
    HasLeft BIT,
    Avatar VARCHAR(255),
);

CREATE TABLE Message(
    MessageId VARCHAR(36) PRIMARY KEY,
    UserId VARCHAR(36) FOREIGN KEY REFERENCES [ChatUser](UserId),
    RoomId VARCHAR(36) FOREIGN KEY REFERENCES Room(RoomId),
    [Text] NVARCHAR(150),
    [Date] DATETIME,
    FromUser BIT,
    isImage BIT,
    ImageUrl VARCHAR(255),
);