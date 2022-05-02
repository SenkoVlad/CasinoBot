DECLARE @uniqueScriptName VARCHAR(100) = '2022-05-02_AddBettingResults'


IF NOT EXISTS
(
    SELECT 1
    FROM Maintenance.ExecutedOneTimeScripts
    WHERE ExecutedOneTimeScripts.name = @uniqueScriptName
)
    BEGIN
        BEGIN TRANSACTION
        BEGIN TRY

            INSERT INTO dbo.BettingResults (id, gameId, coefficient, isWin)
            VALUES (1, 1, 1,0),
                   (2, 1, 4,1),
                   (3, 2, 1,0),
                   (4, 2, 1.5,1)

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