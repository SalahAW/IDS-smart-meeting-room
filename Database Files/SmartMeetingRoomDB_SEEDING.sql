BEGIN TRY
    BEGIN TRANSACTION;

    -- 1. SEEDING ROLES (with idempotency check)
    IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE roleName = 'Admin')
    BEGIN
        INSERT INTO [Roles] (roleName) VALUES ('Admin');
    END
    IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE roleName = 'Employee')
    BEGIN
        INSERT INTO [Roles] (roleName) VALUES ('Employee');
    END
    IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE roleName = 'Guest')
    BEGIN
        INSERT INTO [Roles] (roleName) VALUES ('Guest');
    END
    -- We insert only if the role doesn't exist yet, preventing duplicates.

    -- 2. SEEDING USERS
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE email = 'salahawji2@gmail.com')
    BEGIN
        INSERT INTO [Users] ([name], [email], [passwordHash], [roleId])
        VALUES ('Salah', 'salahawji2@gmail.com', 'salah09@!22', 1);
    END
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE email = 'mishlawispoa@yahoo.com')
    BEGIN
        INSERT INTO [Users] ([name], [email], [passwordHash], [roleId])
        VALUES ('Mishlawi', 'mishlawispoa@yahoo.com', 'mishn3w122', 2);
    END
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE email = 'Jakesap122@outlook.com')
    BEGIN
        INSERT INTO [Users] ([name], [email], [passwordHash], [roleId])
        VALUES ('Jake', 'Jakesap122@outlook.com', 'jakeSapa11', 2);
    END
    IF NOT EXISTS (SELECT 1 FROM [Users] WHERE email = 'Linda9004@gmail.com')
    BEGIN
        INSERT INTO [Users] ([name], [email], [passwordHash], [roleId])
        VALUES ('Linda', 'Linda9004@gmail.com', 'Lindapola012', 3);
    END
    -- Check by email (unique), insert only if user doesn't exist.

    -- 3. SEEDING ROOMS
    IF NOT EXISTS (SELECT 1 FROM [Rooms] WHERE name = '102-15B')
    BEGIN
        INSERT INTO [Rooms] ([name], [location], [capacity])
        VALUES ('102-15B', 'Cluster 15 Block B 2nd Floor', 18);
    END
    IF NOT EXISTS (SELECT 1 FROM [Rooms] WHERE name = '109-15B')
    BEGIN
        INSERT INTO [Rooms] ([name], [location], [capacity])
        VALUES ('109-15B', 'Cluster 15 Block B 2nd Floor', 18);
    END
    IF NOT EXISTS (SELECT 1 FROM [Rooms] WHERE name = '221-15A')
    BEGIN
        INSERT INTO [Rooms] ([name], [location], [capacity])
        VALUES ('221-15A', 'Cluster 15 Block A 3rd Floor', 23);
    END

    -- 4. SEEDING FEATURES
    DECLARE @featureCount INT;
    SET @featureCount = (SELECT COUNT(*) FROM [Features] WHERE featureName IN ('Projector', 'Air Conditioner', 'LCD Screen', 'Speakers', 'Whiteboard', 'Wi-Fi Access', 'Comfortable Seating', 'Smart Lighting', 'Coffee Machine'));
    IF @featureCount < 9 -- Not all features present
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'Projector')
            INSERT INTO [Features] (featureName) VALUES ('Projector');
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'Air Conditioner')
            INSERT INTO [Features] (featureName) VALUES ('Air Conditioner');
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'LCD Screen')
            INSERT INTO [Features] (featureName) VALUES ('LCD Screen');
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'Speakers')
            INSERT INTO [Features] (featureName) VALUES ('Speakers');
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'Whiteboard')
            INSERT INTO [Features] (featureName) VALUES ('Whiteboard');
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'Wi-Fi Access')
            INSERT INTO [Features] (featureName) VALUES ('Wi-Fi Access');
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'Comfortable Seating')
            INSERT INTO [Features] (featureName) VALUES ('Comfortable Seating');
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'Smart Lighting')
            INSERT INTO [Features] (featureName) VALUES ('Smart Lighting');
        IF NOT EXISTS (SELECT 1 FROM [Features] WHERE featureName = 'Coffee Machine')
            INSERT INTO [Features] (featureName) VALUES ('Coffee Machine');
    END

    -- 5. SEEDING ROOMFEATURES
    -- Insert each room-feature pair if not already present
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 1 AND featureId = 1)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (1, 1);
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 1 AND featureId = 2)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (1, 2);
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 1 AND featureId = 4)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (1, 4);
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 1 AND featureId = 5)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (1, 5);
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 1 AND featureId = 6)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (1, 6);
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 2 AND featureId = 9)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (2, 9);
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 2 AND featureId = 2)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (2, 2);
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 3 AND featureId = 8)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (3, 8);
    IF NOT EXISTS (SELECT 1 FROM [RoomFeatures] WHERE roomId = 3 AND featureId = 7)
        INSERT INTO [RoomFeatures] (roomId, featureId) VALUES (3, 7);

    -- 6. SEEDING MEETINGS
    IF NOT EXISTS (SELECT 1 FROM [Meetings] WHERE title = 'Internship Program Launch')
    BEGIN
        INSERT INTO [Meetings] ([roomId], [title], [agenda], [startTime], [endTime], [createdBy], [status])
        VALUES (1, 'Internship Program Launch',
            '- Discuss Launch Details
- Discuss Internship Budget
- Manpower Allocation
- Internship Program Location Decision
- Internship Goals',
            '2025-08-03 13:00:00', '2025-08-03 14:10:00', 1, 'Scheduled');
    END

    -- 7. SEEDING MOMS (Minutes of Meeting)
    IF NOT EXISTS (SELECT 1 FROM [Moms] WHERE meetingId = 1 AND decisions = 'Approved the budget and timeline for launch.')
    BEGIN
        INSERT INTO [Moms] ([meetingId], [notes], [decisions])
        VALUES (1, 'Discussed the internship launch timeline and budget allocation.', 'Approved the budget and timeline for launch.');
    END

    -- 8. SEEDING ACTION ITEMS
    IF NOT EXISTS (SELECT 1 FROM [ActionItems] WHERE momId = 1 AND description = 'Prepare internship launch presentation.')
    BEGIN
        INSERT INTO [ActionItems] ([momId], [description], [assignedTo], [dueDate], [status])
        VALUES (1, 'Prepare internship launch presentation.', 1, '2025-08-01', 'Assigned');
    END

    -- 9. SEEDING ATTACHMENTS
    IF NOT EXISTS (SELECT 1 FROM [Attachments] WHERE momId = 1 AND fileName = 'internship_launch_agenda.pdf')
    BEGIN
        INSERT INTO [Attachments] ([momId], [filePath], [fileName])
        VALUES (1, '/files/internship_launch_agenda.pdf', 'internship_launch_agenda.pdf');
    END

    -- 10. SEEDING ATTENDEES
    IF NOT EXISTS (SELECT 1 FROM [Attendees] WHERE meetingId = 1 AND userId = 1)
    BEGIN
        INSERT INTO [Attendees] ([meetingId], [userId]) VALUES (1, 1);
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
