using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    internal class Program
    {
        static string hiddenNumber;

 
        static void Main(string[] args)
        {
            var tgClient = new TelegramBotClient("7101524591:AAEESUyVwZszLFDiP7gSpz_bwlVfGS-g7Qg");

            tgClient.StartReceiving(HandleUpdate, HandleError);


            Console.ReadKey();

        }


        private static async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var chatId = update.Message!.Chat.Id;

                var message = update.Message.Text;
            

                // user name in telegram profile
                var userName = update.Message.From!.FirstName;
                if(message == "/start")
                {
                    var text = $"Hello {userName}, You have entered the Bulls and Cows game! Please select an action:";

                    var keyboard = new InlineKeyboardMarkup(
                        [
                            [InlineKeyboardButton.WithCallbackData("Game rules", "/rules")],
                            [InlineKeyboardButton.WithCallbackData("Start game", "/gamestart")]
                        ]);

                    await client.SendMessage(chatId, text, replyMarkup: keyboard);
                }
                else if(message?.Length > 4 || message?.Length < 4)
                {

                    var text = "You entered a number longer than four digits or did not enter a valid number.";

                    await client.SendMessage(chatId, text);

                }
                else 
                {
                    //guessed the number
                    (int bullsCount, int cowsCount) = CalculateBullsAndCowsCount(message);

                    var text = $"- Bulls: {bullsCount}\n - Cows: {cowsCount}\n\n";

                    await client.SendMessage(chatId, text);


                    if (bullsCount == 4)
                    {

                        await client.SendMessage(chatId, "What an amazing game! You won, Great job!");

                    }
                }

            }
            else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                var chatId = update.CallbackQuery.Message.Chat.Id;
                var message = update.CallbackQuery.Data;



                switch (message)
                {
                    case "/rules":
                        {
                            var text = "Rules:\r\n\n" +
                                        "The bot generates a random 4-digit number, and your task is to guess it. " +
                                        "Each digit is from 0-9. All digits in the number are unique, meaning numbers like 1233 are not allowed! " +
                                        "The number can start with 0, so numbers like 0829 may appear! " +
                                        "When making your guesses, you can get bulls and cows." +
                                        "\n🐂 Bull – a digit is in its correct position." +
                                        "\n🐄 Cow – a digit exists but is not in its correct position. \r\n\n" +
                                        "For guessing the number, you can earn from 10 points, depending on how many attempts it took you to guess. " +
                                        "For example, if you guessed the number in 6 attempts, you will get 4 points. " +
                                        "You can also go into the negative, in which case a certain number of points will be deducted from your score. " +
                                        "However, your score cannot go below zero; the minimum is 0!\r\n\n" +
                                        "In online mode, the points earned are fixed," +
                                        " meaning you will get 20 points for a win and -10 points for a loss. In online mode, you and your opponent take turns " +
                                        "trying to guess the number. Your goal is to guess it first; otherwise, you will lose!\r\n";


                            var keyboard = new InlineKeyboardMarkup([
                                [
                                    InlineKeyboardButton.WithCallbackData("🎮Play", "/gamestart")
                                ]]);
                            await client.SendMessage(chatId, text, replyMarkup : keyboard);
                            break;
                        }
                    case "/gamestart":
                        {
                            hiddenNumber = GenerateHiddenNumber();

                            var text = $"Bot guessed a number!\nEnter your number:";
                            await client.SendMessage(chatId, text);

                            break;
                        }
                    default:
                        break;
                }
            }
            
        }

        private static (int bullsCount, int cowsCount) CalculateBullsAndCowsCount(string message)
        {
            int bullsCount = 0;
            int cowsCount = 0;


            for (int i = 0; i < message.Length; i++)
            {
                for(int j = 0; j < hiddenNumber.Length; j++)
                {
                    if(message[i] == hiddenNumber[j])
                    {
                        if(i == j)
                        {
                            bullsCount++;
                        }
                        else
                        {
                            cowsCount++;
                        }
                    }
                }
            }

            return (bullsCount, cowsCount);

        }

        private static string GenerateHiddenNumber()
        {
            var digits = Enumerable.Range(0, 10).ToList();
            var rnd = new Random();
            string randomNumber = "";

            while(randomNumber.Length != 4)
            {
                int randomIndex = rnd.Next(digits.Count);
                randomNumber += digits[randomIndex];
                digits.RemoveAt(randomIndex);
            }

            return randomNumber;
        }

        private static async Task HandleError(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
