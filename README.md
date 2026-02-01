📝 SNotes
SNotes is a streamlined productivity application built with .NET MAUI. It allows users to manage personal notes and track daily tasks with an interactive To-Do list, ensuring all data is persisted locally on the device for offline access.
🚀 Features
✅ To-Do List
Task Management: Create, edit, and delete tasks easily.
Status Tracking: Interactive checkboxes that save their state (Done / Not Done) permanently.
Visual Feedback: Completed items automatically gray out and show a strikethrough effect.
Smart Sorting: Active tasks remain at the top, while completed or saved tasks are organized by date.
Swipe Actions: Support for swipe-to-delete gestures on touch devices.
📒 Notes  
Text Editor: Write and save text-based notes.
Timestamps: Automatically tracks creation dates for sorting.
Persistence: All data is saved locally using JSON serialization.
🛠 Technologies
UsedFramework: .NET MAUI (Multi-platform App UI)
Language: C# 10+
Architecture: MVVM (Model-View-ViewModel)
Libraries:
CommunityToolkit.Mvvm (for Source Generators like [ObservableProperty])
System.Text.Json (for robust local data storage)
Storage: Local File System (FileSystem.AppDataDirectory)
📱 How It Works
The Architecture
SNotes follows the MVVM Pattern to separate logic from the UI:
Model (Note.cs/ToDo.cs):
Represents the data structure.
Uses JSON Serialization to save the Task ID, Description, Date, and IsDone status into a single text file (e.g., x8s1.todo.txt).
ViewModel (AllNotesViewModel/AllToDoListViewModel.cs):
Manages the ObservableCollection of tasks.
Handles navigation and passes data between pages using IQueryAttributable.
View (AllNotesPage.xaml/AllToDoList.xaml):
Uses XAML DataTriggers to handle UI changes (gray text/strikethrough) entirely in the view layer, keeping the backend code clean.



🏁 Getting Started
Prerequisites:Visual Studio 2022 (with .NET MAUI workload installed).
Installation:Clone this repository.
Open SNotes.sln in Visual Studio.
Select your target emulator (Android Emulator or Windows Machine).
Press F5 to build and run.


📄 LicenseThis project is for educational purposes. Feel free to use and modify it.
