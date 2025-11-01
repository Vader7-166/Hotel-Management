$(document).ready(function () {
    // Toàn bộ code floating label cũ (cho input-group-outline) đã bị xóa
    // vì trang này (AddNewEmployeeAccount.cshtml)
    // hiện đang dùng 'form-floating' của Bootstrap.

    // === LOGIC CASCADING DROPDOWN (LỌC POSITION THEO ROLE) ===

    var roleDropdown = $('#Role'); // (Tìm thấy ở "Account Info")
    var positionDropdown = $('#Position'); // (Tìm thấy ở "Personal Info")

    // Lắng nghe sự kiện 'change' trên dropdown "Role"
    roleDropdown.on('change', function () {
        var selectedRole = $(this).val();

        // Xóa các lựa chọn cũ của Position (trừ option đầu tiên)
        positionDropdown.find('option:not(:first)').remove();
        positionDropdown.val(''); // Reset về lựa chọn đầu

        if (selectedRole) {
            // Nếu đã chọn Role, bật dropdown Position
            positionDropdown.prop('disabled', false);

            // Gọi AJAX (tái sử dụng Action 'GetPositionsByRole' của bạn)
            $.ajax({
                url: '/Admin/EmployeeManagement/GetPositionsByRole',
                type: 'GET',
                data: { role: selectedRole },
                success: function (positions) {
                    // Lặp qua danh sách JSON trả về
                    $.each(positions, function (i, position) {
                        // Thêm <option> mới vào
                        positionDropdown.append($('<option>', {
                            value: position,
                            text: position
                        }));
                    });
                },
                error: function () {
                    alert('Could not load positions.');
                    positionDropdown.prop('disabled', true);
                }
            });
        } else {
            // Nếu chưa chọn Role (chọn "Select Role First..."), vô hiệu hóa Position
            positionDropdown.prop('disabled', true);
        }
    });
});