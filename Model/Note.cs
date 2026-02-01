using Microsoft.Extensions.Hosting;
using System.IO;  // Required for File, Path, Directory
using System.Linq; // Required for Select, OrderByDescending

namespace SNotes.Model;

// RESPONSIBILITY:
// This class handles Data and Storage. 
// It knows HOW to save to disk, but it doesn't care WHEN or WHY.
internal class Note
{
    // PROPERTIES:
    // FileName: The unique ID on disk (e.g., "x8s1.notes.txt"). User never sees this.
    public string FileName { get; set; }

    // Text: The actual content the user types.
    public string Text { get; set; }

    // Date: When the file was last modified.
    public DateTime Date { get; set; }
    public string Title => string.IsNullOrWhiteSpace(Text)
        ? "New Note"
        : Text.Substring(0, Math.Min(30, Text.Length));


    // CONSTRUCTOR (Create New):
    public Note()
    {
        // Generate a random, unique filename so notes never overwrite each other.
        // Path.GetRandomFileName() returns a random 8.3 string (e.g., "w5k2s9.q2")
        FileName = $"{Path.GetRandomFileName()}.notes.txt";
        Text = "";
        Date = DateTime.Now;
    }

    // FILE OPERATIONS:

    public void Save()
    {
        // 1. Find the safe app folder (Sandbox) on the device.
        string path = Path.Combine(FileSystem.AppDataDirectory, FileName);

        // 2. Write the text to that file.
        File.WriteAllText(path, Text);
    }

    public void Delete()
    {
        string path = Path.Combine(FileSystem.AppDataDirectory, FileName);

        // If the file exists, remove it permanently.
        if (File.Exists(path))
            File.Delete(path);
    }

    // FACTORY METHODS (Loading):
    // These are 'static' because they don't belong to one specific note.
    // They are "Tools" used to create Note objects.

    public static Note Load(string filename)
    {
        // Reconstruct the full path
        string path = Path.Combine(FileSystem.AppDataDirectory, filename);

        if (!File.Exists(path))
            throw new FileNotFoundException("Note file not found", path);

        // Create and return a populated Note object
        return new Note
        {
            FileName = Path.GetFileName(path),
            Text = File.ReadAllText(path),
            Date = File.GetLastWriteTime(path) // Auto-fetch the file timestamp
        };
    }

    public static IEnumerable<Note> LoadAll()
    {
        string appDataPath = FileSystem.AppDataDirectory;

        // LINQ QUERY:
        // 1. EnumerateFiles: Find all files ending in ".notes.txt"
        // 2. Select: Convert the list of Strings (paths) into a list of Note Objects.
        // 3. OrderByDescending: Sort them by date (Newest first).
        return Directory
            .EnumerateFiles(appDataPath, "*.notes.txt")
            .Select(filename => Note.Load(Path.GetFileName(filename)))
            .OrderByDescending(note => note.Date);
    }
}