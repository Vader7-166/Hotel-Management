document.addEventListener('DOMContentLoaded', function () {
    // Xử lý sidebar toggle cho mobile
    const sidenavToggler = document.getElementById('sidenavToggler');
    const sidenav = document.getElementById('sidenav-main');
    const sidenavOverlay = document.getElementById('sidenavOverlay');
    const iconSidenav = document.getElementById('iconSidenav');

    // Debug
    console.log('Sidebar elements:', {
        toggler: sidenavToggler,
        sidenav: sidenav,
        overlay: sidenavOverlay,
        iconClose: iconSidenav
    });

    // Hàm toggle sidebar
    function toggleSidebar(e) {
        e.preventDefault();
        e.stopPropagation();
        console.log('Toggle sidebar clicked');
        sidenav.classList.toggle('show');
        sidenavOverlay.classList.toggle('show');
        console.log('Sidebar classes:', sidenav.classList);
    }

    // Hàm đóng sidebar
    function closeSidebar() {
        console.log('Close sidebar');
        sidenav.classList.remove('show');
        sidenavOverlay.classList.remove('show');
    }

    // Mở/đóng sidebar khi click nút hamburger
    if (sidenavToggler) {
        sidenavToggler.addEventListener('click', toggleSidebar);
        console.log('Hamburger button listener added');
    } else {
        console.error('Hamburger button not found!');
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