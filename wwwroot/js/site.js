// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//api EndPoint

function openSideBar() {
    $('#main').css('display', 'none');
    $('#menu').css('display', 'none');
    $('#title').css('display', 'none');
    $('#sideBar').css('display', 'block');
}

function closeSideBar() {
    $('#main').css('display', 'block');
    $('#menu').css('display', 'block');
    $('#title').css('display', 'block');
    $('#sideBar').css('display', 'none');
}

//Category Moday
async function OpenModal(addEdit, id) {
    if (addEdit == 'add') {
        document.getElementById('ModalLabel').textContent = 'Add Category';
        document.getElementById('SubmitButton').value = 'Add';
    }
    if (addEdit == 'edit') {
        document.getElementById('ModalLabel').textContent = 'Edit Category';
        document.getElementById('SubmitButton').value = 'Update';

        //Get Category By Id
        $.ajax({
            url: `/category/getCategoryById/${id}`,
            type: 'GET',
            contentType: 'application/json',
            success: function (response) {
                document.getElementById('Category_Title').value = response.data.name;
                const label = document.createElement('label');
                label.textContent = response.data.id;
                label.id = 'IdValue';
                label.style.display = 'none';
                const container = document.getElementById('Form-Container');
                container.appendChild(label);
            },
            error: function (error) {
                debugger
                console.error('Error during upsert:', error);
            }
        });

    }
    $('#Category-Main').css('display', 'none');
    $('#Modal').css('display', 'flex');
}

function CloseModal() {
    $('#Modal').css('display', 'none');
    $('#Category-Main').css('display', '');
}

function upsert() {
    id = document.getElementById('IdValue').textContent;
    name = document.getElementById('Category_Title').value;
    if (id) {
        var category = {
            id: +id,
            name: name,
        };
    }
    $.ajax({
        url: '/category/upsert',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(category),
        success: function (response) {
            CloseModal();
            datatable.ajax.reload();
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
        }
    });
}

