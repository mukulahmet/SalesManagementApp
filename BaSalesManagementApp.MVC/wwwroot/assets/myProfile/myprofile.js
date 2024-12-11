$(document).ready(function () {
    $('#profileModal').modal('show');

    // Fotoğraf seçildiğinde tetiklenecek olay
    $('#photoInput').change(function (event) {
        var file = event.target.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#profileImage').attr('src', e.target.result);
                $('#removePhotoInput').val('false'); // Fotoğrafın silinmediğini işaretle
            };
            reader.readAsDataURL(file);
        }
    });

    // Formu gönderdiğinde modal'ı kapat ve yönlendir
    $('#profileForm').on('submit', function (event) {
        event.preventDefault();

        $.ajax({
            type: $(this).attr('method'),
            url: $(this).attr('action'),
            data: new FormData(this),
            processData: false,
            contentType: false,
            success: function (response) {
                $('#profileModal').modal('hide');
                window.location.href = '/Home/Index'; // Burada yönlendirme yapılacak sayfa yolu
            },
            error: function (error) {
                console.error('Form gönderim hatası:', error);
            }
        });
    });
});

function removePhoto() {
    $('#profileImage').attr('src', '/assets/img/ProfilePhotos/photo.jpg');
    $('#removePhotoInput').val('true');
}
