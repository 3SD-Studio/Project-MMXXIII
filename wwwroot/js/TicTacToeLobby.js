let createButton = document.getElementById("create-button");
let refreshButton = document.getElementById("refresh-button");
let joinButton = document.getElementById("join-button");

createButton.addEventListener("click", function () { newLobby(); });
refreshButton.addEventListener("click", function () { location.reload(); });
joinButton.addEventListener("click", function () { joinGame(); })

async function newLobby() {
    let gameId = makeid(6); 
    open("tictactoe/game/" + gameId, "_self");
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function makeid(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
        counter += 1;
    }
    return result;
}

function joinGame() {
    let lobbyIdInput = document.getElementById("lobby-id-input").value;
    open("tictactoe/game/" + lobbyIdInput, "_self");
}