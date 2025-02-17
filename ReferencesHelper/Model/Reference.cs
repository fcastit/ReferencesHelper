namespace ReferencesHelper
{
    public class Reference
    {
        public string ProjectName { get; set; }
        public string ProjectPath { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }

        public override string ToString() =>
            $"{ProjectName};{ProjectPath};{Type};{Path};{Name}";
    }
}
