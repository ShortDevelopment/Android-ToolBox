using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using static Android.Hardware.Camera;

namespace Camera
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.FullScreen", MainLauncher = true)]
    public class MainActivity : Activity
    {

        Android.Hardware.Camera camera;
        SurfaceView surfaceView;

        [Obsolete]
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.LayoutNoLimits | WindowManagerFlags.TranslucentNavigation | WindowManagerFlags.KeepScreenOn);
            var uiOptions =
                SystemUiFlags.HideNavigation |
                SystemUiFlags.LayoutFullscreen |
                SystemUiFlags.Fullscreen |
                SystemUiFlags.ImmersiveSticky;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)uiOptions;

            SetContentView(Resource.Layout.activity_main);

            surfaceView = FindViewById<SurfaceView>(Resource.Id.surfaceView1);

            RequestPermissions(new[] { Android.Manifest.Permission.Camera }, 0);


        }

        bool CameraRunning = false;

        public void InitCamera()
        {
            try
            {
                camera = Android.Hardware.Camera.Open();
                camera.SetPreviewDisplay(surfaceView.Holder);
                camera.SetDisplayOrientation(90);
                Parameters a = camera.GetParameters();
                a.FocusMode = Parameters.FocusModeContinuousPicture;
                camera.SetParameters(a);
                camera.StartPreview();
                CameraRunning = true;
            }
            catch (Exception ex)
            {
                // Snackbar.Make(surfaceView, ex.Message, Snackbar.LengthLong);
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            // if (!CameraRunning)
            InitCamera();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
            {
                InitCamera();
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
