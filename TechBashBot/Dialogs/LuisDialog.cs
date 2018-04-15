using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace TechBashBot.Dialogs
{
    [LuisModel("INSERT_MODEL_ID", "INSERT_SUBSCRIPTION_KEY")]
    [Serializable]
    public class LuisDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.Forward(new QnADialog(), ResumeAfter, context.Activity, CancellationToken.None);
        }

        [LuisIntent("Sessions")]
        public async Task Sessions(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var entity = result.Entities.Where(e => e.Type == "topic").Select(e => e.Entity).ToList();

            await context.Forward(new SessionsDialog(entity), ResumeAfter, context.Activity, CancellationToken.None);

        }

        [LuisIntent("Schedule")]
        public async Task Schedule(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var topic = result.Entities.FirstOrDefault(e => e.Type == "topic")?.Entity;
            var speakers = result.Entities.Where(e => e.Type == "speaker")?.Select(e => e.Entity).ToList();
            var times = result.Entities.Where(e => e.Type == "time")?.Select(e => e.Entity).ToList();
            var day = result.Entities.SingleOrDefault(e => e.Type == "day")?.Entity;
            await context.Forward(new ScheduleDialog(topic, speakers, times, day), ResumeAfter, context.Activity, CancellationToken.None);

        }

        [LuisIntent("Registration")]
        public async Task Registration(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {

            await context.Forward(new RegistrationDialog(), ResumeAfter, context.Activity, CancellationToken.None);

        }
        private async Task ResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            context.Done<object>(null);
        }
    }
}