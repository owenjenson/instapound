window.addEventListener('load', () => {
    updateConfirmation();
});

function updateConfirmation() {
    const elements = document.querySelectorAll('a[data-confirm], button[data-confirm], input[data-confirm]');
    for (const element of elements) {
        element.removeEventListener('click', confirmHandler);
        element.addEventListener('click', confirmHandler);
    }
}

function confirmHandler(e) {
    const confirmed = confirm(e.currentTarget.getAttribute('data-confirm'));
    if (!confirmed)
        e.preventDefault();
}