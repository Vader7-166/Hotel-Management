// Sample Data
let bookings = [
    {id: 'BK001', customer: 'Nguyễn Văn A', phone: '0901234567', email: 'nguyenvana@gmail.com', room: '101', checkIn: '2025-10-10', checkOut: '2025-10-12', status: 'confirmed', price: 2000000, notes: 'Khách VIP'},
    {id: 'BK002', customer: 'Trần Thị B', phone: '0912345678', email: 'tranthib@gmail.com', room: '201', checkIn: '2025-10-11', checkOut: '2025-10-14', status: 'pending', price: 4500000, notes: ''},
    {id: 'BK003', customer: 'Lê Văn C', phone: '0923456789', email: 'levanc@gmail.com', room: '102', checkIn: '2025-10-08', checkOut: '2025-10-09', status: 'completed', price: 1500000, notes: 'Check-out sớm'},
    {id: 'BK004', customer: 'Phạm Thị D', phone: '0934567890', email: 'phamthid@gmail.com', room: '301', checkIn: '2025-10-10', checkOut: '2025-10-12', status: 'pending', price: 9000000, notes: 'Honeymoon package'},
    {id: 'BK005', customer: 'Hoàng Văn E', phone: '0945678901', email: 'hoangvane@gmail.com', room: '202', checkIn: '2025-10-12', checkOut: '2025-10-13', status: 'cancelled', price: 3000000, notes: 'Khách hủy do lý do cá nhân'},
    {id: 'BK006', customer: 'Vũ Thị F', phone: '0956789012', email: 'vuthif@gmail.com', room: '101', checkIn: '2025-10-20', checkOut: '2025-10-22', status: 'pending', price: 2000000, notes: ''},
    {id: 'BK007', customer: 'Đặng Văn G', phone: '0967890123', email: 'dangvang@gmail.com', room: '302', checkIn: '2025-10-25', checkOut: '2025-10-28', status: 'confirmed', price: 12000000, notes: 'Corporate booking'},
];

let currentMonth = new Date().getMonth();
let currentYear = new Date().getFullYear();
let selectedBooking = null;

// Initialize
document.addEventListener('DOMContentLoaded', function() {
    renderBookingsTable();
    setupEventListeners();
    setMinDates();
});

// Setup Event Listeners
// Click nav-link on sidebar
function setupEventListeners() {
    document.querySelectorAll('.sidebar .nav-link').forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const page = this.getAttribute('data-page');
            if (page) {
                showPage(page);
                document.querySelectorAll('.sidebar .nav-link').forEach(l => l.classList.remove('active'));
                this.classList.add('active');
            }
        });
    });

    // Submit form booking
    document.getElementById('bookingForm').addEventListener('submit', function(e) {
        e.preventDefault();
        saveBooking();
    });

    // Search by name/Id...
    document.getElementById('searchInput').addEventListener('input', searchBookings);

    // Search by booking status
    document.getElementById('statusFilter').addEventListener('change', searchBookings);

    // Search by date
    document.getElementById('dateFilter').addEventListener('change', searchBookings);
}

// Show Page
function showPage(pageName) {
    document.querySelectorAll('.page-content').forEach(page => {
        page.style.display = 'none';
    });
    document.getElementById('page-' + pageName).style.display = 'block';

    if (pageName === 'create') {
        resetForm();
    } else if (pageName === 'calendar') {
        renderCalendar();
    }
}

// Render Bookings Table
function renderBookingsTable(filteredBookings = null) {
    const data = filteredBookings || bookings;
    const tbody = document.getElementById('bookingsTableBody');
    tbody.innerHTML = '';

    console.log("Data: ", data);

    if (data.length === 0) {
        const emptyRow = document.createElement('tr');
        emptyRow.innerHTML = `
        <td colspan="9" class="text-center text-muted py-3">
            Không có booking tương ứng
        </td>
    `;
        tbody.appendChild(emptyRow);
        return;
    }

    data.forEach(booking => {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td><strong>${booking.id}</strong></td>
            <td>${booking.customer}</td>
            <td>${booking.phone}</td>
            <td>Phòng ${booking.room}</td>
            <td>${formatDate(booking.checkIn)}</td>
            <td>${formatDate(booking.checkOut)}</td>
            <td><span class="badge ${getStatusClass(booking.status)}">${getStatusText(booking.status)}</span></td>
            <td>${formatCurrency(booking.price)}</td>
            <td class="action-buttons">
                <button class="btn btn-sm btn-info" onclick="viewBooking('${booking.id}')">
                    <i class="bi bi-eye"></i>
                </button>
                <button class="btn btn-sm btn-warning" onclick="editBooking('${booking.id}')">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-danger" onclick="confirmDelete('${booking.id}')">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        `;
        tbody.appendChild(row);
    });
}

