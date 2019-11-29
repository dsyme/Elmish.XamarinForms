using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace FabulousWeather.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadApplication(new FabulousWeather.App());
        }
    }
}
