<template>
  <div
    v-if="isShown"
    id="characterCreator"
    class="uk-background-secondary uk-light uk-position-center uk-padding"
  >
    <div :class="currentAnimation">
      <form>
        <fieldset class="uk-fieldset">
          <legend class="uk-legend uk-text-center">Tworzenie postaci</legend>

          <div class="uk-margin">
            <div class="uk-inline">
              <span class="uk-form-icon" uk-icon="icon: user"></span>
              <input v-model="name" class="uk-input" type="text" placeholder="Imię" />
            </div>
          </div>

          <div class="uk-margin uk-inline">
            <span class="uk-form-icon" uk-icon="icon: user"></span>
            <input v-model="surname" class="uk-input" type="text" placeholder="Nazwisko" />
          </div>

          <div class="uk-margin">
            <button
              @click.prevent="sendRequest"
              class="uk-button uk-button-default uk-width-1-1"
            >Stwórz postać</button>
          </div>
        </fieldset>
      </form>
    </div>
  </div>
</template>

<script>
export default {
  name: "CharacterCreatorComponent",
  mounted: function () {
    window.emitter.on("showCharacterCreatorWindow", () => {
      this.showWindow = true;
    });
    window.emitter.on("hideCharacterCreatorWindow", () => {
      this.showWindow = false;
    });
  },
  data: function () {
    return {
      isShown: false,
      currentAnimation: "",
      name: "",
      surname: "",
    };
  },
  methods: {
    sendRequest: function () {
      mp.trigger("CreateNewCharacter", this.name, this.surname);
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
