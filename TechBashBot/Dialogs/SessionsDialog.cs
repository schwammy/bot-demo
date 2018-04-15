using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using TechBashBot.Model;

namespace TechBashBot.Dialogs
{
    [Serializable]
    public class SessionsDialog : IDialog<object>
    {
        private readonly List<string> _topics;
        private readonly List<Session> _sessions;

        public SessionsDialog(List<string> topics)
        {
            _topics = topics;
            _sessions = GetData();
        }


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            var response = context.MakeMessage();
            var sessions = _sessions.Where(s => _topics.Any(s.Title.ToLower().Contains)).ToList();

            if (!_topics.Any())
            {
                response.Text = "We have lots of sessions at TechBash. Try asking \"Do you have any sessions about Azure?\"";
                await context.PostAsync(response);
                context.Done(true);
                
            }


            if (!sessions.Any())
            {
                response.Text = "Sorry, no sessions found";
                await context.PostAsync(response);
                context.Done(true);
            }

            StringBuilder sb = new StringBuilder();

            // * is markdown for bullet list
            foreach (var session in sessions)
            {
                sb.Append("* " +  session.Title + Environment.NewLine);
            }
            sb.Append(Environment.NewLine + $"I found {sessions.Count} sessions of interest for you!");
            response.Text = sb.ToString();
            await context.PostAsync(response);
            context.Done(true);

        }

        public List<Session> GetData()
        {
            return JsonConvert.DeserializeObject<List<Session>>(SessionData.GetJsonString());

        }

       
    }
}