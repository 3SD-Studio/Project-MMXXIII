let createButton = document.getElementById("create-button");
let refreshButton = document.getElementById("refresh-button");
var gameCounter = 1;

createButton.addEventListener("click", function () { newLobby(); });
refreshButton.addEventListener("click", function () { location.reload(); });

async function newLobby() {

    let addNewLobby =  "/tictactoe/createlobby/" + gameCounter;
    console.log(addNewLobby);
    console.log("TUTAJ");
    fetch(addNewLobby, {
        method: 'POST',
    });

   
    //await sleep(3000);
    open("tictactoe/game/" + gameCounter);
    gameCounter += 1;
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}