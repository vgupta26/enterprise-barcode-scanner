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
    public partial class TypeSelection : ContentPage
    {
        public TypeSelection()
        {
            InitializeComponent();
        }
        private void CheckFabric_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FabricCheck());
        }


        private void CheckImage_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ScanforImages());
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}