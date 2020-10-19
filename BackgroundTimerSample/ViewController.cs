using CoreFoundation;
using System;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace BackgroundTimerSample
{
    public partial class ViewController : UIViewController
    {
        bool? timerRunning = null;
        DispatchSource.Timer _timer;

        public ViewController(IntPtr handle) : base(handle) {}

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            await CheckAndRequestPermissionAsync(new Permissions.LocationAlways());

            _timer = new DispatchSource.Timer(DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Background));

            _timer.SetEventHandler(async () =>
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest()
                {
                    Timeout = TimeSpan.FromSeconds(5),
                    DesiredAccuracy = GeolocationAccuracy.Best
                });

                System.Diagnostics.Debug.WriteLine($"{location?.Latitude}, {location?.Longitude}, {DateTime.Now}");
            });
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            _timer?.SetEventHandler(null);
            _timer?.Cancel();

            /* 
             * If the timer is suspended, calling cancel without resuming
             * triggers a crash. This is documented here https://forums.developer.apple.com/thread/15902
             */
            if(!timerRunning ?? false)
            {
                _timer?.Resume();
            }

            _timer?.Dispose();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        async Task StartTimer()
        {
            var status = await CheckAndRequestPermissionAsync(new Permissions.LocationAlways());
            if (status != PermissionStatus.Granted)
            {
                // Notify user permission was denied
                System.Diagnostics.Debug.WriteLine("Doesn't have required permissions");
                return;
            }

            toggleTimerButton.SetTitle("Stop", UIControlState.Normal);


            _timer?.SetTimer(DispatchTime.Now, (long)TimeSpan.FromSeconds(30).TotalMilliseconds * 1000000, (long)TimeSpan.FromSeconds(60).TotalMilliseconds * 1000000);

            _timer?.Resume();

            timerRunning = true;

            System.Diagnostics.Debug.WriteLine("Timer Resumed");
        }

        void StopTimer()
        {
            toggleTimerButton.SetTitle("Start", UIControlState.Normal);

            _timer?.Suspend();

            timerRunning = false;

            System.Diagnostics.Debug.WriteLine("Timer Suspended");
        }

        partial void ToggleTimer(Foundation.NSObject sender)
        {
            if(timerRunning ?? false)
            {
                StopTimer();
            }
            else
            {
                StartTimer();
            }
        }

        async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : BasePermission
        {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                status = await permission.RequestAsync();
            }

            return status;
        }
    }
}