// Search Bookings
function searchBookings() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    const statusFilter = document.getElementById('statusFilter').value;
    const dateFilter = document.getElementById('dateFilter').value;

    let filtered = bookings.filter(booking => {
        const matchSearch = booking.customer.toLowerCase().includes(searchTerm) ||
                            booking.phone.includes(searchTerm) ||
                            booking.id.toLowerCase().includes(searchTerm);
        const matchStatus = !statusFilter || booking.status === statusFilter;
        const matchDate = !dateFilter || booking.checkIn === dateFilter || booking.checkOut === dateFilter;
        return matchSearch && matchStatus && matchDate;
    });
    renderBookingsTable(filtered);
}

function resetFilter(){
    document.getElementById('searchInput').value = '';
    document.getElementById('statusFilter').value = '';
    document.getElementById('dateFilter').value = '';

    searchBookings();
}

// View Booking Details
function viewBooking(id) {
    const booking = bookings.find(b => b.id === id);
    if (!booking) return;

    const modalBody = document.getElementById('modalBody');
    modalBody.innerHTML = `
        <div class="row">
            <div class="col-md-6">
                <h6><i class="bi bi-bookmark"></i> Thông Tin Booking</h6>
                <table class="table table-sm">
                    <tr><td><strong>Mã Booking:</strong></td><td>${booking.id}</td></tr>
                    <tr><td><strong>Trạng Thái:</strong></td><td><span class="badge ${getStatusClass(booking.status)}">${getStatusText(booking.status)}</span></td></tr>
                    <tr><td><strong>Tổng Tiền:</strong></td><td>${formatCurrency(booking.price)}</td></tr>
                </table>
            </div>
            <div class="col-md-6">
                <h6><i class="bi bi-person"></i> Thông Tin Khách</h6>
                <table class="table table-sm">
                    <tr><td><strong>Họ Tên:</strong></td><td>${booking.customer}</td></tr>
                    <tr><td><strong>Điện Thoại:</strong></td><td>${booking.phone}</td></tr>
                    <tr><td><strong>Email:</strong></td><td>${booking.email || 'N/A'}</td></tr>
                </table>
            </div>
            <div class="col-md-12 mt-3">
                <h6><i class="bi bi-door-open"></i> Thông Tin Phòng</h6>
                <table class="table table-sm">
                    <tr><td><strong>Phòng:</strong></td><td>Phòng ${booking.room}</td></tr>
                    <tr><td><strong>Check-in:</strong></td><td>${formatDate(booking.checkIn)}</td></tr>
                    <tr><td><strong>Check-out:</strong></td><td>${formatDate(booking.checkOut)}</td></tr>
                    <tr><td><strong>Số Đêm:</strong></td><td>${calculateNights(booking.checkIn, booking.checkOut)} đêm</td></tr>
                </table>
            </div>
            ${booking.notes ? `
            <div class="col-md-12 mt-3">
                <h6><i class="bi bi-sticky"></i> Ghi Chú</h6>
                <p class="bg-light p-3 rounded">${booking.notes}</p>
            </div>
            ` : ''}
        </div>
    `;

    const modal = new bootstrap.Modal(document.getElementById('bookingModal'));
    modal.show();
}

// Edit Booking
function editBooking(id) {
    const booking = bookings.find(b => b.id === id);
    if (!booking) return;

    selectedBooking = booking;
    showPage('create');

    document.getElementById('formTitle').textContent = 'Chỉnh Sửa Booking';
    document.getElementById('bookingId').value = booking.id;
    document.getElementById('customerName').value = booking.customer;
    document.getElementById('customerPhone').value = booking.phone;
    document.getElementById('customerEmail').value = booking.email || '';
    document.getElementById('roomType').value = booking.room;
    document.getElementById('checkInDate').value = booking.checkIn;
    document.getElementById('checkOutDate').value = booking.checkOut;
    document.getElementById('bookingStatus').value = booking.status;
    document.getElementById('totalPrice').value = booking.price;
    document.getElementById('bookingNotes').value = booking.notes || '';
    document.getElementById('deleteBtn').style.display = 'block';
}

