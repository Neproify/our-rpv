function SendCharacterSelectionRequest(UID)
{
    mp.trigger("SelectCharacter", UID);
}

function LoadCharacters(input)
{
	var characters = input;
	for(key in characters)
	{
        var button = document.createElement("button");
	    button.setAttribute("onclick", "SendCharacterSelectionRequest('"+characters[key]['UID']+"');");
	    var name = document.createTextNode(characters[key]['name']);
		button.appendChild(name);
        document.getElementById("characters").appendChild(button);
	}
}