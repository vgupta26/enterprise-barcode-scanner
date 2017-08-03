using Acr.BarCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoginPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanforImages : ContentPage
    {
        public ScanforImages()
        {
            InitializeComponent();
        }

        private async Task Scan_button_Clicked(object sender, EventArgs e)
        {

            try
            {
                var result = await BarCodes.Instance.Read();
                if (!result.Success)
                {
                    await this.DisplayAlert("Sorry ! Scan Failed ! ", "Sorry! failed to read the barcode !", "ok");
                }
                else
                {

                    await Navigation.PushAsync(new ScanResults());

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                displayErrorMessage();
            }
        }

        private async void displayErrorMessage()
        {
            await this.DisplayAlert("An error occured ", "while scanning the bar code. ", "Please try again");
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
