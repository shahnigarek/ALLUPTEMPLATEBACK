$(document).on("click", ".modalshowbtn", function (e) {
    e.preventDefault()
    fetch($(this).attr('href'))
        .then(response => response.text())
        .then(data => {
            $('.modal-body').html(data);
        })
})

