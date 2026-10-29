# Plani: ridizajnimi i portalit ZëriYnë (vetëm portali)

## 1. Prompt-i i strukturuar

**Qëllimi:** Ridizajno portalin publik të ZëriYnë (ASP.NET Core 8 MVC) me një dizajn editorial ultra-modern, me light mode dhe dark mode, të përshtatur për telefon, tablet dhe kompjuter, gati për publikim.

**Konteksti:**
- Projekti: `PortalApp` (.NET 8, Razor Views, Identity, SQL Server Express).
- Portali = `Views/Shared/_Layout.cshtml` + `Views/Home`, `Views/Sections`, `Views/Tag`, `Views/Shared/Partials/_PortalListing.cshtml`, faqet e Identity (`Areas/Identity/Pages/Account`).
- Admin Panel = `Views/Admin/**`, `Views/Shared/Admin/_Layout.cshtml`, `wwwroot/css/admin.css`, `wwwroot/js/admin.js`. Admin-i ngarkon edhe `wwwroot/css/site.css`.

**Kufizimet:**
- Mos prek asnjë skedar të Admin Panel-it, as `site.css` dhe `site.js` (admin-i i përdor).
- Mos ndrysho databazën, modelet, migrimet dhe logjikën e kontrollerëve.
- Pa librari të reja dhe pa hapa build për frontend: CSS dhe JavaScript i pastër.
- Çdo tekst i dukshëm në shqip me ë dhe ç.

**Si kontrollohet rezultati:**
- `dotnet build` pa asnjë error dhe pa asnjë warning të ri.
- Çdo faqe e portalit hapet pa gabime në konsolë: Ballina, kategori, tag, kërkim, artikull, komentet, hyrja, regjistrimi.
- Light dhe dark mode punojnë, mbahen mend pas rifreskimit, dhe nuk ka "flash" të bardhë.
- Faqet kontrollohen në 375 px (telefon), 768 px (tablet) dhe 1280 px+ (kompjuter).
- Admin Panel-i duket dhe punon njësoj si para ndryshimeve.

## 2. Arkitektura e re

```
Shfletuesi
   │
   ▼
_Layout.cshtml (portali)          Shared/Admin/_Layout.cshtml (pa ndryshim)
   ├── css/portal.css  (I RI)        ├── css/site.css   (pa ndryshim)
   ├── js/portal.js    (I RI)        ├── css/admin.css  (pa ndryshim)
   └── pa Bootstrap, pa jQuery       └── Bootstrap + jQuery
        (jQuery vetëm te faqet me formularë, për validimin)
```

## 3. Hapat (milestones)

1. **Sistemi i dizajnit** — `wwwroot/css/portal.css`: ngjyrat si variabla (tokens) për light dhe dark, tipografia (Fraunces për titujt, Inter për tekstin), hapësirat, rrezet, hijet, animacionet. Respekton `prefers-reduced-motion`.
2. **Layout-i i ri** — header "glass" që ngjitet lart, navigimi i kategorive që rrëshqet horizontalisht në telefon, menyja anësore (drawer) për telefon, kërkimi si dritare (Ctrl+K ose `/`), butoni light/dark, footer-i i ri, butoni "kthehu lart".
3. **Ballina** — shiriti i lajmeve të fundit (ticker), hero "bento" me lajmin kryesor, rrjeta e lajmeve të fundit me kartën e parë të theksuar, seksionet e kategorive me ngjyrat e tyre dhe lista të numëruara.
4. **Faqja e artikullit** — shiriti i progresit të leximit, breadcrumb, tipografi e madhe për lexim, butonat e ndarjes (kopjo linkun, Facebook, X, WhatsApp, Web Share në telefon), sidebar që ngjitet, komentet e ridizajnuara.
5. **Faqet e listimit** — kategoria, tag-u, kërkimi: hero me ngjyrën e kategorisë, gjendja bosh e qartë.
6. **Hyrja dhe regjistrimi** — kartat e reja, fushat me etiketa lundruese (floating labels) pa Bootstrap; faqet Logout dhe ResetPasswordConfirmation marrin dizajn.
7. **Gati për publikim** — meta description, Open Graph, `theme-color` për të dy modet, JSON-LD `NewsArticle` te artikulli, `loading="lazy"` te fotot, foto rezervë (placeholder) kur mungon foto, faqja e gabimit në shqip, rregullimi i adresës së gabimit te `Program.cs` (`/Error` nuk ekziston → `/Home/Error`), cache i gjatë për skedarët statikë me version.
8. **Verifikimi** — build, çdo faqe në 3 madhësi ekrani, të dy modet, konsola pa gabime, Admin Panel-i i paprekur.

## 4. Bug-e të gjetura dhe të rregulluara gjatë punës

- **Komentet nuk ruheshin kurrë.** Formulari dërgon `CommentForm.Name`, kurse kontrolleri priste `Name`. U rregullua me `[Bind(Prefix = "CommentForm")]` te `HomeController.Comment`.
- **Fjalë të ngjitura te përmbledhjet** ("DemakuKosova"): etiketat HTML hiqeshin pa lënë hapësirë.
- **Foto rezervë që nuk ekzistonte** (`lajmi_kryesor.png`) → `images/portal/placeholder.svg`.
- **Faqja e gabimit në production** tregonte te `/Error`, që nuk ekziston → `/Home/Error`.

## 5. Çfarë NUK bëhet në këtë plan (për më vonë)

- `.gitignore` aktual është për projekte AL (Business Central), prandaj `bin/`, `obj/` dhe `.vs/` futen në Git. Duhet `.gitignore` për .NET.
- `Show.cshtml` përdor `Html.Raw` për përmbajtjen e artikullit: nëse një gazetar fut `<script>`, ai ekzekutohet te lexuesit. Duhet pastrim i HTML-së (HtmlSanitizer) para publikimit.
- Fjalëkalimi `Password123!` për përdoruesit fillestarë te `Program.cs` duhet ndryshuar para publikimit.
