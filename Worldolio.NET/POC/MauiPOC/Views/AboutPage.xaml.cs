#if ANDROID
using Android.Content.PM;
using AndroidX.Core.Content.PM;
#endif
using System.Windows.Input;

namespace MauiPOC.Views;

public partial class AboutPage : ContentPage
{
    public string AppVersion { get; set; } = "";
    public string DotNetVersion { get; set; } = "";
    public string Package { get; set; } = "";
    public ICommand NavigateBack { get; }

    public AboutPage()
	{
		InitializeComponent();

        TxtAppVersion.Text = $"{AppInfo.Current.Version.Major}.{AppInfo.Current.Version.Minor}.{AppInfo.Current.Version.Build} ({GetVersionCode()}) ({GetBuildType()})";
        TxtNetVersion.Text = Environment.Version.ToString();
        // Get the MAUI version
        Version mauiVersion = typeof(MauiApp).Assembly.GetName().Version;
        TxtMauiVersion.Text = $"{mauiVersion.ToString()}";
        TxtPackage.Text = $"{AppInfo.Current.PackageName}";
    }

    private string GetVersionCode()
    {
#if ANDROID
            var context = Android.App.Application.Context;
            PackageInfo? package = (
                //Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu ?
                OperatingSystem.IsAndroidVersionAtLeast(33) ?
                context.PackageManager?.GetPackageInfo(context.PackageName ?? "", PackageManager.PackageInfoFlags.Of((long)0)) :
                context.PackageManager?.GetPackageInfo(context.PackageName ?? "", 0)
            );
            long longVersionCode = package != null ? PackageInfoCompat.GetLongVersionCode(package) : 0;
            return longVersionCode.ToString();
#else
        return AppInfo.Current.Version.Revision.ToString();
#endif
    }

    private string GetBuildType()
    {
#if DEBUG
        return "Debug";
#else
            return "Release";
#endif
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
}