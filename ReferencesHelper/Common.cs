namespace ReferencesHelper
{
    public static class Common
    {
        // Function to check if the input is a folder
        public static bool IsFolder(string input) =>
            System.IO.Directory.Exists(input);

        // Function to check if the input is a solution
        public static bool IsSolution(string input) =>
            input.EndsWith(".sln", StringComparison.OrdinalIgnoreCase);

        // Function to check if the input is a project
        public static bool IsProject(string input) =>
            input.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) ||
            input.EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase);
    }
}

