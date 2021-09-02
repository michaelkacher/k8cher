using System.Text.Json;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace k8cher.store
{
    public interface ISvelteStoreActor : IActor
    {
        Task<string> SetState(string json);

        Task<string> UpdateProperty(string properyName, string json);
        
        Task<string> GetState();

        Task RegisterReminder();
        Task UnregisterReminder();
    }



    public class SvelteStoreActor : Actor, ISvelteStoreActor, IRemindable
    {
        private Dictionary<string, object> _jsonObj;
        private const string STORE_NAME = "sveltestore";
        public SvelteStoreActor(ActorHost host) : base(host)
        {
        }

        protected override async Task OnActivateAsync()
        {
            // Provides opportunity to perform some optional setup.
            Console.WriteLine($"Activating actor id: {this.Id}");

            // a unique actor per user per store
            // var state = await this.StateManager.TryGetStateAsync<string>(STORE_NAME);
            
            // if (state.HasValue)
            // {
            //     _jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>(state.Value);
            // } 
            // else
            // {
            //     _jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>("{}");
            // }
            _jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>("{}");
        }



        public async Task<string> SetState(string json)
        {
            //_document = JsonDocument.Parse(json);
            try
            {
                // todo - mbk: this is inefficient going to string the desrializing, fix
                _jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error deserializing json: {ex.Message}");
            }
            try
            {
                await this.StateManager.SetStateAsync(STORE_NAME, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting state: {ex.Message}");
            }

            return "success";
        }

        public async Task<string> UpdateProperty(string propertyName, string json)
        {
            _jsonObj[propertyName] = JsonSerializer.Deserialize<object>(json);
            await this.StateManager.SetStateAsync(STORE_NAME, JsonSerializer.Serialize(_jsonObj));
            return "success";
        }

        public async Task<string> GetState()
        {
            return JsonSerializer.Serialize(_jsonObj);
        }


        protected override Task OnDeactivateAsync()
        {
            // Provides Opporunity to perform optional cleanup.
            Console.WriteLine($"Deactivating actor id: {this.Id}");
            return Task.CompletedTask;
        }

        public Task RegisterReminder()
        {
            throw new System.NotImplementedException();
        }

        public Task UnregisterReminder()
        {
            throw new System.NotImplementedException();
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            throw new NotImplementedException();
        }

    }


}
