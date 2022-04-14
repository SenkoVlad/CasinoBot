namespace Casino.BLL.Games;

public abstract class Game
{
    public async Task PlayRoundAsync()
    {
        await SentStartMessageAsync();
        await PlayGameRoundAsync();

        await Task.Delay(3500);

        await SendRoundResultAsync();
        await InitGameAsync();
    }
    protected abstract Task InitGameAsync();
    protected abstract Task SentStartMessageAsync();
    protected abstract Task PlayGameRoundAsync();
    protected abstract Task SendRoundResultAsync();
}