using ListManager.Common;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ListManager.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class About : NavigationPage
    {
        public About()
        {
            this.InitializeComponent();

            ApplicationAbbreviation.Text = "(SLM)";
            ApplicationName.Text = "Simple List Manager";
            ApplicationVersion.Text = "Version " + Info.ApplicationVersion;
            
        }
    }
}
