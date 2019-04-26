function CreateCharacter() {
    var name = document.getElementById("nameInput").value;
    var surname = document.getElementById("surnameInput").value;

    mp.trigger("CreateNewCharacter", name, surname);
}