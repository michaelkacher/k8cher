using Dapr.Actors.Runtime;
using System;
using System.Threading.Tasks;

namespace k8cher.store
{
    public interface ISvelteStoreActor : IActor
    {
        Task<string> SetState(string son);
        Task<string> GetState();
        Task RegisterReminder();
        Task UnregisterReminder();
    }


    public class SvelteStoreActor : Actor, ISvelteStoreActor, IRemindable
    {
        // todo - mbk: change this from string to System.Json.Text, ran into issue with how it serilizes Dictionaries
        private string _json;
        private const string STORE_NAME = "sveltestore";
        public SvelteStoreActor(ActorHost host) : base(host)
        {
        }

        protected override async Task OnActivateAsync()
        {
            // Provides opportunity to perform some optional setup.
            Console.WriteLine($"Activating actor id: {this.Id}");

            // a unique actor per user per store
            var state = await this.StateManager.TryGetStateAsync<string>(STORE_NAME);

            if (state.HasValue)
            {
                _json = state.Value;
            }
            else
            {
                _json = "{}";
            }
        }


        public async Task<string> SetState(string json)
        {
            try
            {
                _json = json;
                await this.StateManager.SetStateAsync(STORE_NAME, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting state: {ex.Message}");
            }

            return "success";
        }

        public async Task<string> GetState()
        {
            return _json;
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
