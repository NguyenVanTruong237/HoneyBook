﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll",
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <a href="Product/Upsert/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                            <i class="far fa-edit"></i>
                        </a>
                        <a  onclick=Delete("/Admin/Product/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
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

                    } else
                    {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
  }
    