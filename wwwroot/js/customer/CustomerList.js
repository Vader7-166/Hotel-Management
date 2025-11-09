$(document).ready(function () {

    // ============ LOGIC "NHÃN NỔI" (FLOATING LABEL) ============

    // Hàm này sẽ kiểm tra tất cả các input và select, thêm class 'is-filled' nếu chúng có giá trị
    function initializeFloatingLabels() {
        $('.input-group .form-control').each(function () {
            const input = $(this);
            const inputGroup = input.closest('.input-group');

            // Nếu input có giá trị (và không phải là giá trị rỗng của dropdown), thêm class 'is-filled'
            if (input.val() && input.val().trim() !== '') {
                inputGroup.addClass('is-filled');
            } else {
                inputGroup.removeClass('is-filled');
            }
        });
    }

    // SỬ DỤNG EVENT DELEGATION cho các sự kiện focus và blur
    // Gắn sự kiện vào 'body' để nó hoạt động với cả các element được tạo sau này bằng AJAX

    // Khi một input hoặc select được focus
    $('body').on('focus', '.input-group .form-control', function () {
        $(this).closest('.input-group').addClass('is-focused');
    });

    // Khi một input hoặc select bị mất focus (blur)
    $('body').on('blur', '.input-group .form-control', function () {
        const input = $(this);
        const inputGroup = input.closest('.input-group');

        inputGroup.removeClass('is-focused');
        // Kiểm tra lại trạng thái 'is-filled'
        if (input.val() && input.val().trim() !== '') {
            inputGroup.addClass('is-filled');
        } else {
            inputGroup.removeClass('is-filled');
        }
    });

    // Chạy lần đầu khi trang tải xong cho các input có sẵn (như ô search)
    initializeFloatingLabels();

    // ============ LOGIC LỌC, PHÂN TRANG, VÀ RESET ============

    // Gán sự kiện 'click' cho nút Filter
    $('#filterButton').on('click', function () {
        loadPage(1); // Luôn tải trang 1 với bộ lọc mới
        //khi nhấn nút filter thì sẽ gọi đến hàm loadpage, lấy giá trị tại các bộ lọc, hàm loadpage nhận vào số trang
    });

    // Gán sự kiện nhấn phím 'Enter' trong ô tìm kiếm
    $('#searchInput').on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            loadPage(1);
        }
    });

    // Gán sự kiện 'click' cho các nút phân trang
    $('body').on('click', 'li.page-item a', function (e) {
        e.preventDefault();
        var $pageItem = $(this).parent();//khởi tạo đối tượng Jquery
        if ($pageItem.hasClass('disabled') || $pageItem.hasClass('active')) {
            return;
        }
        var page = parseInt($pageItem.attr('page'));
        loadPage(page);
    });

    //Gán sự kiện click cho các nút Action trên mỗi đối tượng
    //sự kiên Víew details
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


    // =======================================================
    // LOGIC CHO NÚT RESET
    // =======================================================
    $('#resetFilterButton').on('click', function () {
        $('#searchInput').val('');
        $('#GenderFilter').val('');
        $('#NationalityFilter').val('');

        // Gọi đúng hàm để cập nhật lại giao diện cho tất cả các ô
        initializeFloatingLabels();

        loadPage(1);
    });

    // Hàm chính để tải dữ liệu thông qua AJAX
    function loadPage(pageIndex) { 
        //lấy các giá trị đang có tại bộ lọc
        var searchQuery = $('#searchInput').val();
        var gender = $('#GenderFilter').val();
        var nationality = $('#NationalityFilter').val();

        //tạo một biến map giống như dict trong python để lưu dữ liệu
        var data = {
            pageIndex: pageIndex,
            searchQuery: searchQuery,
            gender: gender,
            nationality: nationality
        };

        //sử dụng AJAX
        $.ajax({
            url: '/Admin/Customer/CustomerTableAndPagination',
            type: 'GET',
            data: data,
            success: function (response) { //nếu thành công thì thay nội dung của .table-with-pagination bằng kết quả trà về
                $('.table-with-pagination').html(response);
            },
            error: function () {
                alert('Có lỗi xảy ra khi tải dữ liệu!');
            }
        });
    }


    // =======================================================
    // LOGIC CHO ACTION
    // =======================================================

    // ============ AJAX XỬ LÍ ACTION TRÊN MỖI NGƯỜI DÙNG ============
    // === AJAX GET ĐỂ LẤY FORM EDIT ===
    // Class của nút bấm là '.edit-details-btn'
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

    // === AJAX POST ĐỂ GỬI FORM EDIT ĐI ===
    $('#detailsModal').on('submit', '#editCustomerForm', function (e) {
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
                    location.reload();
                } else {
                    $('#detailsModal .modal-content').html(response);
                }
            },
            error: function () {
                alert('Đã có lỗi xảy ra trong quá trình gửi dữ liệu.');
            }
        });
    });

    // Thêm vào file CustomerList.js

    // === AJAX ĐỂ HIỂN THỊ MODAL XÁC NHẬN XÓA ===
    $('body').on('click', '.delete-button', function (e) {
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


    // === AJAX ĐỂ SUBMIT FORM XÓA ===
    $('#detailsModal').on('submit', '#deleteConfirmForm', function (e) {
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

