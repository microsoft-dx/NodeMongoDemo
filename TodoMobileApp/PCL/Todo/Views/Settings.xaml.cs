using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Todo.Views
{
    public partial class Settings : ContentPage
    {
        public Settings()
        {
            InitializeComponent();
            if (Device.OS == TargetPlatform.Android)
            {
                apiUrlEntry.Text = "http://1ab2d0e2.ngrok.io";
            }
        }

        public async void ContinueClicked(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(apiUrlEntry.Text))
            {
                App.CurrentApp.ApiUrl = apiUrlEntry.Text;
                await Navigation.PopModalAsync();
            }
        }
     }
}
