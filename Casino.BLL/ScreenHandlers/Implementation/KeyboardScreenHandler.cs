using Casino.BLL.ButtonsGenerators;
using Casino.BLL.ScreenHandlers.Interfaces;
using Casino.Common.AppConstants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ScreenHandlers.Implementation
{
    public class KeyboardScreenHandler : IScreenHandler
    {
        private readonly Chat _chat;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly CancellationToken _cancellationToken;
        private string[] _availableCommands;

        private ReplyKeyboardMarkup? _replyKeyboardButtons;
        private string? _replyText;
        private KeyboardButtonsGenerator _keyboardButtonsGenerator;

        public string? GetReplyText => _replyText;
        public ReplyKeyboardMarkup? GetReplyKeyboardButtons => _replyKeyboardButtons;
        
        public KeyboardScreenHandler(Message message,
            ITelegramBotClient telegramBotClient,
            CancellationToken cancellationToken)
        {
            _chat = message.Chat;
            _telegramBotClient = telegramBotClient;
            _cancellationToken = cancellationToken;
            _replyText = message.Text;

            _keyboardButtonsGenerator = new KeyboardButtonsGenerator();
            _keyboardButtonsGenerator.InitStartButtons();
            _availableCommands = _keyboardButtonsGenerator.GetAvailableCommands;
        }

        public async Task PushButtonAsync(string? commandText)
        {
            if (!_availableCommands.Contains(commandText))
            {
                _replyText = ReplyConstants.UnavailableCommandReplyText;
            }
            else
            {
                switch (commandText)
                {
                    case ButtonTextConstants.StartGameButtonText:
                        PushStartGameButton();
                        break;
                    case ButtonTextConstants.GetInfoButtonText:
                        PushGetInfoButton();
                        break;
                    case ButtonTextConstants.DiceGameButtonText:
                        PushDiceGameButton();
                        break;
                    case ButtonTextConstants.BackButtonText:
                        PushBackButton();
                        break;
                }
            }
            
            await _telegramBotClient.SendTextMessageAsync(_chat.Id, _replyText,
                cancellationToken: _cancellationToken, replyMarkup: _replyKeyboardButtons); ;
        }

        private void PushBackButton()
        {
            _keyboardButtonsGenerator.InitStartButtons();
            _replyKeyboardButtons = _keyboardButtonsGenerator.GetReplyKeyboardMarkup;
            _availableCommands = _keyboardButtonsGenerator.GetAvailableCommands;
            _replyText = ButtonTextConstants.BackButtonText;
        }

        private void PushDiceGameButton()
        {
            _keyboardButtonsGenerator.InitDiceGameButtons();
            _replyKeyboardButtons = _keyboardButtonsGenerator.GetReplyKeyboardMarkup;
            _availableCommands = _keyboardButtonsGenerator.GetAvailableCommands;
            _replyText = ButtonTextConstants.DiceGameButtonText;
        }

        private void PushGetInfoButton()
        {
            _keyboardButtonsGenerator.InitStartButtons();
            _replyKeyboardButtons = _keyboardButtonsGenerator.GetReplyKeyboardMarkup;
            _availableCommands = _keyboardButtonsGenerator.GetAvailableCommands;
            _replyText = ButtonTextConstants.GetInfoButtonText;
        }

        private void PushStartGameButton()
        {
            _keyboardButtonsGenerator.InitGamesButtons();
            _replyKeyboardButtons = _keyboardButtonsGenerator.GetReplyKeyboardMarkup;
            _availableCommands = _keyboardButtonsGenerator.GetAvailableCommands;
            _replyText = ButtonTextConstants.StartGameButtonText;
        }
    }
}
