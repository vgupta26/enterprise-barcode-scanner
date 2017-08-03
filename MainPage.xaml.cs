using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Xml.Linq;
using System.Net.Http;
using System.Diagnostics;

namespace LoginPage
{
    public partial class MainPage : ContentPage
    {
        public NavigationPage MainPage1 { get; private set; }
        public object NavigationController { get; private set; }
        public Color BarBackgroundColor { get; private set; }

        private User user;

        public MainPage()
        {
            InitializeComponent();
        }

        async void Login_button_Clicked(object sender, EventArgs e)
        {
            int responseCode =0;
            try
            {

                user = new User(Entry_Username.Text, Entry_Password.Text);
                LoginPage.MainPage ws = new LoginPage.MainPage();


                if (user.Check())
                {
                    responseCode = await PostString("http://10.0.0.221/Brentwood/Services/Scanner.asmx?op=chkLogin");

                        Debug.WriteLine(responseCode);

                    switch (responseCode)
                    {
                        case 0:
                            await DisplayAlert("Login Error", "Response Code:" + responseCode, "Try Again");
                            break;
                        case 838:
                            await DisplayAlert("", "Login Successful", "Ok");
                            await Navigation.PushAsync(new TypeSelection());
                            break;
                        default:
                            break;
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                displayErrorMessage();
            }
        }

        async void ChkImage_button_Clicked(object sender, EventArgs e)
        {
            string responseCode = "";
            try
            {

                LoginPage.MainPage ws = new LoginPage.MainPage();


                if (user.Check())
                {
                    responseCode = await PostImageString("http://10.0.0.221/Services/Scanner.asmx?op=chkTicketImages");

                    Debug.WriteLine(responseCode);

                    switch (responseCode)
                    {
                        case "Fabric Not Found":
                            await DisplayAlert("Fabric Checker", "Error while checking for Fabric.", "Try Again");
                            break;
                        case "Fabric is Active":
                            await DisplayAlert("Fabric Checker", "Fabric is Active", "Ok");
                            break;
                        case "Fabric is Discontinued":
                            await DisplayAlert("Fabric Checker", "Fabric is discontinued", "Ok");
                            break;
                        default:
                            break;
                    }
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                displayErrorMessage();
            }
        }

        async Task<string> PostImageString(string v)
        {
            try
            {
                string LoginName = user.Username;
                string password = user.Password;
                string responseCode = "";
                var soapString = ConstructImageSoapRequest(838, "97509");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/chkTicketImages");
                    var content = new StringContent(soapString, Encoding.UTF8, "text/xml");
                    using (var response = await client.PostAsync("http://10.0.0.221/Services/Scanner.asmx?op=chkTicketImages", content))
                    {
                        var soapResponse = await response.Content.ReadAsStringAsync();
                        responseCode = ParseImageSoapResponse(soapResponse);
                    }
                }

                return responseCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MainPage().displayErrorMessage();
                return "0";
            }

        }

        async Task<int> PostString(string address)
        {
            try
            {
                string LoginName = user.Username;
                string password = user.Password;
                int responseCode = 0;
                var soapString = ConstructSoapRequest(LoginName, password);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/chkLogin");
                    var content = new StringContent(soapString, Encoding.UTF8, "text/xml");
                    using (var response = await client.PostAsync("http://brentwood.appsondemand.ca/Services/Scanner.asmx?op=chkLogin", content))
                    {
                        var soapResponse = await response.Content.ReadAsStringAsync();
                        responseCode = ParseSoapResponse(soapResponse);
                    }
                }

                return responseCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                new MainPage().displayErrorMessage();
                return 0;
            }
        }
        private static string ConstructSoapRequest(string a, string b)
        {
            return String.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
        <soap:Body>
            <chkLogin xmlns=""http://tempuri.org/"">
                <loginName>{0}</loginName>
                <password>{1}</password>
            </chkLogin>
        </soap:Body>
    </soap:Envelope>", a, b);
        }

        private static string ConstructFabricSoapRequest(int UserID, string BarcodeValue)
        {
            return String.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
        <soap:Body>
            <chkFabricBarcode xmlns=""http://tempuri.org/"">
                <UserID>{0}</UserID>
                <barcode>{1}</barcode>
            </chkFabricBarcode>
        </soap:Body>
    </soap:Envelope>", UserID, BarcodeValue);
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

        private static int ParseSoapResponse(string response)
        {
            var soap = XDocument.Parse(response);
            XNamespace ns = "http://tempuri.org/";
            var result = soap.Descendants(ns + "chkLoginResponse").First().Element(ns + "chkLoginResult").Value;
            return Int32.Parse(result);
        }

        private static string ParseFabricSoapResponse(string response)
        {
            var soap = XDocument.Parse(response);
            XNamespace ns = "http://tempuri.org/";
            var result = soap.Descendants(ns + "chkFabricBarcodeResponse").First().Element(ns + "chkFabricBarcodeResult").Value;
            return result.ToString();
        }

        private static string ParseImageSoapResponse(string response)
        {
            var soap = XDocument.Parse(response);
            XNamespace ns = "http://tempuri.org/";
            var result = soap.Descendants(ns + "chkTicketImagesResponse").First().Element(ns + "chkTicketImagesResult").Value;
            return result.ToString();
        }

        private async void displayErrorMessage()
        {
            await this.DisplayAlert("Invalid Login", "Please enter the Username and Password ", "Try again");
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
       
}
}
