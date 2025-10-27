document.addEventListener('DOMContentLoaded', function () {
    // Xử lý sidebar toggle cho mobile
    const sidenavToggler = document.getElementById('sidenavToggler');
    const sidenav = document.getElementById('sidenav-main');
    const sidenavOverlay = document.getElementById('sidenavOverlay');
    const iconSidenav = document.getElementById('iconSidenav');

    // Hàm toggle sidebar
    function toggleSidebar(e) {
        e.preventDefault();
        e.stopPropagation();
        sidenav.classList.toggle('show');
        sidenavOverlay.classList.toggle('show');
    }

    // Hàm đóng sidebar
    function closeSidebar() {
        sidenav.classList.remove('show');
        sidenavOverlay.classList.remove('show');
    }

    // Mở/đóng sidebar khi click nút hamburger
    if (sidenavToggler) {
        sidenavToggler.addEventListener('click', toggleSidebar);
    }

    // Đóng sidebar khi click vào overlay
    if (sidenavOverlay) {
        sidenavOverlay.addEventListener('click', closeSidebar);
    }

    // Đóng sidebar khi click vào nút X trong sidebar
    if (iconSidenav) {
        iconSidenav.addEventListener('click', closeSidebar);
    }

    // Đóng sidebar khi click vào link trong sidebar (trên mobile)
    if (sidenav) {
        const sidenavLinks = sidenav.querySelectorAll('.nav-link:not([data-bs-toggle="collapse"])');
        sidenavLinks.forEach(function (link) {
            link.addEventListener('click', function () {
                if (window.innerWidth < 1200) {
                    closeSidebar();
                }
            });
        });
    }
});