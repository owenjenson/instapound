const connection = new signalR.HubConnectionBuilder().withUrl('/chatHub').build();
let isSendingDisabled = true;

connection.on('ReceivedMessageFromUser', async (userId) => {
    const messagesList = document.getElementById('messages-list');
    const chattingWithUserId = messagesList.getAttribute('data-user-id');

    if (chattingWithUserId.toLowerCase() != userId.toLowerCase()) {
        return;
    }

    const latestMessageTime = getLastMessageTime();

    try {
        const response = await fetch(`/Messages/MessagesSince/${chattingWithUserId}?latestMessageTime=${latestMessageTime > 0 ? latestMessageTime.toString() : null}`, {
            method: 'GET',
        });
        const result = await response.text();

        messagesList.innerHTML = result + messagesList.innerHTML;
        messagesList.scrollTop = messagesList.scrollHeight;
    }
    catch (e) {
        console.error(e);
    }
});

connection.start().then(() => {
    updateDisabled(false);
}).catch((err) => {
    return console.error(err.toString());
});

window.addEventListener('load', () => {
    document.body.scrollTop = document.body.scrollHeight;

    const sendButton = document.getElementById('send-button');
    const textArea = document.getElementById('message-textarea');
    sendButton.onclick = sendMessage;
    textArea.onkeydown = async (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            await sendMessage();
        }
    };
});

async function sendMessage() {
    updateDisabled(true);

    const textArea = document.getElementById('message-textarea');
    const messagesList = document.getElementById('messages-list');
    const userId = messagesList.getAttribute('data-user-id');
    const message = textArea.value.trim();

    if (message === '') {
        updateDisabled(false);
        textArea.focus();
        return;
    }

    textArea.value = '';

    const latestMessageTime = getLastMessageTime();

    try {
        const response = await fetch(`/Messages/Chat/${userId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Text: message, LatestMessageTime: latestMessageTime > 0 ? latestMessageTime : null }),
        });
        const result = await response.text();

        connection.invoke('NotifyUser', userId).catch((err) => {
            return console.error(err.toString());
        });

        messagesList.innerHTML = result + messagesList.innerHTML;
        messagesList.scrollTop = messagesList.scrollHeight;
    }
    catch (e) {
        console.error(e);
    }

    updateDisabled(false);

    textArea.focus();
}

function getLastMessageTime() {
    return [...document.querySelectorAll('.chat-message')]
        .map((li) => li.getAttribute('data-chat-message-time'))
        .reduce((prev, curr) => prev < curr ? curr : prev, 0);
}

function updateDisabled(disabled) {
    isSendingDisabled = disabled;

    const sendButton = document.getElementById('send-button');
    const textArea = document.getElementById('message-textarea');

    sendButton.disabled = disabled;
    textArea.disabled = disabled;
}