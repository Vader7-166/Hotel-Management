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

//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Customer Filter Logic
(function () {
    'use strict';

    // Lấy các elements
    const searchInput = document.getElementById('searchInput');
    const statusFilter = document.getElementById('statusFilter');
    const typeFilter = document.getElementById('typeFilter');
    const customerTable = document.querySelector('#customerTable tbody');

    // Hàm chuẩn hóa chuỗi (loại bỏ dấu tiếng Việt, chuyển về lowercase)
    function normalizeString(str) {
        if (!str) return '';
        return str.toString().toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/đ/g, 'd')
            .replace(/Đ/g, 'D')
            .trim();
    }

    // Hàm kiểm tra xem text có chứa searchTerm không (không phân biệt hoa thường, dấu)
    function containsText(text, searchTerm) {
        if (!searchTerm) return true;
        return normalizeString(text).includes(normalizeString(searchTerm));
    }

    // Hàm lấy thông tin từ một row
    function getRowData(row) {
        // Lấy tên khách hàng
        const nameElement = row.querySelector('h6.text-sm');
        const name = nameElement ? nameElement.textContent.trim() : '';

        // Lấy email (dòng đầu tiên trong cell thứ 2)
        const emailElement = row.querySelectorAll('td')[1]?.querySelector('p.text-xs:first-child');
        const email = emailElement ? emailElement.textContent.trim() : '';

        // Lấy số điện thoại (dòng thứ hai trong cell thứ 2)
        const phoneElement = row.querySelectorAll('td')[1]?.querySelector('p.text-xs.text-secondary');
        const phone = phoneElement ? phoneElement.textContent.trim() : '';

        // Lấy loại khách hàng (Customer Type)
        const typeElement = row.querySelectorAll('td')[2]?.querySelector('.badge');
        const type = typeElement ? typeElement.textContent.trim() : '';

        // Lấy trạng thái (Status)
        const statusElement = row.querySelectorAll('td')[3]?.querySelector('.badge');
        const status = statusElement ? statusElement.textContent.trim() : '';

        return {
            name: name,
            email: email,
            phone: phone,
            type: type,
            status: status
        };
    }

    // Hàm filter chính
    function applyFilters() {
        const searchTerm = searchInput.value.trim();
        const selectedStatus = statusFilter.value.toLowerCase();
        const selectedType = typeFilter.value.toLowerCase();

        // Lấy tất cả các rows
        const rows = customerTable.querySelectorAll('tr');
        let visibleCount = 0;

        rows.forEach(row => {
            const data = getRowData(row);

            // Kiểm tra search term (tìm trong name, email, phone)
            const matchesSearch = !searchTerm ||
                containsText(data.name, searchTerm) ||
                containsText(data.email, searchTerm) ||
                containsText(data.phone, searchTerm);

            // Kiểm tra status filter
            const matchesStatus = !selectedStatus ||
                normalizeString(data.status) === normalizeString(selectedStatus);

            // Kiểm tra type filter
            const matchesType = !selectedType ||
                normalizeString(data.type) === normalizeString(selectedType);

            // Hiển thị hoặc ẩn row
            if (matchesSearch && matchesStatus && matchesType) {
                row.style.display = '';
                visibleCount++;
            } else {
                row.style.display = 'none';
            }
        });

        // Cập nhật thông tin số lượng kết quả (nếu có phần pagination)
        updateResultsInfo(visibleCount, rows.length);

        // Hiển thị thông báo nếu không có kết quả
        showNoResultsMessage(visibleCount);
    }

    // Hàm cập nhật thông tin số lượng kết quả
    function updateResultsInfo(visibleCount, totalCount) {
        const resultsInfo = document.querySelector('.border-top.py-3.px-4 p.text-sm');
        if (resultsInfo) {
            resultsInfo.textContent = `Showing ${visibleCount} of ${totalCount} entries`;
        }
    }

    // Hàm hiển thị thông báo không có kết quả
    function showNoResultsMessage(visibleCount) {
        // Xóa thông báo cũ nếu có
        const oldMessage = customerTable.querySelector('.no-results-message');
        if (oldMessage) {
            oldMessage.remove();
        }

        // Nếu không có kết quả, thêm thông báo
        if (visibleCount === 0) {
            const messageRow = document.createElement('tr');
            messageRow.className = 'no-results-message';
            messageRow.innerHTML = `
                <td colspan="7" class="text-center py-4">
                    <div class="d-flex flex-column align-items-center">
                        <i class="material-symbols-rounded text-secondary mb-2" style="font-size: 3rem;">search_off</i>
                        <h6 class="text-secondary mb-1">No customers found</h6>
                        <p class="text-xs text-secondary mb-0">Try adjusting your filters or search term</p>
                    </div>
                </td>
            `;
            customerTable.appendChild(messageRow);
        }
    }

    // Hàm reset filters
    function resetFilters() {
        searchInput.value = '';
        statusFilter.value = '';
        typeFilter.value = '';
        applyFilters();
    }

    // Event listeners
    // Lắng nghe sự kiện click nút Filter
    const filterButton = document.querySelector('button[onclick="applyFilters()"]');
    if (filterButton) {
        filterButton.removeAttribute('onclick'); // Xóa onclick cũ
        filterButton.addEventListener('click', applyFilters);
    }

    // Lắng nghe sự kiện Enter trong ô search
    searchInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            applyFilters();
        }
    });

    // Lắng nghe sự kiện thay đổi của select filters (tự động filter)
    statusFilter.addEventListener('change', applyFilters);
    typeFilter.addEventListener('change', applyFilters);

    // Lắng nghe sự kiện input trong search box (real-time search - tùy chọn)
    // Bỏ comment dòng dưới nếu muốn tìm kiếm real-time khi gõ
    // searchInput.addEventListener('input', debounce(applyFilters, 300));

    // Hàm debounce để tránh gọi hàm quá nhiều lần khi typing
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    // Thêm nút Reset (tùy chọn)
    function addResetButton() {
        const filterButton = document.querySelector('button[class*="bg-gradient-info"]');
        if (filterButton && !document.getElementById('resetFilterBtn')) {
            const resetBtn = document.createElement('button');
            resetBtn.id = 'resetFilterBtn';
            resetBtn.className = 'btn btn-outline-secondary ms-2';
            resetBtn.innerHTML = '<i class="material-symbols-rounded">refresh</i> Reset';
            resetBtn.addEventListener('click', resetFilters);
            filterButton.parentElement.appendChild(resetBtn);
        }
    }

    // Gọi hàm thêm nút reset khi trang load
    addResetButton();

    // Export hàm applyFilters để có thể gọi từ bên ngoài nếu cần
    window.applyFilters = applyFilters;
    window.resetFilters = resetFilters;

    // Log để debug
    console.log('Customer filter script loaded successfully');
})();
//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Pagination AJAX Logic
$(document).ready(function () {
    // Xử lý click pagination
    $('body').on('click', 'li.page-item a', function (e) {
        console.log("Click event fired!"); // <-- THÊM DÒNG NÀY
        e.preventDefault();

        var $pageItem = $(this).parent();//lấy ra thẻ <li> cha của thẻ a vừa rồi
        if ($pageItem.hasClass('disabled') || $pageItem.hasClass('active')) { //nếu thẻ vừa được lấy ra có chứa lớp DIsable hoặc ACtivate thì không làm gì cả
            return;
        }

        //lấy ra số trang của the li đó
        var page = parseInt($pageItem.attr('page'));
        //sau đó gọi hàm loadpage
        loadPage(page);
    });

    // Load trang
    function loadPage(pageIndex) {
        $.ajax({
            url: '/Admin/Customer/CustomerTableAndPagination', //URL của action 
            type: 'GET', //Phương thức GET
            data: { pageIndex: pageIndex }, //tham số pageIndex gán giá trị bằng giá tri của parameter
            success: function (response) {
                //chuyển chuỗi HTML thành đối tương JQuery
                var $response = $(response);
                //tìm body trong resphonse đem thay thế với body cũ
                $('#customerTable tbody').replaceWith($response.find('tbody'));
                //tìm container của pagination trong response và cập nhật lại toàn bộ 
                $('#paginationContainer').html($response.find('.border-top'));
            },
            error: function () {
                alert('Có lỗi xảy ra!'); //nếu có lỗi thì in ra cảnh báo
            }
        });
    }
});
