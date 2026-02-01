namespace SNotes.View;

public partial class AllNotesPage : ContentPage
{

    public AllNotesPage()
    {
        InitializeComponent();
    }



    private void OnSwipeChanging(object sender, SwipeChangingEventArgs e)
    {
        var swipeView = sender as SwipeView;
        var grid = swipeView.Content as Grid;

        if (grid == null) return;

        // Calculate opacity based on drag distance (max 100 pixels)
        double opacity = Math.Min(e.Offset / 100.0, 0.5); // Cap opacity at 0.5

        if (e.Offset > 0)
        {
            grid.BackgroundColor = Color.FromRgba(1.0, 0, 0, opacity);
        }
        else
        {
            grid.BackgroundColor = Colors.Transparent;
        }
    }

    private void OnSwipeEnded(object sender, SwipeEndedEventArgs e)
    {
        // Safety check: When the user lets go, if the swipe didn't stay open, reset the color.
        var swipeView = sender as SwipeView;
        var grid = swipeView.Content as Grid;

        if (!e.IsOpen)
        {
            grid.BackgroundColor = Colors.Transparent;
        }
    }
}


