var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Category/GetAll",
        },
        "columns": [
            { "data": "name", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                    <a href="Category/Upsert/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                            <i class="far fa-edit"></i>
                        </a>
                        <a class="btn btn-danger text-white" style="cursor:pointer">
                            <i class="fas fa-trash"></i>
                        </a>`;
                }
            }
        ]

    });
}