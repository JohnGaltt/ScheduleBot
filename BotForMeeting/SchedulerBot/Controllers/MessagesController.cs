using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System.Text;
using System;

namespace SchedulerBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                StateClient sc = activity.GetStateClient();
                BotData userdata = sc.BotState.GetPrivateConversationData(
                    activity.ChannelId, activity.Conversation.Id, activity.From.Id);

                var boolProfileComplete = userdata.GetProperty<bool>("ProfileComplete");
                if (!boolProfileComplete)
                {
                    await Conversation.SendAsync(activity, MakeRootDialog);
                }
                else
                {
                    var TimeOfTheMeeting = userdata.GetProperty<string>("TimeOfTheMeeting");
                    var TitleOfTheMeeting = userdata.GetProperty<string>("TitleOfTheMeeting");

                    StringBuilder sb = new StringBuilder();
                    sb.Append("Your profile was completed.\n\n");
                    sb.Append(String.Format("The time of the meeting is : '{0}'\n",TimeOfTheMeeting));
                    sb.Append(String.Format("The title of the meeting is : '{0}'",TitleOfTheMeeting));



                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    Activity replyMessage = activity.CreateReply(sb.ToString());
                    await connector.Conversations.ReplyToActivityAsync(replyMessage);

                }
               
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        internal static IDialog<Models.BotScheduler> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(Models.BotScheduler.BuildForm));
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}