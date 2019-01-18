function loadItems(input)
{

    var items = JSON.parse(input);
    for (key in items) {
        var table = document.getElementById("itemsTable");
        var tableRow = document.createElement("tr");
        var tableDataUID = document.createElement("td");
        tableDataUID.innerText = items[key]["UID"];
        tableRow.appendChild(tableDataUID);
        var tableDataName = document.createElement("td");
        tableDataName.innerText = items[key]["name"];
        tableDataName.setAttribute("onclick", "useItem(" + items[key]["UID"] + ");");
        tableRow.appendChild(tableDataName);
        table.appendChild(tableRow);
    }
}

function useItem(UID)
{
    mp.trigger("UseItem", parseInt(UID));
}