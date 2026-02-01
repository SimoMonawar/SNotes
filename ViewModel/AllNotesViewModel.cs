using CommunityToolkit.Mvvm.Input; // For AsyncRelayCommand
using SNotes.Model; // To access the Note model
using System.Collections.ObjectModel; // For ObservableCollection
using System.Linq; // For LINQ queries (Select, Where, FirstOrDefault)
using System.Windows.Input; // For ICommand

namespace SNotes.ViewModel;

// RESPONSIBILITY: 
// This class manages the LIST of notes. It connects the "AllNotesPage" (UI) 
// to the data on the disk.
internal partial class AllNotesViewModel : IQueryAttributable
{
    // DATA SOURCE:
    // We use ObservableCollection because it automatically notifies the UI 
    // when items are Added, Removed, or Moved.
    // If we used a standard List<T>, the screen wouldn't update when we delete a note.
    public ObservableCollection<NoteViewModel> AllNotes { get; }

    // COMMANDS:
    // Actions triggered by buttons in the UI.
    public ICommand NewCommand { get; }
   
   
    public AllNotesViewModel()
    {
        // 1. LOAD DATA:
        // When the app starts, we ask the Model to "LoadAll()" files.
        // We then convert every "Note" (data) into a "NoteViewModel" (wrapper).
        var notes = Note.LoadAll().Select(n => new NoteViewModel(n));
        AllNotes = new ObservableCollection<NoteViewModel>(notes);

        // 2. SETUP COMMANDS:
        NewCommand = new AsyncRelayCommand(NewNoteAsync);
        
        
    }

    // NAVIGATION ACTIONS:

    public async Task NewNoteAsync()
    {
        // Navigate to the NotePage. No arguments means "Create New".
        await Shell.Current.GoToAsync(nameof(View.NotePage));
    }


    // THE "RETURN" HANDLER:
    // This method runs automatically when we navigate BACK to this page 
    // from the NotePage. It checks if we "saved" or "deleted" something.
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // CASE 1: A note was DELETED
        if (query.ContainsKey("deleted"))
        {
            string noteID = query["deleted"].ToString();

            // Find the note in our list with that ID
            NoteViewModel matchedNote = AllNotes.FirstOrDefault(n => n.Identifier == noteID);

            // If it exists in our list, remove it.
            // Because this is an ObservableCollection, the row disappears from the UI instantly.
            if (matchedNote != null)
                AllNotes.Remove(matchedNote);
        }

        // CASE 2: A note was SAVED (New or Edited)
        else if (query.ContainsKey("saved"))
        {
            string noteID = query["saved"].ToString();
            NoteViewModel matchedNote = AllNotes.FirstOrDefault(n => n.Identifier == noteID);

            // Sub-case A: It was an EXISTING note that was edited
            if (matchedNote != null)
            {
                matchedNote.Reload(); // Refresh the text/date on the card

                // Move it to index 0 (top of the list) so recent stuff is first
                AllNotes.Move(AllNotes.IndexOf(matchedNote), 0);
            }

            // Sub-case B: It is a BRAND NEW note
            else
            {
                // Load the new file from disk and Insert it at the top (index 0)
                AllNotes.Insert(0, new NoteViewModel(Note.Load(noteID)));
            }
        }
    }


   
}