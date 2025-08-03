CREATE Table [Roles](

[Id] INT PRIMARY KEY IDENTITY(1,1),
[roleName] VARCHAR(50) UNIQUE NOT NULL,
[createdAt] DATETIME NOT NULL DEFAULT GETDATE(),
[updatedAt] DATETIME NULL
);


CREATE Table [Users](

[Id] INT PRIMARY KEY IDENTITY(1,1),
[name] VARCHAR(80) NOT NULL,
[email] VARCHAR(100) UNIQUE NOT NULL CHECK (LEN(email) > 5
AND email LIKE '%_@__%.__%' -- regex for email
AND email NOT LIKE '%  %' -- blocks spaces
AND email NOT LIKE '%@%@%' -- blocks multiple @
),
[passwordHash] VARCHAR(255) NOT NULL,
[roleId] INT NOT NULL REFERENCES [Roles]([Id]),
[createdAt] DATETIME NOT NULL DEFAULT GETDATE(),
[updatedAt] DATETIME NULL
);

CREATE NONCLUSTERED INDEX IX_Users_roleId ON Users(roleId);

CREATE Table [Rooms](

[Id] INT PRIMARY KEY IDENTITY(1,1),
[name] VARCHAR(50) NOT NULL UNIQUE,
[location] VARCHAR(50) NOT NULL,
[capacity] INT NOT NULL CHECK(capacity > 0),
[createdAt] DATETIME NOT NULL DEFAULT GETDATE(),
[updatedAt] DATETIME NULL
);

CREATE Table [Features](

[Id] INT PRIMARY KEY IDENTITY(1,1),
[featureName] VARCHAR(60) NOT NULL UNIQUE,
[createdAt] DATETIME NOT NULL DEFAULT GETDATE(),
[updatedAt] DATETIME NULL
);


CREATE Table [RoomFeatures](

[roomId] INT NOT NULL REFERENCES [Rooms]([Id]) ON DELETE CASCADE,
[featureId] INT NOT NULL REFERENCES [Features]([Id]) ON DELETE CASCADE,
PRIMARY KEY([roomId] , [featureId])
);

CREATE NONCLUSTERED INDEX IX_RoomFeatures_roomId ON RoomFeatures(roomId);
CREATE NONCLUSTERED INDEX IX_RoomFeatures_featureId ON RoomFeatures(featureId);


CREATE Table [Meetings](

[Id] INT PRIMARY KEY IDENTITY(1,1),
[roomId] INT NOT NULL REFERENCES[Rooms]([Id]) ,
[title] VARCHAR(100) NOT NULL UNIQUE,
[agenda] VARCHAR(MAX),
[startTime] DATETIME NOT NULL,
[endTime] DATETIME NOT NULL,
CHECK (startTime < endTime),
[createdBy] INT NULL REFERENCES[Users]([Id]) ON DELETE SET NULL,
[createdAt] DATETIME NOT NULL DEFAULT GETDATE(),
[updatedAt] DATETIME NULL,
[status] VARCHAR(40) CHECK ([status] IN ('Scheduled' , 'Completed' , 'Cancelled'))
);

CREATE NONCLUSTERED INDEX IX_Meetings_roomId ON Meetings(roomId);
CREATE NONCLUSTERED INDEX IX_Meetings_createdBy ON Meetings(createdBy);

CREATE Table [Moms](

[Id] INT PRIMARY KEY IDENTITY(1,1),
[meetingId] INT NOT NULL REFERENCES[Meetings]([Id]) ON DELETE CASCADE,
[notes] VARCHAR(MAX),
[decisions] VARCHAR(MAX) NOT NULL,
[createdAt] DATETIME NOT NULL DEFAULT GETDATE(),
[updatedAt] DATETIME NULL
);

CREATE NONCLUSTERED INDEX IX_Moms_meetingId ON Moms(meetingId);

CREATE Table [ActionItems](

[Id] INT PRIMARY KEY IDENTITY(1,1),
[momId] INT NOT NULL REFERENCES[Moms]([Id]) ON DELETE CASCADE,
[description] VARCHAR(MAX) NOT NULL,
[assignedTo] INT NULL REFERENCES[Users]([Id]),
[dueDate] DATE NOT NULL,
[createdAt] DATETIME NOT NULL DEFAULT GETDATE(),
[updatedAt] DATETIME NULL,
[status] VARCHAR(50) NOT NULL CHECK([status] IN ('Assigned' , 'Pending' , 'Complete'))
);

CREATE NONCLUSTERED INDEX IX_ActionItems_momId ON ActionItems(momId);
CREATE NONCLUSTERED INDEX IX_ActionItems_assignedTo ON ActionItems(assignedTo);


CREATE Table [Attachments](

[Id] INT PRIMARY KEY IDENTITY(1,1),
[momId] INT NOT NULL REFERENCES[Moms]([Id]) ON DELETE CASCADE,
[filePath] VARCHAR(MAX) NOT NULL,
[fileName] VARCHAR(200) NOT NULL,
[createdAt] DATETIME NOT NULL DEFAULT GETDATE(),
[updatedAt] DATETIME NULL
);

CREATE NONCLUSTERED INDEX IX_Attachments_momId ON Attachments(momId);

CREATE Table [Attendees](

[meetingId] INT NOT NULL REFERENCES[Meetings]([Id]) ON DELETE CASCADE,
[userId] INT NOT NULL REFERENCES[Users]([Id]) ON DELETE CASCADE,
PRIMARY KEY([meetingId] , [userId])

);

CREATE NONCLUSTERED INDEX IX_Attendees_meetingId ON Attendees(meetingId);
CREATE NONCLUSTERED INDEX IX_Attendees_userId ON Attendees(userId);
