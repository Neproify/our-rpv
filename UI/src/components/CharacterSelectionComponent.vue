<template>
  <div
    v-if="isShown"
    id="characterSelection"
    class="uk-background-secondary uk-light uk-position-center uk-padding"
  >
    <div :class="currentAnimation">
      <legend class="uk-legend uk-text-center">Wybierz postać</legend>
      <table class="uk-table uk-table-large uk-table-divider" v-if="characters.length > 0">
        <thead>
          <tr>
            <th>Nazwa</th>
            <th>Zdrowie</th>
            <th>Stan konta</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="character in characters">
            <td @click="selectCharacter(character.UID);">{{ character.name }}</td>
            <td>
              <span class="uk-text-danger">{{ character.health }}%</span>
            </td>
            <td>
              <span class="uk-text-success">${{ character.money }}</span>
            </td>
          </tr>
        </tbody>
      </table>
      <div v-else>
        <h5>Nie masz żadnych postaci. Stwórz pierwszą poniżej.</h5>
      </div>
      <button class="uk-button uk-button-primary uk-width-1-1">Utwórz nową postać</button>
    </div>
  </div>
</template>

<script>
export default {
  name: "CharacterSelectionComponent",
  mounted: function () {
    window.emitter.on("showCharacterSelectionWindow", () => {
      this.showWindow = true;
    });
    window.emitter.on("hideCharacterSelectionWindow", () => {
      this.showWindow = false;
    });
    window.emitter.on("characterSelectionLoaded", (characters) => {
      this.characters = characters;
    });
  },
  data: function () {
    return {
      isShown: false,
      currentAnimation: "",
      characters: [],
    };
  },
  methods: {
    selectCharacter: function (UID) {
      mp.trigger("SelectCharacter", UID);
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
