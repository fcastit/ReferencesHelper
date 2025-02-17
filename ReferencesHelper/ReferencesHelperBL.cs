using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ReferencesHelper
{
    public class ReferencesHelperBL
    {
        public static List<Reference> GetReferences(string projUrl, ref string errore)
        {
            var result = new List<Reference>();
            var msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";

            try
            {
                var projDefinition = XDocument.Load(projUrl);
                var projName = GetProjectNameFromPath(projUrl);

                var references = GetReferencesFromProject(projDefinition, msbuild, "Reference", projName, projUrl);
                var projectReferences = GetReferencesFromProject(projDefinition, msbuild, "ProjectReference", projName, projUrl);

                result.AddRange(references);
                result.AddRange(projectReferences);
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return result;
        }

        private static IEnumerable<Reference> GetReferencesFromProject(XDocument projDefinition, XNamespace msbuild, string referenceElement, string projName, string projUrl)
        {
            var references = projDefinition
                .Element(msbuild + "Project")
                .Elements(msbuild + "ItemGroup")
                .Elements(msbuild + referenceElement)
                .Select(refElem => new Reference
                {
                    ProjectName = projName,
                    ProjectPath = projUrl,
                    Name = refElem.Value,
                    Path = refElem.FirstAttribute?.Value,
                    Type = referenceElement
                });

            return references;
        }

        private static string GetProjectNameFromPath(string projPath)
        {
            var nmspc = "x";
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(projPath);
            var mgr = new XmlNamespaceManager(xmlDoc.NameTable);
            mgr.AddNamespace(nmspc, xmlDoc.DocumentElement.NamespaceURI);
            var node = xmlDoc.SelectSingleNode($"//{nmspc}:PropertyGroup//{nmspc}:AssemblyName", mgr);
            return node?.FirstChild?.Value;
        }

        public static List<Reference> GetReferencesPerSolution(string solPath, ref string errore)
        {
            var references = new List<Reference>();

            try
            {
                var sol = new Solution(solPath);
                foreach (var projName in sol.Projects)
                {
                    var projPath = Path.Combine(Path.GetDirectoryName(solPath), projName.RelativePath);
                    references.AddRange(GetReferences(projPath, ref errore));
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return references;
        }

        public static List<Reference> GetReferencesPerFolder(string folder, ref string errore)
        {
            var references = new List<Reference>();

            try
            {
                var projFiles = Directory.GetFiles(folder, "*.vbproj", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles(folder, "*.csproj", SearchOption.AllDirectories))
                    .ToList();

                foreach (var projFile in projFiles)
                {
                    try
                    {
                        references.AddRange(GetReferences(projFile, ref errore));
                    }
                    catch (Exception ex)
                    {
                        references.Add(new Reference { Name = ex.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                errore = ex.Message;
            }

            return references;
        }

        public static bool ExportReferencesToCSV(string outputFilePath, List<Reference> references, ref string errore)
        {
            try
            {
                var csv = new StringBuilder();
                foreach (var reference in references)
                {
                    csv.AppendLine(reference.ToString());
                }

                File.WriteAllText(Path.Combine(outputFilePath, "references.csv"), csv.ToString());
                return true;
            }
            catch (Exception ex)
            {
                errore = ex.Message;
                return false;
            }
        }
    }
}
