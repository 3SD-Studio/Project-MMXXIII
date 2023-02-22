let createButton = document.getElementById("create-button");
var gameCounter = 1;

createButton.addEventListener("click", function () { newLobby(); });
refreshButton.addEventListener("click", function () { location.reload(); });

function newLobby() {

    let addNewLobby =  "/tictactoe/createlobby/" + gameCounter;
    console.log(addNewLobby);
    console.log("TUTAJ");
    fetch(addNewLobby, {
        method: 'POST',
    });

    gameCounter += 1;
    sleep(100);
    open("tictactoe/game/" + gameCounter);
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}