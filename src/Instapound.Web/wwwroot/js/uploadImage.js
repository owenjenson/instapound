window.addEventListener('load', () => {
    document.getElementById('removeImageButton').addEventListener('click', (e) => {
        e.preventDefault();
        updateChangeImage();
        document.getElementById('newImageInput').value = null;
        updateImageElement(null);
    });

    document.getElementById('newImageInput').addEventListener('change', (e) => {
        if (e.currentTarget.files.length === 0) {
            e.preventDefault();
            return;
        }

        updateChangeImage();

        const file = e.currentTarget.files[0];

        updateImageElement(file ?
            URL.createObjectURL(file) :
            null);
    });
});

function updateChangeImage() {
    const input = document.getElementById('changedImageInput');

    if (input) {
        input.value = true;
    }
}

function updateImageElement(src) {
    const image = document.getElementById('image');
    const display = src ? 'block' : 'none';

    document.getElementById('removeImageButton').style.display = display;
    image.style.display = display;
    image.src = src;
}