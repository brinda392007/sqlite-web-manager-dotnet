CREATE FUNCTION dbo.IsEmailDuplicate
(
    @Email NVARCHAR(200)
)
RETURNS BIT
AS
BEGIN
    DECLARE @Exists BIT = 0;

    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
        SET @Exists = 1;

    RETURN @Exists;
END;
    