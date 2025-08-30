window.addEventListener('load', () => {
    updateAjaxButtons();
});

function updateAjaxButtons() {
    const buttons = document.querySelectorAll('.ajax-btn');

    for (const button of buttons) {
        const id = button.getAttribute('data-id');
        const url = button.getAttribute('data-url');

        button.onclick = async () => {
            const response = await fetch(`${url}/${id}`, { method: 'POST' });
            const result = await response.text();

            button.innerHTML = result;
        };
    }
}