using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
//using Windows.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace CompassDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Compass compass;
        String appversion = GetAppVersion();
        private ApplicationDataContainer localSettings;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
           // String value=localSettings.Values["initSetting"].ToString();
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            localSettings = ApplicationData.Current.LocalSettings;
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (!e.Handled && Frame.CurrentSourcePageType.FullName == "CompassDemo.MainPage")
                Application.Current.Exit();

        }

        async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //DO I NEED TO DIRECT TO THE HELP PAGE?
            if (localSettings.Values.ContainsKey("ok"))
            {
                compass = Compass.GetDefault();
                if (compass == null)
                {
                    await new MessageDialog("Compass sensor is not supported").ShowAsync();
                    return;
                }
                compass.ReadingChanged += compass_ReadingChanged;
            }

            else
            {
                localSettings.Values["ok"] = "ok";
                Frame.Navigate(typeof(Help),appversion);
            }
        }

        private async void compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CompassReading reading = args.Reading;
                magneticNorth.Text = String.Format("{0,5:0.00}°\nAccuracy:\n", reading.HeadingMagneticNorth);
               // magneticNorth.Text = "Magnetic North \nAccuracy:\n";
                if (reading.HeadingTrueNorth != null)
                {
                   trueNorth.Text = String.Format("{0,5:0}°", reading.HeadingTrueNorth);
                }
                else
                {
                    trueNorth.Text = "No data";
                }
                switch (reading.HeadingAccuracy)
                {
                    case MagnetometerAccuracy.Unknown:
                        magneticNorth.Text += "Unknown";
                        break;
                    case MagnetometerAccuracy.Unreliable:
                        magneticNorth.Text += "Unreliable";
                        break;
                    case MagnetometerAccuracy.Approximate:
                        magneticNorth.Text += "Approximate";
                        break;
                    case MagnetometerAccuracy.High:
                        magneticNorth.Text += "High";
                        break;
                    default:
                        magneticNorth.Text += "No data";
                        break;
                }
                double TrueHeading = reading.HeadingTrueNorth.Value;
                double ReciprocalHeading;
                if ((180 <= TrueHeading) && (TrueHeading <= 360))
                    ReciprocalHeading = TrueHeading - 180;
                else
                    ReciprocalHeading = TrueHeading + 180;
                CompassFace.RenderTransformOrigin = new Point(0.5, 0.5);
                //EllipseGlass.RenderTransformOrigin = new Point(0.5, 0.5);
                RotateTransform transform = new RotateTransform();
                transform.Angle = 360 - TrueHeading;
                CompassFace.RenderTransform = transform;
                //EllipseGlass.RenderTransform = transform;
            });
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.
        }

        #region AddInfo
        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            string temp = String.Format("{0}.{0}.{0}.{0}", version.Major, version.Minor, version.Build, version.Revision);
            return temp;
        }

        //Info Page
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new Uri("/Info_Page.xaml", UriKind.Relative));
            Frame.Navigate(typeof(Help), appversion);
        }

        private async void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(
    new Uri(string.Format("ms-windows-store:reviewapp?appid=" + "f6406bbd-cf07-43af-ae48-26e9641d369e")));
        }
        #endregion

    }
}
