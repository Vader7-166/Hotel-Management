$(document).ready(function () {

    // ============ LOGIC LỌC, PHÂN TRANG, VÀ RESET ============

    loadPage(1);


    $('#filterAccountButton').on('click', function () {
        loadPage(1);
    });


    $('#searchAccountInput').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            loadPage(1);
        }
    });

    //  sự kiện 'click' cho các nút phân trang
    // Sửa lại logic click phân trang
    $('body').on('click', '.account-table-container .pagination a', function (e) {
        e.preventDefault(); // Ngăn hành vi mặc định của thẻ <a>

        // Tìm thẻ <li> cha gần nhất của thẻ <a> được click
        var li = $(this).closest('li');

        if (li.hasClass('disabled') || li.hasClass('active')) {
            return;
        }

        var page = li.attr('page');


        if (page) {
            loadPage(parseInt(page));
        }
    });


    $('#resetAccountButton').on('click', function () {
        $('#searchAccountInput').val('');
        $('#roleFilter').val('');
        $('#positionFilter').val('');
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
            position: position 
        };

        $.ajax({
            url: '/Admin/EmployeeManagement/AccountTableAndPagination',
            type: 'GET',
            data: data,
            success: function (response) {
                $('.account-table-container').html(response);
            },
            error: function () {
                alert('Có lỗi xảy ra khi tải dữ liệu tài khoản!');
            }
        });
    }

    //Hàm lọc Position theo Role

    function updatePositionDropdown(role, positionDropdown, currentPositionValue) {

        // 1. Vô hiệu hóa dropdown "Position" trong khi tải
        positionDropdown.prop('disabled', true);

        // 2. Gọi AJAX đến Action 'GetPositionsByRole'
        $.ajax({
            url: '/Admin/EmployeeManagement/GetPositionsByRole',
            type: 'GET',
            data: { role: role },
            success: function (positions) {

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
    // =======================================================

    // Sự kiện View details
    $('body').on('click', '.view-details-btn', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
  
        var modalContent = $('#detailsModal .modal-content');

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

            //Tìm dropdown Role và Position BÊN TRONG MODAL
            var roleDropdown = $('#detailsModal #Role');
            var positionDropdown = $('#detailsModal #Position');

            // Lấy giá trị HIỆN TẠI của chúng 
            var currentRole = roleDropdown.val();
            var currentPosition = positionDropdown.data('current-position');

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
        var formData = form.serialize();

        $.ajax({
            type: 'POST',
            url: url,
            data: formData,
            success: function (response) {
                if (response.success) {
                    $('#detailsModal').modal('hide');
                    alert(response.message);

                    const currentPage = parseInt($('.account-table-container .pagination .active').text()) || 1;
                    loadPage(currentPage);
                } else {
                    $('#detailsModal .modal-content').html(response);
                    var roleDropdown = $('#detailsModal #Role');
                    var positionDropdown = $('#detailsModal #Position');

                    // 3. Lấy Role và Position mà người dùng đã chọn (server trả về)
                    var currentRole = roleDropdown.val();
                    var currentPosition = positionDropdown.val();

                    // 4. Gọi hàm helper để lọc danh sách
                    if (currentRole) {

                        updatePositionDropdown(currentRole, positionDropdown, currentPosition);
                    }
                }
            },
            error: function () {
                alert('An error occurred while submitting the form.');
            }
        });
    });

    // Lắng nghe sự kiện 'change' trên dropdown #Role (BÊN TRONG MODAL)
    $('#detailsModal').on('change', '#Role', function () {
        var selectedRole = $(this).val();
        var positionDropdown = $('#detailsModal #Position');
        // Gọi hàm helper, không cần giữ giá trị cũ (vì người dùng đang chọn mới)
        updatePositionDropdown(selectedRole, positionDropdown, null);
    });

    // Sự kiện hiển thị modal xác nhận xóa
    $('body').on('click', '.delete-account-btn', function (e) {
        e.preventDefault();

        const accountId = $(this).data('id'); // Đây là Username
        const accountName = $(this).data('name');
        const deleteUrl = `/Admin/EmployeeManagement/Delete/${accountId}`; 

        // Target đến đúng vùng nội dung của modal xác nhận
        const modalContent = $('#confirmationModal .modal-content');

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

        modalContent.html(confirmationHtml);
        $('#confirmationModal').modal('show');
    });


    // === AJAX ĐỂ SUBMIT FORM XÓA ===
    $('#confirmationModal').on('submit', '#deleteConfirmForm', function (e) {
        e.preventDefault();

        const form = $(this);
        const url = form.attr('action'); 

        // Lấy Anti-Forgery Token từ form ẩn trên trang chính
        const antiForgeryToken = $('#antiForgeryForm input[name=__RequestVerificationToken]').val();

        // Gửi yêu cầu AJAX POST
        $.ajax({
            type: 'POST',
            url: url,
            data: { __RequestVerificationToken: antiForgeryToken },
            success: function (response) {
                $('#confirmationModal').modal('hide');

                if (response.success) {
                    alert(response.message);

                    // Tải lại trang hiện tại của bảng để cập nhật sau khi xóa
                    const currentPage = parseInt($('.account-table-container .pagination .active').text()) || 1;
                    loadPage(currentPage);
                } else {
                    alert(response.message);
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

        // 2. Gọi AJAX đến Action GetPositionsByRole
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

                // 6. Cập nhật lại giao diện floating label
                initializeFloatingLabels();
            },
            error: function () {
                alert('Có lỗi xảy ra khi tải danh sách vị trí.');
                positionDropdown.prop('disabled', false);
            }
        });
    });
});