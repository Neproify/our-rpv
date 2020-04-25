function enableChatInput(enable) {
    var input = document.getElementById("chatInput");
    if (enable) {
        input.style.display = "initial";
        input.focus();
    }
    else {
        input.style.display = "none";
        input.value = "";
    }
}

function showChat(enable) {
    var body = document.body;

    if (enable) {
        body.style.display = "initial";
    }
    else {
        body.style.display = "none";
    }
}

function sendMessage() {
    var input = document.getElementById("chatInput");
    var value = input.value;

    if (value != null) {
        if (value[0] == "/") {
            value = value.substr(1);

            mp.invoke("command", value);
        }
        else {
            mp.invoke("chatMessage", value);
        }
    }
}

var chatAPI =
{
    push: (text) => {
        var history = document.getElementById("chatHistory");
        var message = document.createElement("div");
        var match = text.match("!\{(.*?)\}");
        while (match != null) {
            text = text.replace(match[0], '<span style="color: ' + match[1] + ';">');
            match = text.match("!\{(.*?)\}");
        }
        message.innerHTML = text;
        history.appendChild(message);
        history.scrollTop = history.scrollHeight;
    },
    clear: () => {
        var history = document.getElementById("chatHistory");
        var input = document.getElementById("chatInput");
        history.innerHTML = "";
        input.nodeValue = "";
    },
    activate: (toggle) => {
        enableChatInput(toggle);
    },
    show: (toggle) => {
    }
};

document.addEventListener("DOMContentLoaded", function () {
    enableChatInput(false);
}, false);

var api = { "chat:push": chatAPI.push, "chat:clear": chatAPI.clear, "chat:activate": chatAPI.activate, "chat:show": chatAPI.show };

for (var fn in api) {
    mp.events.add(fn, api[fn]);
}