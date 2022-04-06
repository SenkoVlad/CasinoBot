using Casino.BLL.ButtonPushHandlers.Interfaces;
using Casino.BLL.ButtonsHelper;
using Casino.Common.AppConstants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonPushHandlers.Implementation
{
    public class KeyboardButtonPushHandler : IButtonPushHandler
    {
        private readonly Message _message;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly CancellationToken _cancellationToken;
        private ReplyKeyboardMarkup? _previousButtons;

        private ReplyKeyboardMarkup? _replyButtons;
        private string? _replyText;

        public KeyboardButtonPushHandler(Message message,
            ITelegramBotClient telegramBotClient,
            CancellationToken cancellationToken)
        {
            _message = message;
            _telegramBotClient = telegramBotClient;
            _cancellationToken = cancellationToken;
            _replyText = message.Text;
        }

        public async Task PushAsync(string? commandText)
        {
            switch (commandText)
            {
                case ButtonTextConstants.StartGameButtonText:
                    _replyButtons = ButtonsGeneratorHelper.GetGamesButtons();
                    _previousButtons = ButtonsGeneratorHelper.GetStartButtons();
                    break;
                case ButtonTextConstants.GetInfoButtonText:
                    _replyText = "This is info";
                    _previousButtons = ButtonsGeneratorHelper.GetStartButtons();
                    break;  
                case ButtonTextConstants.BackButtonText:
                    _previousButtons = ButtonsGeneratorHelper.GetGamesButtons();
                    _replyButtons = _previousButtons;
                    break;
            }
            await _telegramBotClient.SendTextMessageAsync(_message.Chat!, _replyText,
                cancellationToken: _cancellationToken, replyMarkup: _replyButtons);
        }

        public string? GetReplyText()
        {
            return _replyText;
        }

        public ReplyKeyboardMarkup? GetReplyButtons()
        {
            return _replyButtons;
        }
    }
}
