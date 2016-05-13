using System;
using System.Collections.Generic;
using System.Linq;
using System.Json;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using RunnerTracker.Core.Model;
using RunnerTracker.Core.Service;
using RunnerTracker.Adapter;
using Android.Locations;
using System.IO;

using Microsoft.Azure.Devices.Client;


namespace RunnerTracker
{
    using System.Net;

    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    [Activity(Label = "RunnerTracker", MainLauncher = true, Icon = "@drawable/Icon")]
    public class RunnerTrackerMenuActivity : Activity, ILocationListener
    {   
        
        private ListView runnerTrackerMenuListView;
        private List<RunnerTrackerMenu> runnerTrackerMenuList;
        private RunnerTrackerService runnerTrackerDataService;
        private Button externtalMapButton;
        private LocationManager locationManager;
        private Location currentLocation;
        private string locationProvider;
        private Button btnGetCurrentLocation;
        private Button btnGetWeather;
        private EditText txtCurrentLocation;
        private TextView txtCurrentWeather;
        //private TextView txtDeviceData;
        private Button btnGetDeviceData;

        private const string DEFAULT_CONSUMER_GROUP = "$Default";

        private const string connectionString = "HostName=iothubsensorpilot.azure-devices.net;DeviceId=ConfRoomIstanbul;SharedAccessKey=B4cUuO6rK/62NIptKs4L7GQ3kG+nsFV2dGxw2o3G5qM=";
        //private static string iotHubD2cEndPoint = "message/events";
        // Create your application here
        DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connectionString, Microsoft.Azure.Devices.Client.TransportType.Http1);

        
        public async void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if (currentLocation == null)
            {
                txtCurrentLocation.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
                txtCurrentLocation.Text = string.Format("{0:f6},{1:f6}", currentLocation.Latitude, currentLocation.Longitude);
                Address address = await ReverseGeocodeCurrentLocation();
                DisplayAddress(address);
            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras) { }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            

            SetContentView(Resource.Layout.RunnerTrackerMenuView);

            runnerTrackerMenuListView = FindViewById<ListView>(Resource.Id.RunnerTrackMenuListView);
            externtalMapButton = FindViewById<Button>(Resource.Id.externalMapButton);
            btnGetCurrentLocation = FindViewById<Button>(Resource.Id.btnGetCurrrentLocation);
            txtCurrentLocation = FindViewById<EditText>(Resource.Id.txtCurrentLocation);
            btnGetWeather = FindViewById<Button>(Resource.Id.btnGetWeather);

            txtCurrentWeather = FindViewById<TextView>(Resource.Id.txtWeatherData);

            runnerTrackerDataService = new RunnerTrackerService();
            runnerTrackerMenuList = runnerTrackerDataService.GetMenuList();

            runnerTrackerMenuListView.Adapter = new RunnerTrackerMenuAdapter(this, runnerTrackerMenuList);

            runnerTrackerMenuListView.FastScrollEnabled = true;

            runnerTrackerMenuListView.ItemClick += RunnerTrackerMenuListView_ItemClick;
            externtalMapButton.Click += ExterntalMapButton_Click;
            btnGetCurrentLocation.Click += BtnGetCurrentLocation_Click;
            btnGetWeather.Click += BtnGetWeather_Click;
            
            btnGetDeviceData = FindViewById<Button>(Resource.Id.btnDeviceData);

            btnGetDeviceData.Click += BtnGetDeviceData_Click;


