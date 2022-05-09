IF OBJECT_ID('[dbo].[BettingResults]') IS NULL
    CREATE TABLE BettingResults
    (
        id          INT   NOT NULL
            CONSTRAINT BettingResults_pk
                PRIMARY KEY NONCLUSTERED,
        gameId      INT   NOT NULL,
        coefficient FLOAT NOT NULL,
        isWin       BIT   NOT NULL
    )
GO