// Save Booking
function saveBooking() {
    const id = document.getElementById('bookingId').value;
    const bookingData = {
        id: id || 'BK' + String(bookings.length + 1).padStart(3, '0'),
        customer: document.getElementById('customerName').value,
        phone: document.getElementById('customerPhone').value,
        email: document.getElementById('customerEmail').value,
        room: document.getElementById('roomType').value,
        checkIn: document.getElementById('checkInDate').value,
        checkOut: document.getElementById('checkOutDate').value,
        status: document.getElementById('bookingStatus').value,
        price: parseInt(document.getElementById('totalPrice').value),
        notes: document.getElementById('bookingNotes').value
    };

    if (id) {
        // Update existing
        const index = bookings.findIndex(b => b.id === id);
        if (index !== -1) {
            bookings[index] = bookingData;
            showNotification('Cập nhật booking thành công!', 'success');
        }
    } else {
        // Create new
        bookings.unshift(bookingData);
        showNotification('Tạo booking mới thành công!', 'success');
    }

    renderBookingsTable();
    renderCalendar();
    showPage('bookings');
}

// Delete Booking (Sau này kết nối db sẽ xóa trong db và lịch sẽ cập nhật lại mỗi lần vào view)
function confirmDelete(id) {
    if (confirm('Bạn có chắc chắn muốn xóa booking này?\n\nMã booking: ' + id)) {
        const index = bookings.findIndex(b => b.id === id);
        if (index !== -1) {
            bookings.splice(index, 1);
            renderBookingsTable();
            //renderCalendar(); Render lại Lịch 
            showNotification('Đã xóa booking thành công!', 'danger');
        }
    }
}

function deleteBooking() {
    const id = document.getElementById('bookingId').value;
    if (id && confirm('Bạn có chắc chắn muốn xóa booking này?\n\nMã booking: ' + id)) {
        const index = bookings.findIndex(b => b.id === id);
        if (index !== -1) {
            bookings.splice(index, 1);
            renderBookingsTable();
            renderCalendar();
            showNotification('Đã xóa booking thành công!', 'danger');
            showPage('bookings');
        }
    }
}

// Reset Form
function resetForm() {
    document.getElementById('formTitle').textContent = 'Tạo Booking Mới';
    document.getElementById('bookingForm').reset();
    document.getElementById('bookingId').value = '';
    document.getElementById('deleteBtn').style.display = 'none';
    selectedBooking = null;
    setMinDates();
}

// Set minimum dates
function setMinDates() {
    const today = new Date().toISOString().split('T')[0];
    document.getElementById('checkInDate').min = today;
    document.getElementById('checkInDate').addEventListener('change', function() {
        document.getElementById('checkOutDate').min = this.value;
    });
}

// Utility Functions
function formatDate(dateStr) {
    const date = new Date(dateStr);
    return date.toLocaleDateString('vi-VN');
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}

function calculateNights(checkIn, checkOut) {
    const start = new Date(checkIn);
    const end = new Date(checkOut);
    const diffTime = Math.abs(end - start);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays;
}

function getStatusClass(status) {
    const classes = {
        'pending': 'status-pending',
        'confirmed': 'status-confirmed',
        'cancelled': 'status-cancelled',
        'completed': 'status-completed'
    };
    return classes[status] || 'bg-secondary';
}

function getStatusText(status) {
    const texts = {
        'pending': 'Chờ xác nhận',
        'confirmed': 'Đã xác nhận',
        'cancelled': 'Đã hủy',
        'completed': 'Hoàn thành'
    };
    return texts[status] || status;
}

function showNotification(message, type) {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3`;
    alertDiv.style.zIndex = '9999';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    document.body.appendChild(alertDiv);

    setTimeout(() => {
        alertDiv.remove();
    }, 3000);
}
