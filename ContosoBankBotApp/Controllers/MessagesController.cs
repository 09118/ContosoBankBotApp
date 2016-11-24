using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using ContosoBankBotApp.Models;
using System.Collections.Generic;
using ContosoBankBotApp.DataModels;

namespace ContosoBankBotApp
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
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                String userMessage = activity.Text.ToUpper();

                string endOutput = "Hello";

                bool isCurrencyRequest = true;

                if (userMessage.Contains("HELLO"))
                {
                    endOutput = "Hello";
                    isCurrencyRequest = false;
                    // calculate something for us to return
                    if (userData.GetProperty<bool>("SentGreeting"))
                    {
                        endOutput = "Hello again";
                    }
                    else
                    {
                        userData.SetProperty<bool>("SentGreeting", true);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    }
                }                

                if (userMessage.Contains("CLEAR"))
                {
                    endOutput = "User data cleared";
                    await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    isCurrencyRequest = false;
                }
                
                if (userMessage.ToLower().Equals("get timelines"))
                {
                    List<Timeline> timelines = await AzureManager.AzureManagerInstance.GetTimelines();
                    endOutput = "";
                    foreach (Timeline t in timelines)
                    {
                        endOutput += "[" + t.Date + "] Happiness " + t.Happiness + ", Sadness " + t.Sadness + "\n\n";
                    }
                    isCurrencyRequest = false;

                }

                if (userMessage.ToLower().Equals("new timeline"))
                {
                    Timeline timeline = new Timeline()
                    {
                        Anger = 0.1,                       
                        Happiness = 0.3,
                        Sadness = 0.4,
                        Date = DateTime.Now
                    };

                    await AzureManager.AzureManagerInstance.AddTimeline(timeline);

                    isCurrencyRequest = false;

                    endOutput = "New timeline added [" + timeline.Date + "]";
                }
                
            
                HttpClient client = new HttpClient();

                string currency = null; //The specific currency user wants to convert. 
                string baseCurrency = "USD"; //The default base currency is set to USD.
                string dateTime = "latest";
                bool SupportedCurrencyFormat = true; //Check whether user have typed in supported currency format

                if (userMessage.Length > 9)
                {
                    if (userMessage.ToLower().Substring(0, 8).Equals("set base") && userMessage.Length == 12)
                    {
                        baseCurrency = userMessage.Substring(9);
                        userData.SetProperty<string>("BaseCurrency", baseCurrency);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        endOutput = "Now your base currency is " + baseCurrency;
                        isCurrencyRequest = false;
                    }
                    if (userMessage.ToLower().Substring(0, 8).Equals("set time"))
                    {
                        dateTime = userMessage.Substring(9);
                        userData.SetProperty<string>("DateTime", dateTime);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        endOutput = "Now your currency time taken is " + dateTime;
                        isCurrencyRequest = false;
                    }
                }

                if (userData.GetProperty<string>("BaseCurrency") != null)
                {
                    baseCurrency = userData.GetProperty<string>("BaseCurrency");
                }

                if (userData.GetProperty<string>("DateTime") != null)
                {
                    dateTime = userData.GetProperty<string>("DateTime");
                    if (userData.GetProperty<string>("DateTime").Contains("LATEST") || userData.GetProperty<string>("DateTime").Contains("NOW") || userData.GetProperty<string>("DateTime").Contains("TODAY"))
                    {
                        dateTime = "latest";
                    }

                }

                if (!isCurrencyRequest)
                {
                    // return our reply to the user
                    Activity infoReply = activity.CreateReply(endOutput);
                    await connector.Conversations.ReplyToActivityAsync(infoReply);
                }
                else
                {

                    if (userMessage.Length == 3)
                    {
                        string x = await client.GetStringAsync(new Uri("http://api.fixer.io/" + dateTime + "?base=" + baseCurrency + "&symbols=" + userMessage));
                        CurrencyObject.RootObject rootObject;
                        rootObject = JsonConvert.DeserializeObject<CurrencyObject.RootObject>(x);

                        if (userMessage.Contains("AUD"))
                        {
                            currency = "$" + rootObject.rates.AUD;
                        }
                        else if (userMessage.Contains("BGN"))
                        {
                            currency = "$" + rootObject.rates.BGN;
                        }
                        else if (userMessage.Contains("BRL"))
                        {
                            currency = "$" + rootObject.rates.BRL;
                        }
                        else if (userMessage.Contains("CAD"))
                        {
                            currency = "$" + rootObject.rates.CAD;
                        }
                        else if (userMessage.Contains("CHF"))
                        {
                            currency = "$" + rootObject.rates.CHF;
                        }
                        else if (userMessage.Contains("CNY"))
                        {
                            currency = "$" + rootObject.rates.CNY;
                        }
                        else if (userMessage.Contains("CZK"))
                        {
                            currency = "$" + rootObject.rates.CZK;
                        }
                        else if (userMessage.Contains("DKK"))
                        {
                            currency = "$" + rootObject.rates.DKK;
                        }
                        else if (userMessage.Contains("GBP"))
                        {
                            currency = "$" + rootObject.rates.GBP;
                        }
                        else if (userMessage.Contains("HKD"))
                        {
                            currency = "$" + rootObject.rates.HKD;
                        }
                        else if (userMessage.Contains("HRK"))
                        {
                            currency = "$" + rootObject.rates.HRK;
                        }
                        else if (userMessage.Contains("HUF"))
                        {
                            currency = "$" + rootObject.rates.HUF;
                        }
                        else if (userMessage.Contains("IDR"))
                        {
                            currency = "$" + rootObject.rates.IDR;
                        }
                        else if (userMessage.Contains("ILS"))
                        {
                            currency = "$" + rootObject.rates.ILS;
                        }
                        else if (userMessage.Contains("INR"))
                        {
                            currency = "$" + rootObject.rates.INR;
                        }
                        else if (userMessage.Contains("JPY"))
                        {
                            currency = "$" + rootObject.rates.JPY;
                        }
                        else if (userMessage.Contains("KRW"))
                        {
                            currency = "$" + rootObject.rates.KRW;
                        }
                        else if (userMessage.Contains("MXN"))
                        {
                            currency = "$" + rootObject.rates.MXN;
                        }
                        else if (userMessage.Contains("MYR"))
                        {
                            currency = "$" + rootObject.rates.MYR;
                        }
                        else if (userMessage.Contains("NOK"))
                        {
                            currency = "$" + rootObject.rates.NOK;
                        }
                        else if (userMessage.Contains("NZD"))
                        {
                            currency = "$" + rootObject.rates.NZD;
                        }
                        else if (userMessage.Contains("PHP"))
                        {
                            currency = "$" + rootObject.rates.PHP;
                        }
                        else if (userMessage.Contains("PLN"))
                        {
                            currency = "$" + rootObject.rates.PLN;
                        }
                        else if (userMessage.Contains("RON"))
                        {
                            currency = "$" + rootObject.rates.RON;
                        }
                        else if (userMessage.Contains("RUB"))
                        {
                            currency = "$" + rootObject.rates.RUB;
                        }
                        else if (userMessage.Contains("SEK"))
                        {
                            currency = "$" + rootObject.rates.SEK;
                        }
                        else if (userMessage.Contains("SGD"))
                        {
                            currency = "$" + rootObject.rates.SGD;
                        }
                        else if (userMessage.Contains("THB"))
                        {
                            currency = "$" + rootObject.rates.THB;
                        }
                        else if (userMessage.Contains("TRY"))
                        {
                            currency = "$" + rootObject.rates.TRY;
                        }
                        else if (userMessage.Contains("USD"))
                        {
                            currency = "$" + rootObject.rates.USD;
                        }
                        else if (userMessage.Contains("ZAR"))
                        {
                            currency = "$" + rootObject.rates.ZAR;
                        }
                        else
                        {
                            endOutput = "We currently do not support currency " + userMessage;
                            Activity infoReply = activity.CreateReply(endOutput);
                            infoReply = activity.CreateReply(endOutput);
                            await connector.Conversations.ReplyToActivityAsync(infoReply);
                            SupportedCurrencyFormat = false;
                        }

                        if (currency == "$0")
                        {
                            currency = "$1"; //This means user is converting the same currency, therefore 1 to 1 conversion.
                        }

                        if (SupportedCurrencyFormat == true)
                        {
                            // return our reply to the user
                            //Activity reply = activity.CreateReply($"Currency exchange rate for {activity.Text} is {currency}");
                            //await connector.Conversations.ReplyToActivityAsync(reply);

                            // return our reply to the user
                            Activity currencyReply = activity.CreateReply($"Currency exchange rate for {userMessage} from the base currency {baseCurrency}");
                            currencyReply.Recipient = activity.From;
                            currencyReply.Type = "message";
                            currencyReply.Attachments = new List<Attachment>();

                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: "http://www.wrench.at/img/ExchangeRates/icon/currencies@2x.png"));

                            List<CardAction> cardButtons = new List<CardAction>();
                            CardAction plButton = new CardAction()
                            {
                                Value = "http://fixer.io",
                                Type = "openUrl",
                                Title = "More API Info"
                            };
                            cardButtons.Add(plButton);

                            ThumbnailCard plCard = new ThumbnailCard()
                            {
                                Title = baseCurrency + " to" +"\n" + userMessage + "\n" + "conversion",
                                Subtitle = currency,
                                Images = cardImages,
                                Buttons = cardButtons
                            };

                            Attachment plAttachment = plCard.ToAttachment();
                            currencyReply.Attachments.Add(plAttachment);
                            await connector.Conversations.SendToConversationAsync(currencyReply);

                        }
                    }
                }
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