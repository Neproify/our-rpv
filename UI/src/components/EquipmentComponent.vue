<template>
  <div
    v-if="isShown"
    id="equipmentWindow"
    class="uk-background-secondary uk-light uk-position-center uk-padding"
  >
    <div :class="currentAnimation">
      <legend class="uk-legend uk-text-center">Przedmioty</legend>
      <table class="uk-table uk-table-small uk-table-striped">
        <thead>
          <tr>
            <th>Numer</th>
            <th>Nazwa</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in items">
            <td>{{ index }}</td>
            <td>{{ item.name }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
export default {
  name: "EquipmentComponent",
  data: function () {
    return {
      isShown: false,
      currentAnimation: "",
      items: [
        {
          ID: "12453",
          name: "Desert Eagle",
        },
        {
          ID: "12453",
          name: "Glock",
        },
        {
          ID: "12453",
          name: "Prawo jazdy",
        },
        {
          ID: "12453",
          name: "Dowód osobisty",
        },
        {
          ID: "12453",
          name: "AK-47",
        },
        {
          ID: "12453",
          name: "Kebab",
        },
        {
          ID: "12453",
          name: "Cola",
        },
        {
          ID: "12453",
          name: "Desert Eagle",
        },
        {
          ID: "12453",
          name: "Uzi",
        },
        {
          ID: "12453",
          name: "Kluczyki do samochodu",
        },
        {
          ID: "12453",
          name: "Desert Eagle",
        },
        {
          ID: "12453",
          name: "Desert Eagle",
        },
        {
          ID: "12453",
          name: "Desert Eagle",
        },
        {
          ID: "12453",
          name: "Desert Eagle",
        },
      ],
    };
  },
  methods: {},
  props: {
    showWindow: false,
  },
  watch: {
    showWindow: function (value) {
      if (value == this.isShown) return;
      if (value == true) {
        this.currentAnimation = "uk-animation-scale-up";
        this.isShown = true;
        setTimeout(
          function (self) {
            self.currentAnimation = "";
          },
          500,
          this
        );
      } else {
        this.currentAnimation = "uk-animation-scale-up uk-animation-reverse";
        setTimeout(
          function (self) {
            self.isShown = false;
            self.currentAnimation = "";
          },
          500,
          this
        );
      }
    },
  },
};
</script>
