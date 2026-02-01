using CommunityToolkit.Mvvm.ComponentModel; // Required for ObservableObject
using CommunityToolkit.Mvvm.Input;        // Required for AsyncRelayCommand
using System.Windows.Input;

namespace SNotes.ViewModel;

// INHERITANCE:
// 1. ObservableObject: Gives us 'OnPropertyChanged' so the UI updates automatically.
// 2. IQueryAttributable: Allows this class to receive data (like "load this file") from the navigation system.
internal class NoteViewModel : ObservableObject, IQueryAttributable
{
    // THE MODEL: 
    // This is the raw data object that does the actual file reading/writing.
    // We wrap it here so the View never talks to the file system directly.
    private Model.Note _note;

    // CONSTRUCTORS:

    // 1. Default: Called when creating a BRAND NEW note.
    public NoteViewModel()
    {
        _note = new Model.Note(); // Create a blank file in memory
        SetupCommands();
    }

    // 2. Overload: Called when we already have a note loaded (e.g. in the list view).
    public NoteViewModel(Model.Note note)
    {
        _note = note;
        SetupCommands();
    }

    // PROPERTIES:

    // The text displayed in the editor.
    public string Text
    {
        get => _note.Text;
        set
        {
            // Only update if the text actually changed to save performance
            if (_note.Text != value)
            {
                _note.Text = value;
                // NOTIFY: Tells the UI "The text changed, please redraw the text box!"
                OnPropertyChanged();
            }
        }
    }



    public string Title => _note.Title;
 


    // Read-only properties for display
    public DateTime Date => _note.Date;
    public string Identifier => _note.FileName;

    // COMMANDS:
    // These connect buttons in the UI (Save/Delete) to methods in this code.
    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }
    public ICommand EditCommand { get; private set; }


    private void SetupCommands()
    {
        SaveCommand = new AsyncRelayCommand(Save);
        DeleteCommand = new AsyncRelayCommand(Delete);
        EditCommand = new AsyncRelayCommand(Edit);

    }

    // ACTIONS:

    private async Task Save()
    {
        _note.Date = DateTime.Now; // Update timestamp
        _note.Save();              // Write to disk

        // NAVIGATION TRICK:
        // ".." means "Go back one page".
        // "?saved=..." passes data back to the previous page (AllNotesPage) 
        // so it knows which note to update in the list.
        await Shell.Current.GoToAsync($"..?saved={_note.FileName}");
    }

    private async Task Delete()
    {
        _note.Delete(); // Delete from disk

        // Pass "deleted=..." back so the list knows to remove this item.
        await Shell.Current.GoToAsync($"..?deleted={_note.FileName}");
    }

    private async Task Edit()
    {
        // The note knows its own ID ('Identifier'), so it can navigate easily
        await Shell.Current.GoToAsync($"{nameof(View.NotePage)}?load={Identifier}");
    }

    // NAVIGATION HANDLING:
    // This runs automatically when you navigate TO this page.
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Did we arrive here with a request to load a specific file?
        // e.g., Navigation path was "...?load=filename.notes.txt"
        if (query.ContainsKey("load"))
        {
            // 1. Load the data from disk
            _note = Model.Note.Load(query["load"].ToString());

            // 2. Tell the UI to redraw with the new data
            RefreshProperties();
        }
    }

   
    // HELPER METHODS:

    public void Reload()
    {
        // Re-reads the file from disk (useful if changed elsewhere)
        _note = Model.Note.Load(_note.FileName);
        RefreshProperties();
    }

    private void RefreshProperties()
    {
        // Manually trigger the update event for these properties
        OnPropertyChanged(nameof(Text));
        OnPropertyChanged(nameof(Date));
    }


   

}