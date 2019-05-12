var vm = new Vue({
    el: "#inputs",
    data: {
        gender: 0,
        faceOptions: null,
        clothOptions: null,
        propOptions: null
    },
    methods: {
        updateGender: function (event) {
            var gender = 'male';

            if (event.target.value == 1) {
                gender = 'female';
            }
            mp.trigger("UpdateGender", gender);
        },
        updateFaceOption: function (index, event) {
            mp.trigger("UpdateFaceOption", parseInt(index), parseFloat(event.target.value));
        },
        updateClothOption: function (index, event) {
            mp.trigger("UpdateClothOption", parseInt(index), parseInt(event.target.value));
        },
        updatePropOption: function (index, event) {
            mp.trigger("UpdatePropOption", parseInt(index), parseInt(event.target.value));
        },
        saveCustomization: function () {
            var genderToSend = 'male';

            if (this.gender == 1) {
                genderToSend = 'female';
            }

            mp.trigger("SaveCustomization", genderToSend, JSON.stringify(this.faceOptions), JSON.stringify(this.clothOptions), JSON.stringify(this.propOptions));
        }
    }
});