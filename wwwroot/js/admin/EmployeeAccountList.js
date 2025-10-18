$(document).ready(function () {

    // ============ LOGIC "NHÃN NỔI" (FLOATING LABEL) ============
    // (Phần này không cần thay đổi vì nó hoạt động chung cho cả trang)
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
    // Chạy lần đầu
    initializeFloatingLabels();

    // ============ LOGIC LỌC, PHÂN TRANG, VÀ RESET ============

    // Tải dữ liệu ban đầu cho trang 1
    loadPage(1);

    // THAY ĐỔI 1: Gán sự kiện 'click' cho nút Filter mới
    $('#filterAccountButton').on('click', function () {
        loadPage(1);
    });

    // THAY ĐỔI 2: Gán sự kiện nhấn phím 'Enter' trong ô tìm kiếm mới
    $('#searchAccountInput').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            loadPage(1);
        }
    });

    // THAY ĐỔI 3: Gán sự kiện 'click' cho các nút phân trang trong container mới
    $('body').on('click', '.account-table-container .pagination a', function (e) {
        e.preventDefault();
        // Logic phân trang giữ nguyên, chỉ đổi selector cha
        var page = $(this).attr('href').split('pageIndex=')[1];
        if (page) {
            loadPage(parseInt(page));
        }
    });

    // THAY ĐỔI 4: Gán sự kiện 'click' cho nút Reset mới
    $('#resetAccountButton').on('click', function () {
        $('#searchAccountInput').val('');
        $('#roleFilter').val('');
        initializeFloatingLabels();
        loadPage(1);
    });

    // Hàm chính để tải dữ liệu thông qua AJAX
    function loadPage(pageIndex) {
        // THAY ĐỔI 5: Lấy giá trị từ các bộ lọc mới
        var searchQuery = $('#searchAccountInput').val();
        var role = $('#roleFilter').val();

        var data = {
            pageIndex: pageIndex,
            searchQuery: searchQuery,
            role: role // Đổi 'gender' và 'nationality' thành 'role'
        };

        $.ajax({
            // THAY ĐỔI 6: URL trỏ đến EmployeeManagementController
            url: '/Admin/EmployeeManagement/AccountTableAndPagination',
            type: 'GET',
            data: data,
            success: function (response) {
                // THAY ĐỔI 7: Cập nhật nội dung của container mới
                $('.account-table-container').html(response);
            },
            error: function () {
                alert('Có lỗi xảy ra khi tải dữ liệu tài khoản!');
            }
        });
    }

    // =======================================================
    // LOGIC CHO CÁC ACTION (VIEW, EDIT, DELETE)
    // (Phần này không cần thay đổi vì nó được thiết kế để tái sử dụng)
    // =======================================================

    // Sự kiện View details
    $('body').on('click', '.view-details-btn', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        // Target vào .modal-content để nhất quán với logic Edit
        var modalContent = $('#detailsModal .modal-content');

        // Hiển thị một spinner đơn giản
        modalContent.html('<div class="modal-body text-center p-5"><div class="spinner-border text-primary"></div></div>');

        $.get(url, function (response) {
            modalContent.html(response);
        }).fail(function () {
            modalContent.html('<div class="modal-body"><p>Could not load the Details!</p></div>');
        });
    });

    // Sự kiện Edit (GET)
    $('body').on('click', '.edit-details-btn', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        var modalContent = $('#detailsModal .modal-content');

        var loadingSpinner = `
        <div class="modal-body text-center p-5">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            <p class="mt-2 text-muted">Đang tải dữ liệu...</p>
        </div>`;
        modalContent.html(loadingSpinner);

        $.get(url, function (response) {
            modalContent.html(response);
            initializeFloatingLabels();
        }).fail(function () {
            var errorAlert = `<div class="modal-body"><div class="alert alert-danger">Không thể tải dữ liệu.</div></div>`;
            modalContent.html(errorAlert);
        });
    });

    // Sự kiện Edit (POST)
    $('#detailsModal').on('submit', '#editAccountForm', function (e) {
        e.preventDefault();
        var form = $(this);
        var url = form.attr('action');
        var formData = form.serialize();//lấy dữ liệu từ form

        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (response) {
                if (response.success) {
                    $('#detailsModal').modal('hide');
                    alert(response.message);

                    // Tải lại bảng để thấy thay đổi
                    const currentPage = parseInt($('.account-table-container .pagination .active').text()) || 1;
                    loadPage(currentPage);
                } else {
                    // Nếu validation fail, cập nhật lại modal với form và lỗi
                    $('#detailsModal .modal-content').html(response);
                    alert(response.message);
                    $('#detailsModal').modal('hide');
                }
            },
            error: function () {
                alert('An error occurred while submitting the form.');
            }
        });
    });

    // Sự kiện hiển thị modal xác nhận xóa
    $('body').on('click', '.delete-account-btn', function (e) { // Đổi selector nếu bạn dùng class khác
        e.preventDefault();

        const customerId = $(this).data('id');
        const customerName = $(this).data('name');
        const url = `/Admin/Customer/Delete/${customerId}`; // Xây dựng URL cho action POST

        const modalContent = $('#detailsModal .modal-content');

        // Tạo HTML cho modal xác nhận
        const confirmationHtml = `
        <div class="modal-header">
            <h5 class="modal-title">Confirm Deletion</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
            <p>Are you sure you want to delete customer: <strong>${customerName}</strong>?</p>
            <p class="text-danger">This action cannot be undone.</p>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
            <form id="deleteConfirmForm" action="${url}" method="post" class="d-inline">
                <button type="submit" class="btn btn-danger">Delete</button>
            </form>
        </div>
    `;

        // Cập nhật và hiển thị modal
        modalContent.html(confirmationHtml);
        $('#detailsModal').modal('show');
    });

    // Sự kiện submit form xóa
    $('#confirmationModal').on('submit', '#deleteConfirmForm', function (e) {
        e.preventDefault();

        const form = $(this);
        const url = form.attr('action');

        // Lấy Anti-Forgery Token từ form ẩn và gộp vào dữ liệu gửi đi
        const antiForgeryToken = $('#antiForgeryForm input[name=__RequestVerificationToken]').val();
        const formData = form.serialize() + '&__RequestVerificationToken=' + antiForgeryToken;

        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (response) {
                if (response.success) {
                    $('#detailsModal').modal('hide');
                    alert(response.message); // Hoặc dùng Toastr

                    // Tải lại bảng dữ liệu ở trang hiện tại
                    const currentPage = parseInt($('ul.pagination li.active').attr('page')) || 1;
                    loadPage(currentPage);
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert('An error occurred while trying to delete.');
            }
        });
    });
});