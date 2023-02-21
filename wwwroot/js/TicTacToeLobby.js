let createButton = document.getElementById("create-button");
var gameCounter = 1;

createButton.addEventListener("click", function () { newLobby(); });

function newLobby() {
    let newLink = document.createElement('li');
    let a = document.createElement("a");
    let br = document.createElement("br");
    a.href = "/tictactoe/game/" + gameCounter;

    let div = document.createElement("div");
    let p1 = document.createElement("p");
    let p2 = document.createElement("p");
    p1.innerHTML = "Game: " + gameCounter;
    p2.innerHTML = "1/2";

    div.appendChild(p1);
    div.appendChild(p2);

    a.appendChild(div);

    newLink.appendChild(a);
    document.getElementById("lobbies").appendChild(newLink);
    document.getElementById("lobbies").appendChild(br);
    gameCounter += 1;
}