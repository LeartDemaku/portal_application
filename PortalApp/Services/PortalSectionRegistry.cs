using System.Globalization;
using System.Text;

namespace PortalApp.Services
{
    public record PortalSectionDefinition(
        string Slug,
        string Name,
        string Description,
        string AccentColor);

    public static class PortalSectionRegistry
    {
        private static readonly IReadOnlyList<PortalSectionDefinition> Sections =
        [
            new("ekonomi", "Ekonomi", "Analiza për biznesin, financat, tregjet dhe zhvillimet rajonale.", "#0f766e"),
            new("teknologji", "Teknologji", "Inovacion, produkte të reja dhe transformimi digjital.", "#2563eb"),
            new("sport", "Sport", "Lajmet kryesore, rezultatet dhe prapaskenat e garës.", "#ea580c"),
            new("shendetesi", "Shëndetësi", "Udhëzime, kërkime dhe histori që ndikojnë te mirëqenia.", "#e11d48"),
            new("globale", "Globale", "Lajme dhe zhvillime nga arena ndërkombëtare dhe ngjarjet botërore.", "#4f46e5"),
            new("politike", "Politikë", "Zhvillimet e fundit politike, vendimet qeveritare dhe analizat.", "#c026d3"),
            new("siguri", "Siguri", "Lajme nga bota e sigurisë, krimit dhe rendit publik.", "#d97706"),
            new("drejtesi", "Drejtësi", "Gjykatat, proceset gjyqësore dhe sundimi i ligjit.", "#7c3aed")
        ];

        public static IReadOnlyList<PortalSectionDefinition> All => Sections;

        public static PortalSectionDefinition Resolve(string? categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return new PortalSectionDefinition("kategori", "Kategori", "Përmbajtje editoriale e organizuar sipas temës.", "#64748b");
            }

            if (categoryName.Contains("Sh", StringComparison.OrdinalIgnoreCase) && categoryName.Contains("det", StringComparison.OrdinalIgnoreCase))
            {
                return Sections.First(x => x.Slug == "shendetesi");
            }

            var normalized = Normalize(categoryName);
            var section = Sections.FirstOrDefault(x => Normalize(x.Name) == normalized || Normalize(x.Slug) == normalized);

            if (section != null)
            {
                return section;
            }

            return new PortalSectionDefinition(ToSlug(categoryName), categoryName.Trim(), "Përmbajtje editoriale e organizuar sipas temës.", "#64748b");
        }

        public static string ToSlug(string value)
        {
            var normalized = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            var lastWasDash = false;

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                var lower = char.ToLowerInvariant(character);

                if (char.IsLetterOrDigit(lower))
                {
                    builder.Append(lower);
                    lastWasDash = false;
                    continue;
                }

                if (!lastWasDash && builder.Length > 0)
                {
                    builder.Append('-');
                    lastWasDash = true;
                }
            }

            return builder.ToString().Trim('-');
        }

        private static string Normalize(string value)
        {
            return ToSlug(value).Replace("-", string.Empty, StringComparison.Ordinal);
        }
    }
}
