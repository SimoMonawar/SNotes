using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SNotes.Model;
using System.Windows.Input;

namespace SNotes.ViewModel
{
    // CLASS NOTE: Must be 'partial' for CommunityToolkit to generate code.
    // IQueryAttributable is used to receive data (like an ID) when navigating to this page.
    internal partial class ToDoViewModel : ObservableObject, IQueryAttributable
    {
        // The actual data model (Private)
        private ToDo _toDo;

        // --- CONSTRUCTORS ---

        public ToDoViewModel()
        {
            _toDo = new ToDo();
            SetupCommands();
        }

        public ToDoViewModel(ToDo toDo)
        {
            _toDo = toDo;
            // Initialize the CheckBox state from the loaded file
            _isDone = toDo.IsDone;
            SetupCommands();
        }

        // --- PROPERTIES ---

        // 1. ToDoName: Acts as the Unique ID / Filename
        public string ToDoName
        {
            get => _toDo.ToDoName;
            set
            {
                if (_toDo.ToDoName != value)
                {
                    _toDo.ToDoName = value;
                    OnPropertyChanged(); // Notifies UI to update
                }
            }
        }

        // 2. ToDoDescription: The actual text shown in the list
        public string ToDoDescription
        {
            get => _toDo.ToDoDescription;
            set
            {
                if (_toDo.ToDoDescription != value)
                {
                    _toDo.ToDoDescription = value;
                    OnPropertyChanged();
                }
            }
        }

        // Helper properties for display
        public string Identifier => _toDo.ToDoName;
        public DateTime Date => _toDo.CreatedDate;

        // 3. IsDone: The CheckBox State
        // [ObservableProperty] automatically generates 'public bool IsDone' 
        // and the 'OnIsDoneChanged' partial method hook.
        [ObservableProperty]
        private bool _isDone;

        // --- COMMANDS ---

        public ICommand SaveToDoCommand { get; private set; }
        public ICommand DeleteToDoCommand { get; private set; }
        public ICommand EditToDoCommand { get; private set; }

        private void SetupCommands()
        {
            SaveToDoCommand = new AsyncRelayCommand(SaveToDo);
            DeleteToDoCommand = new AsyncRelayCommand(DeleteToDo);
            EditToDoCommand = new AsyncRelayCommand(EditToDo);
        }

        // --- ACTIONS & LOGIC ---

        // This method runs AUTOMATICALLY when IsDone changes (checkbox clicked)
        partial void OnIsDoneChanged(bool value)
        {
            // 1. Update the underlying model
            _toDo.IsDone = value;

            // 2. Save to file immediately so state persists after restart
            _toDo.SaveToDo();

            // Note: No need to update colors here; XAML DataTriggers handle the gray text!
        }

        public async Task SaveToDo()
        {
            _toDo.CreatedDate = DateTime.Now;
            _toDo.SaveToDo();

            // Navigate back (..) and pass the ID so the list can reload just this item
            await Shell.Current.GoToAsync($"..?saved={_toDo.ToDoName}");
        }

        public async Task DeleteToDo()
        {
            _toDo.DeleteToDo();
            // Navigate back (..) and pass ID so the list knows what to remove
            await Shell.Current.GoToAsync($"..?deleted={_toDo.ToDoName}");
        }

        public async Task EditToDo()
        {
            // Navigate to the Detail Page and pass the ID ("load=Filename")
            await Shell.Current.GoToAsync($"{nameof(View.ToDoPage)}?load={_toDo.ToDoName}");
        }

        // --- NAVIGATION DATA RECEIVER ---

        // This runs when we arrive on this page (or the page this VM is attached to)
        void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("load"))
            {
                string id = query["load"].ToString();

                // Load the data from disk
                _toDo = ToDo.LoadToDo(id);

                // Sync the ViewModel properties with the new Model data
                RefreshProperties();
            }
        }

        // Call this to update the UI if the underlying Model changes
        public void ReloadToDo()
        {
            _toDo = ToDo.LoadToDo(_toDo.ToDoName);
            RefreshProperties();
        }

        private void RefreshProperties()
        {
            OnPropertyChanged(nameof(ToDoName));
            OnPropertyChanged(nameof(ToDoDescription));
            OnPropertyChanged(nameof(Date));

            // Sync the checkbox state manually
            IsDone = _toDo.IsDone;
        }
    }
}