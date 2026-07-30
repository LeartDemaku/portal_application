(() => {
    const header = document.querySelector('[data-site-header]');
    if (header) {
        const sync = () => header.classList.toggle('is-scrolled', window.scrollY > 12);
        sync();
        window.addEventListener('scroll', sync, { passive: true });
    }

    // Toggle hamburger icon animation on Bootstrap collapse events
    const toggler = document.querySelector('.portal-toggler');
    const collapseEl = document.getElementById('portalNavbarNav');
    if (toggler && collapseEl) {
        collapseEl.addEventListener('show.bs.collapse', () => toggler.classList.add('is-open'));
        collapseEl.addEventListener('hide.bs.collapse', () => toggler.classList.remove('is-open'));
    }

    // Admin sidebar toggle
    const sidebarToggle = document.getElementById('adminSidebarToggle');
    const sidebar = document.getElementById('adminSidebar');
    const overlay = document.getElementById('adminOverlay');
    if (sidebarToggle && sidebar) {
        const toggleSidebar = () => {
            sidebar.classList.toggle('is-open');
            if (overlay) overlay.classList.toggle('is-visible');
        };
        sidebarToggle.addEventListener('click', toggleSidebar);
        if (overlay) overlay.addEventListener('click', toggleSidebar);
    }
})();
