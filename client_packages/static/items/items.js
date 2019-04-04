﻿var items = null;

var vm = new Vue({
    el: "#itemsTable",
    data: {
        items: items
    },
    methods: {
        useItem: function (UID) {
            mp.trigger("UseItem", parseInt(UID));
        },
        dropItem: function(UID) {
            mp.trigger("DropItem", parseInt(UID));
        }
    }
});

function loadItems(input) {
    items = JSON.parse(input);
    vm.items = items;
}