using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Todo
{
	public class TodoItemDatabase 
	{
        private HttpClient client;

		public TodoItemDatabase()
		{
            client = new HttpClient();
        }

        public async Task<IEnumerable<TodoItem>> GetItems ()
		{
            var result = await client.GetStringAsync(App.CurrentApp.ApiUrl + "/todo");
            return JsonConvert.DeserializeObject<TodoItem[]>(result);
        }

		public async Task<IEnumerable<TodoItem>> GetItemsNotDone ()
		{
            var result = await client.GetStringAsync(App.CurrentApp.ApiUrl + "/todo");
            return JsonConvert.DeserializeObject<TodoItem[]>(result);
        }

		public async Task SaveItem (TodoItem item) 
		{
            var body = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
            if (String.IsNullOrWhiteSpace(item.ID))
            {
                await client.PostAsync(App.CurrentApp.ApiUrl + "/todo", body);             
            }
            else
            {
                await client.PutAsync(App.CurrentApp.ApiUrl + "/todo/" + item.ID, body);
            }
        }

		public async Task DeleteItem(TodoItem item)
		{
            await client.DeleteAsync(App.CurrentApp.ApiUrl + "/todo/" + item.ID);
        }
	}
}

