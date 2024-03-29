﻿let scheme = document.location.protocol === "https:" ? "wss" : "ws";
let port = document.location.port ? (":" + document.location.port) : "";
let socket;

let connectionUrl = scheme + "://" + document.location.hostname + port + document.location.pathname;

console.log(connectionUrl);
socket = new WebSocket(connectionUrl);

let gameResult = document.getElementById("game-result");
let restartButton = document.getElementById("restart-button-id");
let resultParagraph = document.getElementById("result-paragraph");
restartButton.disabled = true;

let array = [
    document.getElementById("x1, y1"),
    document.getElementById("x1, y2"),
    document.getElementById("x1, y3"),
    document.getElementById("x2, y1"),
    document.getElementById("x2, y2"),
    document.getElementById("x2, y3"),
    document.getElementById("x3, y1"),
    document.getElementById("x3, y2"),
    document.getElementById("x3, y3"),
];

function clickedField(x, y) {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        alert("Connection error!");
    }
    let data = "x" + x + ", y" + y;
    socket.send(data);
}

restartButton.addEventListener("click", function () { restartButtonClicked(); });

for (let x = 0; x < 3; x++) {
    for (let y = 0; y < 3; y++) {
        array[y + x * 3].addEventListener("click", () => { clickedField(x + 1, y + 1); });
    }
}

function restartButtonClicked() {
    if (!socket || socket.readyState !== WebSocket.OPEN) {
        alert("Connection error!");
    }

    let data = "restart";
    socket.send(data);

    gameResult.classList.add("disabled-game-result-container");
    gameResult.classList.remove("game-result-container");
    restartButton.disabled = false;
    restartButton.classList.add("disabled-restart-button");
    restartButton.classList.remove("restart-button");
    enableAll(array);
}

socket.onmessage = function (event) {
    if (event.data === "x win.") {
        resultParagraph.innerHTML = "X won!";
        gameResult.classList.remove("disabled-game-result-container");
        gameResult.classList.add("game-result-container");
        restartButton.disabled = false;
        restartButton.classList.remove("disabled-restart-button");
        restartButton.classList.add("restart-button");
        disableUnused(array);
        return;
    }

    if (event.data == "o win.") {
        resultParagraph.innerHTML = "O won!";
        gameResult.classList.remove("disabled-game-result-container");
        gameResult.classList.add("game-result-container");
        restartButton.disabled = false;
        restartButton.classList.remove("disabled-restart-button");
        restartButton.classList.add("restart-button");
        disableUnused(array);
        return;
    }

    if (event.data == "d win.") {
        resultParagraph.innerHTML = "Draw!";
        gameResult.classList.remove("disabled-game-result-container");
        gameResult.classList.add("game-result-container");
        restartButton.disabled = false;
        restartButton.classList.remove("disabled-restart-button");
        restartButton.classList.add("restart-button");
        disableUnused(array);
        return;
    }

    let new_values = event.data.split(";");
    console.log(new_values)

    for (i = 0; i < 9; i++) {
        if (new_values[i][7] !== "\x00") {
            array[i].textContent = new_values[i][7];
            array[i].disabled = true;
            array[i].classList.remove("actual-button");
            array[i].classList.add("disabled-button");
        }
    }
}

function disableUnused(array) {
    for (let i = 0; i < 9; i++) {
        if (array[i].classList.contains("actual-button")) {
            array[i].disabled = true;
            array[i].classList.remove("actual-button");
            array[i].classList.add("disabled-button1");
        }
    }
}

function enableAll(array) {
    for (let i = 0; i < 9; i++) {
        if (array[i].classList.contains("disabled-button")) {
            array[i].disabled = false;
            array[i].classList.remove("disabled-button");
            array[i].classList.add("actual-button");
        }
        if (array[i].classList.contains("disabled-button1")) {
            array[i].disabled = false;
            array[i].classList.remove("disabled-button1");
            array[i].classList.add("actual-button");
        }
        array[i].textContent = "";
    }
}