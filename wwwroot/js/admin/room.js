// Scroll to a specific floor
function scrollToFloor(floorNumber) {
    var element = document.getElementById('floor-' + floorNumber);
    if (element) {
        var offset = 100; // Distance from the top (to prevent being hidden by navigation bar)
        var elementPosition = element.getBoundingClientRect().top;
        var offsetPosition = elementPosition + window.pageYOffset - offset;

        window.scrollTo({
            top: offsetPosition,
            behavior: 'smooth'
        });

        // Highlight effect
        element.classList.add('highlight-floor');
        setTimeout(function () {
            element.classList.remove('highlight-floor');
        }, 2000);
    }
}

// Scroll to the top of the page
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// Ajax: View Room Details
function viewRoomDetails(roomId) {
    $.ajax({
        url: `/Admin/Room/Details?id=${roomId}`,
        type: 'GET',
        success: function (data) {
            $('#roomModalBody').html(data);
            $('#roomModalLabel').text('Room Details');
            var modal = new bootstrap.Modal(document.getElementById('roomModal'));
            modal.show();
        },
        error: function () {
            alert('Unable to load room details!');
        }
    });
}

// Ajax: Open Add Room Modal
function openAddRoomModal() {
    $.ajax({
        url: `/Admin/Room/Create`,
        type: 'POST',
        success: function (data) {
            $('#roomModalBody').html(data);
            $('#roomModalLabel').text('Add New Room');
            var modal = new bootstrap.Modal(document.getElementById('roomModal'));
            modal.show();
        },
        error: function () {
            alert('Unable to open Add New Room form!');
        }
    });
}

// Ajax: Save Room (Create or Edit)
function saveRoom(isEdit) {
    var form = $('#roomForm');
    var url = isEdit ? '/Admin/Room/Edit' : '/Admin/Room/Create';

    $.ajax({
        url: url,
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success !== undefined) {
                if (response.success === true) {
                    alert(response.message);
                    $('#roomModal').modal('hide');
                    loadRoomLayout();
                } else {
                    alert(response.message);
                }
            }
        },
        error: function () {
            alert('An error occurred!');
        }
    });
}

// Ajax: Delete Room
function deleteRoom(roomId) {
    if (confirm('Are you sure you want to delete this room?')) {
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
                alert('An error occurred while deleting the room!');
            }
        });
    }
}

// Reload room layout without reloading the entire page
function loadRoomLayout() {
    console.log("Reloading room layout...");

    var container = $("#room-layout-container");
    $.ajax({
        url: '/Admin/Room/_GetRoomLayout',
        type: 'GET',
        success: function (tableHtml) {
            container.html(tableHtml);
        },
        error: function () {
            container.html('<p class="text-danger">Error loading room list.</p>');
        }
    });
}
