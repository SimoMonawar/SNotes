using System.Text.Json; // FIX: Needed to save the object structure (Text + Checkbox)
////// just a comment line to test
namespace SNotes.Model
{
    internal class ToDo
    {
        // --- PROPERTIES ---

        // 1. ToDoName: The Unique Filename (ID)
        public string ToDoName { get; set; }

        // 2. ToDoDescription: The visible text
        public string ToDoDescription { get; set; }

        // 3. IsDone: The Checkbox status
        public bool IsDone { get; set; }

        // 4. CreatedDate: Saved so sorting persists correctly
        public DateTime CreatedDate { get; set; }

        // --- CONSTRUCTOR ---

        public ToDo()
        {
            // Create a unique filename for new items
            ToDoName = $"{Path.GetRandomFileName()}.todo.txt";
            ToDoDescription = "";
            CreatedDate = DateTime.Now;
            IsDone = false;
        }




        //just a comment line to test
        //to see if changes are detected
        // --- METHODS ---

        // Saving logic: Converts this C# Object -> JSON String -> File
        public void SaveToDo()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, ToDoName);

            // Serialize: specific logic to bundle Description, IsDone, and Date into one string
            string json = JsonSerializer.Serialize(this);

            File.WriteAllText(path, json);
        }

        // Deleting logic: Finds the file by name and removes it
        public void DeleteToDo()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, ToDoName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        // Loading logic: Reads File -> JSON String -> C# Object
        public static ToDo LoadToDo(string filename)
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, filename);

            if (!File.Exists(path))
                throw new FileNotFoundException("File not found", path);

            // 1. Read the text from disk
            string json = File.ReadAllText(path);

            // 2. Convert it back into a ToDo object
            // Note: If you have old files that aren't JSON, this line will crash. 
            // Clear app data if that happens!
            ToDo loadedToDo = JsonSerializer.Deserialize<ToDo>(json);

            // 3. Ensure the object knows its own filename
            loadedToDo.ToDoName = filename;

            return loadedToDo;
        }

        // Load All: Scans the folder and converts every file found
        public static IEnumerable<ToDo> LoadAllToDos()
        {
            string path = FileSystem.AppDataDirectory;

            // Ensure the directory exists to avoid crashes on first run
            if (!Directory.Exists(path))
                return new List<ToDo>();

            return Directory
                .EnumerateFiles(path, "*.todo.txt")
                .Select(file => ToDo.LoadToDo(Path.GetFileName(file)))
                .OrderByDescending(x => x.CreatedDate); // Sorts by the date saved in the JSON
        }
    }
}