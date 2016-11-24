using ContosoBankBotApp.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContosoBankBotApp
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Timeline> timelineTable;
        private IMobileServiceTable<Bankdata> bankdataTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("https://msap2contosobanktable.azurewebsites.net/");
            this.timelineTable = this.client.GetTable<Timeline>();
            this.bankdataTable = this.client.GetTable<Bankdata>();
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }

        public async Task<List<Timeline>> GetTimelines()
        {
            return await this.timelineTable.ToListAsync();
        }

        public async Task<List<Bankdata>> GetBankdatas()
        {
            return await this.bankdataTable.ToListAsync();
        }

        public async Task AddTimeline(Timeline timeline)
        {
            await this.timelineTable.InsertAsync(timeline);
        }

        public async Task AddBankdata(Bankdata bankdata)
        {
            await this.bankdataTable.InsertAsync(bankdata);
        }
    }
}