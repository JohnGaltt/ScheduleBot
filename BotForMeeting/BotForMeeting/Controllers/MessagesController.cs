using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Text;
namespace BotForMeeting
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
                string strTime = "";
                string strTitleOfTheMeeting = "";
                string strSayCreate = "";
                bool boolAskForMeeting = false;
                StateClient sc = activity.GetStateClient();
                BotData userdata = sc.BotState.GetPrivateConversationData(
                    activity.ChannelId, activity.Conversation.Id, activity.From.Id);
                boolAskForMeeting = userdata.GetProperty<bool>("AskedForMeeting");
                strTitleOfTheMeeting = userdata.GetProperty<string>("TitleOfMeeting") ?? "";
                strTime = userdata.GetProperty<string>("TimeOfMeeting") ?? "";
                strSayCreate = userdata.GetProperty<string>("Create") ?? "";
                StringBuilder strMessage = new StringBuilder();
                if (boolAskForMeeting == false)
                {
                    strMessage.Append($"Hello, I am bot-scheduler\n");
                    strMessage.Append($"Say create and I will make a meeting inside me");
                    userdata.SetProperty<bool>("AskedForMeeting", true);

                  
                }
                else
                {
                    if (activity.Text.ToLower().Contains("create"))
                    {
                        strMessage.Append($"Enter a time and title");
                        if (strTime == "" && strTitleOfTheMeeting == "")
                        {
                            strMessage.Append($"Enter a time and title");
                            string[] mes = activity.Text.Split(' ');
                            userdata.SetProperty<string>($"TimeOfMeeting", mes[0]);
                            userdata.SetProperty<string>($"TitleOfMeeting", mes[1]);
                            //strMessage.Append($"Hello {activity.Text}");
                            //userdata.SetProperty<string>("")
                         }
                        
                       else{
                            strMessage.Append($"Your time is {strTime}, The title is {strTitleOfTheMeeting}");
                        }
                    }
                    else
                    {

                    }
                    sc.BotState.SetPrivateConversationData(activity.ChannelId, activity.Conversation.Id, activity.From.Id, userdata);
                    // Create reply message
                    ConnectorClient connector = new ConnectorClient(new System.Uri(activity.ServiceUrl));
                    Activity replyMessage = activity.CreateReply(strMessage.ToString());
                    await connector.Conversations.ReplyToActivityAsync(replyMessage);
                    //await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());

                }

                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
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