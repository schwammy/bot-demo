using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using TechBashBot.Model;

namespace TechBashBot.Dialogs
{
    [Serializable]
    public class ScheduleDialog : IDialog<object>
    {
        private readonly string _topicInput;
        private readonly List<string> _speakerNamesInput;
        private readonly List<string> _timesInput;
        private string _dayInput;

        private List<Session> _sessions;

        public ScheduleDialog(string topic, List<string> speakerNames, List<string> times, string day)
        {
            _topicInput = topic;
            _speakerNamesInput = speakerNames ?? new List<string>();
            _timesInput = times ?? new List<string>();
            _dayInput = day;
            _sessions = GetData();
        }


        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            var query = _sessions.Select(s => s);
            List<string> afternoon = new List<string>{"1","2","3","4","5"};
            var replyMessage = context.MakeMessage();

            if (!string.IsNullOrWhiteSpace(_topicInput))
                query = query.Where(s => s.Title.ToLower().Contains(_topicInput.ToLower()));


            if (_speakerNamesInput.Any())
            {
                var speakerSearch = string.Join(" ", _speakerNamesInput);
                query = query.Where(s => s.SpeakerName.ToLower().Contains(speakerSearch));
            }

            if (!_speakerNamesInput.Any() && string.IsNullOrWhiteSpace(_topicInput))
            {
                replyMessage.Text = "Sorry, I can't find any sessions about that. Please ask about a specific topic or speaker (or try a different topic or speaker)";
                await context.PostAsync(replyMessage);
                context.Done(true);
                return;
            }

            
            if (_timesInput.Any())
            {
                // if there is a time with no date, we assume the user means "today" but since this is fake, assume today = thursday
                if (string.IsNullOrWhiteSpace(_dayInput)) _dayInput = "thurs";

                string startTime;
                var hour = _timesInput.FirstOrDefault().ToLower();
                //this is a bug in the luis logic
                if (hour.StartsWith("at"))
                {
                    hour = hour.Remove(0, 3);
                }

                var minutes = "00";
                if (_timesInput.Count == 2)
                {
                    minutes = _timesInput[1];
                }

                if (hour == "next")
                {
                    startTime = GetNextTimeSlot();
                }
                else
                {

                    if (afternoon.Contains(hour))
                    {
                        hour = (int.Parse(hour) + 12).ToString();
                    }
                    hour = hour.PadLeft(2, '0');
                    startTime = $"{hour}:{minutes}";
                }

                query = query.Where(s => s.StartTime.Contains(startTime));

                
            }
            if (!string.IsNullOrWhiteSpace(_dayInput))
            {
                string day;
                switch (_dayInput.ToLower())
                {
                    case "wed":
                    case "wednesday":
                        day = "04";
                        break;
                    case "thur":
                    case "thurs":
                    case "thursday":
                    case "today":
                        day = "05";
                        break;
                    case "fri":
                    case "friday":
                    case "tomorrow":
                        day = "06";
                        break;
                    default:
                        day = "05";
                        break;
                }
                string dateFilter = $"2017-10-{day}";

                query = query.Where(s => s.StartTime.Contains(dateFilter));
            }


            var result = query.ToList();


            

            if (!result.Any())
            {
                replyMessage.Text = "Sorry, no sessions found";
                await context.PostAsync(replyMessage);
                context.Done(true);
                return;
            }


            foreach (var session in result)
            {
                var attachment = ToCard(session);
                replyMessage.Attachments.Add(attachment);


            }
            await context.PostAsync(replyMessage);
            context.Done(true);

        }

        private string GetNextTimeSlot()
        {
            // insert some logic here:
            return "15:30";
        }

        private Attachment ToCard(Session session)
        {
            var card = new AdaptiveCard();
            card.Body.Add(new TextBlock() { Text = session.Title, Weight = TextWeight.Bolder, Size = TextSize.Medium });
            card.Body.Add(new TextBlock() { Text = $"{session.SpeakerName} at {session.StartTime}", Size = TextSize.Medium });
            card.Body.Add(new TextBlock() { Text = session.Description, Wrap = true });


            ShowCardAction showCardAction = new ShowCardAction();
            showCardAction.Title = "Rate this session";
            card.Actions.Add(showCardAction);

            var reviewCard = new AdaptiveCard();
            var ratings = new ChoiceSet()
            {
                Style = ChoiceInputStyle.Compact,
                IsMultiSelect = false,

                Choices = new List<Choice>()
                {
                    new Choice() {Title = "1", Value = "1"},
                    new Choice() {Title = "2", Value = "2"},
                    new Choice() {Title = "3", Value = "3"},
                    new Choice() {Title = "4", Value = "4"},
                    new Choice() {Title = "5", Value = "5"},
                }
            };
            reviewCard.Body.Add(new TextBlock() { Text = "Let us know what you thought of this session" });
            reviewCard.Body.Add(ratings);
            var data = new { session.Title, Action = "ReviewSession" };
            reviewCard.Actions.Add(new SubmitAction() { Title = "Submit feedback", Data = data, DataJson = JsonConvert.SerializeObject(data)});
            
            showCardAction.Card = reviewCard;

            Attachment attachment = new Attachment()

            {

                ContentType = AdaptiveCard.ContentType,

                Content = card

            };

            return attachment;
        }

        public List<Session> GetData()
        {
            return JsonConvert.DeserializeObject<List<Session>>(SessionData.GetJsonString());

        }
       
    }
}