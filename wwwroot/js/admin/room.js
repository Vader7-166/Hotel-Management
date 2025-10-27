function scrollToFloor(floorNumber) {
    var element = document.getElementById('floor-' + floorNumber);
    if (element) {
        var offset = 100; // Khoảng cách từ top (để không bị che bởi thanh navigation)
        var elementPosition = element.getBoundingClientRect().top;
        var offsetPosition = elementPosition + window.pageYOffset - offset;

        window.scrollTo({
            top: offsetPosition,
            behavior: 'smooth'
        });

        // Hiệu ứng highlight
        element.classList.add('highlight-floor');
        setTimeout(function () {
            element.classList.remove('highlight-floor');
        }, 2000);
    }
}

// Scroll về đầu trang
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

function viewRoomDetails(roomId) {
    $.ajax({
        url: `/Admin/Room/Details?id=${roomId}`,
        type: 'GET',
        success: function (data) {
            $('#roomModalBody').html(data);
            $('#roomModalLabel').text('Chi Tiết Phòng');
            var modal = new bootstrap.Modal(document.getElementById('roomModal'));
            modal.show();
        },
        error: function () {
            alert('Không thể tải thông tin phòng!');
        }
    });
}

function openAddRoomModal() {
    $.ajax({
        url: `/Admin/Room/Create`,
        type: 'POST',
        success: function (data) {
            $('#roomModalBody').html(data);
            $('#roomModalLabel').text('Thêm Phòng Mới');
            var modal = new bootstrap.Modal(document.getElementById('roomModal'));
            modal.show();
        },
        error: function () {
            alert('Không thể mở form thêm phòng!');
        }
    });
}

function saveRoom(isEdit) {
    console.log('Hàm saveRoom đã được gọi! Đang chỉnh sửa: ' + isEdit);
    var form = $('#roomForm');
    var url = isEdit ? '/Admin/Room/Edit' : '/Admin/Room/Create';

    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success !== undefined) {
                // NẾU LÀ JSON (Thành công hoặc Lỗi Server)
                if (response.success === true) {
                    // Thành công
                    alert(response.message); 
                    $('#roomModal').modal('hide');
                    loadRoomLayout();
                } else {
                    // Thất bại (ví dụ: lỗi server, lỗi CSDL)
                    alert(response.message); 
                }
            }
        },
        error: function () {
            alert('Có lỗi xảy ra!');
        }
    });
}

function deleteRoom(roomId) {
    if (confirm('Bạn có chắc chắn muốn xóa phòng này?')) {
        $.ajax({
            url: `/Admin/Room/Delete/${roomId}`,
            type: 'POST',
            data: { __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val() },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#roomModal').modal('hide');
                    loadRoomLayout();
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert('Có lỗi xảy ra khi xóa phòng!');
            }
        });
    }
}

// Highlight tầng đang xem khi scroll
$(window).scroll(function () {
    $('.floor-section').each(function () {
        var elementTop = $(this).offset().top;
        var elementBottom = elementTop + $(this).outerHeight();
        var viewportTop = $(window).scrollTop();
        var viewportBottom = viewportTop + $(window).height();

        if (elementBottom > viewportTop && elementTop < viewportBottom) {
            var floorNumber = $(this).attr('id').replace('floor-', '');
        }
    });
});
// Hàm này dùng để tải lại nội dung bảng
function loadRoomLayout() {
    console.log("Đang tải lại bảng phòng...");

    // Giả sử trang Index.cshtml của bạn có một <div> với ID này
    // để chứa bảng
    var container = $("#room-layout-container");

    $.ajax({
        url: '/Admin/Room/_GetRoomLayout', // Đường dẫn tới Action mới
        type: 'GET',
        success: function (tableHtml) {
            // Thay thế HTML cũ của bảng bằng HTML mới
            container.html(tableHtml);
        },
        error: function () {
            container.html('<p class="text-danger">Lỗi khi tải danh sách phòng.</p>');
        }
    });
}
