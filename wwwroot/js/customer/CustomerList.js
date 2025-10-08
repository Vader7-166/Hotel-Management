// Luôn đợi cho toàn bộ nội dung HTML được tải xong rồi mới chạy script
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

            // Xử lý trường hợp trang tải lại và input đã có sẵn giá trị
            if (input.value !== '') {
                group.classList.add('is-filled');
            }
        }
    });
});
// --- Filter Logic ---
// Hàm này được đặt ở global scope để thuộc tính onclick trong HTML có thể gọi được
function applyFilters() {
    // 1. Lấy giá trị từ các trường filter
    const searchValue = document.getElementById('searchInput').value;
    const statusValue = document.getElementById('statusFilter').value;
    const typeValue = document.getElementById('typeFilter').value;

    // 2. In giá trị ra console để kiểm tra (đây là phần placeholder)
    // Bạn có thể mở DevTools (F12) -> Console để xem kết quả khi bấm nút Filter
    console.log('Đang áp dụng bộ lọc với các giá trị:');
    console.log('Từ khóa tìm kiếm:', searchValue);
    console.log('Trạng thái:', statusValue);
    console.log('Loại khách hàng:', typeValue);

    // 3. BƯỚC TIẾP THEO: Gửi các giá trị này lên server để lọc dữ liệu
    // Bạn sẽ sử dụng AJAX (Fetch API) để gửi request đến một Action trong Controller
    // và nhận lại danh sách khách hàng đã được lọc. Sau đó, bạn sẽ cập nhật lại
    // bảng trên giao diện.

    // Ví dụ (code này cần được hoàn thiện):
    // const url = `/Admin/Customer/FilterCustomers?search=${encodeURIComponent(searchValue)}&status=${statusValue}&type=${typeValue}`;
    // fetch(url)
    //     .then(response => response.json())
    //     .then(filteredCustomers => {
    //         console.log('Dữ liệu đã lọc nhận được:', filteredCustomers);
    //         // Viết code để cập nhật lại bảng ở đây...
    //     })
    //     .catch(error => console.error('Có lỗi xảy ra khi lọc dữ liệu:', error));
}

