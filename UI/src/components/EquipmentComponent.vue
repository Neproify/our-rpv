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
            <td @click="useItem(item.UID)">{{ item.name }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script>
export default {
  name: "EquipmentComponent",
  mounted: function () {
    window.emitter.on("showItemsWindow", () => {
      this.showWindow = true;
    });

    window.emitter.on("hideItemsWindow", () => {
      this.showWindow = false;
    });

    window.emitter.on("onItemsLoaded", (items) => {
      this.items = items;
    });
  },
  data: function () {
    return {
      isShown: false,
      currentAnimation: "",
      items: [],
    };
  },
  methods: {
    useItem: function (UID) {
      mp.trigger("UseItem", UID);
    },
    dropItem: function (UID) {
      mp.trigger("DropItem", UID);
    },
  },
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
