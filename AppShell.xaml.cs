namespace SNotes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Register the route so "GoToAsync" can find the page
            Routing.RegisterRoute(nameof(View.NotePage), typeof(View.NotePage));
            Routing.RegisterRoute(nameof(View.ToDoPage), typeof(View.ToDoPage));
        }
    }

}
    