﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll",
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },

            //{
            //    "data": "id",
            //    "render": function (data) {
            //        return `
            //        <a href="Category/Upsert/${data}" class="btn btn-primary text-white" style="cursor:pointer">
            //                <i class="far fa-edit"></i>
            //            </a>
            //            <a  onclick=Delete("/Admin/Category/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
            //                <i class="fas fa-trash"></i>
            //            </a>`;
            //    }
            //}
        ]

    });
}