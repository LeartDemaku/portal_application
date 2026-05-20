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
            new("ekonomi", "Ekonomi", "Analiza per biznes, financa, tregje dhe zhvillime rajonale.", "#0f766e"),
            new("teknologji", "Teknologji", "Inovacion, produkte te reja dhe transformimi digjital.", "#1d4ed8"),
            new("sport", "Sport", "Lajmet kryesore, rezultatet dhe prapaskenat e gares.", "#ea580c"),
            new("shendetesi", "Shendetesi", "Udhezime, kerkime dhe histori qe ndikojne te mireqenia.", "#be123c")
        ];

        public static IReadOnlyList<PortalSectionDefinition> All => Sections;

        public static PortalSectionDefinition Resolve(string? categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return new PortalSectionDefinition("kategori", "Kategori", "Permbajtje editoriale e organizuar sipas tematikes.", "#475569");
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

            return new PortalSectionDefinition(ToSlug(categoryName), categoryName.Trim(), "Permbajtje editoriale e organizuar sipas tematikes.", "#475569");
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
