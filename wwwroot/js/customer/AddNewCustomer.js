document.addEventListener('DOMContentLoaded', function () {

    // --- Floating Label for Inputs ---
    // Tìm tất cả các nhóm input có outline trên trang
    const inputGroups = document.querySelectorAll('.input-group.input-group-outline');

    // Lặp qua từng nhóm để gán sự kiện
    inputGroups.forEach(function (group) {
        const input = group.querySelector('input, textarea');
        const label = group.querySelector('.form-label');

        if (input) {
            // Sự kiện khi click vào ô input (focus)
            input.addEventListener('focus', function () {
                group.classList.add('is-focused');
            });

            // Sự kiện khi click ra ngoài ô input (blur)
            input.addEventListener('blur', function () {
                // Luôn xóa class is-focused khi blur
                group.classList.remove('is-focused');

                // Kiểm tra xem input có dữ liệu hay không
                if (input.value !== '') {
                    // Nếu có dữ liệu, thêm class is-filled để label không bị rơi xuống
                    group.classList.add('is-filled');
                } else {
                    // Nếu không có dữ liệu, xóa class is-filled
                    group.classList.remove('is-filled');
                }
            });

            if (input.value !== '') {
                group.classList.add('is-filled');
            }
        }
    });
});
