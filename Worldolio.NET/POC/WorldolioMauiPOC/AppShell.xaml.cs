namespace WorldolioMauiPOC
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

#if DEBUG
            this.Title = this.Title + " (Debug)";
#endif
        }
    }
}
