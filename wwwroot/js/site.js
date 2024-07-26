// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

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

function OpenModal() {
    $('#Category-Main').css('display', 'none');
    $('#Modal').css('display', 'flex');
}

function CloseModal() {
    $('#Modal').css('display', 'none');
    $('#Category-Main').css('display', '');
}

function upsertPost() {
    Category_Title
    var data = {
        name: "Sample Item",
    };

    $.ajax({
        url: '/category/upsert',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            console.log('Upsert successful:', response);
        },
        error: function (error) {
            console.error('Error during upsert:', error);
        }
    });
}