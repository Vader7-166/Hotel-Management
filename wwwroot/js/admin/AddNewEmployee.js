$(document).ready(function () {
    // === LOGIC CASCADING DROPDOWN (LỌC POSITION THEO ROLE) ===

    var roleDropdown = $('#Role');
    var positionDropdown = $('#Position');

    var currentPositionValue = positionDropdown.data('current-position');

    // Lắng nghe sự kiện 'change' trên dropdown "Role"
    roleDropdown.on('change', function () {
        var selectedRole = $(this).val();

        // Xóa các lựa chọn cũ của Position (trừ option đầu tiên)
        positionDropdown.find('option:not(:first)').remove();
        positionDropdown.val(''); // Reset về lựa chọn đầu

        if (selectedRole) {
            positionDropdown.prop('disabled', false);

            $.ajax({
                url: '/Admin/EmployeeManagement/GetPositionsByRole',
                type: 'GET',
                data: { role: selectedRole },
                success: function (positions) {

                    // Lặp qua danh sách JSON trả về
                    $.each(positions, function (i, position) {
                        positionDropdown.append($('<option>', {
                            value: position,
                            text: position
                        }));
                    });
                    // Sau khi điền xong, hãy chọn giá trị đã lưu
                    if (currentPositionValue) {
                        positionDropdown.val(currentPositionValue);
                    }

                },
                error: function () {
                    alert('Could not load positions.');
                    positionDropdown.prop('disabled', true);
                }
            });
        } else {
            positionDropdown.prop('disabled', true);
        }
    }).trigger('change');
});