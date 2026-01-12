alter table dbo.GeneratedFiles
DROP CONSTRAINT FK_GeneratedFiles_Uploads;

ALTER TABLE dbo.GeneratedFiles
ADD CONSTRAINT FK_GeneratedFiles_Uploads
FOREIGN KEY (UploadId) REFERENCES Uploads(Id)
ON DELETE CASCADE;

alter table dbo.Logs
DROP CONSTRAINT FK_Logs_Users ;

ALTER TABLE dbo.Logs
ADD CONSTRAINT FK_Logs_Users 
FOREIGN KEY (UserId) REFERENCES Users(Id)
ON DELETE CASCADE;