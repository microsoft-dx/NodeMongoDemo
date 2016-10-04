using System;
using Xamarin.Forms;
using System.Diagnostics;

namespace Todo
{
	public class App : Application
	{
		static TodoItemDatabase database;

        public static App CurrentApp { get { return App.Current as App; } }

        private string _apiUrl;
        public string ApiUrl { get { return _apiUrl; }
            set
            {
                _apiUrl = value;
                ApiServerChanged?.Invoke(this, new EventArgs());
            }
        }
        public event EventHandler ApiServerChanged;

		public App ()
		{
			Resources = new ResourceDictionary ();
			Resources.Add ("primaryGreen", Color.FromHex("91CA47"));
			Resources.Add ("primaryDarkGreen", Color.FromHex ("6FA22E"));

			var nav = new NavigationPage (new TodoItemListX ());
			nav.BarBackgroundColor = (Color)App.Current.Resources["primaryGreen"];
			nav.BarTextColor = Color.White;

			MainPage = nav;
		}

		public static TodoItemDatabase Database {
			get { 
				if (database == null) {
					database = new TodoItemDatabase ();
				}
				return database; 
			}
		}

		public int ResumeAtTodoId { get; set; }

		protected override void OnStart()
		{
			Debug.WriteLine ("OnStart");
		}
		protected override void OnSleep()
		{
			Debug.WriteLine ("OnSleep saving ResumeAtTodoId = " + ResumeAtTodoId);
			// the app should keep updating this value, to
			// keep the "state" in case of a sleep/resume
			Properties ["ResumeAtTodoId"] = ResumeAtTodoId;
		}
		protected override void OnResume()
		{
			Debug.WriteLine ("OnResume");
		}
	}
}

