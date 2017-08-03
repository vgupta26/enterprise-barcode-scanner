using Acr.BarCodes;
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
    public partial class FabricCheck : ContentPage
    {
        public FabricCheck()
        {
            InitializeComponent();
        }
        async void Scan_button_Clicked(object sender, EventArgs e)
        {
            string responseCode = "";

            try
            {
                var result = await BarCodes.Instance.Read();
                if (!result.Success)
                {
                    await this.DisplayAlert("Sorry ! Scan Failed ! ", "Sorry! failed to read the barcode !", "ok");
                }
                else
                {

//                    var msg = String.Format("Barcode Format : {0} \nBarcode Value : {1}", result.Format, result.Code);
                    //await this.DisplayAlert("Scan Successfull!", msg, "ok");
  //                  Lb_value.Text = "The Barcode Value is  " + result.Code;

                    responseCode = await PostFabricString("http://10.0.0.221/Brentwood/Services/Scanner.asmx?op=chkFabricBarcode");

                    Debug.WriteLine(responseCode);

                    switch (responseCode)
                    {
                        case "Fabric Not Found":
                            await DisplayAlert("Fabric Checker", "Error while checking for Fabric.", "Try Again");
                            break;
                        case "Fabric is Active":
                            await DisplayAlert("Fabric Checker", "Fabric is Active", "Ok");
                            break;
                        case "Fabric is discontinued":
                            await DisplayAlert("Fabric Checker", "Fabric is Discontinued", "Ok");
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        async Task<string> PostFabricString(string v)
        {
            try
            {
                string responseCode = "";
                var soapString = ConstructFabricSoapRequest(838, "1-2");
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("SOAPAction", "http://tempuri.org/chkFabricBarcode");
                    var content = new StringContent(soapString, Encoding.UTF8, "text/xml");
                    using (var response = await client.PostAsync("http://10.0.0.221/Brentwood/Services/Scanner.asmx?op=chkFabricBarcode", content))
                    {
                        var soapResponse = await response.Content.ReadAsStringAsync();
                        responseCode = ParseFabricSoapResponse(soapResponse);
                    }
                }

                return responseCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
              //  new FabricCheck().displayErrorMessage();
                return "0";
            }

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

        private static string ParseFabricSoapResponse(string response)
        {
            var soap = XDocument.Parse(response);
            XNamespace ns = "http://tempuri.org/";
            var result = soap.Descendants(ns + "chkFabricBarcodeResponse").First().Element(ns + "chkFabricBarcodeResult").Value;
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