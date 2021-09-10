var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll",
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "streetAddress", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
            { "data": "postalCode", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data) {
                        return` <input type="checkbox" disabled checked />`
                    }
                    else {
                        return ` <input type="checkbox" disabled />`

                    }
                },
                "width": "10%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <a href="Company/Upsert/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                            <i class="far fa-edit"></i>
                        </a>
                        <a  onclick=Delete("/Admin/Company/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                            <i class="fas fa-trash"></i>
                        </a>`;
                }
            }
        ]

    });
}
function Delete(url) {
    swal({
        title: "Bạn có muốn xóa không?",
        text: "Xóa sẽ không khôi phục lại dữ liệu, bạn có chắc chắn?",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();

                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}
