using ReferencesHelper;
using System.Reflection;

class Program
{
    static void Main()
    {
        try
        {
            // Ask the user for an input string
            var input = GetUserInput("Enter the path of a folder, solution, or project:");

            // Determine the type of input and use the business logic from ReferencesHelper
            ProcessInput(input);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Message to press a key to continue
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    // Function to ask the user for an input string
    static string GetUserInput(string prompt)
    {
        Console.WriteLine(prompt);
        return Console.ReadLine();
    }

    // Function to process the input based on its type
    static void ProcessInput(string input)
    {
        var result = new List<Reference>();
        var error = string.Empty;

        if (Common.IsFolder(input))
        {
            result = ReferencesHelperBL.GetReferencesPerFolder(input, ref error);
        }
        else if (Common.IsSolution(input))
        {
            result = ReferencesHelperBL.GetReferencesPerSolution(input, ref error);
        }
        else if (Common.IsProject(input))
        {
            result = ReferencesHelperBL.GetReferences(input, ref error);
        }
        else
        {
            Console.WriteLine("Unrecognized input.");
            return;
        }

        if (result != null && result.Count > 0)
        {
            ExportReferences(result, ref error);
        }
        else
        {
            Console.WriteLine("No references found or error during processing.");
        }
    }

    // Function to export references to a CSV file
    static void ExportReferences(List<Reference> references, ref string error)
    {
        string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (ReferencesHelperBL.ExportReferencesToCSV(currentPath, references, ref error))
        {
            Console.WriteLine(@"Output file: {0}\references.csv.", currentPath);
        }
        else
        {
            Console.WriteLine($"Error during export: {error}");
        }
    }
}

