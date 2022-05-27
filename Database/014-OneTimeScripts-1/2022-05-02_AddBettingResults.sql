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

            INSERT INTO dbo.BettingResults (id, gameId, coefficient, isWon, diceResult)
            VALUES (1,1,1,0,0),
                    (2,1,4,0,0),
                    (3,2,1,0,0),
                    (4,2,1.5,1,5),
                    (5,2,1.5,1,4),
                    (6,2,1.5,1,3),
                    (7,3,1,0,0),
                    (8,3,3,1,6),
                    (9,3,1.5,1,4),
                    (10,3,1,1,2),
                    (11, 4, 1, 0, 0),
                    (12, 4, 2.5, 1, 5),
                    (13, 4, 1.5, 1, 4);




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