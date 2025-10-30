let bookings = [];
let currentMonth = new Date().getMonth() + 1;
let currentYear = new Date().getFullYear();

document.addEventListener('DOMContentLoaded', function () {
    renderCalendar();
});

// Lấy dữ liệu booking cho một tháng cụ thể từ Controller
async function fetchBookings(year, month) {
    try {
        const data = await $.ajax({
            url: `/Admin/Booking/GetBooking?year=${year}&month=${month}`,
            type: 'GET',
            dataType: 'json'
        });
        bookings = data;
        console.log("Dữ liệu booking nhận được:", bookings);
        return data;
    } catch (error) {
        console.error('Error fetching bookings:', error);
        bookings = [];
        return [];
    }
}


// Render toàn bộ giao diện lịch
async function renderCalendar() {
    await fetchBookings(currentYear, currentMonth);

    const grid = document.getElementById('calendarGrid');
    grid.innerHTML = '';

    const firstDay = new Date(currentYear, currentMonth - 1, 1);
    const lastDay = new Date(currentYear, currentMonth, 0);
    const firstDayOfWeek = firstDay.getDay();

    const monthNames = ["January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"];
    const month = monthNames[currentMonth - 1];

    document.getElementById('currentMonthYear').textContent = `${month}, ${currentYear}`;

    for (let i = 0; i < firstDayOfWeek; i++) {
        grid.appendChild(document.createElement('div')).className = 'calendar-day';
    }

    for (let day = 1; day <= lastDay.getDate(); day++) {
        const dateStr = `${currentYear}-${String(currentMonth).padStart(2, '0')}-${String(day).padStart(2, '0')}`;

        const dayBookings = bookings.filter(b => dateStr >= b.checkInDate && dateStr < b.checkOutDate);

        const countConfirmed = dayBookings.filter(b => b.status === 'Confirmed').length;
        const countPending = dayBookings.filter(b => b.status === 'Pending').length;
        const conutCancelled = dayBookings.filter(b => b.status === 'Cancelled').length;

        const dayDiv = document.createElement('div');
        dayDiv.className = 'calendar-day' + (dayBookings.length > 0 ? ' has-booking' : '');
        dayDiv.innerHTML = `
                <div class="calendar-day-number">${day}</div>
                ${dayBookings.length > 0
                ? `<div class="calendar-booking-info">
                            ${countConfirmed > 0 ? `<div title="${countConfirmed} Confirmed"><span class="badge status-confirmed">${countConfirmed}</span></div>` : ''}
                            ${countPending > 0 ? `<div title="${countPending} Pending"><span class="badge status-pending">${countPending}</span></div>` : ''}
                            ${conutCancelled > 0 ? `<div title="${conutCancelled} Pending"><span class="badge status-cancelled">${conutCancelled}</span></div>` : ''}
                        </div>`
                : ''}
            `;
        dayDiv.onclick = () => showDayBookings(dateStr, dayBookings);
        grid.appendChild(dayDiv);
    }
}

// Chuyển tháng
async function changeMonth(delta) {
    currentMonth += delta;
    if (currentMonth < 1) {
        currentMonth = 12;
        currentYear--;
    } else if (currentMonth > 12) {
        currentMonth = 1;
        currentYear++;
    }
    await renderCalendar();
}

function showDayBookings(date, dayBookings) {
    const container = document.getElementById('selectedDateBookings');
    if (dayBookings.length === 0) {
        container.innerHTML = `<div class="text-center text-muted py-4"><i class="bi bi-calendar-x" style="font-size: 2.5rem;"></i><p class="mt-3">There are no bookings<br>for ${formatDate(date)}</p></div>`;
        return;
    }

    container.innerHTML = `
            <div class="mb-3 px-3 pt-3">
                <h6 class="text-primary"><i class="bi bi-calendar-check"></i> ${formatDate(date)}</h6>
                <small class="text-muted">${dayBookings.length} room(s) booked</small>
            </div>
            <div class="list-group list-group-flush">
                ${dayBookings.map(b => `
                    <div class="list-group-item list-group-item-action" onclick='viewBooking(${JSON.stringify(b)})'>
                        <div class="d-flex justify-content-between align-items-center">
                            <div>
                                <h6 class="mb-1 text-dark">Room ${b.roomNumber}</h6>
                                <small class="text-muted">${b.customerName}</small>
                            </div>
                            <span class="badge ${getStatusClass(b.status)}">${b.status}</span>
                        </div>
                    </div>
                `).join('')}
            </div>
        `;
}

function viewBooking(booking) {
    if (!booking) return;

    const modalBody = document.getElementById('modalBody');
    modalBody.innerHTML = `
            <div class="row">
                <div class="col-md-6">
                    <h6><i class="bi bi-person"></i> Customer Information</h6>
                    <table class="table table-sm table-borderless">
                        <tr><td style="width: 100px;"><strong>Full Name:</strong></td><td>${booking.customerName}</td></tr>
                        <tr><td><strong>Phone:</strong></td><td>${booking.phone || 'N/A'}</td></tr>
                        <tr><td><strong>Email:</strong></td><td>${booking.email || 'N/A'}</td></tr>
                    </table>
                </div>
                <div class="col-md-6">
                    <h6><i class="bi bi-bookmark-check"></i> Booking Details</h6>
                    <table class="table table-sm table-borderless">
                        <tr><td style="width: 100px;"><strong>Booking ID:</strong></td><td>#${booking.bookingId}</td></tr>
                        <tr><td><strong>Status:</strong></td><td><span class="badge ${getStatusClass(booking.status)}">${booking.status}</span></td></tr>
                        <tr><td><strong>Room Price:</strong></td><td>${formatCurrency(booking.price)} / night</td></tr>
                    </table>
                </div>
                <div class="col-md-12 mt-2">
                    <h6><i class="bi bi-door-open"></i> Room Stay</h6>
                    <table class="table table-sm table-borderless">
                        <tr>
                            <td style="width: 100px;"><strong>Room:</strong></td><td>Room ${booking.roomNumber}</td>
                            <td style="width: 100px;"><strong>Nights:</strong></td><td>${calculateNights(booking.checkInDate, booking.checkOutDate)} nights</td>
                        </tr>
                        <tr>
                            <td><strong>Check-in:</strong></td><td>${formatDate(booking.checkInDate)}</td>
                            <td><strong>Check-out:</strong></td><td>${formatDate(booking.checkOutDate)}</td>
                        </tr>
                    </table>
                </div>
                ${booking.notes ? `
                <div class="col-md-12 mt-2">
                    <h6><i class="bi bi-sticky"></i> Notes</h6>
                    <p class="bg-light p-2 rounded small">${booking.notes}</p>
                </div>` : ''}
            </div>
        `;
    const modal = new bootstrap.Modal(document.getElementById('bookingModal'));
    modal.show();
}

function formatDate(dateStr) {
    return new Date(dateStr).toLocaleDateString('en-GB'); // dd/mm/yyyy
}
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
}
function calculateNights(checkIn, checkOut) {
    const diffTime = Math.abs(new Date(checkOut) - new Date(checkIn));
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
}
function getStatusClass(status) {
    const classes = {
        'Pending': 'status-pending', 'Confirmed': 'status-confirmed',
        'Cancelled': 'status-cancelled', 'Completed': 'status-completed',
        'CheckedIn': 'status-checkedin', 'CheckedOut': 'status-checkedout'
    };
    return classes[status] || 'bg-secondary';
}
