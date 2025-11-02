$(document).ready(function () {

    // ============ LOGIC "NHÃN NỔI" (FLOATING LABEL) ============
    // (Phần này không cần thay đổi vì nó hoạt động chung cho cả trang)
    //function initializeFloatingLabels() {
    //    $('.input-group .form-control').each(function () {
    //        //const input = $(this);
    //        //const inputGroup = input.closest('.input-group');
    //        //if (input.val() && input.val().trim() !== '') {
    //        //    inputGroup.addClass('is-filled');
    //        //} else {
    //        //    inputGroup.removeClass('is-filled');
    //        //}
    //    });
    //}
    //$('body').on('focus', '.input-group .form-control', function () {
    //    //$(this).closest('.input-group').addClass('is-focused');
    //});
    //$('body').on('blur', '.input-group .form-control', function () {
    //    //const input = $(this);
    //    //const inputGroup = input.closest('.input-group');
    //    //inputGroup.removeClass('is-focused');
    //    //if (input.val() && input.val().trim() !== '') {
    //    //    inputGroup.addClass('is-filled');
    //    //} else {
    //    //    inputGroup.removeClass('is-filled');
    //    //}
    //});
    //// Chạy lần đầu
    //initializeFloatingLabels();

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
    // Sửa lại logic click phân trang
    $('body').on('click', '.account-table-container .pagination a', function (e) {
        e.preventDefault(); // Ngăn hành vi mặc định của thẻ <a>

        // Tìm thẻ <li> cha gần nhất của thẻ <a> được click
        var li = $(this).closest('li');

        // Nếu thẻ <li> bị vô hiệu hóa (disabled) hoặc đang active, thì không làm gì cả
        if (li.hasClass('disabled') || li.hasClass('active')) {
            return;
        }

        // Đọc số trang từ thuộc tính 'page' của thẻ <li>
        var page = li.attr('page');

        // Nếu 'page' tồn tại (luôn luôn là số), gọi hàm loadPage
        if (page) {
            loadPage(parseInt(page));
        }
    });
    // THAY ĐỔI 4: Gán sự kiện 'click' cho nút Reset mới
    $('#resetAccountButton').on('click', function () {
        $('#searchAccountInput').val('');
        $('#roleFilter').val('');
        $('#positionFilter').val('');
        initializeFloatingLabels();
        loadPage(1);
    });

    // Hàm chính để tải dữ liệu thông qua AJAX
    function loadPage(pageIndex) {
        var searchQuery = $('#searchAccountInput').val();
        var role = $('#roleFilter').val();
        var position = $('#positionFilter').val();

        var data = {
            pageIndex: pageIndex,
            searchQuery: searchQuery,
            role: role,
            position: position // <-- GỬI LÊN SERVER
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

    // HÀM HELPER: Tải và điền danh sách Position
    // (currentPositionValue dùng cho form "Edit" để chọn lại giá trị cũ)
    function updatePositionDropdown(role, positionDropdown, currentPositionValue) {

        // 1. Vô hiệu hóa dropdown "Position" trong khi tải
        positionDropdown.prop('disabled', true);

        // 2. Gọi AJAX (tái sử dụng Action 'GetPositionsByRole' của bạn)
        $.ajax({
            url: '/Admin/EmployeeManagement/GetPositionsByRole',
            type: 'GET',
            data: { role: role },
            success: function (positions) {

                // 3. Xóa các <option> cũ (trừ option "Select Position...")
                positionDropdown.find('option:not([value=""])').remove();

                // 4. Lặp qua JSON và thêm các <option> mới
                $.each(positions, function (i, position) {
                    positionDropdown.append($('<option>', {
                        value: position,
                        text: position
                    }));
                });

                // 5. CHỌN LẠI GIÁ TRỊ HIỆN TẠI (nếu có)
                if (currentPositionValue) {
                    positionDropdown.val(currentPositionValue);
                }

                // 6. Kích hoạt lại dropdown
                positionDropdown.prop('disabled', false);
            },
            error: function () {
                alert('Could not load positions for editing.');
                positionDropdown.prop('disabled', false);
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
            // 1. Tải nội dung HTML (modal) vào
            modalContent.html(response);

            // 2. Tìm dropdown Role và Position BÊN TRONG MODAL
            var roleDropdown = $('#detailsModal #Role');
            var positionDropdown = $('#detailsModal #Position');

            // 3. Lấy giá trị HIỆN TẠI của chúng (đã được tải từ ViewModel)
            var currentRole = roleDropdown.val();
            var currentPosition = positionDropdown.val(); // Đây là giá trị ta muốn giữ

            // 4. Gọi hàm helper để lọc danh sách, 
            //    nhưng vẫn giữ nguyên giá trị 'currentPosition' đã chọn
            if (currentRole) {
                updatePositionDropdown(currentRole, positionDropdown, currentPosition);
            }

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

    // Lắng nghe sự kiện 'change' trên dropdown #Role (BÊN TRONG MODAL)
    // Dùng "event delegation" vì modal (#detailsModal) đã tồn tại
    $('#detailsModal').on('change', '#Role', function () {
        var selectedRole = $(this).val();
        // Tìm dropdown Position bên trong modal
        var positionDropdown = $('#detailsModal #Position');

        // Gọi hàm helper, không cần giữ giá trị cũ (vì người dùng đang chọn mới)
        updatePositionDropdown(selectedRole, positionDropdown, null);
    });

    // Sự kiện hiển thị modal xác nhận xóa
    $('body').on('click', '.delete-account-btn', function (e) {
        e.preventDefault(); // Ngăn hành vi mặc định của thẻ <a>

        // Lấy thông tin tài khoản từ thuộc tính data-*
        const accountId = $(this).data('id'); // Đây là Username
        const accountName = $(this).data('name');
        const deleteUrl = `/Admin/EmployeeManagement/Delete/${accountId}`; // Xây dựng URL cho action POST

        // Target đến đúng vùng nội dung của modal xác nhận
        const modalContent = $('#confirmationModal .modal-content');

        // Tạo mã HTML cho hộp thoại xác nhận
        const confirmationHtml = `
        <div class="modal-header">
            <h5 class="modal-title" id="confirmationModalLabel">Xác nhận Xóa</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
        </div>
        <div class="modal-body">
            <p>Bạn có chắc muốn xóa tài khoản: <strong>${accountName}</strong>?</p>
            <p class="text-danger"><small>Hành động này không thể hoàn tác.</small></p>
        </div>
        <div class="modal-footer">
            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
            
            <form id="deleteConfirmForm" action="${deleteUrl}" method="post" class="d-inline">
                <button type="submit" class="btn btn-danger">Xóa Tài Khoản</button>
            </form>
        </div>
    `;

        // Đổ nội dung HTML vào modal và hiển thị modal
        modalContent.html(confirmationHtml);
        $('#confirmationModal').modal('show');
    });


    // === AJAX ĐỂ SUBMIT FORM XÓA ===
    // Sử dụng event delegation trên modal đã tồn tại ở view chính
    $('#confirmationModal').on('submit', '#deleteConfirmForm', function (e) {
        e.preventDefault(); // Ngăn form submit theo cách thông thường

        const form = $(this);
        const url = form.attr('action'); // Lấy URL từ thuộc tính action của form

        // Lấy Anti-Forgery Token từ form ẩn trên trang chính
        const antiForgeryToken = $('#antiForgeryForm input[name=__RequestVerificationToken]').val();

        // Gửi yêu cầu AJAX POST
        $.ajax({
            type: 'POST',
            url: url,
            // Chỉ cần gửi token, ID đã nằm trong URL rồi
            data: { __RequestVerificationToken: antiForgeryToken },
            success: function (response) {
                $('#confirmationModal').modal('hide'); // Đóng modal xác nhận

                if (response.success) {
                    alert(response.message); // Hiển thị thông báo thành công

                    // Tải lại trang hiện tại của bảng để cập nhật sau khi xóa
                    const currentPage = parseInt($('.account-table-container .pagination .active').text()) || 1;
                    loadPage(currentPage);
                } else {
                    alert(response.message); // Hiển thị thông báo lỗi từ controller
                }
            },
            error: function () {
                // Xử lý lỗi mạng hoặc lỗi server
                alert('Đã có lỗi xảy ra trong quá trình xóa tài khoản.');
                $('#confirmationModal').modal('hide');
            }
        });
    });

    // =======================================================
    // LOGIC CHO DROPDOWN ĐỘNG (CASCADING DROPDOWNS)
    // =======================================================
    $('#roleFilter').on('change', function () {
        var selectedRole = $(this).val(); // Lấy role (Admin, Employee, "")
        var positionDropdown = $('#positionFilter');

        // 1. Vô hiệu hóa dropdown "Position" trong khi đang tải dữ liệu
        positionDropdown.prop('disabled', true);

        // 2. Gọi AJAX đến Action mới ta vừa tạo
        $.ajax({
            url: '/Admin/EmployeeManagement/GetPositionsByRole',
            type: 'GET',
            data: { role: selectedRole },
            success: function (positions) {

                // 3. Xóa tất cả các <option> cũ (trừ option "All Positions")
                positionDropdown.find('option:not(:first)').remove();

                // 4. Lặp qua danh sách JSON trả về và thêm các <option> mới
                $.each(positions, function (i, position) {
                    positionDropdown.append($('<option>', {
                        value: position,
                        text: position
                    }));
                });

                // 5. Kích hoạt lại dropdown
                positionDropdown.prop('disabled', false);

                // 6. Cập nhật lại giao diện floating label (nếu bạn dùng)
                initializeFloatingLabels();
            },
            error: function () {
                alert('Có lỗi xảy ra khi tải danh sách vị trí.');
                positionDropdown.prop('disabled', false);
            }
        });
    });
});