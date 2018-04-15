using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using QnAMakerDialog;
using QnAMakerDialog.Models;

namespace TechBashBot.Dialogs
{
    [Serializable]
    [QnAMakerService("INSERT_SUBSCRIPTION_KEY", "INSERT_KNOWLEDGEBASEID")]
    public class QnADialog : QnAMakerDialog<object>
    {
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync($"Sorry, I couldn't find an answer for '{originalQueryText}'.");
            context.Wait(MessageReceived);
        }

        [QnAMakerResponseHandler(50)]
        public async Task LowScoreHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            await context.PostAsync($"I found an answer that might help...{result.Answers[0].Answer}.");
            context.Wait(MessageReceived);
        }
    }
}