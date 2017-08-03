using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoginPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageGrid : ContentPage
    {
        public ImageGrid()
        {
            InitializeComponent();
            //try
            //{

            //    imageGrid.BackgroundColor = Color.White;
            //    imageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            //    imageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //    var alpha = new Image();
            //    alpha.Source = ImageSource.FromResource("LoginPage.Image6.jpg");
            //    alpha.Aspect = Aspect.Fill;


            //    var beta = new Image();
            //    beta.Source = ImageSource.FromResource("LoginPage.Image7.jpg");
            //    beta.Aspect = Aspect.Fill;


            //    var gamma = new Image();
            //    gamma.Source = ImageSource.FromResource("LoginPage.Image8.jpeg");
            //    gamma.Aspect = Aspect.Fill;


            //    var delta = new Image();
            //    delta.Source = ImageSource.FromResource("LoginPage.Image9.jpg");
            //    delta.Aspect = Aspect.Fill;


            //    var gamma1 = new Image();
            //    gamma1.Source = ImageSource.FromResource("LoginPage.Image10.jpeg");
            //    gamma1.Aspect = Aspect.Fill;


            //    var delta1 = new Image();
            //    delta1.Source = ImageSource.FromResource("LoginPage.Image11.jpeg");
            //    delta1.Aspect = Aspect.Fill;


            //    imageGrid.Children.Add(alpha, 0, 0);
            //    imageGrid.Children.Add(beta, 0, 1);
            //    imageGrid.Children.Add(gamma, 0, 2);
            //    imageGrid.Children.Add(delta, 0, 3);
            //    imageGrid.Children.Add(gamma1, 0, 4);
            //    imageGrid.Children.Add(delta1, 0, 5);

            //    Content = new ScrollView
            //    {
            //        VerticalOptions = LayoutOptions.CenterAndExpand,
            //        HorizontalOptions = LayoutOptions.CenterAndExpand,
            //        Content = imageGrid,
            //    };
            //}
            //catch(Exception ex)
            //{
            //    displayErrorMessage();
            //}

            Scan_button_Clicked();



        }

        private async void displayErrorMessage()
        {
            await this.DisplayAlert("An error occured ", "while loading the images. ", "Please try again");
        }

        async void Scan_button_Clicked()
        {
            try
            {

               
                


               string[] responseCode = await PostImageString("http://10.0.0.221/Services/Scanner.asmx?op=chkTicketImages");

                imageGrid.BackgroundColor = Color.White;
                imageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                imageGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                for(var i =0; i < responseCode.Length; i++)
                {
                    if (responseCode[i].Equals(""))
                    {

                    }
                    else
                    {
                        var alpha = new Image();
                        alpha.Source = ImageSource.FromUri(new Uri(responseCode[i]));
                        alpha.Aspect = Aspect.Fill;

                        imageGrid.Children.Add(alpha, 0, i);

                    }

                }

                Content = new ScrollView
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Content = imageGrid,
                };

            }
            catch (Exception ex)
            {
                ex.ToString();
                displayErrorMessage();
            }
        }

        async Task<string[]> PostImageString(string v)
        {
            string[] responseCode;
            try
            {
                
                var soapString = ConstructImageSoapRequest(838, "97509");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/chkTicketImages");
                    var content = new StringContent(soapString, Encoding.UTF8, "text/xml");
                    using (var response = await client.PostAsync("http://10.0.0.221/Brentwood/Services/Scanner.asmx?op=chkTicketImages", content))
                    {
                        var soapResponse = await response.Content.ReadAsStringAsync();
                        responseCode = ParseImageSoapResponse(soapResponse);
                    }
                }

                return responseCode;
            }
            catch (Exception ex)
            {
                string[] responseCode1 = { "Exception" };
                Debug.WriteLine(ex.ToString());
                 displayErrorMessage();
                return responseCode1;
            }

        }

        private static string ConstructImageSoapRequest(int UserID, string TicketId)
        {
            return String.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
        <soap:Body>
            <chkTicketImages xmlns=""http://tempuri.org/"">
                <UserID>{0}</UserID>
                <TicketId>{1}</TicketId>
            </chkTicketImages>
        </soap:Body>
    </soap:Envelope>", UserID, TicketId);
        }

        private static string[] ParseImageSoapResponse(string response)
        {
            var soap = XDocument.Parse(response);
            XNamespace ns = "http://tempuri.org/";
            var result = soap.Descendants(ns + "chkTicketImagesResponse").First().Element(ns + "chkTicketImagesResult").Value;
            string[] delimiters = { "~" };
            string[] resultArray = result.Split(delimiters, StringSplitOptions.None);
            return resultArray;
        }
    }
}