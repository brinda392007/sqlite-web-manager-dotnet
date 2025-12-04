-- User SP ---


CREATE PROC dbo.Users_CRUD
(
    @Id        INT            = 0,
    @Username  NVARCHAR(100)  = NULL,
    @Email     NVARCHAR(255)  = NULL,
    @Password  NVARCHAR(255)  = NULL,
    @EVENT     VARCHAR(50)    = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@EVENT = 'SELECT')
    BEGIN
        SELECT Id, Username, Email, CreatedAt
        FROM dbo.Users;
    END

    ELSE IF (@EVENT = 'INSERT')
    BEGIN
        INSERT INTO dbo.Users (Username, Email, [Password])
        VALUES (@Username, @Email, @Password);

        SELECT SCOPE_IDENTITY() AS NewUserId;
    END

    ELSE IF (@EVENT = 'UPDATE')
    BEGIN
        UPDATE dbo.Users
        SET Username = @Username,
            Email    = @Email,
            [Password] = @Password
        WHERE Id = @Id;
    END

    ELSE IF (@EVENT = 'DELETE')
    BEGIN
        DELETE FROM dbo.Users
        WHERE Id = @Id;
    END

    ELSE IF (@EVENT = 'SELECT_BY_ID')
    BEGIN
        SELECT Id, Username, Email, CreatedAt
        FROM dbo.Users
        WHERE Id = @Id;
    END

    ELSE IF (@EVENT = 'LOGIN')
    BEGIN
        SELECT Id, Username, Email, CreatedAt
        FROM dbo.Users
        WHERE Username = @Username
          AND [Password] = @Password;
    END
END




---- Upload SP ---

CREATE PROC dbo.Uploads_CRUD
(
    @Id          INT              = 0,
    @UserId      INT              = 0,
    @FileName    NVARCHAR(255)    = NULL,
    @ContentType NVARCHAR(100)    = NULL,
    @Content     VARBINARY(MAX)   = NULL,
    @Size        BIGINT           = NULL,
    @EVENT       VARCHAR(50)      = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@EVENT = 'SELECT')
    BEGIN
        IF (@UserId > 0)
        BEGIN
            SELECT Id, FileName, Size, UploadedAt
            FROM dbo.Uploads
            WHERE UserId = @UserId
            ORDER BY UploadedAt DESC;
        END
        ELSE
        BEGIN
            SELECT Id, UserId, FileName, Size, UploadedAt
            FROM dbo.Uploads
            ORDER BY UploadedAt DESC;
        END
    END

    ELSE IF (@EVENT = 'INSERT')
    BEGIN
        INSERT INTO dbo.Uploads (UserId, FileName, ContentType, Content, Size)
        VALUES (@UserId, @FileName, @ContentType, @Content, @Size);

        SELECT SCOPE_IDENTITY() AS NewUploadId;
    END

    ELSE IF (@EVENT = 'UPDATE')
    BEGIN
        UPDATE dbo.Uploads
        SET FileName    = @FileName,
            ContentType = @ContentType,
            Content     = @Content,
            Size        = @Size
        WHERE Id = @Id;
    END

    ELSE IF (@EVENT = 'DELETE')
    BEGIN
        DELETE FROM dbo.Uploads
        WHERE Id = @Id
          AND (@UserId = 0 OR UserId = @UserId); -- for safety
    END

    ELSE IF (@EVENT = 'SELECT_BY_ID')
    BEGIN
        SELECT Id, UserId, FileName, ContentType, Content, Size, UploadedAt
        FROM dbo.Uploads
        WHERE Id = @Id
          AND (@UserId = 0 OR UserId = @UserId);
    END
END


---- Generate Files ----

CREATE PROC dbo.GeneratedFiles_CRUD
(
    @FileID        INT             = 0,
    @UserId        INT             = 0,
    @UploadId      INT             = NULL,
    @FileName      NVARCHAR(255)   = NULL,
    @FilePath      NVARCHAR(500)   = NULL,
    @OperationsInfo NVARCHAR(500)  = NULL,
    @EVENT         VARCHAR(50)     = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF (@EVENT = 'SELECT')
    BEGIN
        -- If UserId passed, filter by it; else select all
        IF (@UserId > 0)
        BEGIN
            SELECT FileID, UploadId, FileName, FilePath, OperationsInfo, GeneratedDate
            FROM dbo.GeneratedFiles
            WHERE UserId = @UserId
            ORDER BY GeneratedDate DESC;
        END
        ELSE
        BEGIN
            SELECT FileID, UserId, UploadId, FileName, FilePath, OperationsInfo, GeneratedDate
            FROM dbo.GeneratedFiles
            ORDER BY GeneratedDate DESC;
        END
    END

    ELSE IF (@EVENT = 'INSERT')
    BEGIN
        INSERT INTO dbo.GeneratedFiles (UserId, UploadId, FileName, FilePath, OperationsInfo)
        VALUES (@UserId, @UploadId, @FileName, @FilePath, @OperationsInfo);

        SELECT SCOPE_IDENTITY() AS NewFileId;
    END

    ELSE IF (@EVENT = 'UPDATE')
    BEGIN
        UPDATE dbo.GeneratedFiles
        SET FileName      = @FileName,
            FilePath      = @FilePath,
            OperationsInfo = @OperationsInfo
        WHERE FileID = @FileID;
    END

    ELSE IF (@EVENT = 'DELETE')
    BEGIN
        DELETE FROM dbo.GeneratedFiles
        WHERE FileID = @FileID
          AND (@UserId = 0 OR UserId = @UserId);
    END

    ELSE IF (@EVENT = 'SELECT_BY_ID')
    BEGIN
        SELECT FileID, UserId, UploadId, FileName, FilePath, OperationsInfo, GeneratedDate
        FROM dbo.GeneratedFiles
        WHERE FileID = @FileID
          AND (@UserId = 0 OR UserId = @UserId);
    END
END