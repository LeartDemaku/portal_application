// ZëriYnë — JavaScript-i i portalit publik (pa librari të jashtme).
(() => {
    "use strict";

    const root = document.documentElement;
    const THEME_KEY = "zy-theme";
    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    /* ---------- Tema: light / dark ---------- */
    const systemDark = window.matchMedia("(prefers-color-scheme: dark)");

    const readStoredTheme = () => {
        try {
            return localStorage.getItem(THEME_KEY);
        } catch {
            return null;
        }
    };

    const syncThemeControls = (theme) => {
        document.querySelectorAll("[data-theme-toggle]").forEach((button) => {
            button.setAttribute("aria-label", theme === "dark" ? "Kalo në light mode" : "Kalo në dark mode");
        });
        document.querySelectorAll("[data-theme-set]").forEach((button) => {
            button.setAttribute("aria-pressed", String(button.dataset.themeSet === theme));
        });
        const meta = document.querySelector('meta[name="theme-color"]:not([media])');
        if (meta) {
            meta.setAttribute("content", theme === "dark" ? "#08090c" : "#f6f5f1");
        }
    };

    const applyTheme = (theme, persist) => {
        // Ndalojmë përkohësisht animacionet që ndërrimi të jetë i menjëhershëm
        root.classList.add("theme-switching");
        root.setAttribute("data-theme", theme);
        syncThemeControls(theme);
        if (persist) {
            try {
                localStorage.setItem(THEME_KEY, theme);
            } catch {
                /* localStorage mund të jetë i bllokuar: tema punon edhe pa u ruajtur */
            }
        }
        requestAnimationFrame(() => requestAnimationFrame(() => root.classList.remove("theme-switching")));
    };

    syncThemeControls(root.getAttribute("data-theme") || "light");

    document.querySelectorAll("[data-theme-toggle]").forEach((button) => {
        button.addEventListener("click", () => {
            applyTheme(root.getAttribute("data-theme") === "dark" ? "light" : "dark", true);
        });
    });

    document.querySelectorAll("[data-theme-set]").forEach((button) => {
        button.addEventListener("click", () => applyTheme(button.dataset.themeSet, true));
    });

    // Nëse përdoruesi s'ka zgjedhur vetë, ndjekim ndryshimin e sistemit
    systemDark.addEventListener("change", (event) => {
        if (!readStoredTheme()) {
            applyTheme(event.matches ? "dark" : "light", false);
        }
    });

    /* ---------- Header-i gjatë scroll-it + butoni "kthehu lart" + progresi i leximit ---------- */
    const header = document.querySelector("[data-site-header]");
    const toTop = document.querySelector("[data-to-top]");
    const progress = document.querySelector("[data-read-progress]");
    const articleBody = document.querySelector("[data-article-body]");

    let ticking = false;
    const onScroll = () => {
        const y = window.scrollY;
        header?.classList.toggle("is-scrolled", y > 8);
        toTop?.classList.toggle("is-visible", y > 900);

        if (progress && articleBody) {
            const rect = articleBody.getBoundingClientRect();
            const total = rect.height - window.innerHeight * 0.6;
            const read = Math.min(1, Math.max(0, (window.innerHeight * 0.4 - rect.top) / Math.max(total, 1)));
            progress.style.transform = `scaleX(${read})`;
        }
        ticking = false;
    };

    window.addEventListener("scroll", () => {
        if (!ticking) {
            ticking = true;
            requestAnimationFrame(onScroll);
        }
    }, { passive: true });
    onScroll();

    toTop?.addEventListener("click", () => {
        window.scrollTo({ top: 0, behavior: prefersReducedMotion ? "auto" : "smooth" });
    });

    /* ---------- Dialogët: kërkimi dhe menyja në telefon ---------- */
    const openDialog = (dialog) => {
        if (!dialog || dialog.open || typeof dialog.showModal !== "function") {
            return false;
        }
        dialog.showModal();
        document.body.style.overflow = "hidden";
        return true;
    };

    document.querySelectorAll("dialog").forEach((dialog) => {
        dialog.addEventListener("close", () => {
            document.body.style.overflow = "";
        });
        // Klikimi jashtë panelit (te sfondi) e mbyll dialogun
        dialog.addEventListener("click", (event) => {
            if (event.target === dialog) {
                dialog.close();
            }
        });
    });

    document.querySelectorAll("[data-dialog-close]").forEach((button) => {
        button.addEventListener("click", () => button.closest("dialog")?.close());
    });

    const searchDialog = document.getElementById("search-dialog");
    const openSearch = () => {
        if (openDialog(searchDialog)) {
            searchDialog.querySelector("input")?.focus();
            return true;
        }
        return false;
    };

    document.querySelectorAll("[data-search-open]").forEach((trigger) => {
        trigger.addEventListener("click", (event) => {
            // Pa <dialog> (shfletues shumë i vjetër) linku të çon te faqja /kerko
            if (openSearch()) {
                event.preventDefault();
            }
        });
    });

    const drawer = document.getElementById("menu-drawer");
    document.querySelectorAll("[data-menu-open]").forEach((button) => {
        button.addEventListener("click", () => {
            if (openDialog(drawer)) {
                button.setAttribute("aria-expanded", "true");
                drawer.addEventListener("close", () => button.setAttribute("aria-expanded", "false"), { once: true });
            }
        });
    });

    // Shkurtesat: Ctrl/⌘ + K ose "/" hapin kërkimin
    document.addEventListener("keydown", (event) => {
        const target = event.target;
        const typing = target instanceof HTMLElement && (target.isContentEditable || /^(INPUT|TEXTAREA|SELECT)$/.test(target.tagName));
        const isShortcut = (event.key.toLowerCase() === "k" && (event.ctrlKey || event.metaKey)) || (event.key === "/" && !typing);
        if (isShortcut && openSearch()) {
            event.preventDefault();
        }
    });

    /* ---------- Navigimi i kategorive: hijet anësore shfaqen vetëm kur ka ende për të rrëshqitur ---------- */
    document.querySelectorAll(".category-scroller").forEach((scroller) => {
        const list = scroller.querySelector(".category-links");
        if (!list) {
            return;
        }
        const syncFades = () => {
            scroller.classList.toggle("fade-left", list.scrollLeft > 4);
            scroller.classList.toggle("fade-right", list.scrollLeft + list.clientWidth < list.scrollWidth - 4);
        };
        // Në telefon, kategoria aktive sillet në pamje
        const active = list.querySelector("a.active");
        if (active && active.offsetLeft + active.offsetWidth > list.clientWidth) {
            list.scrollLeft = active.offsetLeft - 16;
        }
        syncFades();
        list.addEventListener("scroll", syncFades, { passive: true });
        window.addEventListener("resize", syncFades);
    });

    /* ---------- Shiriti i lajmeve: kopjojmë listën që lëvizja të jetë pa ndërprerje ---------- */
    document.querySelectorAll("[data-ticker]").forEach((track) => {
        const list = track.querySelector("ul");
        if (!list || prefersReducedMotion) {
            return;
        }
        const clone = list.cloneNode(true);
        clone.setAttribute("aria-hidden", "true");
        clone.querySelectorAll("a").forEach((link) => link.setAttribute("tabindex", "-1"));
        track.appendChild(clone);
        // Shpejtësi e njëjtë pavarësisht sa tituj ka
        const duration = Math.max(25, Math.round(list.scrollWidth / 60));
        track.style.setProperty("--ticker-duration", `${duration}s`);
    });

    /* ---------- Animacionet kur elementet hyjnë në ekran ---------- */
    const revealItems = document.querySelectorAll("[data-reveal]");
    if (revealItems.length) {
        if (prefersReducedMotion || !("IntersectionObserver" in window)) {
            revealItems.forEach((item) => item.classList.add("is-visible"));
        } else {
            const observer = new IntersectionObserver((entries) => {
                entries.forEach((entry) => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add("is-visible");
                        observer.unobserve(entry.target);
                    }
                });
            }, { rootMargin: "0px 0px -8% 0px", threshold: 0.08 });
            revealItems.forEach((item) => observer.observe(item));
        }
    }

    /* ---------- Koha relative: "para 3 orësh" ---------- */
    const relative = "Intl" in window && "RelativeTimeFormat" in Intl ? new Intl.RelativeTimeFormat("sq", { numeric: "auto" }) : null;
    if (relative) {
        const units = [
            ["minute", 60],
            ["hour", 3600],
            ["day", 86400]
        ];
        document.querySelectorAll("time[data-relative]").forEach((time) => {
            const date = new Date(time.getAttribute("datetime"));
            const seconds = Math.round((date.getTime() - Date.now()) / 1000);
            // Vetëm për 7 ditët e fundit; më vjetër mbetet data e plotë nga serveri
            if (Number.isNaN(seconds) || seconds > 60 || seconds < -7 * 86400) {
                return;
            }
            let text = "tani";
            for (let i = units.length - 1; i >= 0; i--) {
                const [unit, size] = units[i];
                if (Math.abs(seconds) >= size) {
                    text = relative.format(Math.round(seconds / size), unit);
                    break;
                }
            }
            time.title = time.textContent.trim();
            time.textContent = text;
        });
    }

    /* ---------- Ndarja e artikullit ---------- */
    if (typeof navigator.share === "function") {
        root.classList.add("can-share");
    }

    document.querySelectorAll("[data-native-share]").forEach((button) => {
        button.addEventListener("click", async () => {
            try {
                await navigator.share({ title: document.title, url: button.dataset.url });
            } catch {
                /* Përdoruesi e anuloi ndarjen: s'ka nevojë për veprim */
            }
        });
    });

    document.querySelectorAll("[data-copy-link]").forEach((button) => {
        const originalLabel = button.getAttribute("aria-label");
        button.addEventListener("click", async () => {
            try {
                await navigator.clipboard.writeText(button.dataset.url);
                button.classList.add("is-copied");
                button.setAttribute("aria-label", "Linku u kopjua");
                setTimeout(() => {
                    button.classList.remove("is-copied");
                    button.setAttribute("aria-label", originalLabel);
                }, 2000);
            } catch {
                window.prompt("Kopjo linkun:", button.dataset.url);
            }
        });
    });
})();
