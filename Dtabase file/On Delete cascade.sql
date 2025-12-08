alter table dbo.GeneratedFiles
DROP CONSTRAINT FK_GeneratedFiles_Uploads;

ALTER TABLE dbo.GeneratedFiles
ADD CONSTRAINT FK_GeneratedFiles_Uploads
FOREIGN KEY (UploadId) REFERENCES Uploads(Id)
ON DELETE CASCADE;