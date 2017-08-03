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
    public partial class ScanResults : ContentPage
    {
        private int x;

        public ScanResults()
        {
            InitializeComponent();
            x= Convert.ToInt32(lb_Response_Code.Text);
            if (x==0)
            {
                btn_images.IsEnabled = true;
            }
            else
            {
                 DisplayAlert("Error", "No Images available of this product", "Please try again"); 
            }
        }


        private void btn_images_Clicked(object sender, EventArgs e)
        {
           
            Navigation.PushAsync(new ImageGrid());
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}