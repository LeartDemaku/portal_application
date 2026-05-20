(() => {
    const header = document.querySelector('[data-site-header]');

    if (!header) {
        return;
    }

    const syncHeader = () => {
        header.classList.toggle('is-scrolled', window.scrollY > 12);
    };

    syncHeader();
    window.addEventListener('scroll', syncHeader, { passive: true });
})();
