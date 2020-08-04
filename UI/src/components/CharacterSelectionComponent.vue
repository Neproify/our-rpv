<template>
  <div
    v-if="isShown"
    id="characterSelection"
    class="uk-background-secondary uk-light uk-position-center uk-padding"
  >
    <div :class="currentAnimation">
      <legend class="uk-legend uk-text-center">Wybierz postać</legend>
      <table class="uk-table uk-table-large uk-table-divider">
        <thead>
          <tr>
            <th>Nazwa</th>
            <th>Zdrowie</th>
            <th>Stan konta</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="character in characters">
            <td>{{ character.name }}</td>
            <td>
              <span class="uk-text-danger">{{ character.health }}%</span>
            </td>
            <td>
              <span class="uk-text-success">${{ character.money }}</span>
            </td>
          </tr>
        </tbody>
      </table>
      <button class="uk-button uk-button-primary">Utwórz nową postać</button>
    </div>
  </div>
</template>

<script>
export default {
  name: "CharacterSelectionComponent",
  data: function () {
    return {
      isShown: false,
      currentAnimation: "",
      characters: [
        {
          name: "Test_Testowy",
          health: 100,
          money: 500,
        },
        {
          name: "Test_Testowy",
          health: 90,
          money: 500,
        },
        {
          name: "Test_Testowy",
          health: 100,
          money: 500,
        },
        {
          name: "Test_Testowy",
          health: 100,
          money: 7000,
        },
        {
          name: "Test_Testowy",
          health: 100,
          money: 500,
        },
        {
          name: "Test_Testowy",
          health: 100,
          money: 500,
        },
        {
          name: "Test_Testowy",
          health: 100,
          money: 500,
        },
        {
          name: "Test_Testowy",
          health: 100,
          money: 500,
        },
        {
          name: "Test_Testowy",
          health: 100,
          money: 500,
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
