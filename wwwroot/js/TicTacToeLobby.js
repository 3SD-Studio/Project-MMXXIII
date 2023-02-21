let create_button = document.getElementById("create-button");

create_button.addEventListener("click", function () { newLobby(); });

function newLobby() {
    let newLink = document.createElement('li');
    let a = document.createElement("a");
    let br = document.createElement("br");
    a.href = "~/tictactoegame";

    let div = document.createElement("div");
    let p1 = document.createElement("p");
    let p2 = document.createElement("p");
    p1.innerHTML = "game_id";
    p2.innerHTML = "player_amount";

    div.appendChild(p1);
    div.appendChild(p2);

    a.appendChild(div);

    newLink.appendChild(a);
    document.getElementById("lobbies").appendChild(newLink);
    document.getElementById("lobbies").appendChild(br);
}