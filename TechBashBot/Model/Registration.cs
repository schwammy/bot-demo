using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;

namespace TechBashBot.Model
{
    [Serializable]
    public class Registration
    {
        public RegistrationTypes? RegistrationType;
        public DayOptions? DayOption;
        public DietaryRestrictionOptions DietaryRestriction;
        [Prompt("What is your your {&}? {||}")]
        public string Name;
        public string EmailAddress;
        public List<InterestsOptions> Interests;


        public static IForm<Registration> BuildForm()
        {

            return new FormBuilder<Registration>()
                .Message("Start your registration for TechBash!")
                .Field(nameof(Name))
                .Field(nameof(EmailAddress))
                .Field(nameof(RegistrationType))
                .Field(nameof(DayOption))
                .Confirm(async (state) =>
                {
                    var cost = 0.0;
                    switch (state.DayOption)
                    {
                        case DayOptions.Wednesday:
                        case DayOptions.Thursday:
                        case DayOptions.Friday:
                            cost = 100;
                            break;                            
                        case DayOptions.ThreeDay:
                            cost = 200;
                            break;
                        case DayOptions.ThreeDayPlusWorkshops:
                                cost = 300;
                                break;
                    }
                    return new PromptAttribute($"Your ticket for TechBash will cost {cost:C2} is that ok?");
                })
                .Field(nameof(DietaryRestriction))
                .Field(nameof(Interests),
                    validate: async (state, value) =>
                    {
                        var values = ((List<object>)value).OfType<InterestsOptions>();
                        var result = new ValidateResult { IsValid = true, Value = values };
                        if (values != null && values.Contains(InterestsOptions.Everything))
                        {
                            result.Value = (from InterestsOptions interest in Enum.GetValues(typeof(InterestsOptions))
                                where interest != InterestsOptions.Everything && !values.Contains(interest)
                                select interest).ToList();
                        }
                        return result;
                    })
                .Confirm(async (state) =>
                {
                    string message = string.Empty;
                    if (state.DietaryRestriction != 0 && state.DietaryRestriction != DietaryRestrictionOptions.NoDietaryRestrictions)
                    {
                        message = $"Note, we've got your dietary restriction: {state.DietaryRestriction}.";
                    }
                    return new PromptAttribute($"Would you like to complete your registration as: {state.RegistrationType}? " + message);
                })
                .OnCompletion(async (context, order) =>
                {
                    var message = $"Thanks for your order. We will send an email confirmation to { order.EmailAddress}";
                    await context.PostAsync(message);
                })
                .Build();
        }
    }

    public enum RegistrationTypes
    {
        Attendee,
        Staff,
        Speaker,
        Sponsor
    }

    public enum DayOptions
    {
        ThreeDay,
        ThreeDayPlusWorkshops,
        Wednesday,
        Thursday,
        Friday
    }

    public enum DietaryRestrictionOptions
    {
        Vegetarian,
        Vegan,
        GluttenFree,
        Kosher,
        NoDietaryRestrictions
    }

    public enum InterestsOptions
    {
        [Terms("except", "but", "not", "no", "all", "everything")]
        Everything = 1,
        Azure,
        BotFramework,
        Patterns,
        AspNetCore,
        DotNetCore,
        Sharepoint
    }
}
