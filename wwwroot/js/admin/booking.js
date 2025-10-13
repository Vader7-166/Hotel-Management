// Biến để lưu trữ modal instance
let deleteModal = null;

document.addEventListener('DOMContentLoaded', function () {
    // Khởi tạo đối tượng Modal của Bootstrap một lần khi trang được tải
    if (document.getElementById('deleteConfirmModal')) {
        deleteModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
    }
});

function showDeleteModal(bookingId, customerName) {
    // Lấy form trong modal
    const deleteForm = document.getElementById('deleteForm');
    // Cập nhật thuộc tính 'action' của form để trỏ đúng đến URL Delete/{id}
    deleteForm.action = `/Admin/Booking/Delete/${bookingId}`; // Chú ý: sửa '/Admin/Booking/' nếu route của bạn khác

    // Cập nhật nội dung text xác nhận
    const confirmText = document.getElementById('deleteConfirmText');
    confirmText.textContent = `Are you sure you want to delete the booking for customer "${customerName}" (ID: ${bookingId})? This action cannot be undone.`;

    // Hiển thị modal
    if (deleteModal) {
        deleteModal.show();
    }
}

let detailsModal = null;

document.addEventListener('DOMContentLoaded', function () {
    if (document.getElementById('bookingModal')) {
        detailsModal = new bootstrap.Modal(document.getElementById('bookingModal'));
    }
});

async function viewBookingDetails(bookingId) {
    const modalBody = document.getElementById('modalBody');

    if (!modalBody) {
        console.error("Element with ID 'modalBody' was not found.");
        return;
    }

    // Hiển thị trạng thái "Loading..."
    modalBody.innerHTML = '<div class="text-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>';
    if (detailsModal) {
        detailsModal.show();
    }
    try {
        const response = await fetch(`/Admin/Booking/GetBookingDetails/${bookingId}`);

        if (!response.ok) {
            modalBody.innerHTML = `<div class="alert alert-danger">Error: Could not fetch booking details. Status: ${response.status}</div>`;
            return;
        }

        const data = await response.json();

        modalBody.innerHTML = `
            <div class="row">
                <div class="col-md-6">
                    <h6><i class="bi bi-bookmark"></i> Booking Information</h6>
                    <table class="table table-sm table-borderless">
                        <tr><td><strong>Booking ID:</strong></td><td>${data.bookingId}</td></tr>
                        <tr><td><strong>Status:</strong></td><td><span class="badge ${getStatusClass(data.status)}">${data.status}</span></td></tr>
                        <tr><td><strong>Total Price:</strong></td><td>${new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(data.totalPrice)}</td></tr>
                    </table>
                </div>
                <div class="col-md-6">
                    <h6><i class="bi bi-person"></i> Customer Information</h6>
                    <table class="table table-sm table-borderless">
                        <tr><td><strong>Full Name:</strong></td><td>${data.customerName}</td></tr>
                        <tr><td><strong>Phone:</strong></td><td>${data.customerPhone}</td></tr>
                        <tr><td><strong>Email:</strong></td><td>${data.customerEmail}</td></tr>
                    </table>
                </div>
                <div class="col-md-12 mt-3">
                    <h6><i class="bi bi-door-open"></i> Room Information</h6>
                    <table class="table table-sm table-borderless">
                        <tr><td><strong>Room:</strong></td><td>Room ${data.roomNumber} (${data.roomType})</td></tr>
                        <tr><td><strong>Check-in:</strong></td><td>${data.checkInDate}</td></tr>
                        <tr><td><strong>Check-out:</strong></td><td>${data.checkOutDate}</td></tr>
                        <tr><td><strong>Number of Nights:</strong></td><td>${data.numberOfNights} nights</td></tr>
                    </table>
                </div>
                ${data.notes ? `
                <div class="col-md-12 mt-3">
                    <h6><i class="bi bi-sticky"></i> Notes</h6>
                    <p class="bg-light p-3 rounded">${data.notes}</p>
                </div>
                ` : ''}
            </div>
        `;
    } catch (error) {
        console.error("Fetch error:", error);
        modalBody.innerHTML = '<div class="alert alert-danger">A network error occurred while fetching details.</div>';
    }
}

// Hàm helper để lấy CSS class cho status (tái sử dụng từ view)
function getStatusClass(status) {
    switch (status) {
        case 'Pending': return 'status-pending';
        case 'Confirmed': return 'status-confirmed';
        case 'Cancelled': return 'status-cancelled';
        case 'CheckedIn': return 'status-checkedin';
        case 'CheckedOut': return 'status-completed';
        default: return 'bg-secondary';
    }
}