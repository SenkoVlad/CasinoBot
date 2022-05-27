DECLARE @uniqueScriptName VARCHAR(100) = '2022-05-02_AddGames'


IF NOT EXISTS
(
    SELECT 1
    FROM Maintenance.ExecutedOneTimeScripts
    WHERE ExecutedOneTimeScripts.name = @uniqueScriptName
)
    BEGIN
        BEGIN TRANSACTION
        BEGIN TRY

            INSERT INTO dbo.Games (name)
            VALUES ('Dice'),
                   ('Football'),
                   ('Darts'),
                   ('Basketball')

            INSERT INTO Maintenance.ExecutedOneTimeScripts(name)
            VALUES
            (
               @uniqueScriptName
            )

            IF @@TRANCOUNT > 0
                COMMIT TRANSACTION;
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;
            DECLARE @ErrorMessage NVARCHAR(4000);
            DECLARE @ErrorSeverity INT;
            DECLARE @ErrorState INT;

            SELECT @ErrorMessage = ERROR_MESSAGE(),
                   @ErrorSeverity = ERROR_SEVERITY(),
                   @ErrorState = ERROR_STATE();

            RAISERROR(@ErrorMessage,@ErrorSeverity,@ErrorState);
        END CATCH
    END