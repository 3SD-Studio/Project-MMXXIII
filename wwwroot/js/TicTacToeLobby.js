let createButton = document.getElementById("create-button");
let refreshButton = document.getElementById("refresh-button");

createButton.addEventListener("click", function () { newLobby(); });
refreshButton.addEventListener("click", function () { location.reload(); });

async function newLobby() {
    let gameId = makeid(6); 
    let addNewLobby = "/tictactoe/createlobby/" + gameId;
    console.log(addNewLobby);
    console.log("TUTAJ");
    fetch(addNewLobby, {
        method: 'POST',
    });

   
    //await sleep(3000);
    open("tictactoe/game/" + gameId);
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