using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using TechBashBot.Model;

namespace TechBashBot.Dialogs
{
    [Serializable]
    public class RegistrationDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var myform = new FormDialog<Registration>(new Registration(), Registration.BuildForm, FormOptions.PromptInStart, null);

            context.Call<Registration>(myform, ResumeAfter);
        }

        private async Task ResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("done with registration");
            context.Done<object>(null);
        }
    }
}