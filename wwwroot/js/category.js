var datatable;
$(document).ready(function () {
    LoadDataTable();
    $('#dt-search-0').attr('placeholder', 'Search...');
    
});
function LoadDataTable() {
    datatable = $("#tblData").DataTable({
        "language": {
            "search": "",
            "lengthMenu": "_MENU_ Records per page"
        },
        "initComplete": function (settings, json) {
            // Set placeholder for search input
            var searchInput = $('.dataTables_filter input');
            searchInput.attr('placeholder', 'Search...');
        },
        ajax: {
            url: "/Category/GetAll",
        },
        columns: [
            {
                data: "name",
                width: "70%",
            },
            {
                data: "id",
                render: function (data) {
                    return `
                      <div class="text-center">
                      <a class="btn btn-success" style="font-size: .9rem;" href="/Admin/Category/Upsert/${data}"><i class="fas fa-edit"></i></a>
                      <a class="btn btn-danger" style="font-size: .9rem;"  OnClick=Delete("/Admin/Category/Delete/${data}")><i class="fas fa-trash-alt"></i></a>
                      </div>
                      `;
                },
            },
        ],
    });
}
function Delete(url) {
    //alert(url);
    swal({
        title: "Want To Delete Data ?",
        text: "Delete Information",
        buttons: true,
        icon: "warning",
        //dangerModel: true
    }).then((WillDelete) => {
        if (WillDelete) {
            $.ajax({
                url: url,
                type: "Delete",
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        datatable.ajax.reload();
                    }
                },
            });
        }
    });
}