            InitializeLocationManager();

        }

        private void BtnGetDeviceData_Click(object sender, EventArgs e)
        {
            var txtDeviceData = FindViewById<EditText>(Resource.Id.receiveDataMsgBox);
            txtDeviceData.Text += "Receiving Data from IoT Hub Device...\r\n";


            //var asyncTask = Task.Run(() => deviceClient.ReceiveAsync());

            //var receivedMessage = asyncTask.Result;

            //if (receivedMessage != null)
            //{
            //    txtDeviceData.Text = Encoding.Default.GetString(receivedMessage.GetBytes()) + "\r\n";
            //}

            //Console.WriteLine("Message received, now completing it...");
            ////Task.Run(() => { deviceClient.CompleteAsync(receivedMessage); });
            //Console.WriteLine("Message completed.");
            var ctsForDataMonitoring = new CancellationTokenSource();

            //MonitorEventHubAsync(DateTime.Now, ctsForDataMonitoring.Token, );

        }

        private async void BtnGetWeather_Click(object sender, EventArgs e)
        {
            string url = "http://api.geonames.org/findNearByWeatherJSON?lat=" +
                "47.5" +
                "&lng=" +
                "-122" +
                "&username=demo";

            // Fetch the weather information asynchronously, 
            // parse the results, then update the screen:
            JsonValue json = await FetchWeatherAsync(url);
             ParseAndDisplay (json);
        }

        private void ParseAndDisplay(JsonValue json)
        {

            // Extract the array of name/value results for the field name "weatherObservation". 
            JsonValue weatherResults = json["weatherObservation"];

            StringBuilder weatherInfo = new StringBuilder(weatherResults["stationName"].ToString());

            // Extract the "stationName" (location string) and write it to the location TextBox:
            //txtCurrentWeather.Text = weatherResults["stationName"];

            // The temperature is expressed in Celsius:
            double temp = weatherResults["temperature"];
            // Convert it to Fahrenheit:
            temp = ((9.0 / 5.0) * temp) + 32;
            // Write the temperature (one decimal place) to the temperature TextBox:
            //temperature.Text = String.Format("{0:F1}", temp) + "бу F";

            weatherInfo.Append("," + String.Format("{0:F1}", temp) + "бу F");

            // Get the percent humidity and write it to the humidity TextBox:
            double humidPercent = weatherResults["humidity"];
            //humidity.Text = humidPercent.ToString() + "%";

            weatherInfo.Append("," + humidPercent.ToString() + "%");

            string weatherDetail = weatherInfo.ToString();

            txtCurrentWeather.Text = weatherDetail;

            // Get the "clouds" and "weatherConditions" strings and 
            // combine them. Ignore strings that are reported as "n/a":
            //string cloudy = weatherResults["clouds"];
            //if (cloudy.Equals("n/a"))
            //    cloudy = "";
            //string cond = weatherResults["weatherCondition"];
            //if (cond.Equals("n/a"))
            //    cond = "";

            // Write the result to the conditions TextBox:
            //conditions.Text = cloudy + " " + cond;
        }
        private async void BtnGetCurrentLocation_Click(object sender, EventArgs e)
        {
            if (currentLocation == null)
            {
                txtCurrentLocation.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }

            Address address = await ReverseGeocodeCurrentLocation();
            DisplayAddress(address);
        }

        private async Task<JsonValue> FetchWeatherAsync(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Return the JSON document:
                    return jsonDoc;
                }
            }
        }

        private async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(currentLocation.Latitude, currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        private void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                txtCurrentLocation.Text = deviceAddress.ToString();
            }
            else
            {
                txtCurrentLocation.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }

        void InitializeLocationManager()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);

            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };

            IList<string> acceptableLocationProviders = locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                locationProvider = string.Empty;
            }

            //Log.Debug(Tag, "Using " + locationProvider + ".");
        }
        private void ExterntalMapButton_Click(object sender, EventArgs e)
        {
            //Android.Net.Uri rayLocationUri = Android.Net.Uri.Parse("geo:50.846704, -4.356704");

            //Intent mapIntent = new Intent(Intent.ActionView, rayLocationUri);

            //StartActivity(mapIntent);

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            SignUpFragment signUp = new SignUpFragment();
            signUp.Show(transaction, "Dialog Sign Up");

        }

        private void RunnerTrackerMenuListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var runnerTrack = runnerTrackerMenuList[e.Position];

            var intent = new Intent();

            intent.SetClass(this, typeof(RunnerTrackerDetailActivity));

            intent.PutExtra("selectedMenuId", runnerTrack.Index);

            StartActivityForResult(intent, 100);

        }
    }
}