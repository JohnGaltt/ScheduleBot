using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Dialogs;

namespace SchedulerBot.Models
{
    [Serializable]
    public class BotScheduler
    {
        [Prompt("Enter the time of the meeting")]
        public string TimeOfTheMeeting;
        [Prompt("Enter the title of the meeting")]
        public string TitleOfTheMeeting;

        public static IForm<BotScheduler> BuildForm()
        {
            return new FormBuilder<BotScheduler>()
                .Message("Welcome to The BotScheduler!")
                .OnCompletion(async (context, profileForm) =>
                {
                    context.PrivateConversationData.SetValue<bool>(
                        "ProfileComplete", true);
                    context.PrivateConversationData.SetValue<string>(
                        "TimeOfTheMeeting", profileForm.TimeOfTheMeeting);
                    context.PrivateConversationData.SetValue<string>(
                        "TitleOfTheMeeting", profileForm.TitleOfTheMeeting);
                  

                    await context.PostAsync("Your profile was completed");
                }).Build();
        }
    }
}