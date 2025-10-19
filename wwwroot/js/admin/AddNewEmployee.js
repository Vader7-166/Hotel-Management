$(document).ready(function () {
    // ============ LOGIC "NHÃN NỔI" (FLOATING LABEL) ============

    // Hàm này sẽ kiểm tra tất cả các input và select, thêm class 'is-filled' nếu chúng có giá trị
    function initializeFloatingLabels() {
        $('.input-group .form-control').each(function () {
            const input = $(this);
            const inputGroup = input.closest('.input-group');
            if (input.val() && input.val().trim() !== '') {
                inputGroup.addClass('is-filled');
            } else {
                inputGroup.removeClass('is-filled');
            }
        });
    }

    // Gắn sự kiện vào 'body' để nó hoạt động với cả các element được tạo sau này bằng AJAX
    $('body').on('focus', '.input-group .form-control', function () {
        $(this).closest('.input-group').addClass('is-focused');
    });

    $('body').on('blur', '.input-group .form-control', function () {
        const input = $(this);
        const inputGroup = input.closest('.input-group');
        inputGroup.removeClass('is-focused');
        if (input.val() && input.val().trim() !== '') {
            inputGroup.addClass('is-filled');
        } else {
            inputGroup.removeClass('is-filled');
        }
    });

    // Chạy lần đầu khi trang tải xong
    initializeFloatingLabels();
});