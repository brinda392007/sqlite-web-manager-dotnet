--- Create Databse -----
CREATE DATABASE ASPWeBSM_DB;
GO

USE ASPWeBSM_DB;
GO

--- User table --

CREATE TABLE dbo.Users
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    [Password] NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
);

------ Upload Table ---

CREATE TABLE dbo.Uploads
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    ContentType NVARCHAR(100) NOT NULL,
    Content VARBINARY(MAX) NOT NULL,
    Size BIGINT NOT NULL,
    UploadedAt DATETIME NOT NULL DEFAULT(GETDATE()),
    CONSTRAINT FK_Uploads_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
);


---- Generated file -- 

CREATE TABLE dbo.GeneratedFiles
    (
        FileID INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        UploadId INT NULL, 
        FileName NVARCHAR(255) NOT NULL,
        FilePath NVARCHAR(500) NOT NULL,
        OperationsInfo NVARCHAR(500) NULL, 
        GeneratedDate DATETIME NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT FK_GeneratedFiles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT FK_GeneratedFiles_Uploads FOREIGN KEY (UploadId) REFERENCES dbo.Uploads(Id)
    );


-------- Log Table ------- 

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Logs')
BEGIN
    CREATE TABLE dbo.Logs
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        LogType NVARCHAR(20) NOT NULL,   -- INFO / SUCCESS / ERROR
        Message NVARCHAR(MAX) NOT NULL,
        LogTime DATETIME NOT NULL DEFAULT(GETDATE()),

        CONSTRAINT FK_Logs_Users 
        FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );
END

SELECT * FROM Users


Drop table Logs
