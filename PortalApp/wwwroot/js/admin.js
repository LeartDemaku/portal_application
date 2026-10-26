(() => {
    const root = document.documentElement;
    const body = document.body;
    const themeKey = "zy-admin-theme";
    const themeMeta = document.querySelector('meta[name="theme-color"]');

    const applyTheme = (theme) => {
        root.setAttribute("data-admin-theme", theme);
        if (themeMeta) {
            themeMeta.setAttribute("content", theme === "dark" ? "#07111f" : "#f4f7fc");
        }
        document.querySelectorAll("[data-theme-toggle]").forEach((button) => {
            button.setAttribute("aria-pressed", theme === "dark" ? "true" : "false");
            button.setAttribute("aria-label", theme === "dark" ? "Aktivizo temën e ndritshme" : "Aktivizo temën e errët");
        });
    };

    applyTheme(root.getAttribute("data-admin-theme") || "light");

    document.querySelectorAll("[data-theme-toggle]").forEach((button) => {
        button.addEventListener("click", () => {
            const nextTheme = root.getAttribute("data-admin-theme") === "dark" ? "light" : "dark";
            applyTheme(nextTheme);
            try {
                localStorage.setItem(themeKey, nextTheme);
            } catch (error) {
                return;
            }
        });
    });

    const sidebar = document.getElementById("adminSidebar");
    const overlay = document.getElementById("adminOverlay");
    const sidebarToggle = document.getElementById("adminSidebarToggle");
    const sidebarClose = document.getElementById("adminSidebarClose");

    const closeSidebar = () => {
        if (!sidebar) {
            return;
        }
        sidebar.classList.remove("is-open");
        if (overlay) {
            overlay.classList.remove("is-visible");
        }
        body.classList.remove("admin-nav-open");
        if (sidebarToggle) {
            sidebarToggle.setAttribute("aria-expanded", "false");
        }
    };

    const openSidebar = () => {
        if (!sidebar) {
            return;
        }
        sidebar.classList.add("is-open");
        if (overlay) {
            overlay.classList.add("is-visible");
        }
        body.classList.add("admin-nav-open");
        if (sidebarToggle) {
            sidebarToggle.setAttribute("aria-expanded", "true");
        }
    };

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener("click", () => {
            if (sidebar.classList.contains("is-open")) {
                closeSidebar();
            } else {
                openSidebar();
            }
        });
    }

    if (overlay) {
        overlay.addEventListener("click", closeSidebar);
    }

    if (sidebarClose) {
        sidebarClose.addEventListener("click", closeSidebar);
    }

    if (sidebar) {
        sidebar.querySelectorAll(".admin-nav a, .admin-sidebar-cta").forEach((link) => {
            link.addEventListener("click", closeSidebar);
        });
    }

    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeSidebar();
        }
    });

    const desktopQuery = window.matchMedia("(min-width: 1181px)");
    const handleDesktopChange = (event) => {
        if (event.matches) {
            closeSidebar();
        }
    };

    if (desktopQuery.addEventListener) {
        desktopQuery.addEventListener("change", handleDesktopChange);
    } else if (desktopQuery.addListener) {
        desktopQuery.addListener(handleDesktopChange);
    }

    document.querySelectorAll("[data-admin-alert-close]").forEach((button) => {
        button.addEventListener("click", () => {
            const alert = button.closest(".admin-alert");
            if (alert) {
                alert.remove();
            }
        });
    });

    document.querySelectorAll("[data-admin-search]").forEach((input) => {
        const targetSelector = input.getAttribute("data-admin-search");
        const target = targetSelector ? document.querySelector(targetSelector) : null;
        if (!target) {
            return;
        }

        const countSelector = input.getAttribute("data-result-count");
        const countTarget = countSelector ? document.querySelector(countSelector) : null;
        const filters = targetSelector ? document.querySelectorAll(`[data-admin-filter="${targetSelector}"]`) : [];
        const isTable = target.matches("table");
        const items = Array.from(
            isTable
                ? target.querySelectorAll("tbody tr:not(.admin-empty-row):not(.admin-search-empty)")
                : target.querySelectorAll("[data-search-item]")
        );
        const noResultsRow = target.querySelector("[data-search-empty]");
        const noResultsInline = target.querySelector("[data-search-empty-inline]");
        let activeFilter = "all";

        const applySearch = () => {
            const query = input.value.trim().toLocaleLowerCase("sq");
            let visible = 0;

            items.forEach((item) => {
                const text = (item.getAttribute("data-search-text") || item.textContent || "").toLocaleLowerCase("sq");
                const status = item.getAttribute("data-status") || "all";
                const matchesQuery = !query || text.includes(query);
                const matchesFilter = activeFilter === "all" || status === activeFilter;
                const shouldShow = matchesQuery && matchesFilter;
                item.classList.toggle("is-filtered-out", !shouldShow);
                if (shouldShow) {
                    visible += 1;
                }
            });

            if (countTarget) {
                countTarget.textContent = `${visible} nga ${items.length}`;
            }

            if (noResultsRow) {
                noResultsRow.classList.toggle("is-visible", visible === 0 && items.length > 0);
            }

            if (noResultsInline) {
                noResultsInline.classList.toggle("is-visible", visible === 0 && items.length > 0);
            }
        };

        filters.forEach((button) => {
            button.addEventListener("click", () => {
                activeFilter = button.getAttribute("data-value") || "all";
                filters.forEach((filterButton) => {
                    const isActive = filterButton === button;
                    filterButton.classList.toggle("active", isActive);
                    filterButton.setAttribute("aria-pressed", isActive ? "true" : "false");
                });
                applySearch();
            });
        });

        input.addEventListener("input", applySearch);
        input.addEventListener("search", applySearch);

        const searchForm = input.closest("form");
        if (searchForm) {
            searchForm.addEventListener("submit", (event) => {
                event.preventDefault();
            });
        }

        applySearch();
    });

    document.addEventListener("keydown", (event) => {
        if (event.key !== "/" || event.ctrlKey || event.metaKey || event.altKey) {
            return;
        }
        const activeElement = document.activeElement;
        const tagName = activeElement ? activeElement.tagName : "";
        if (tagName === "INPUT" || tagName === "TEXTAREA" || tagName === "SELECT" || (activeElement && activeElement.isContentEditable)) {
            return;
        }
        const searchInput = document.querySelector("[data-admin-search]");
        if (searchInput) {
            event.preventDefault();
            searchInput.focus();
        }
    });

    document.querySelectorAll("[data-password-toggle]").forEach((button) => {
        const selector = button.getAttribute("data-password-toggle");
        const input = selector ? document.querySelector(selector) : null;
        if (!input) {
            return;
        }

        button.addEventListener("click", () => {
            const isVisible = input.getAttribute("type") === "text";
            input.setAttribute("type", isVisible ? "password" : "text");
            button.setAttribute("aria-pressed", isVisible ? "false" : "true");
            button.setAttribute("aria-label", isVisible ? "Shfaq fjalëkalimin" : "Fshih fjalëkalimin");
        });
    });

    document.querySelectorAll("[data-image-input]").forEach((input) => {
        const previewSelector = input.getAttribute("data-image-preview");
        const preview = previewSelector ? document.querySelector(previewSelector) : null;
        const nameSelector = input.getAttribute("data-upload-name");
        const nameTarget = nameSelector ? document.querySelector(nameSelector) : null;

        if (!preview) {
            return;
        }

        input.addEventListener("change", () => {
            const file = input.files && input.files[0];
            if (!file) {
                preview.hidden = true;
                preview.removeAttribute("src");
                if (nameTarget) {
                    nameTarget.textContent = "Zgjidh një imazh";
                }
                return;
            }

            if (preview.dataset.objectUrl) {
                URL.revokeObjectURL(preview.dataset.objectUrl);
            }

            const objectUrl = URL.createObjectURL(file);
            preview.dataset.objectUrl = objectUrl;
            preview.src = objectUrl;
            preview.hidden = false;

            if (nameTarget) {
                nameTarget.textContent = file.name;
            }
        });
    });

    document.querySelectorAll("[data-fill-target]").forEach((button) => {
        button.addEventListener("click", () => {
            const target = document.getElementById(button.getAttribute("data-fill-target"));
            if (!target) {
                return;
            }
            target.value = button.getAttribute("data-fill-value") || button.textContent.trim();
            target.dispatchEvent(new Event("input", { bubbles: true }));
            target.focus();
        });
    });

    const editor = document.getElementById("editor-container");
    const editorStats = document.querySelector("[data-editor-stats]");
    if (editor && editorStats) {
        let statsTimer = 0;
        const updateStats = () => {
            const editorBody = editor.querySelector(".ql-editor");
            const text = editorBody ? editorBody.innerText.trim() : "";
            const words = text ? text.split(/\s+/).filter(Boolean).length : 0;
            const minutes = words ? Math.max(1, Math.ceil(words / 200)) : 0;
            editorStats.textContent = `${words} fjalë · ${minutes} min lexim`;
        };
        const scheduleStats = () => {
            window.clearTimeout(statsTimer);
            statsTimer = window.setTimeout(updateStats, 120);
        };

        editor.addEventListener("input", scheduleStats, true);
        editor.addEventListener("keyup", scheduleStats);

        const observer = new MutationObserver(scheduleStats);
        observer.observe(editor, { childList: true, subtree: true, characterData: true });
        window.setTimeout(updateStats, 80);
    }

    document.querySelectorAll("form[data-admin-form]").forEach((form) => {
        form.addEventListener("submit", (event) => {
            if (form.dataset.submitting === "true") {
                event.preventDefault();
                return;
            }
            form.dataset.submitting = "true";
            const submitButton = form.querySelector('button[type="submit"]');
            if (submitButton) {
                submitButton.classList.add("is-loading");
                submitButton.disabled = true;
            }
        });
    });
})();
