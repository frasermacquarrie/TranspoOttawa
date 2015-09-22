using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Windows.Media;
namespace TranspoOttawa.Pages.StopInfo
{
        internal class GenericListCreationJsonConverter<T> : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer.Deserialize<List<T>>(reader);
            }
            else
            {
                T t = serializer.Deserialize<T>(reader);
                return new List<T>(new[] { t });
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class StopResultWrapper
    {
        public StopResult GetRouteSummaryForStopResult { get; set; }
    }

    public class StopResult
    {
        public string StopNo { get; set; }
        public string StopDescription { get; set; }
        public string Error { get; set; }
        public RouteWrapper Routes { get; set; }
    }
    public class RouteWrapper
    {
        [JsonConverter(typeof(GenericListCreationJsonConverter<RouteClass>))]
        public List<RouteClass> Route { get; set; }
    }

    public class RouteClass
    {
        public int RouteNo { get; set; }
        public int DirectionID { get; set; }
        public string Direction { get; set; }
        public string RouteHeading { get; set; }
        [JsonConverter(typeof(GenericListCreationJsonConverter<Trip>))]
        public List<Trip> Trips {get; set; }

        public StackPanel panelOut()
        {
            if (Trips == null || Trips.Count == 0) {
                return null;
            }

            StackPanel panel = new StackPanel();

            panel.Margin = new Thickness(30, 10, 0, 10);

            TextBlock title = new TextBlock();
            title.Text = String.Format("{0} {1}", RouteNo, RouteHeading).ToUpper();
            panel.Children.Add(title);

            foreach (Trip t in Trips) {

                Grid tripGrid = new Grid();
                for (int i = 0; i < 3; i++)
                {
                    tripGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                TextBlock time = new TextBlock();
                TextBlock route = new TextBlock();
                TextBlock dest = new TextBlock();
                TextBlock update = new TextBlock();

                tripGrid.ColumnDefinitions[0].Width = new GridLength(70);
                //tripGrid.ColumnDefinitions[1].Width = new GridLength(40);
                //tripGrid.ColumnDefinitions[2].SetValue()
                tripGrid.ColumnDefinitions[2].Width = new GridLength(90);

                //route.Width = 25;
                dest.Width = 200;
                dest.HorizontalAlignment = HorizontalAlignment.Left;
                //update.Width = 25;

                time.Text = String.Format("{0} min", t.AdjustedScheduleTime);
                dest.Text = t.TripDestination;

                var adjTime = Convert.ToDouble(t.AdjustmentAge);
                if (adjTime > 0)
                {
                    update.Text = String.Format("GPS ({0}s)", (int)(adjTime * 60));

                    time.FontWeight = FontWeights.Bold;
                    time.Foreground = new SolidColorBrush(Colors.Green);
                    //route.FontWeight = FontWeights.Bold;
                    //route.Foreground = new SolidColorBrush(Colors.Green);
                    //dest.FontWeight = FontWeights.Bold;
                    //dest.Foreground = new SolidColorBrush(Colors.Green);
                    update.FontWeight = FontWeights.Bold;
                    update.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    update.Text = "schedule";

                    time.Opacity = 0.75;
                    update.Opacity = 0.75;
                }


                time.SetValue(Grid.ColumnProperty, 0);
                dest.SetValue(Grid.ColumnProperty, 1);
                update.SetValue(Grid.ColumnProperty, 2);

                tripGrid.Children.Add(time);
                tripGrid.Children.Add(dest);
                tripGrid.Children.Add(update);

                panel.Children.Add(tripGrid);
            }

            return panel;
        }

        public Grid quickOut()
        {
            if (Trips == null || Trips.Count == 0) {
                return null;
            }

            Grid routeGrid = new Grid();
            routeGrid.Margin = new Thickness(30, 0, 0, 0);

            for (int i = 0; i < 4; i++)
            {
                routeGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            TextBlock dest = new TextBlock();
            TextBlock [] times = new TextBlock[3];
            
            for (int i = 0; i < 3; i++)
            {
                times[i] = new TextBlock();
                times[i].SetValue(Grid.ColumnProperty, i+1);
            }

            routeGrid.ColumnDefinitions[1].Width = new GridLength(40);
            routeGrid.ColumnDefinitions[2].Width = new GridLength(40);
            routeGrid.ColumnDefinitions[3].Width = new GridLength(40);

            dest.Text = String.Format("{0} {1}", RouteNo, RouteHeading).ToUpper();
            dest.SetValue(Grid.ColumnProperty, 0);
            dest.Width = 300;
            routeGrid.Children.Add(dest);


            int count = 0;
            foreach (Trip t in Trips) {
                times[count].Text = String.Format("{0}", t.AdjustedScheduleTime);

                if (Convert.ToDouble(t.AdjustmentAge) > 0)
                {
                    times[count].FontWeight = FontWeights.Bold;
                    times[count].Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    times[count].Opacity = 0.75;
                }

                routeGrid.Children.Add(times[count++]);
            }

            return routeGrid;
        }
    }

    public class Trip
    {
        public string TripDestination { get; set; }
        public string TripStartTime { get; set; }
        public string AdjustedScheduleTime { get; set; }
        public string AdjustmentAge { get; set; }
        public bool LastTripOfSchedule { get; set; }
        public string BusType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string GPSSpeed { get; set; }

        public override string ToString()
        {
            var time = Convert.ToDouble(AdjustmentAge);
            if (time < 0)
            {
                return String.Format("{0,3} min\t{1,-20}\t schedule\n", AdjustedScheduleTime, (TripDestination.Length <= 20 ? TripDestination : TripDestination.Substring(0, 20)));
            }
            else
            {
                return String.Format("{0,3} min\t{1,-20}\t GPS ({2} s)\n", AdjustedScheduleTime, (TripDestination.Length <= 20 ? TripDestination : TripDestination.Substring(0, 20)), (int)(time * 60));
            }
        }
        
        
        
    }

    public class TripItem
    {
        public int routeNo { get; set; }
        public int directionID { get; set; }
        public string direction { get; set; }
        public string routeHeading { get; set; }
        public  Trip trip { get; set; }


        public TripItem(int rNo, int dirID, string dir, string rteH, Trip t)
        {
            routeNo = rNo;
            directionID = dirID;
            direction = dir;
            routeHeading = rteH;
            trip = t;
        }

        public override string ToString()
        {
            var time = Convert.ToDouble(trip.AdjustmentAge);
            if (time < 0)
            {
                return String.Format("{0,3} min\t{1,3}\t{2,-20}\t schedule\n", trip.AdjustedScheduleTime, routeNo, (trip.TripDestination.Length <= 20 ? trip.TripDestination : trip.TripDestination.Substring(0, 20)));
            }
            else 
            {
                return String.Format("{0,3} min\t{1,3}\t{2,-20}\t GPS ({3} s)\n", trip.AdjustedScheduleTime, routeNo, (trip.TripDestination.Length <= 20 ? trip.TripDestination : trip.TripDestination.Substring(0, 20)), (int)(time * 60));
            }
        }

        public Grid panelOut()
        {

            Grid tripGrid = new Grid();
            for (int i = 0; i < 4; i++) {
                tripGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            TextBlock time = new TextBlock();
            TextBlock route = new TextBlock();
            TextBlock dest = new TextBlock();
            TextBlock update = new TextBlock();

            tripGrid.ColumnDefinitions[0].Width = new GridLength(70);
            tripGrid.ColumnDefinitions[1].Width = new GridLength(40);
            //tripGrid.ColumnDefinitions[2].SetValue()
            tripGrid.ColumnDefinitions[3].Width = new GridLength(90);

            //route.Width = 25;
            dest.Width = 200;
            dest.HorizontalAlignment = HorizontalAlignment.Left;
            //update.Width = 25;

            time.Text = String.Format("{0} min", trip.AdjustedScheduleTime);
            route.Text = String.Format("{0}",routeNo);
            dest.Text = trip.TripDestination;

            var adjTime = Convert.ToDouble(trip.AdjustmentAge);
            if (adjTime > 0) {
                update.Text = String.Format("GPS ({0}s)",(int)(adjTime * 60));

                time.FontWeight = FontWeights.Bold;
                time.Foreground = new SolidColorBrush(Colors.Green);
                //route.FontWeight = FontWeights.Bold;
                //route.Foreground = new SolidColorBrush(Colors.Green);
                //dest.FontWeight = FontWeights.Bold;
                //dest.Foreground = new SolidColorBrush(Colors.Green);
                update.FontWeight = FontWeights.Bold;
                update.Foreground = new SolidColorBrush(Colors.Green);
            }
            else {

                time.Opacity = 0.75;
                update.Opacity = 0.75;

                update.Text = "schedule";

            }

            time.SetValue(Grid.ColumnProperty, 0);
            route.SetValue(Grid.ColumnProperty, 1);
            dest.SetValue(Grid.ColumnProperty, 2);
            update.SetValue(Grid.ColumnProperty, 3);

            tripGrid.Children.Add(time);
            tripGrid.Children.Add(route);
            tripGrid.Children.Add(dest);
            tripGrid.Children.Add(update);



            return tripGrid;
        }


        
    }



    public partial class PivotPage1 : PhoneApplicationPage
    {

        string stop = "";
        List<int> selectedRoutes = new List<int>();
        public PivotPage1()
        {
            InitializeComponent();
        }

            protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StopInfoProgressText.Text = "Loading Stop Data";
            StopInfoProgressPanel.Visibility = System.Windows.Visibility.Visible;
            StopInfoPivot.Visibility = System.Windows.Visibility.Collapsed;

            if (NavigationContext.QueryString.TryGetValue("stop", out stop)) {
                
                /*
                string route = "";
                NavigationContext.QueryString.TryGetValue("route", out route);
                if (route != "")
                {
                    selectedRoutes.Add(Convert.ToInt32(route));
                }
                */

                string requestUri = "https://api.octranspo1.com/v1.2/GetNextTripsForStopAllRoutes";
                string postData = String.Format("appID={0}&apiKey={1}&stopNo={2}&format=json", "071395c8", "00fe02b13da30d9b3372021ca362fb53", stop);

                WebClient webClient = new WebClient();
                webClient.UploadStringCompleted += webClient_UploadStringCompleted;
                webClient.Headers["Content-Length"] = postData.Length.ToString();
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                webClient.UploadStringAsync(new Uri(requestUri), "POST", postData);         
            }

           
        }

            private void webClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
            {
                if (e.Error == null)
                {
                    //StopInfoProgressText.Text = "Data Acquired";

                    var result = (JsonConvert.DeserializeObject<StopResultWrapper>(e.Result)).GetRouteSummaryForStopResult;
                    StopInfoPivot.Title = String.Format("{0} ({1})", result.StopDescription, result.StopNo);
                    Dictionary<string, List<RouteClass>> routesByDirection = new Dictionary<string, List<RouteClass>>();

                    foreach (RouteClass r in result.Routes.Route)
                    {
                        if (!routesByDirection.ContainsKey(r.Direction))
                        {
                            routesByDirection.Add(r.Direction, new List<RouteClass>()); 
                        }
                        routesByDirection[r.Direction].Add(r);
                    }

                    //string nextText = "";
                    //string routeText = "";
                    foreach (KeyValuePair<string, List<RouteClass>> dir in routesByDirection)
                    {
                        if (dir.Value.Count > 0)
                        {
                            StackPanel nextTripsFiltered = NextTrips(dir.Value, 3);
                            if (nextTripsFiltered != null)
                            {
                                StackPanel g = new StackPanel();
                                TextBlock title = new TextBlock();
                                title.Text = dir.Key;
                                title.Style = (Style)Application.Current.Resources["PhoneTextTitle2Style"];
                                g.Children.Add(title);
                                g.Children.Add(nextTripsFiltered);

                                nextStack.Children.Add(g);
                            }

                            StackPanel routeTripsFiltered = RouteTrips(dir.Value);
                            if (routeTripsFiltered != null && routeTripsFiltered.Children.Count > 0)
                            {
                                StackPanel g = new StackPanel();
                                TextBlock title = new TextBlock();
                                title.Text = dir.Key;
                                title.Style = (Style)Application.Current.Resources["PhoneTextTitle2Style"];

                                g.Children.Add(title);
                                g.Children.Add(routeTripsFiltered);

                                routeStack.Children.Add(g);
                            }

                            StackPanel quickTripsFiltered = QuickTrips(dir.Value);
                            if (quickTripsFiltered != null && quickTripsFiltered.Children.Count > 0)
                            {
                                StackPanel g = new StackPanel();
                                TextBlock title = new TextBlock();
                                title.Text = dir.Key;
                                title.Style = (Style)Application.Current.Resources["PhoneTextTitle2Style"];

                                g.Children.Add(title);
                                g.Children.Add(quickTripsFiltered);

                                quickStack.Children.Add(g);
                            }
                        }
                    }

                    StopInfoProgressPanel.Visibility = System.Windows.Visibility.Collapsed;
                    StopInfoPivot.Visibility = System.Windows.Visibility.Visible;
                }
                else
                    MessageBox.Show("WebClient: " + e.Error.Message);
            }

            private StackPanel NextTrips(List<RouteClass> routes, int numRoutes)
            {
                if (routes == null || routes.Count == 0)
                {
                    return null;
                }

                StackPanel g = new StackPanel();
                g.Margin = new Thickness(30, 10, 0, 20);

                List<TripItem> tripList = new List<TripItem>();

                foreach (RouteClass r in routes)
                {
                    if (r.Trips != null)
                    {
                        foreach (Trip t in r.Trips)
                        {
                            tripList.Add(new TripItem(r.RouteNo, r.DirectionID, r.Direction, r.RouteHeading, t));
                        }
                    }
                }
                var sortedTrips = from element in tripList
                              orderby Convert.ToInt32(element.trip.AdjustedScheduleTime)
                              select element;

                for (int i = 0; i < Math.Min(numRoutes, tripList.Count); i++)
                {
                    g.Children.Add(sortedTrips.ElementAt(i).panelOut());
                }

                return g;
            }

            private StackPanel RouteTrips(List<RouteClass> routes)
            {
                StackPanel rtes = new StackPanel();

                foreach (RouteClass r in routes)
                {
                    if (r.Trips != null && r.Trips.Count > 0 && (selectedRoutes.Count == 0 || selectedRoutes.Contains(r.RouteNo)))
                    {
                        rtes.Children.Add(r.panelOut());
                    }

                }
                

                return rtes;
            }

            private StackPanel QuickTrips(List<RouteClass> routes)
            {
                StackPanel rtes = new StackPanel();

                int count = 0;

                foreach (RouteClass r in routes)
                {

                    if (r.Trips != null && r.Trips.Count > 0 && (selectedRoutes.Count == 0 || selectedRoutes.Contains(r.RouteNo)))
                    {
                        Grid g = r.quickOut();
                        if (count % 2 == 0) {
                            //g.Opacity = 0.75;
                        }

                        rtes.Children.Add(g);
                        
                        count++;
                    }

                }


                return rtes;
            }

    }


}

