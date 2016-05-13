using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RunnerTracker.Core.Service;
using RunnerTracker.Core.Model;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Threading.Tasks;

namespace RunnerTracker
{
    [Activity(Label = "RunnerTrackerDetailActivity")]
    public class RunnerTrackerDetailActivity : Activity, ILocationListener
    {

        private RunnerTrackerMenu runnerTracker;
        private RunnerTrackerService runnerDataService;

        private GoogleMap googleMap;
        private LatLng runnerLocation;
        private MapFragment mapFragment;
        private FrameLayout mapFrameLayout;

        public void OnLocationChanged(Location location) { }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        { }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            runnerLocation = new LatLng(50.874654, 4.356787);

            // Create your application here
            SetContentView(Resource.Layout.RunnerTrackerDetailView);

            var selectedRunnerTrackerId = Intent.Extras.GetInt("selectedMenuId");

            runnerDataService = new RunnerTrackerService();
            runnerTracker = runnerDataService.GetMenuById(selectedRunnerTrackerId);

            var runnerTrackDetailView = FindViewById<TextView>(Resource.Id.RunnerTrackDetailTextView);
            var runnerTrackIdView = FindViewById<TextView>(Resource.Id.RunnerTrackerId);

            runnerTrackDetailView.Text = runnerTracker.Details;
            runnerTrackIdView.Text = runnerTracker.Index.ToString();

            CreateMapFragment();
            UpdateMapView();

        }

        private void UpdateMapView()
        {
            var mapReadyCallback = new LocalMapReady();

            mapReadyCallback.MapReady += (sender, args) =>
            {
                googleMap = (sender as LocalMapReady).Map;

                if (googleMap != null)
                {
                    MarkerOptions markerOptions = new MarkerOptions();
                    markerOptions.SetPosition(runnerLocation);
                    markerOptions.SetTitle("runner's current location");
                    googleMap.AddMarker(markerOptions);

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(runnerLocation, 15);
                    googleMap.MoveCamera(cameraUpdate);
                }
            };

            mapFragment.GetMapAsync(mapReadyCallback);
        }

        private void CreateMapFragment()
        {
            mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;

            if(mapFragment == null)
            {
                var googleMapOptions = new GoogleMapOptions()
                    .InvokeMapType(GoogleMap.MapTypeNormal)
                    .InvokeZoomControlsEnabled(true)
                    .InvokeCompassEnabled(true);

                FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
                mapFragment = MapFragment.NewInstance(googleMapOptions);
                fragmentTransaction.Add(Resource.Id.mapFrameLayout, mapFragment, "map");
                fragmentTransaction.Commit();
            }
        }

        private class LocalMapReady:Java.Lang.Object, IOnMapReadyCallback
        {
            public GoogleMap Map { get; private set; }

            public event EventHandler MapReady;

            public void OnMapReady(GoogleMap googleMap)
            {
                this.Map = googleMap;
                var handler = MapReady;
                if(null != handler)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }


    }

        
}