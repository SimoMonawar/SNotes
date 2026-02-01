using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SNotes.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SNotes.ViewModel
{
    // Inheriting from ObservableObject ensures this VM is ready for future features (like "IsBusy")
    internal partial class AllToDoListViewModel : ObservableObject, IQueryAttributable
    {
        // The collection bound to the XAML CollectionView
        public ObservableCollection<ToDoViewModel> AllToDoList { get; }

        public ICommand NewToDoCommand { get; }

        public AllToDoListViewModel()
        {
            // 1. Load raw models from file
            // 2. Convert them to ViewModels
            var items = ToDo.LoadAllToDos().Select(t => new ToDoViewModel(t));

            // 3. Initialize the observable collection
            AllToDoList = new ObservableCollection<ToDoViewModel>(items);

            NewToDoCommand = new AsyncRelayCommand(NewToDoAsync);
        }

        public async Task NewToDoAsync()
        {
            // Navigate to the empty editor page to create a new task
            await Shell.Current.GoToAsync(nameof(View.ToDoPage));
        }

        // --- NAVIGATION RECEIVER ---

        // This method runs automatically when we navigate BACK to this list from the Detail Page.
        // It checks if an item was "deleted" or "saved" and updates the list accordingly.
        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // SCENARIO 1: An item was deleted
            if (query.ContainsKey("deleted"))
            {
                string toDoID = query["deleted"].ToString();

                // Find the item in our current list using the ID
                ToDoViewModel matchedToDo = AllToDoList.FirstOrDefault(t => t.Identifier == toDoID);

                // Remove it from the UI immediately
                if (matchedToDo != null)
                {
                    AllToDoList.Remove(matchedToDo);
                }
            }

            // SCENARIO 2: An item was saved (New or Edited)
            if (query.ContainsKey("saved"))
            {
                string toDoID = query["saved"].ToString();

                // Check if this item is already in our list
                ToDoViewModel matchedToDo = AllToDoList.FirstOrDefault(t => t.Identifier == toDoID);

                if (matchedToDo != null)
                {
                    // Existing Item: Reload its data to show changes
                    matchedToDo.ReloadToDo();

                    // Move it to the top of the list (Most recently modified)
                    AllToDoList.Move(AllToDoList.IndexOf(matchedToDo), 0);
                }
                else
                {
                    // New Item: Load it from disk and insert at the very top
                    AllToDoList.Insert(0, new ToDoViewModel(ToDo.LoadToDo(toDoID)));
                }
            }
        }
    }
}