using Telegram.Bot.Types.ReplyMarkups;

namespace Casino.BLL.ButtonsGenerators;

public class InlineKeyboardButtonsGenerator
{
    private InlineKeyboardMarkup _inlineKeyboardMarkup;

    public InlineKeyboardMarkup GetInlineKeyboardMarkup => _inlineKeyboardMarkup;

    public void InitGetInfoButtons()
    {
        var urlButton = new InlineKeyboardButton("gerg");
        var urlButton2 = new InlineKeyboardButton("ger");
        var urlButton3 = new InlineKeyboardButton("gerghh");
        var urlButton4 = new InlineKeyboardButton("gerhtr");

        urlButton.Text = "Go URL1";
        urlButton.Url = "https://www.google.com/";

        urlButton2.Text = "Go URL2";
        urlButton2.Url = "https://www.bing.com/";

        urlButton3.Text = "Go URL3";
        urlButton3.Url = "https://www.duckduckgo.com/";

        urlButton4.Text = "Go URL4";
        urlButton4.Url = "https://stackoverflow.com/";

        // Rows, every row is InlineKeyboardButton[], You can put multiple buttons!
        InlineKeyboardButton[] row1 = { urlButton };
        InlineKeyboardButton[] row2 = { urlButton2, urlButton3 };
        InlineKeyboardButton[] row3 = { urlButton4 };


        // Buttons by rows
        InlineKeyboardButton[][] buttons = { row1, row2, row3 };

        _inlineKeyboardMarkup = new InlineKeyboardMarkup(buttons);
    }
}