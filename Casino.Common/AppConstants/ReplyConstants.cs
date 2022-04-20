namespace Casino.Common.AppConstants;

public class ReplyConstants
{
    public const string UnavailableCommandReplyText = "unavailable command";
    public static readonly int[] GoalScores = {5, 3, 4};

    public static string GetMyBalanceMessage(string balance) => $"Your balance is {balance}💲";
}